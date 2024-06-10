using Timer = System.Timers.Timer;


namespace Webview
{
    public partial class App : Application
    {
        Timer IdleTimer = new Timer(10 * 10  * 1000);
        public App()
        {
            InitializeComponent();
            IdleTimer.Elapsed += Idletimer_Elapsed;
            IdleTimer.Start();
            MainPage = new NavigationPage(new MainPage());
        }

        public void ResetIdleTimer()
        {
            IdleTimer.Stop();
            IdleTimer.Start();
        }

        async void Idletimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (MainThread.IsMainThread)
            {
                if (Application.Current.MainPage.Navigation.NavigationStack.Count > 0)
                {
                    await Application.Current.MainPage.Navigation.PopToRootAsync();
                }
            }
            else
            {
                if (Application.Current.MainPage.Navigation.NavigationStack.Count > 0)
                {
                    MainThread.BeginInvokeOnMainThread(async () => await Application.Current.MainPage.Navigation.PopToRootAsync());
                }
            }
        }
    }
}
