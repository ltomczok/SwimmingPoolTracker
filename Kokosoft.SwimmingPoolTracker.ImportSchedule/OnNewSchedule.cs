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
        private readonly IServiceProvider services;

        public OnNewSchedule(IBus bus, ILogger<OnNewSchedule> logger, MongoClient mongoClient, IServiceProvider services)
        {
            messageBus = bus;
            this.logger = logger;
            this.mongoClient = mongoClient;
            this.services = services;
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

        public List<string> LoadDocument(string fileName)
        {
            List<string> parsedSchedule = new List<string>();
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
            parsedSchedule.AddRange(extractedText.Split("\r\n"));

            return parsedSchedule;
        }

        public async Task<bool> LoadSchedulle(NewSchedule newSchedule, string fileName)
        {
            string endOfDay = string.Empty;
            List<Schedule> scheduleList = new List<Schedule>();
            DateTime startDate = new DateTime(newSchedule.YearFrom, newSchedule.MonthFrom, newSchedule.DayFrom);
            DateTime endDate = new DateTime(newSchedule.YearTo, newSchedule.MonthTo, newSchedule.DayTo);
            double itterations = ((endDate - startDate).Days) + 1;

            List<string> parsedSchedule = LoadDocument(fileName);
            List<int> hours = LoadHours(parsedSchedule); new List<int>();

            for (int i = 0; i < hours.Count; i++)
            {
                DateTime date = startDate;

                int current = hours[i];
                int next;
                int iterations = 7;
                if (current != hours.Last())
                {
                    next = hours[i + 1];
                    if ((next - current - 1) < 7)
                    {
                        iterations = next - current - 1;
                    }
                }
                else
                {
                    iterations = (parsedSchedule.Count - current) - 1;
                }


                for (int j = 1; j <= iterations; j++)
                {
                    Schedule schedule = new Schedule();
                    scheduleList.Add(schedule);
                    schedule.StartTime = parsedSchedule[current];
                    schedule.Day = date;
                    if (parsedSchedule[current + j].Contains("x") || parsedSchedule[current + j].Contains("wypł") || parsedSchedule[current + j].Equals("0"))
                    {
                        schedule.Tracks = parsedSchedule[current + j].Split(',', 'i').Select(x => x.Trim().Replace("m", string.Empty).Replace("wypł", "shallow")).ToList();
                    }
                    else
                    {
                        var aaa = 1;
                    }
                    date = date.AddDays(1);
                }
            }

            var mergedSchedules = MergeSchedule(scheduleList);
            try
            {

                using (PoolsContext dc = this.services.GetService(typeof(PoolsContext)) as PoolsContext)
                {
                    await dc.Schedules.AddRangeAsync(mergedSchedules);
                    await dc.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return true;
        }

        string endOfDay = string.Empty;
        private List<int> LoadHours(List<string> parsedSchedule)
        {
            string lastHour = "00:00";
            List<int> hours = new List<int>();
            for (int i = 0; i < parsedSchedule.Count(); i++)
            {
                string time = parsedSchedule[i];

                if (IsValidTimeFormat(time))
                {
                    if (TimeSpan.Parse(time) > TimeSpan.Parse(lastHour))
                    {
                        lastHour = time;
                    }
                    hours.Add(i);
                }
            }
            endOfDay = lastHour;
            return hours;
        }

        private List<Schedule> MergeSchedule(List<Schedule> poolSchedule)
        {
            List<Schedule> mergedSchedules = new List<Schedule>();
            poolSchedule.RemoveAll(c => c.IsEmpty);
            Schedule mergedSchedule = null;
            foreach (Schedule schedule in poolSchedule.OrderBy(p => p.Day).ThenBy(p => p.StartTime))
            {
                if (mergedSchedule != null)
                {
                    if (!mergedSchedules.Contains(mergedSchedule))
                    {
                        mergedSchedules.Add(mergedSchedule);
                    }

                    mergedSchedule.EndTime = schedule.StartTime;

                    if (TimeSpan.Parse(mergedSchedule.EndTime) < TimeSpan.Parse(mergedSchedule.StartTime))
                    {
                        mergedSchedule.EndTime = endOfDay;
                    }

                    if (mergedSchedule.Day != schedule.Day || !mergedSchedule.Tracks.SequenceEqual(schedule.Tracks))
                    {
                        mergedSchedule = new Schedule() { Day = schedule.Day, StartTime = schedule.StartTime, Tracks = new List<string>(schedule.Tracks), Pool = schedule.Pool };
                    }
                }
                else
                {
                    mergedSchedule = new Schedule() { Day = schedule.Day, StartTime = schedule.StartTime, Tracks = new List<string>(schedule.Tracks), Pool = schedule.Pool };
                }
            }

            if (TimeSpan.Parse(mergedSchedules.Last().EndTime) < TimeSpan.Parse(endOfDay))
            {
                mergedSchedules.Last().EndTime = endOfDay;
            }

            return mergedSchedules;
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
