using EasyNetQ;
using HtmlAgilityPack;
using Kokosoft.SwimmingPoolTracker.CheckNewSchedule.Helpers;
using Kokosoft.SwimmingPoolTracker.CheckNewSchedule.Model;
using Kokosoft.SwimmingPoolTracker.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Kokosoft.SwimmingPoolTracker.CheckNewSchedule
{
    public class CheckNewScheduleBackgroundService : IHostedService
    {
        private readonly IMonthHelpers monthHelper;
        private readonly IMongoClient mongoClient;
        private readonly ILogger<CheckNewScheduleBackgroundService> logger;
        private readonly IBus messageBus;
        private readonly IApplicationLifetime app;
        public CheckNewScheduleBackgroundService(IMonthHelpers monthHelper, MongoClient mongoClient, ILogger<CheckNewScheduleBackgroundService> logger, IBus messageBus, IApplicationLifetime app)
        {
            this.monthHelper = monthHelper;
            this.mongoClient = mongoClient;
            this.logger = logger;
            this.messageBus = messageBus;
            this.app = app;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            app.ApplicationStarted.Register(() => { logger.LogInformation("Host CheckNewSchedule started."); });
            app.ApplicationStopping.Register(() => { logger.LogInformation("Host CheckNewSchedule stoping."); });
            app.ApplicationStopped.Register(() => { logger.LogInformation("Host CheckNewSchedule stopped."); });
            await CheckNewSchedule();
            app.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task<FileParseResult> ParseFile(string remoteFile)
        {
            FileParseResult parseResult = new FileParseResult();
            PoolSchedule poolSchedule = new PoolSchedule();
            poolSchedule.Link = remoteFile;
            parseResult.Success = true;
            parseResult.Schedule = poolSchedule;
            string extractedText = string.Empty;

            try
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(remoteFile);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    Stream contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                    PdfLoadedDocument loadedDocument = new PdfLoadedDocument(contentStream);
                    DateTime modyficationDate = loadedDocument.DocumentInformation.ModificationDate;
                    poolSchedule.ModificationDate = modyficationDate;
                    // Loading page collections
                    PdfLoadedPageCollection loadedPages = loadedDocument.Pages;
                    extractedText = loadedPages[0].ExtractText();
                    //Close the document.
                    loadedDocument.Close(true);
                    List<string> pdfList = new List<string>();
                    pdfList.AddRange(extractedText.Split("\r\n"));
                    string header = pdfList.Where(s => s.StartsWith("Harmonogram")).SingleOrDefault();
                    if (header != null) //checking if the header is compatible with the schema
                    {
                        int linkLenght = header.Length;
                        int fromIndex = header.IndexOf("od");
                        string dates = header.Substring(fromIndex, (linkLenght - fromIndex));
                        string startDateString = dates.Substring(2, dates.IndexOf("do") - 2).Trim();
                        string endDateString = dates.Substring(dates.IndexOf("do") + 2, dates.Length - 4 - (dates.IndexOf("do") + 2)).Trim();

                        GetValuesFromDateString(startDateString, poolSchedule, DateType.From);
                        GetValuesFromDateString(endDateString, poolSchedule, DateType.To);
                    }
                }
                poolSchedule.CheckDates();
                httpResponseMessage.Dispose();
                httpClient.Dispose();
                return parseResult;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Błąd podczas parsowania pliku PDF");
                parseResult.Success = false;
                parseResult.Schedule = null;
            }
            return parseResult;
        }

        private async Task CheckNewSchedule()
        {
            logger.LogInformation("Check if new schedule exists.");
            try
            {
                var collectionName = "PoolSchedules";
                var database = mongoClient.GetDatabase("SwimmingPoolTracker");
                var collection = database.GetCollection<PoolSchedule>(collectionName);

                List<PoolSchedule> links = new List<PoolSchedule>();
                string url = "https://mzuk.gliwice.pl/jednostka/kryte-plywalnie/kryta-plywalnia-olimpijczyk/";
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                HtmlNodeCollection siteLinks = doc.DocumentNode.SelectNodes(".//a[contains(@href,'niecka')]");
                foreach (HtmlNode link in siteLinks)
                {
                    FileParseResult parseResult = await ParseFile(link.Attributes["href"].Value);
                    if (parseResult.Success)
                    {
                        PoolSchedule exist = await collection.Find(x => x.Id == parseResult.Schedule.Id).SingleOrDefaultAsync();
                        if (exist == null)
                        {
                            await collection.InsertOneAsync(parseResult.Schedule);
                            await SendNotification("New schedule", parseResult.Schedule);
                        }
                        else
                        {
                            // MongoDB stores dates in UTC, hosting machine timezone is set to UTC+1
                            if (exist.ModificationDate.ToLocalTime() != parseResult.Schedule.ModificationDate)
                            {
                                exist.ModificationDate = parseResult.Schedule.ModificationDate;
                                await collection.ReplaceOneAsync(d => d.Id == exist.Id, exist);
                                await SendNotification("Changes in schedule", parseResult.Schedule);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during checking new schedule");
            }
        }

        private async Task SendNotification(string loggerInformation, PoolSchedule poolSchedule)
        {
            logger.LogInformation($"{loggerInformation}. Id: {poolSchedule.Id}");
            var message = new NewSchedulle
            {
                Id = poolSchedule.Id,
                Link = poolSchedule.Link,
                ModificationDate = poolSchedule.ModificationDate,
                DayFrom = poolSchedule.DayFrom,
                DayTo = poolSchedule.DayTo,
                MonthFrom = poolSchedule.MonthFrom,
                MonthTo = poolSchedule.MonthTo,
                YearFrom = poolSchedule.YearFrom,
                YearTo = poolSchedule.YearTo
            };

            await messageBus.SendAsync<NewSchedulle>("swimmingpooltracker", message);
        }

        private void GetValuesFromDateString(string dateString, PoolSchedule poolSchedule, DateType dateType)
        {
            Type type = poolSchedule.GetType();
            string dateTypeString = dateType.ToString();
            string year = $"Year{dateTypeString}";
            string month = $"Month{dateTypeString}";
            string day = $"Day{dateTypeString}";
            int iteration = 1;
            foreach (var item in dateString.Split(null))
            {
                switch (iteration)
                {
                    case 1: //day
                        type.GetProperty(day).SetValue(poolSchedule, int.Parse(item));
                        break;
                    case 2: //month
                        if (monthHelper.CheckIfMonth(item))
                        {
                            type.GetProperty(month).SetValue(poolSchedule, monthHelper.GetMonthNumber(item));
                        }
                        break;
                    case 3: //year
                        type.GetProperty(year).SetValue(poolSchedule, int.Parse(item)); int.Parse(item);
                        break;
                }
                iteration++;
            }

        }
    }

    enum DateType
    {
        From,
        To
    }

    public class FileParseResult
    {
        public bool Success { get; set; }
        public PoolSchedule Schedule { get; set; }
    }
}
