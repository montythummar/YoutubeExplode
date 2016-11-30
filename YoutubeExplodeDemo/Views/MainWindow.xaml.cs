// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplodeDemo>
//  File: <MainWindow.xaml.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using YoutubeExplodeDemo.ViewModels;

namespace YoutubeExplodeDemo.Views
{
    public partial class MainWindow
    {
        private readonly DispatcherTimer _positionTimer;
        private bool _isPlaying;

        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => Locator.Cleanup();

            ((MainViewModel) DataContext).PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(MainViewModel.SelectedStream) && _isPlaying)
                    Stop();
            };

            // Sorry WPF gods but MediaElement sucks
            _positionTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(0.5)};
            _positionTimer.Tick += (sender, args) =>
            {
                if (!VideoMediaElement.NaturalDuration.HasTimeSpan) return;
                VideoPositionSlider.Value = VideoMediaElement.Position.TotalMilliseconds/
                                            VideoMediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
                if (VideoMediaElement.Position >= VideoMediaElement.NaturalDuration)
                    Stop();
            };
        }

        private void Play()
        {
            VideoMediaElement.Play();
            _positionTimer.Start();
            _isPlaying = true;
        }

        private void Pause()
        {
            VideoMediaElement.Pause();
            _positionTimer.Stop();
        }

        private void Stop()
        {
            VideoMediaElement.Position = TimeSpan.Zero;
            VideoMediaElement.Stop();
            _positionTimer.Stop();
            VideoPositionSlider.Value = 0;
            _isPlaying = false;
        }

        private void tbVideoID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            ((MainViewModel) DataContext).SubmitCommand.Execute(null);
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

        private void VideoPositionSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (!VideoMediaElement.NaturalDuration.HasTimeSpan) return;
            var newPos =
                TimeSpan.FromMilliseconds(VideoPositionSlider.Value*VideoMediaElement.NaturalDuration.TimeSpan.TotalMilliseconds);
            VideoMediaElement.Position = newPos;
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }
    }
}