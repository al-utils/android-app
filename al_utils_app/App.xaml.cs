using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using al_utils_app;
using al_utils_app.Views;

namespace al_utils_app
{
    public partial class App : Application
    {
        public static int ScreenWidth;
        public static int ScreenHeight;
        public App()
        {
            InitializeComponent();

            if (Authentication.IsAuthenticated())
                MainPage = new AndroidNavigationPage(new MainPage());
            else
                MainPage = new AndroidNavigationPage(new AuthenticatePage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
