using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace al_utils_app
{
    public partial class ImagePage : ContentPage
    {
        public ImagePage(ImageSource url)
        {
            InitializeComponent();
            image.Source = url;
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
