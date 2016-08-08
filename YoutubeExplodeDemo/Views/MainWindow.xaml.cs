// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplodeDemo>
//  File: <MainWindow.xaml.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System.Windows.Controls;
using System.Windows.Data;
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
        }

        private void tbVideoID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            var tBox = (TextBox) sender;
            BindingOperations.GetBindingExpression(tBox, TextBox.TextProperty)?.UpdateSource();
            ((MainViewModel) DataContext).SubmitCommand.Execute(null);
        }
    }
}