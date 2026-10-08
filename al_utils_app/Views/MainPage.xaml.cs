using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Forms;
using static Xamarin.Essentials.Permissions;
using Xamarin.Forms.Shapes;
using Xamarin.Essentials;
using System.Windows.Input;
using MultiGestureViewPlugin;
using al_utils_app.Models;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Xamarin.CommunityToolkit.Effects;
using al_utils_app.ViewModels;

namespace al_utils_app.Views
{
    public partial class MainPage : TabbedPage
    {
        private const string URL = "https://graphql.anilist.co";
        private static readonly HttpClient client = new HttpClient();
        private const int numCols = 2;

        private string currentUser = Preferences.Get("currentUser", "");
        private int userId = Preferences.Get("userId", -1);

        // todo:
        // completed but current
        // full schedule
        // hide update
        


        private string forUser;
        public string ForUser
        {
            get { return forUser; }
            set
            {
                forUser = value;
                OnPropertyChanged(nameof(ForUser));
            }
        }

        private int releasingCount;
        public int ReleasingCount
        {
            get { return releasingCount; }
            set
            {
                releasingCount = value;
                OnPropertyChanged(nameof(ReleasingCount));
            }
        }

        private int notYetReleasedCount;
        public int NotYetReleasedCount
        {
            get { return notYetReleasedCount; }
            set
            {
                notYetReleasedCount = value;
                OnPropertyChanged(nameof(NotYetReleasedCount));
            }
        }

        private MainPageViewModel viewModel;
        public MainPage(User user = null)
        {
            InitializeComponent();
            BindingContext = this;
            Hidden.LoadHiddenList();
            //Hidden.Reset();

            menuIcon.Source = ImageSource.FromResource("al_utils_app.Images.menu.png");

            ICommand refreshCommand = new Command(() =>
            {
                //UpdateData(releasedGrid);
                refreshView.IsRefreshing = false;
            });
            ICommand refreshCommand2 = new Command(() =>
            {
                //UpdateData(notYetReleasedGrid);
                refreshView2.IsRefreshing = false;
            });
            ICommand refreshCommand3 = new Command(() =>
            {
                //UpdateData(notYetReleasedGrid);
                refreshView3.IsRefreshing = false;
            });
            refreshView.Command = refreshCommand;
            refreshView2.Command = refreshCommand2;
            refreshView3.Command = refreshCommand3;

            viewModel = new MainPageViewModel(this, user);
            releasedGrid.BindingContext = viewModel;
            notYetReleasedGrid.BindingContext = viewModel;
            activityList.BindingContext = viewModel;
        }

        internal async void LongPressMenu(MediaListEntry media)
        {
            var result = await DisplayActionSheet("Options", "Cancel", null, "Copy Title", "Hide");
            switch (result)
            {
                case "Cancel":
                    break;
                case "Copy Title":
                    await Clipboard.SetTextAsync(media.Details.Title.GetTitle);
                    break;
                case "Hide":
                    Hidden.Hide(media.Details.Id);
                    //RemoveCard(gestureView, g);
                    viewModel.RemoveMedia(media);
                    Hidden.SaveHiddenList();

                    break;
            }
        }

        protected override async void OnAppearing()
        {
            await viewModel.OnAppearing();

            // TODO: maybe update before appearing
            if (ReleasingCount != viewModel.ReleasingList.Count ||
                BindableLayout.GetItemsSource(releasedGrid) == null)
            {
                ReleasingCount = viewModel.ReleasingList.Count;
                BindableLayout.SetItemsSource(releasedGrid, viewModel.ReleasingList);
            }

            if (NotYetReleasedCount != viewModel.NotYetReleasedList.Count || 
                BindableLayout.GetItemsSource(notYetReleasedGrid) == null)
            {
                NotYetReleasedCount = viewModel.NotYetReleasedList.Count;
                BindableLayout.SetItemsSource(notYetReleasedGrid, viewModel.NotYetReleasedList);
            }

            activityList.ItemsSource = viewModel.ActivityList;
        }

        private int IsID(string s)
        {
            try
            {
                var x = Int32.Parse(s);
                return x;
            }
            catch
            {
                return -1;
            }
        }

        private async Task SearchUserByID(int ID)
        {
            var query = $@"query {{
  User (id: {ID}) {{
    id
    name
  }}
}}
";
            await SearchUser(query, "" + ID);
        }

        private async Task SearchUserByUsername(string username)
        {
            var query = $@"query {{
  User (name: ""{username}"") {{
    id
    name
  }}
}}
";
            await SearchUser(query, username);
        }

        private async Task SearchUser(string query, string input)
        {
            Dictionary<string, string> json = new Dictionary<string, string>();

            json.Add("query", query);
            string jsonString = JsonSerializer.Serialize(json);

            var response = await client.PostAsync(URL, new StringContent(jsonString, Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("Error", "User not Found: " + input, "Retry");
                await DisplaySearchUserPrompt();
                return;
            }
            jsonString = await response.Content.ReadAsStringAsync();

            Debug.WriteLine(jsonString);

            Response data = JsonSerializer.Deserialize<Response>(jsonString);
            User user = data.Data.User;

            await Navigation.PushAsync(new MainPage(user));
        }

        private async void menuIcon_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayActionSheet("Menu", "Cancel", null, "Open User in WebView", "Search User", "Search Media", "Settings");
            switch (result)
            {
                case "Cancel":
                    break;
                case "Open User in WebView":
                    User user = viewModel.getUser();
                    if (user == null)
                        user = new User(currentUser, userId);
                    await Navigation.PushAsync(new WebViewPage("https://anilist.co/user/" + user.Name, user.Name + "'s Profile"));
                    break;
                case "Search User":
                    await DisplaySearchUserPrompt();
                    break;
                case "Search Media":
                    await Navigation.PushAsync(new SearchPage());
                    break;
                case "Settings":
                    await Navigation.PushAsync(new SettingsPage(this));
                    break;
            }
        }

        private async Task DisplaySearchUserPrompt(bool persist=false)
        {
            var result = await DisplayPromptAsync("Search for User", "Enter username or ID: ");
            if (persist && result == null && result != "")
            {
                await DisplaySearchUserPrompt(true);
            }
            else if (result != null && result != "")
            {
                var x = IsID(result);
                if (x != -1)
                {
                    await SearchUserByID(x);
                }
                else
                {
                    await SearchUserByUsername(result);
                }
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await DisplaySearchUserPrompt();
        }

        private void CollectionView_RemainingItemsThresholdReached(object sender, EventArgs e)
        {

        }

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            var id = (int)((TappedEventArgs)e).Parameter;
            await Navigation.PushAsync(new MediaPage(id, TypeEnum.Type.Anime));
        }
    }
}
