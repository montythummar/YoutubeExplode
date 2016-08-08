// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplodeDemo>
//  File: <MainViewModel.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using NegativeLayer.Extensions;
using YoutubeExplode;
using YoutubeExplode.Models;

namespace YoutubeExplodeDemo.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
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
            set { Set(ref _thumbnailImageSource, value); }
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

        public MainViewModel()
        {
            // Commands
            SubmitCommand = new RelayCommand(Submit);
            DownloadVideoCommand = new RelayCommand(DownloadVideo, () => SelectedStream != null);
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

        private void DownloadVideo()
        {
        }
    }
}