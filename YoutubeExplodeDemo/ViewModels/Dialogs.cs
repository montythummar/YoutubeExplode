// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplodeDemo>
//  File: <Dialogs.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System.Windows;
using GalaSoft.MvvmLight.Threading;

namespace YoutubeExplodeDemo.ViewModels
{
    public static class Dialogs
    {
        /// <summary>
        /// Shows an error dialog
        /// </summary>
        public static void Error(string message)
        {
            DispatcherHelper.UIDispatcher.Invoke(
                () => MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error));
        }

        /// <summary>
        /// Prompts the user with a given message, returns true if user pressed Yes
        /// </summary>
        public static bool PromptYesNo(string message)
        {
            return DispatcherHelper.UIDispatcher.Invoke(() =>
                MessageBox.Show(message, "Prompt", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                MessageBoxResult.Yes);
        }

        /// <summary>
        /// Prompts the user with a given message, returns true if user pressed Yes
        /// </summary>
        public static bool ErrorPromptYesNo(string message)
        {
            return DispatcherHelper.UIDispatcher.Invoke(() =>
                MessageBox.Show(message, "Error", MessageBoxButton.YesNo, MessageBoxImage.Error) ==
                MessageBoxResult.Yes);
        }

        /// <summary>
        /// Shows a notification popup
        /// </summary>
        public static void Notification(string message)
        {
            DispatcherHelper.UIDispatcher.Invoke(
                () => MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information));
        }
    }
}