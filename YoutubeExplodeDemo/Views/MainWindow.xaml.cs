// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplodeDemo>
//  File: <MainWindow.xaml.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Navigation;
using YoutubeExplodeDemo.ViewModels;

namespace YoutubeExplodeDemo.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => Locator.Cleanup();
        }

        private void tbVideoID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            ((MainViewModel) DataContext).GetDataCommand.Execute(null);
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
            e.Handled = true;
        }
    }
}