using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace al_utils_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebViewPage : ContentPage
    {
        public WebViewPage(string url)
        {
            BindingContext = this;
            InitializeComponent();
            backIcon.Source = ImageSource.FromResource("al_utils_app.Images.arrow-left.png");
            URL = url;
        }

        private string url;
        public string URL
        {
            get { return url; }
            set
            {
                url = value;
                OnPropertyChanged(nameof(URL));
            }
        }

        private void backIcon_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}