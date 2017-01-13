// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplodeDemo>
//  File: <MainWindow.xaml.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System.Windows;
using System.Windows.Input;
using YoutubeExplodeDemo.ViewModels;

namespace YoutubeExplodeDemo.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => Locator.Cleanup();

            NavigateVideoPlayerButton.IsEnabled = false;
        }

        private void tbVideoID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            ((MainViewModel) DataContext).GetDataCommand.Execute(null);
        }

        private void NavigateVideoPlayerButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigateVideoPlayerButton.IsEnabled = false;
            NavigateVideoDataButton.IsEnabled = true;

            ContentFrame.Navigate(new VideoPlayerPage());
        }

        private void NavigateVideoDataButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigateVideoPlayerButton.IsEnabled = true;
            NavigateVideoDataButton.IsEnabled = false;

            ContentFrame.Navigate(new VideoDataPage());
        }
    }
}