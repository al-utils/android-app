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
        // username
        // completed but current
        // full schedule
        
        public string BuildActivityQuery(int page)
        {
            var s = $@"query {{
  Page(page: {page}, perPage: 50) {{
    activities(userId: {userId}, sort: ID_DESC) {{
      __typename
      ... on ListActivity{{
        id
        status
        progress
        createdAt
        media {{
          id
          coverImage {{
            extraLarge
          }}
          title {{
            romaji
            english
            native
          }}
        }}
        likes {{
          id
        }}
      }}
    }}
  }}
}}
";
            return s;
        }

        private static readonly int activityPageNum = 1;
        private async Task<List<Activity>> GetActivityData()
        {
            //Debug.WriteLine("woeifjwofiej");
            Response data = await Request.RequestDataAsync(BuildActivityQuery(activityPageNum), new Dictionary<string, object>());
            //Debug.WriteLine("woeifjwofiej");
            Debug.WriteLine(data);
            return data.Data.Page.Activities;
        }

        private ObservableCollection<Activity> ActivityList { get; set; } = new ObservableCollection<Activity>();
        public async Task CreateActivities()
        {
            List<Activity> activities = await GetActivityData();
            Debug.WriteLine("EOIFJEOIFj");
            Debug.WriteLine(activities.Count);
            foreach (Activity activity in activities)
            {
                Debug.WriteLine(activity.Id);
            }
            activityList.ItemsSource = activities;
            // banner.Source = user.BannerURL;
            // about.Text = user.About;
        }

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
        public MainPage(string user="")
        {
            Console.Out.WriteLine("EF:OEIFJ:EOIFJE:OFIJE:OFj");
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


            if (user != "")
                //{
                //    // search for user
                //    DisplaySearchUserPrompt(true);

                //}
                currentUser = user;

            viewModel = new MainPageViewModel(this);
            releasedGrid.BindingContext = viewModel;
            notYetReleasedGrid.BindingContext = viewModel;

            CreateActivities();
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

            Console.Out.WriteLine(jsonString);

            var response = await client.PostAsync(URL, new StringContent(jsonString, Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("Error", "User not Found: " + input, "Retry");
                await DisplaySearchUserPrompt();
                return;
            }
            jsonString = await response.Content.ReadAsStringAsync();

            Console.Out.WriteLine(jsonString);

            Response data = JsonSerializer.Deserialize<Response>(jsonString);
            User user = data.Data.User;
            //currentUser = user.Name;
            //Preferences.Set("currentUser", currentUser);

            // regenerate chart
            //await CreateCards();
            await Navigation.PushAsync(new MainPage(user.Name));
        }

        private async void menuIcon_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayActionSheet("Menu", "Cancel", null, "Search User", "Search Media", "Settings");
            Console.Out.WriteLine(result);
            switch (result)
            {
                case "Cancel":
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
