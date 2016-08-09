// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplodeDemo>
//  File: <FileDownloaderService.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System;
using System.IO;
using System.Net;
using NegativeLayer.Extensions;

namespace YoutubeExplodeDemo.Services
{
    public class FileDownloaderService
    {
        private double _progress;
        public double Progress
        {
            get { return _progress; }
            private set
            {
                _progress = value;
                ProgressChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler ProgressChanged;

        private HttpWebRequest CreateGenericRequest(Uri uri)
        {
            var request = (HttpWebRequest) WebRequest.Create(uri);
            request.Method = "GET";
            return request;
        }

        private void PerformRequest(WebRequest request, Stream outputStream)
        {
            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream == null) return;

                // Get file size
                int fileSize = int.Parse(response.Headers["Content-Length"]);

                // Read the response and copy it to output stream
                var buffer = new byte[1024];
                int bytesRead;
                do
                {
                    bytesRead = responseStream.Read(buffer, 0, 1024);
                    outputStream.Write(buffer, 0, bytesRead);
                    Progress += 1.0*bytesRead/fileSize;
                } while (bytesRead > 0);

                outputStream.Seek(0, SeekOrigin.Begin);
            }
        }

        public void DownloadFile(string url, string destinationFilePath)
        {
            using (var outputStream = File.Create(destinationFilePath))
                PerformRequest(CreateGenericRequest(url.ToUri()), outputStream);
        }
    }
}