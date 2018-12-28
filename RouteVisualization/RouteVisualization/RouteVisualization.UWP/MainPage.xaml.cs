
namespace RouteVisualization.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            Xamarin.FormsMaps.Init("88yPY0kZcOGX7RaKeHM8~ogAnZjvVpFAWmF1mTZUDZQ~AnnZzfuaCDOzl0HmlTs8aFZ9zjIgW8JFlm69BS6UUPsppWQgusCRU1C0VRk0wVHR");
            LoadApplication(new RouteVisualization.App());
            
        }
    }
}
