// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplodeDemo>
//  File: <MainViewModel.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System;
using System.Diagnostics;
using System.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using Tyrrrz.Extensions;
using YoutubeExplode;
using YoutubeExplode.Models;

namespace YoutubeExplodeDemo.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly YoutubeClient _client;

        private string _videoId;
        private VideoInfo _videoInfo;
        private double _downloadProgress;
        private bool _isDownloading;

        public string VideoId
        {
            get { return _videoId; }
            set { Set(ref _videoId, value); }
        }

        public VideoInfo VideoInfo
        {
            get { return _videoInfo; }
            private set
            {
                Set(ref _videoInfo, value);
                RaisePropertyChanged(() => VideoInfoVisible);
            }
        }

        public bool VideoInfoVisible => VideoInfo != null;

        public double DownloadProgress
        {
            get { return _downloadProgress; }
            set { Set(ref _downloadProgress, value); }
        }

        public bool IsDownloading
        {
            get { return _isDownloading; }
            set
            {
                Set(ref _isDownloading, value);
                DownloadVideoCommand.RaiseCanExecuteChanged();
            }
        }

        // Commands
        public RelayCommand GetDataCommand { get; }
        public RelayCommand<VideoStream> OpenVideoCommand { get; }
        public RelayCommand<VideoStream> DownloadVideoCommand { get; }

        public MainViewModel()
        {
            _client = new YoutubeClient();

            // Commands
            GetDataCommand = new RelayCommand(GetDataAsync);
            OpenVideoCommand = new RelayCommand<VideoStream>(vse => Process.Start(vse.Url));
            DownloadVideoCommand = new RelayCommand<VideoStream>(DownloadVideoAsync, vse => !IsDownloading);
        }

        private async void GetDataAsync()
        {
            // Check params
            if (VideoId.IsBlank())
                return;

            // Reset data
            VideoInfo = null;

            // Parse URL if necessary
            string id;
            if (!YoutubeClient.TryParseVideoId(VideoId, out id))
                id = VideoId;

            // Perform the request
            try
            {
                VideoInfo = await _client.GetVideoInfoAsync(id);
            }
            catch (Exception ex)
            {
                Dialogs.Error(ex.Message);
            }
        }

        private async void DownloadVideoAsync(VideoStream videoStream)
        {
            // Check params
            if (videoStream == null) return;
            if (VideoInfo == null) return;

            // Copy values
            string title = VideoInfo.Title;
            string ext = videoStream.FileExtension;

            // Select destination
            var sfd = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ext,
                FileName = $"{title}.{ext}".Without(Path.GetInvalidFileNameChars()),
                Filter = $"{ext.ToUpperInvariant()} Video Files|*.{ext}|All files|*.*"
            };
            if (sfd.ShowDialog() == false) return;
            string filePath = sfd.FileName;

            // Try download
            IsDownloading = true;
            DownloadProgress = 0;
            try
            {
                using (var output = File.Create(filePath))
                using (var input = await _client.DownloadVideoAsync(videoStream))
                {
                    // Read the response and copy it to output stream
                    var buffer = new byte[1024];
                    int bytesRead;
                    do
                    {
                        bytesRead = await input.ReadAsync(buffer, 0, buffer.Length);
                        await output.WriteAsync(buffer, 0, bytesRead);

                        if (videoStream.FileSize > 0)
                            DownloadProgress += 1.0*bytesRead/videoStream.FileSize;
                    } while (bytesRead > 0);
                }

                // Prompt to open
                if (Dialogs.PromptYesNo(
                    $"Video ({title}) has been downloaded!{Environment.NewLine}Do you want to open it?"))
                    Process.Start(filePath);
            }
            catch (Exception ex)
            {
                Dialogs.Error(ex.Message);
            }
            IsDownloading = false;
        }
    }
}