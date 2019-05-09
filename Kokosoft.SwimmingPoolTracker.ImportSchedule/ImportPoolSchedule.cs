using EasyNetQ;
using Kokosoft.SwimmingPoolTracker.Core;
using Kokosoft.SwimmingPoolTracker.ImportSchedule.Data;
using Kokosoft.SwimmingPoolTracker.ImportSchedule.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kokosoft.SwimmingPoolTracker.ImportSchedule
{
    public class ImportPoolSchedule : IImportPoolSchedule
    {
        string message = string.Empty;
        private readonly ILogger<ImportPoolSchedule> logger;
        private readonly PoolsContext poolsContext;
        private readonly DownloadFileService fileService;
        private readonly IHttpClientFactory httpClientFactory;

        public ImportPoolSchedule(ILogger<ImportPoolSchedule> logger, PoolsContext poolsContext, DownloadFileService fileService)
        {
            this.logger = logger;
            this.poolsContext = poolsContext;
            this.fileService = fileService;
        }

        public async Task ImportSchedule(string poolName, DateTime startDate, DateTime endDate, string filePath)
        {
            message = $"new schedule of pool: {poolName} for {startDate.ToShortDateString()} to {endDate.ToShortDateString()} from file {filePath}";
            try
            {
                logger.LogInformation($"Start import {message}.");
                string fileName = filePath.Substring(filePath.LastIndexOf('/') + 1, (filePath.Length - filePath.LastIndexOf('/')) - 1);
                bool fileDownloaded = await DownloadFile(filePath, fileName);
                if (fileDownloaded)
                {
                    await LoadSchedulle(poolName, startDate, endDate, fileName);
                    logger.LogInformation($"Imported {message}.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error during import {message}.");
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

        public async Task<bool> LoadSchedulle(string poolName, DateTime startDate, DateTime endDate, string fileName)
        {
            try
            {
                logger.LogInformation($"Loading schedule {message}.");
                logger.LogInformation("Check if schedules exists");
                var existingSchedules = await poolsContext.Schedules.Where(s => s.PoolId == poolName && (s.Day >= startDate && s.Day <= endDate)).ToListAsync();
                if (existingSchedules.Count > 0)
                {
                    logger.LogInformation("Remove existing schedules");
                    poolsContext.RemoveRange(existingSchedules);
                    await poolsContext.SaveChangesAsync();
                }

                Pool databasePool = await poolsContext.SwimmingPools.Where(p => p.ShortName == poolName).SingleOrDefaultAsync();
                string exitTime = databasePool.ExitTime;

                List<Schedule> scheduleList = new List<Schedule>();
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
                        schedule.PoolId = databasePool.ShortName;
                        scheduleList.Add(schedule);
                        schedule.StartTime = parsedSchedule[current];
                        schedule.Day = date;
                        if (parsedSchedule[current + j].Contains("x") || parsedSchedule[current + j].Contains("wypł") || parsedSchedule[current + j].Equals("0"))
                        {
                            schedule.Tracks = parsedSchedule[current + j].Split(',', 'i').Select(x => x.Trim().Replace("m", string.Empty).Replace("wypł", "shallow")).ToList();
                        }
                        date = date.AddDays(1);
                    }
                }

                var mergedSchedules = MergeSchedule(scheduleList, exitTime);

                await poolsContext.Schedules.AddRangeAsync(mergedSchedules);
                await poolsContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }

        private List<int> LoadHours(List<string> parsedSchedule)
        {
            List<int> hours = new List<int>();
            for (int i = 0; i < parsedSchedule.Count(); i++)
            {
                string time = parsedSchedule[i];

                if (IsValidTimeFormat(time))
                {
                    hours.Add(i);
                }
            }
            return hours;
        }

        private List<Schedule> MergeSchedule(List<Schedule> poolSchedule, string exitTime)
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
                        mergedSchedule.EndTime = exitTime;
                    }

                    if (mergedSchedule.Day != schedule.Day || !mergedSchedule.Tracks.SequenceEqual(schedule.Tracks))
                    {
                        mergedSchedule = new Schedule() { Day = schedule.Day, StartTime = schedule.StartTime, Tracks = new List<string>(schedule.Tracks), PoolId = schedule.PoolId };
                    }
                }
                else
                {
                    mergedSchedule = new Schedule() { Day = schedule.Day, StartTime = schedule.StartTime, Tracks = new List<string>(schedule.Tracks), PoolId = schedule.PoolId };
                }
            }

            if (TimeSpan.Parse(mergedSchedules.Last().EndTime) < TimeSpan.Parse(exitTime))
            {
                mergedSchedules.Last().EndTime = exitTime;
            }

            return mergedSchedules;
        }

        public bool IsValidTimeFormat(string input)
        {
            if (input.Length < 5)
            {
                return false;
            }
            DateTime dummyOutput;
            return DateTime.TryParse(input, out dummyOutput);
        }

        public async Task<bool> DownloadFile(string remoteFile, string localFileName)
        {
            return await fileService.DownloadFile(remoteFile, localFileName);
        }
    }
}

public interface IImportPoolSchedule
{
    Task ImportSchedule(string poolName, DateTime startDate, DateTime endDate, string filePath);
}
