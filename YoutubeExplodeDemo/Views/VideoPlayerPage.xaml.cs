// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplodeDemo>
//  File: <VideoPlayerPage.xaml.cs>
//  Created By: Alexey Golub
//  Date: 13/01/2017
// ------------------------------------------------------------------ 

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using YoutubeExplodeDemo.ViewModels;

namespace YoutubeExplodeDemo.Views
{
    public partial class VideoPlayerPage
    {
        private readonly DispatcherTimer _syncTimer;
        private bool _freezePositionSlider;

        public VideoPlayerPage()
        {
            InitializeComponent();

            ((MainViewModel) DataContext).PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(MainViewModel.SelectedStream) && VideoMediaElement.IsPlaying)
                    Stop();
            };

            _syncTimer = new DispatcherTimer();
            _syncTimer.Interval = TimeSpan.FromSeconds(0.5);
            _syncTimer.Tick += (sender, args) =>
            {
                if (_freezePositionSlider) return;
                if (VideoMediaElement.NaturalDuration <= 0) return;
                VideoPositionSlider.Value =
                    (double) VideoMediaElement.Position/VideoMediaElement.NaturalDuration;
            };
        }

        private void Play()
        {
            VideoMediaElement.Play();
            _syncTimer.Start();
        }

        private void Pause()
        {
            VideoMediaElement.Pause();
            _syncTimer.Stop();
        }

        private void Stop()
        {
            VideoMediaElement.Stop();
            _syncTimer.Stop();
            VideoPositionSlider.Value = 0;
        }

        private void PlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            Play();
        }

        private void PauseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Pause();
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void VideoPositionSlider_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _freezePositionSlider = true;
        }

        private void VideoPositionSlider_PreviewMouseLeftButtonUp(object sender,
            MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (VideoMediaElement.NaturalDuration <= 0) return;
            VideoMediaElement.Position = (decimal) (VideoPositionSlider.Value*VideoMediaElement.NaturalDuration);
            _freezePositionSlider = false;
        }
    }
}