using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using al_utils_app;

namespace al_utils_app
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AndroidNavigationPage(new MainPage());
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
