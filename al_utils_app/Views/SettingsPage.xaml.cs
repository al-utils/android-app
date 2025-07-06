using al_utils_app.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using al_utils_app;
using System.Text.Json;

namespace al_utils_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        private ObservableCollection<MediaDetails> HiddenList { get; set; } = new ObservableCollection<MediaDetails>();
        private MainPage home;

        private string BuildQuery()
        {
            var s = "query {";
            var i = 1;
            foreach (var id in Hidden.GetHiddenIds())
            {
                s += $@"
  media{i}: Media(id: {id}) {{
    id
    title {{
      romaji
      english
      native
    }}
  }}
";
                i++;
            }
            return s + "}";
        }

        public SettingsPage(MainPage home)
        {
            this.home = home;
            InitializeComponent();
            backIcon.Source = ImageSource.FromResource("al_utils_app.Images.arrow-left.png");

            LoadHiddenList();
        }

        private ImageSource deleteImageURL;
        public ImageSource DeleteImageURL
        {
            get { return deleteImageURL; }
            set
            {
                deleteImageURL = value;
                OnPropertyChanged(nameof(DeleteImageURL));
            }
        }

        private async void LoadHiddenList()
        {
            if (Hidden.GetHiddenIds().Count == 0)
                return;

            // request data for each id in list
            Response data = await Request.RequestDataAsync(BuildQuery(), new Dictionary<string, object>());

            var dict = data.Data.Pages;
            List<MediaDetails> mediaList = new List<MediaDetails>();

            // combine into one list
            foreach (KeyValuePair<string, object> page in dict)
            {
                var jsonElement = page.Value;
                var jsonString2 = jsonElement.ToString();
                MediaDetails data2 = JsonSerializer.Deserialize<MediaDetails>(jsonString2);
                HiddenList.Add(data2);
            }

            hiddenList.ItemsSource = HiddenList;
        }


        private async void backIcon_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var id = (int)((TappedEventArgs)e).Parameter;
            await Navigation.PushAsync(new MediaPage(id));
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            // remove
            var id = (int)((ImageButton)sender).BindingContext;
            Hidden.Show(id);
            Hidden.SaveHiddenList();

            // update observable collection
            HiddenList = new ObservableCollection<MediaDetails>(HiddenList.Where(x => x.Id != id));
            hiddenList.ItemsSource = HiddenList;

            // reload mainpage
            home.CreateCards();
        }
    }
}