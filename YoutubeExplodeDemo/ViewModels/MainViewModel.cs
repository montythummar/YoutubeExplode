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
        private readonly YoutubeClient _client;

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
            set
            {
                Set(ref _selectedStream, value);
                DownloadVideoCommand.RaiseCanExecuteChanged();
            }
        }

        private double _downloadProgress;
        public double DownloadProgress
        {
            get { return _downloadProgress;}
            set { Set(ref _downloadProgress, value); }
        }

        private bool _isDownloading;
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
        public RelayCommand SubmitCommand { get; }
        public RelayCommand DownloadVideoCommand { get; }

        public MainViewModel(FileDownloaderService downloader)
        {
            _downloader = downloader;
            _client = new YoutubeClient();

            // Commands
            SubmitCommand = new RelayCommand(SubmitAsync);
            DownloadVideoCommand = new RelayCommand(DownloadVideoAsync, () => SelectedStream != null && !IsDownloading);

            // Events
            _downloader.ProgressChanged += (sender, args) => DownloadProgress = _downloader.Progress;
        }

        private async void SubmitAsync()
        {
            // Check params
            if (VideoID.IsBlank())
                return;

            // Do the heavy lifting async
            await Task.Run(() =>
            {
                try
                {
                    string id;

                    // Parse URL if necessary
                    if (!_client.TryParseVideoID(VideoID, out id))
                        id = VideoID;

                    // Populate video info
                    VideoInfo = _client.GetVideoInfo(id);
                }
                catch (Exception ex)
                {
                    VideoInfo = null;
                    Dialogs.Error(ex.Message);
                }
            });

            // Thumbnail
            if (VideoInfo?.ImageHighQuality != null)
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(VideoInfo.ImageHighQuality, UriKind.Absolute);
                bmp.EndInit();
                ThumbnailImageSource = bmp;
            }

            // Selected stream
            if (VideoInfo?.Streams != null && VideoInfo.Streams.Any())
            {
                SelectedStream = VideoInfo.Streams.First();
            }
        }

        private async void DownloadVideoAsync()
        {
            // Check params
            if (SelectedStream == null) return;
            if (VideoInfo == null) return;

            IsDownloading = true;

            // Copy values
            string id = VideoInfo.ID;
            string ext = SelectedStream.FileExtension;

            // Select destination
            var sfd = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ext,
                FileName = $"youtube_export_{id}.{ext}",
                Filter = $"{ext.ToUpperInvariant()} Video Files|*.{ext}|All files|*.*"
            };
            if (!sfd.ShowDialog().GetValueOrDefault())
            {
                IsDownloading = false;
                return;
            }
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

            IsDownloading = false;
        }
    }
}