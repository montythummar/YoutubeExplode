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
        private HttpWebRequest CreateGenericRequest(Uri uri)
        {
            var request = (HttpWebRequest) WebRequest.Create(uri);
            request.Method = "GET";
            return request;
        }

        private MemoryStream PerformRequest(HttpWebRequest request)
        {
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream == null) return null;

                var outputStream = new MemoryStream();
                var buffer = new byte[1024];
                int bytesRead;
                do
                {
                    bytesRead = responseStream.Read(buffer, 0, 1024);
                    outputStream.Write(buffer, 0, bytesRead);
                } while (bytesRead > 0);

                outputStream.Seek(0, SeekOrigin.Begin);
                return outputStream;
            }
        }

        public void DownloadFile(string url, string destinationFilePath)
        {
            using (var inputStream = PerformRequest(CreateGenericRequest(url.ToUri())))
            using (var outputStream = new FileStream(destinationFilePath, FileMode.CreateNew))
                inputStream.CopyTo(outputStream);
        }
    }
}