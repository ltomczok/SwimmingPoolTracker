using EasyNetQ;
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

namespace Kokosoft.SwimmingPoolTracker.ImportSchedule
{
    public class OnNewSchedule : IHostedService
    {
        IBus messageBus;
        private readonly ILogger<OnNewSchedule> logger;
        private readonly MongoClient mongoClient;

        public OnNewSchedule(IBus bus, ILogger<OnNewSchedule> logger, MongoClient mongoClient)
        {
            messageBus = bus;
            this.logger = logger;
            this.mongoClient = mongoClient;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            messageBus.Receive<NewSchedulle>("swimmingpooltracker", message => OnNewMessage(message));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            messageBus.Dispose();
            return Task.CompletedTask;
        }

        public async Task OnNewMessage(NewSchedulle newSchedulle)
        {
            try
            {
                logger.LogInformation($"New schedule: {newSchedulle.Link}");
                string fileName = newSchedulle.Link.Substring(newSchedulle.Link.LastIndexOf('/') + 1, (newSchedulle.Link.Length - newSchedulle.Link.LastIndexOf('/')) - 1);
                bool fileDownloaded = await DownloadFile(newSchedulle.Link, fileName);
                if (fileDownloaded)
                {
                    bool fileLoaded = await LoadSchedulle(newSchedulle, fileName);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "");
            }
        }

        public async Task<bool> LoadSchedulle(NewSchedulle newSchedulle, string fileName)
        {
            return await Task.Run<bool>(async () =>
              {

                  List<Schedule> poolSchedules = new List<Schedule>();
                  bool fileLoaded = true;
                  DateTime startDate = new DateTime(newSchedulle.YearFrom, newSchedulle.MonthFrom, newSchedulle.DayFrom);
                  DateTime endDate = new DateTime(newSchedulle.YearTo, newSchedulle.MonthTo, newSchedulle.DayTo);
                  int itterations = (endDate - startDate).Days;
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

                  List<string> pdfList = new List<string>();
                  pdfList.AddRange(extractedText.Split("\r\n"));

                  
                  }
                  //foreach (var item in pdfList)
                  //{
                  //    if (IsValidTimeFormat(item))
                  //    {
                  //        Schedule newSchedule = new Schedule()
                  //        {
                  //            Time = item
                  //        };
                  //        poolSchedules.Add(new Schedule());
                  //    };
                  //}
                  return false;
              });
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
            try
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(remoteFile);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    Stream contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                    FileStream fileStream = new FileStream(localFileName, FileMode.Create);
                    await contentStream.CopyToAsync(fileStream);
                    fileStream.Close();
                }
                httpResponseMessage.Dispose();
                httpClient.Dispose();
                return fileDownloaded;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Błąd podczas pobierania pliku PDF");
            }
            return fileDownloaded;
        }
    }

    public class Schedule
    {
        public string Time { get; set; }
        public List<string> WorkingHours { get; set; } = new List<string>();
        //public DateTime Date { get; set; }
        //public string Time { get; set; }
        //public string WorkingHours { get; set; }
    }
    class MyClass
    {
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string WorkingHours { get; set; }
    }
}
//try
//{
//    //Load the PDF document.
//    FileStream docStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

//    //Load the PDF document.
//    PdfLoadedDocument loadedDocument = new PdfLoadedDocument(docStream);

//    // Loading page collections
//    PdfLoadedPageCollection loadedPages = loadedDocument.Pages;

//    string extractedText = string.Empty;

//    // Extract text from existing PDF document pages
//    foreach (PdfLoadedPage loadedPage in loadedPages)
//    {
//        extractedText += loadedPage.ExtractText();
//    }
//    //Close the document.
//    loadedDocument.Close(true);
//    List<Scheduler> poolSchedule = new List<Scheduler>();
//    List<int> schedulers = new List<int>();
//    List<string> pdfList = new List<string>();
//    pdfList.AddRange(extractedText.Split("\r\n"));

//    for (int i = 0; i < pdfList.Count(); i++)
//    {
//        string time = pdfList[i];

//        if (time.Count() > 4 && IsValidTimeFormat(time))
//        {
//            schedulers.Add(i);
//        }

//    }

//    for (int i = 0; i < schedulers.Count; i++)
//    {
//        Scheduler schedul = new Scheduler();
//        poolSchedule.Add(schedul);
//        int current = schedulers[i];
//        int next;
//        int iterations = 7;
//        if (current != schedulers.Last())
//        {
//            next = schedulers[i + 1];
//            if ((next - current - 1) < 7)
//            {
//                iterations = next - current - 1;
//            }
//        }
//        else
//        {
//            iterations = (pdfList.Count - current) - 1;
//        }

//        schedul.Time = pdfList[current];
//        for (int j = 1; j <= iterations; j++)
//        {
//            var hours = pdfList[current + j];
//            schedul.WorkingHours.Add(hours);
//        }
//    }
//    //var collectionName = "PoolTime";
//    //var database = mongoClient.GetDatabase("SwimmingPoolTracker");
//    //var collection = database.GetCollection<Scheduler>(collectionName);
//    //await collection.InsertManyAsync(poolSchedule);


//    foreach (var item in poolSchedule)
//    {
//    }
//    return fileLoaded;
//}
//catch (Exception ex)
//{
//    fileLoaded = false;
//    logger.LogError(ex, "");
//}
//return fileLoaded;
//});
