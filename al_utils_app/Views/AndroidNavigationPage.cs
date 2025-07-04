using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific.AppCompat;

namespace al_utils_app.Views
{
    class AndroidNavigationPage : Xamarin.Forms.NavigationPage
    {
        public AndroidNavigationPage(Page page)
        {
            On<Android>().SetBarHeight(160); //set the navigation bar height
            //BarBackgroundColor = Color.FromHex("#141414");
            BarBackgroundColor = Color.FromHex("#262524");
            PushAsync(page);
        }
    }
}
