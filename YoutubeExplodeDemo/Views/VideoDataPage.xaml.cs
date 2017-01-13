// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplodeDemo>
//  File: <VideoPlayerData.xaml.cs>
//  Created By: Alexey Golub
//  Date: 13/01/2017
// ------------------------------------------------------------------ 

using System.Diagnostics;
using System.Windows.Navigation;

namespace YoutubeExplodeDemo.Views
{
    public partial class VideoDataPage
    {
        public VideoDataPage()
        {
            InitializeComponent();
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
            e.Handled = true;
        }
    }
}
