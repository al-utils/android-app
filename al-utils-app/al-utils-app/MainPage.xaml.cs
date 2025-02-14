﻿using System;
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

namespace al_utils_app
{
    public partial class MainPage : ContentPage
    {
        private const string URL = "https://graphql.anilist.co";
        private static readonly HttpClient client = new HttpClient();
        private const int numCols = 2;
        private const int maxPages = 10; // idk

        private string currentUser = Preferences.Get("currentUser", "");

        // todo:
        // username
        // options?
        
        public string BuildQuery()
        {
            var s = "query ($name: String) {";
            for (int i = 1; i <= maxPages; i++)
            {
                s += $@"
  page{i}: Page(page: {i}, perPage: 50) {{
    mediaList(userName: $name, status_in: [CURRENT, PLANNING], type: ANIME) {{
      progress
      media {{
        id
        title {{
          english
          romaji
        }}
        episodes
        coverImage {{
          extraLarge
        }}
        nextAiringEpisode {{
          id
          timeUntilAiring
          episode
        }}
      }}
    }}
  }}
";
            }
            return s + "}";
        }

        private async Task<List<Media>> GetData()
        {
            Dictionary<string, string> json = new Dictionary<string, string>();
            string query = BuildQuery();

            json.Add("query", query);
            Dictionary<string, string> variables = new Dictionary<string, string>();
            variables.Add("name", currentUser);
            json.Add("variables", JsonSerializer.Serialize(variables));
            string jsonString = JsonSerializer.Serialize(json);

            // request
            var response = await client.PostAsync(URL, new StringContent(jsonString, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            jsonString = await response.Content.ReadAsStringAsync();

            Response data = JsonSerializer.Deserialize<Response>(jsonString);

            var dict = data.Data.Pages;
            List<Media> mediaList = new List<Media>();

            // combine into one list
            foreach (KeyValuePair<string, object> page in dict)
            {
                var jsonElement = page.Value;
                var jsonString2 = jsonElement.ToString();
                ResponsePage data2 = JsonSerializer.Deserialize<ResponsePage>(jsonString2);
                mediaList.AddRange(data2.MediaList);
                //Console.Out.WriteLine(data2.MediaList.Count);
            }
            //Console.Out.WriteLine("MediaList Count: " + mediaList.Count);

            // filter
            mediaList = mediaList.Where(x => x.Details.Airing != null)
                                 .OrderBy(x => x.Details.Airing.TimeUntilAiring)
                                 .ToList();

            Console.Out.WriteLine("MediaList Count: " + mediaList.Count);
            return mediaList;
        }

        private async Task CreateCards()
        {
            List<Media> mediaList = await GetData();
            // clear grid
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            forUser.Text = "for @" + currentUser;

            var total = 0;
            foreach (Media media in mediaList)
            {
                var before = total;
                total += MakeCard(total, media);

                if (total % numCols == 0 && total != before)
                {
                    // add column
                    grid.RowDefinitions.Add(new RowDefinition() { Height = 320 });
                }
            }
        }

        private async Task UpdateData()
        {
            if (grid.Children.Count == 0)
            {
                await CreateCards();
                return;
            }

            List<Media> mediaList = await GetData();
            for (int i = 0; i < grid.Children.Count; i++)
            {
                Media media = mediaList[i];
                var timeUntilAiring = (int)media.Details.Airing.TimeUntilAiring;
                var nextAiringEpisode = media.Details.Airing.Episode;
                var progress = media.Progress;
                var episodes = "";
                if (media.Details.Episodes == null)
                    episodes = "?";
                else
                    episodes = "" + media.Details.Episodes;

                AbsoluteLayout card = (AbsoluteLayout)grid.Children[i];
                StackLayout details = (StackLayout)card.Children[2];
                StackLayout bottom = (StackLayout)details.Children[1];

                Label nextEpisode = (Label)bottom.Children[0];
                Ellipse ellipse = (Ellipse)bottom.Children[1];
                Label progressLabel = (Label)bottom.Children[2];

                if (progress < nextAiringEpisode - 1 && nextAiringEpisode > 1)
                    ellipse.IsVisible = true;
                else
                    ellipse.IsVisible = false;

                nextEpisode.Text = "Ep " + nextAiringEpisode + ": " + SecondsToString(timeUntilAiring);
                progressLabel.Text = "" + progress + "/" + episodes;
            }
        }

        private (int, int) IdToRowCol(int id)
        {
            int row = id / numCols;
            int col = id % numCols;
            return (row, col);
        }

        private Color IntColor(int r, int g, int b, int a=255)
        {
            double dr = ((double)r) / 255;
            double dg = ((double)g) / 255;
            double db = ((double)b) / 255;
            double da = ((double)a) / 255;
            return new Color(dr, dg, db, da);
        }

        private string SecondsToString(int s)
        {
            int d = s / (3600 * 24);
            int h = s % (3600 * 24) / 3600;
            return "" + d + "d" + h + "h";
        }

        private int MakeCard(int id, Media media)
        {
            if (media.Details.Airing == null)
                return 0;
            var timeUntilAiring = (int)media.Details.Airing.TimeUntilAiring;
            var nextAiringEpisode = media.Details.Airing.Episode;
            //Console.Out.WriteLine("" + timeUntilAiring + ", " + nextAiringEpisode);

            var progress = media.Progress;
            var mediaID = media.Details.Id;

            var title = "";
            if (media.Details.Title.English != null)
                title = media.Details.Title.English;
            else
                title = media.Details.Title.Romaji;

            var episodes = "";
            if (media.Details.Episodes == null)
                episodes = "?";
            else
                episodes = "" + media.Details.Episodes;
            var imageURL = media.Details.Image.ExtraLarge;
            
            AbsoluteLayout abs = new AbsoluteLayout();

            // tap events
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) =>
            {
                await Clipboard.SetTextAsync(title);
            };
            abs.GestureRecognizers.Add(tapGestureRecognizer);

            // background image
            Frame backgroundImg = new Frame()
            {
                Style = (Style)Application.Current.Resources["imageFrame"],
                Padding = 0
            };
            Image img = new Image()
            {
                Source = imageURL,
                Aspect = Aspect.AspectFill
            };
            backgroundImg.Content = img;
            abs.Children.Add(backgroundImg);

            // gradient
            Frame gradient = new Frame { Style = (Style)Application.Current.Resources["imageFrame"] };
            LinearGradientBrush grad = new LinearGradientBrush()
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1),
            };
            grad.GradientStops.Add(new GradientStop(Color.Transparent, 0));
            grad.GradientStops.Add(new GradientStop(Color.Transparent, 0));
            grad.GradientStops.Add(new GradientStop(Color.Transparent, 0));
            grad.GradientStops.Add(new GradientStop(IntColor(21, 21, 21, 144), 0));
            grad.GradientStops.Add(new GradientStop(IntColor(21, 21, 21), 0));
            gradient.Background = grad;
            abs.Children.Add(gradient);

