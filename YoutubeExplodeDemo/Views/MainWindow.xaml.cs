namespace YoutubeExplodeDemo.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => Locator.Cleanup();
        }
    }
}