// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplodeDemo>
//  File: <Locator.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using YoutubeExplodeDemo.ViewModels;
using YoutubeExplodeDemo.ViewModels.Interfaces;

namespace YoutubeExplodeDemo
{
    public class Locator
    {
        static Locator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<IMainViewModel, MainViewModel>();
        }

        public static T Resolve<T>() => ServiceLocator.Current.GetInstance<T>();
        public static T Resolve<T>(string id) => ServiceLocator.Current.GetInstance<T>(id);

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }

        public IMainViewModel Main => Resolve<IMainViewModel>();
    }
}