            StackLayout details = new StackLayout
            {
                Padding = 17,
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.End,
                Style = (Style) Application.Current.Resources["otherFrame"]
            };
            Label titleLabel = new Label()
            {
                Text = title,
                FontSize = 18,
                TextColor = Color.White,
                FontFamily = "Jost",
                LineBreakMode = LineBreakMode.TailTruncation,
                MaxLines = 2
            };
            // titleLabel.GestureRecognizers.Add(tapGestureRecognizer);
            StackLayout bottom = new StackLayout() { Orientation = StackOrientation.Horizontal, Spacing = 0 };
            bottom.Children.Add(new Label()
            {
                Text = "Ep " + nextAiringEpisode + ": " + SecondsToString(timeUntilAiring),
                FontSize = 16,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                TextColor = Color.White,
                FontFamily = "Jost"
            });

            Ellipse ellipse = new Ellipse()
            {
                Fill = new SolidColorBrush(IntColor(216, 70, 70)),
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                Margin = 5,
                HeightRequest = 12,
                WidthRequest = 12
            };
            if (!(progress < nextAiringEpisode - 1 && nextAiringEpisode > 1))
                ellipse.IsVisible = false;
            
            bottom.Children.Add(ellipse);

            bottom.Children.Add(new Label()
            {
                Text = "" + progress + "/" + episodes,
                FontSize = 16,
                HorizontalOptions = LayoutOptions.End,
                TextColor = IntColor(149, 149, 149),
                FontFamily = "Jost"
            });
            details.Children.Add(titleLabel);
            details.Children.Add(bottom);
            abs.Children.Add(details);

            var (row, col) = IdToRowCol(id);
            grid.Children.Add(abs, col, row);
            return 1;
        }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            searchIcon.Source = ImageSource.FromResource("al-utils-app.Images.search.png");

            ICommand refreshCommand = new Command(() =>
            {
                UpdateData();
                refreshView.IsRefreshing = false;
            });
            refreshView.Command = refreshCommand;
        }

        protected override async void OnAppearing()
        {
            if (currentUser == "")
            {
                // search for user
                await DisplaySearchUserPrompt(true);

            } else
            {
                await CreateCards();
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
            currentUser = user.Name;
            Preferences.Set("currentUser", currentUser);

            // regenerate chart
            await CreateCards();
        }

        private async void searchIcon_Clicked(object sender, EventArgs e)
        {
            await DisplaySearchUserPrompt();
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
    }
}
