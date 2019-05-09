using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kokosoft.SwimmingPoolTracker.ImportSchedule
{
    public class DownloadFileService
    {
        private readonly ILogger<DownloadFileService> logger;
        public HttpClient Client { get; }

        public DownloadFileService(HttpClient client, ILogger<DownloadFileService> logger)
        {
            Client = client;
            this.logger = logger;
        }

        public async Task<bool> DownloadFile(string remoteFile, string localFileName)
        {
            logger.LogInformation($"Start downloading the file {remoteFile}.");
            bool fileDownloaded = false;
            try
            {
                var httpResponseMessage = await Client.GetAsync(remoteFile);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    Stream contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                    FileStream fileStream = new FileStream(localFileName, FileMode.Create);
                    await contentStream.CopyToAsync(fileStream);
                    fileStream.Close();
                    contentStream.Close();
                    fileDownloaded = true;
                }
                return fileDownloaded;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error during {remoteFile} file download.");
            }
            return fileDownloaded;
        }
    }
}
