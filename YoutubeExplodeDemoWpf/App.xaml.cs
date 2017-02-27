using GalaSoft.MvvmLight.Threading;

namespace YoutubeExplode.DemoWpf
{
    public partial class App
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }
    }
}