// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplodeDemo>
//  File: <MainViewModel.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using NegativeLayer.Extensions;
using YoutubeExplode;
using YoutubeExplode.Models;
using YoutubeExplodeDemo.Services;

namespace YoutubeExplodeDemo.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly FileDownloaderService _downloader;

        private string _videoID;
        public string VideoID
        {
            get { return _videoID; }
            set { Set(ref _videoID, value); }
        }

        private VideoInfo _videoInfo;
        public VideoInfo VideoInfo
        {
            get { return _videoInfo; }
            private set
            {
                Set(ref _videoInfo, value);
                RaisePropertyChanged(() => VideoInfoVisible);
            }
        }

        private ImageSource _thumbnailImageSource;
        public ImageSource ThumbnailImageSource
        {
            get { return _thumbnailImageSource; }
            private set { Set(ref _thumbnailImageSource, value); }
        }

        public bool VideoInfoVisible => VideoInfo != null;

        private VideoStreamEndpoint _selectedStream;
        public VideoStreamEndpoint SelectedStream
        {
            get { return _selectedStream; }
            set { Set(ref _selectedStream, value); }
        }

        // Commands
        public RelayCommand SubmitCommand { get; }
        public RelayCommand DownloadVideoCommand { get; }

        public MainViewModel(FileDownloaderService downloader)
        {
            _downloader = downloader;

            // Commands
            SubmitCommand = new RelayCommand(Submit);
            DownloadVideoCommand = new RelayCommand(DownloadVideo);
        }

        private async void Submit()
        {
            // Check params
            if (VideoID.IsBlank())
                return;

            // Do the heavy lifting async
            await Task.Run(() =>
            {
                try
                {
                    // Get video id from url if necessary
                    string id = VideoID;
                    var match = Regex.Match(VideoID, @"v=(.+?)\b");
                    if (match.Success)
                        id = match.Groups[1].Value;

                    // Populate video info
                    VideoInfo = Youtube.GetVideoInfo(id);
                }
                catch (Exception ex)
                {
                    VideoInfo = null;
                    Dialogs.Error(ex.Message);
                }
            });

            // Thumbnail
            if (VideoInfo?.ThumbnailURL != null)
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(VideoInfo.ThumbnailURL, UriKind.Absolute);
                bmp.EndInit();
                ThumbnailImageSource = bmp;
            }

            // Selected stream
            if (VideoInfo?.Streams != null && VideoInfo.Streams.Any())
            {
                SelectedStream = VideoInfo.Streams.First();
            }
        }

        private async void DownloadVideo()
        {
            // Check params
            if (SelectedStream == null) return;
            if (VideoInfo == null) return;

            // Copy id and title
            string id = VideoInfo.ID;
            string title = VideoInfo.Title;

            // Figure out the file type
            string extension = "mp4";
            if (SelectedStream.Type.ContainsInvariant("webm"))
                extension = "webm";
            else if (SelectedStream.Type.ContainsInvariant("3gpp"))
                extension = "3gpp";
            else if (SelectedStream.Type.ContainsInvariant("flv"))
                extension = "flv";

            // Select destination
            var sfd = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = extension,
                FileName = $"youtube_export_{id}.{extension}",
                Filter = $"{extension.ToUpperInvariant()} Video Files|*.{extension}"
            };
            if (!sfd.ShowDialog().GetValueOrDefault())
                return;
            string filePath = sfd.FileName;

            // Download
            bool success = true;
            await Task.Run(() =>
            {
                try
                {
                    _downloader.DownloadFile(SelectedStream.URL, filePath);
                }
                catch (Exception ex)
                {
                    Dialogs.Error(ex.Message);
                    success = false;
                }
            });

            // Notify
            if (success &&
                Dialogs.PromptYesNo($"Video (ID = {id}) downloaded!{Environment.NewLine}Do you want to open it?"))
                Process.Start(filePath);
        }
    }
}