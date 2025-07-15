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

namespace al_utils_app.Views
{
    public partial class MainPage : TabbedPage
    {
        private const string URL = "https://graphql.anilist.co";
        private static readonly HttpClient client = new HttpClient();
        private const int numCols = 2;
        private const int maxPages = 10; // idk

        private string currentUser = Preferences.Get("currentUser", "");

        // todo:
        // username
        // completed but current
        // full schedule
        
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
        status
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

        private Dictionary<string, object> BuildVariables()
        {
            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables.Add("name", currentUser);
            return variables;
        }

        private async Task<List<MediaListEntry>> GetData(string status = null)
        {
            Response data = await Request.RequestDataAsync(BuildQuery(), BuildVariables());

            var dict = data.Data.Pages;
            List<MediaListEntry> mediaList = new List<MediaListEntry>();

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

            if (status == "RELEASING" || status == "NOT_YET_RELEASED")
                return mediaList.Where(x => x.Details.Status == status).ToList();

            return mediaList;
        }

        public async Task CreateCards()
        {
            List<MediaListEntry> mediaList = await GetData();
            // clear grid
            releasedGrid.Children.Clear();
            releasedGrid.RowDefinitions.Clear();
            notYetReleasedGrid.Children.Clear();
            notYetReleasedGrid.RowDefinitions.Clear();
            ForUser = currentUser;

            // filter by status
            List<MediaListEntry> releasingList = mediaList.Where(x => x.Details.Status == "RELEASING")
                                                 .Where(x => x.Details.Airing != null)
                                                 .Where(x => !Hidden.IsHidden(x.Details.Id))
                                                 .ToList();
            List<MediaListEntry> notYetReleasedList = mediaList.Where(x => x.Details.Status == "NOT_YET_RELEASED")
                                                      .Where(x => x.Details.Airing != null)
                                                      .Where(x => !Hidden.IsHidden(x.Details.Id))
                                                      .ToList();

            ReleasingCount = releasingList.Count;
            NotYetReleasedCount = notYetReleasedList.Count;

            if (releasingList.Count == 0)
                CreateEmptyGrid(releasedGrid);
            else
                CreateCardGrid(releasingList, releasedGrid);

            if (notYetReleasedList.Count == 0)
                CreateEmptyGrid(notYetReleasedGrid);
            else
                CreateCardGrid(notYetReleasedList, notYetReleasedGrid);
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

        private void CreateEmptyGrid(Grid g)
        {
            StackLayout stack = new StackLayout
            {
                WidthRequest=200, 
                HorizontalOptions=LayoutOptions.Center, 
                Padding=new Thickness(0, 30, 0, 0)
            };
            Image image = new Image { Source = ImageSource.FromResource("al_utils_app.Images.mio.png"), };
            Label text = new Label
            {
                Text="So Empty...", 
                FontFamily="Jost", 
                FontSize=20,
                TextColor = new Color(38.0/255, 37.0/255, 36.0/255),
                HorizontalOptions=LayoutOptions.Center
            };

            stack.Children.Add(image);
            stack.Children.Add(text);

            g.RowDefinitions.Add(new RowDefinition() { Height = 320 });
            g.Children.Add(stack, 0, 2, 0, 1);
        }


        private void CreateCardGrid(List<MediaListEntry> list, Grid g)
        {
            var total = 0;
            foreach (MediaListEntry media in list)
            {
                MakeCard(total, media, g);
                if (total % numCols == 0)
                {
                    // add column
                    g.RowDefinitions.Add(new RowDefinition() { Height = 320 });
                }
                total++;
            }
        }


        private async Task UpdateData(Grid g)
        {
            if (g.Children.Count == 0)
            {
                await CreateCards();
                return;
            }

            var page = "RELEASING";
            if (g == notYetReleasedGrid)
                page = "NOT_YET_RELEASED";

            List<MediaListEntry> mediaList = await GetData(page);
            for (int i = 0; i < g.Children.Count; i++)
            {
                MediaListEntry media = mediaList[i];
                var timeUntilAiring = (int)media.Details.Airing.TimeUntilAiring;
                var nextAiringEpisode = media.Details.Airing.Episode;
                var progress = media.Progress;
                var episodes = "";
                if (media.Details.Episodes == null)
                    episodes = "?";
                else
                    episodes = "" + media.Details.Episodes;

                AbsoluteLayout card = (AbsoluteLayout)g.Children[i];
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

        private void RemoveCard(View removeCard, Grid g)
        {
            var cards = g.Children;
            int prevRow, prevCol;
            var nextRow = Grid.GetRow(removeCard);
            var nextCol = Grid.GetColumn(removeCard);

            var index = cards.IndexOf(removeCard);
            for (var i = index + 1; i < cards.Count; i++)
            {
                var card = cards[i];
                prevRow = Grid.GetRow(card);
                prevCol = Grid.GetColumn(card);
                Grid.SetRow(card, nextRow);
                Grid.SetColumn(card, nextCol);
                nextRow = prevRow;
                nextCol = prevCol;

                if (g.Children.Count > 0)
                {
                    g.RowDefinitions.Clear();
                    for (var j = 1; j <= g.Children.Count / 2; j++)
                        g.RowDefinitions.Add(new RowDefinition() { Height = 320 });
                }
            }
            cards.RemoveAt(index);
            var count = cards.Count;
            if (cards.Count == 0)
            {
                CreateEmptyGrid(g);
                count = 0;
            }

            if (g == releasedGrid)
                ReleasingCount = count;
            else if (g == notYetReleasedGrid)
                NotYetReleasedCount = count;
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

        private void MakeCard(int id, MediaListEntry media, Grid g)
        {
            if (media.Details.Airing == null)
                return;

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
            var imageURL = media.Details.CoverImage.ExtraLarge;



            // begin layout

            MultiGestureView gestureView = new MultiGestureView()
            {
                VibrateOnTap = false,
                VibrateOnLongPress = true,
            };

            // events
            gestureView.Tapped += async (s, e) =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new MediaPage(mediaID));
            };

            gestureView.LongPressed += async (s, e) =>
            {
                var result = await DisplayActionSheet("Options", "Cancel", null, "Copy Title", "Hide");
                switch (result)
                {
                    case "Cancel":
                        break;
                    case "Copy Title":
                        await Clipboard.SetTextAsync(title);
                        break;
                    case "Hide":
                        Hidden.Hide(mediaID);
                        RemoveCard(gestureView, g);
                        Hidden.SaveHiddenList();

                        break;
                }
            };


            
            AbsoluteLayout abs = new AbsoluteLayout();

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
            gestureView.Content = abs;

            var (row, col) = IdToRowCol(id);
            g.Children.Add(gestureView, col, row);
        }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            Hidden.LoadHiddenList();
            //Hidden.Reset();

            menuIcon.Source = ImageSource.FromResource("al_utils_app.Images.menu.png");

            ICommand refreshCommand = new Command(() =>
            {
                UpdateData(releasedGrid);
                refreshView.IsRefreshing = false;
            });
            ICommand refreshCommand2 = new Command(() =>
            {
                UpdateData(notYetReleasedGrid);
                refreshView2.IsRefreshing = false;
            });
            refreshView.Command = refreshCommand;
            refreshView2.Command = refreshCommand2;

            if (currentUser == "")
            {
                // search for user
                DisplaySearchUserPrompt(true);

            }
            else
            {
                CreateCards();
            }
        }

        protected override async void OnAppearing()
        {
            //if (currentUser == "")
            //{
            //    // search for user
            //    await DisplaySearchUserPrompt(true);

            //} else
            //{
            //    await CreateCards();
            //}
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

        private async void menuIcon_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayActionSheet("Menu", "Cancel", null, "Change User", "Search Media", "Settings");
            Console.WriteLine(result);
            switch (result)
            {
                case "Cancel":
                    break;
                case "Change User":
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
    }
}
