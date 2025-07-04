using al_utils_app.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace al_utils_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        private ObservableCollection<MediaDetails> Hidden { get; set; } = new ObservableCollection<MediaDetails>();
        public SettingsPage()
        {
            InitializeComponent();
            backIcon.Source = ImageSource.FromResource("al_utils_app.Images.arrow-left.png");

            hiddenList.ItemsSource = Hidden;
        }

        private async void backIcon_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}