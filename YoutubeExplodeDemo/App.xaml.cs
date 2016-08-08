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
