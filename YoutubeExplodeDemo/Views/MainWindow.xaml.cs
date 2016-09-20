// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplodeDemo>
//  File: <MainWindow.xaml.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using YoutubeExplodeDemo.ViewModels;

namespace YoutubeExplodeDemo.Views
{
    public partial class MainWindow
    {
        private readonly DispatcherTimer _positionTimer;

        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => Locator.Cleanup();

            // Sorry WPF gods but MediaElement sucks
            _positionTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(0.5)};
            _positionTimer.Tick += (sender, args) =>
            {
                if (!VideoMediaElement.NaturalDuration.HasTimeSpan) return;
                VideoPositionSlider.Value = VideoMediaElement.Position.TotalMilliseconds/
                                            VideoMediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
            };
        }

        private void tbVideoID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            ((MainViewModel) DataContext).SubmitCommand.Execute(null);
        }

        private void PlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            VideoMediaElement.Play();
            _positionTimer.Start();
        }

        private void PauseButton_OnClick(object sender, RoutedEventArgs e)
        {
            VideoMediaElement.Pause();
            _positionTimer.Start();
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            VideoMediaElement.Stop();
            _positionTimer.Stop();
            VideoPositionSlider.Value = 0;
        }

        private void VideoPositionSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var newPos =
                TimeSpan.FromMilliseconds(VideoPositionSlider.Value*VideoMediaElement.NaturalDuration.TimeSpan.TotalMilliseconds);
            VideoMediaElement.Position = newPos;
        }
    }
}