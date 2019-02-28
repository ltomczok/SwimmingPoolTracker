using EasyNetQ;
using Kokosoft.SwimmingPoolTracker.Core;
using Kokosoft.SwimmingPoolTracker.ImportSchedule.Data;
using Kokosoft.SwimmingPoolTracker.ImportSchedule.Model;
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

namespace Kokosoft.SwimmingPoolTracker.ImportSchedule
{
    public class OnNewSchedule : IHostedService
    {
        IBus messageBus;
        private readonly ILogger<OnNewSchedule> logger;
        private readonly MongoClient mongoClient;

        public PoolsContext Dc { get; }

        public OnNewSchedule(IBus bus, ILogger<OnNewSchedule> logger, MongoClient mongoClient, PoolsContext dc)
        {
            messageBus = bus;
            this.logger = logger;
            this.mongoClient = mongoClient;
            Dc = dc;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
           messageBus.Receive<NewSchedule>("swimmingpooltracker", message => OnNewMessage(message));
           return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            messageBus.Dispose();
            return Task.CompletedTask;
        }

        public async Task OnNewMessage(NewSchedule newSchedule)
        {
            try
            {
                logger.LogInformation($"New schedule: {newSchedule.Link}");
                string fileName = newSchedule.Link.Substring(newSchedule.Link.LastIndexOf('/') + 1, (newSchedule.Link.Length - newSchedule.Link.LastIndexOf('/')) - 1);
                bool fileDownloaded = await DownloadFile(newSchedule.Link, fileName);
                if (fileDownloaded)
                {
                    bool fileLoaded = await LoadSchedulle(newSchedule, fileName);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "");
            }
        }

        public async Task<bool> LoadSchedulle(NewSchedule newSchedule, string fileName)
        {
            List<Schedule> poolSchedules = new List<Schedule>();
            DateTime startDate = new DateTime(newSchedule.YearFrom, newSchedule.MonthFrom, newSchedule.DayFrom);
            DateTime endDate = new DateTime(newSchedule.YearTo, newSchedule.MonthTo, newSchedule.DayTo);
            double itterations = ((endDate - startDate).Days) + 1;
            //Load the PDF document.
            FileStream docStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            //Load the PDF document.
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(docStream);

            // Loading page collections
            PdfLoadedPageCollection loadedPages = loadedDocument.Pages;

            string extractedText = string.Empty;

            // Extract text from existing PDF document pages
            foreach (PdfLoadedPage loadedPage in loadedPages)
            {
                extractedText += loadedPage.ExtractText();
            }
            //Close the document.
            loadedDocument.Close(true);
            docStream.Close();
            List<Schedule> poolSchedule = new List<Schedule>();
            List<string> pdfList = new List<string>();
            pdfList.AddRange(extractedText.Split("\r\n"));
            List<int> schedulers = new List<int>();
            for (int i = 0; i < pdfList.Count(); i++)
            {
                string time = pdfList[i];

                if (IsValidTimeFormat(time))
                {
                    schedulers.Add(i);
                }

            }
            for (int i = 0; i < schedulers.Count; i++)
            {
                DateTime date = startDate;

                int current = schedulers[i];
                int next;
                int iterations = 7;
                if (current != schedulers.Last())
                {
                    next = schedulers[i + 1];
                    if ((next - current - 1) < 7)
                    {
                        iterations = next - current - 1;
                    }
                }
                else
                {
                    iterations = (pdfList.Count - current) - 1;
                }


                for (int j = 1; j <= iterations; j++)
                {
                    Schedule schedule = new Schedule();
                    poolSchedule.Add(schedule);
                    schedule.Time = pdfList[current];
                    schedule.Day = date;
                    string[] s = pdfList[current + j].Split(',', 'i');
                    foreach (var item in s)
                    {
                        if (item.Trim() == "wypł")
                        {
                            schedule.GetType().GetProperty("TrackShallow").SetValue(schedule, 1);
                        }
                        if (item.Contains('x'))
                        {
                            string[] track = item.Split('x');

                            if (track[1].Trim() == "50m")
                            {
                                schedule.GetType().GetProperty("Track50").SetValue(schedule, int.Parse(track[0]));
                            }
                            if (track[1].Trim() == "25m")
                            {
                                schedule.GetType().GetProperty("Track25").SetValue(schedule, int.Parse(track[0]));
                            }
                        }
                    }
                    date = date.AddDays(1);
                }
            }
            poolSchedule.RemoveAll(c => c.IsEmpty);
            await Dc.Schedules.AddRangeAsync(poolSchedule);
            await Dc.SaveChangesAsync();
            return true;
        }

        public bool IsValidTimeFormat(string input)
        {
            if (input.Length < 5)
            {
                return false;
            }
            TimeSpan dummyOutput;
            return TimeSpan.TryParse(input, out dummyOutput);
        }

        public async Task<bool> DownloadFile(string remoteFile, string localFileName)
        {
            bool fileDownloaded = true;
            HttpClient httpClient = null;
            HttpResponseMessage httpResponseMessage = null;
            try
            {
                httpClient = new HttpClient();
                httpResponseMessage = await httpClient.GetAsync(remoteFile);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    Stream contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                    FileStream fileStream = new FileStream(localFileName, FileMode.Create);
                    await contentStream.CopyToAsync(fileStream);
                    fileStream.Close();
                }
                return fileDownloaded;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error durning PDF file download.");
            }
            finally
            {
                httpResponseMessage.Dispose();
                httpClient.Dispose();
            }
            return fileDownloaded;
        }
    }
}
