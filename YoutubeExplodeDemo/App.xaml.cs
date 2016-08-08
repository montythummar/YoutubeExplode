// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplodeDemo>
//  File: <App.xaml.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using GalaSoft.MvvmLight.Threading;

namespace YoutubeExplodeDemo
{
    public partial class App
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }
    }
}