using al_utils_app.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace al_utils_app.Views
{
    //TODO: additional details
    public partial class MediaPage : TabbedPage
    {
        string queryAnime = $@"query ($id: Int) {{
  Media(id: $id) {{ 
    id
    description
    startDate {{
      year
      month
      day
    }}
    endDate {{
      year
      month
      day
    }}
    season
    seasonYear
    status(version: 2)
    format
    episodes
    duration
    coverImage {{
      extraLarge
      color
    }}
    bannerImage
    genres
    tags {{
        name
        rank
    }}
    favourites
    averageScore
    popularity
    countryOfOrigin
    siteUrl
    title {{
      romaji
      english
      native
    }}
    relations {{
      edges {{
        relationType (version: 2)
        node {{
          id
          type
          format
          coverImage {{
            extraLarge
          }}
          title {{
            romaji
            english
			native
          }}
        }}
      }}
    }}
    characters(sort: ROLE) {{
      edges {{
        role
        node {{
          id
          name {{
            full
          }}
          image {{
            large
          }}
        }}
      }}
    }}
  }}
}}";

        string queryManga = $@"query ($id: Int) {{
  Media(id: $id) {{ 
    id
    description
    startDate {{
      year
      month
      day
    }}
    endDate {{
      year
      month
      day
    }}
    status(version: 2)
    format
    chapters
    volumes
    coverImage {{
      extraLarge
      color
    }}
    bannerImage
    genres
    tags {{
        name
        rank
    }}
    favourites
    averageScore
    popularity
    countryOfOrigin
    siteUrl
    title {{
      romaji
      english
      native
    }}
    relations {{
      edges {{
        relationType (version: 2)
        node {{
          id
          type
          format
          coverImage {{
            extraLarge
          }}
          title {{
            romaji
            english
			native
          }}
        }}
      }}
    }}
    characters(sort: ROLE) {{
      edges {{
        role
        node {{
          id
          name {{
            full
          }}
          image {{
            large
          }}
        }}
      }}
    }}
  }}
}}";

        string queryCharacter = "";

        private int mediaID;
        private Dictionary<string, object> BuildVariables()
        {
            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables.Add("id", this.mediaID);
            return variables;
        }

        internal MediaPage(int mediaID, TypeEnum.Type type)
        {
            this.mediaID = mediaID;
            InitializeComponent();
            BindingContext = this;
            backIcon.Source = ImageSource.FromResource("al_utils_app.Images.arrow-left.png");
            menuIcon.Source = ImageSource.FromResource("al_utils_app.Images.menu.png");

            FillData(type);
        }
        private async void backIcon_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void cover_Clicked(object sender, EventArgs e)
        {
            ImageSource imageSource = ((ImageButton)sender).Source;
            await Navigation.PushAsync(new ImagePage(imageSource));
        }

        private async Task<MediaDetails> GetData(TypeEnum.Type type)
        {
            string query;
            switch (type)
            {
                case TypeEnum.Type.Anime:
                    query = queryAnime;
                    break;
                case TypeEnum.Type.Manga:
                    query = queryManga;
                    break;
                case TypeEnum.Type.Character:
                    query = queryCharacter;
                    break;
                default:
                    query = queryAnime;
                    break;
            }
            Response data = await Request.RequestDataAsync(query, BuildVariables());
            var dict = data.Data.Media;
            return dict;
        }

        private string UpperToCapitalize(string s)
        {
            return s[0] + s.Substring(1).ToLower();
        }

        private ObservableCollection<MediaEdge> RelatedMediaList { get; set; } = new ObservableCollection<MediaEdge>();
        private ObservableCollection<MediaEdge> RelatedCharacterList { get; set; } = new ObservableCollection<MediaEdge>();

        private async Task FillData(TypeEnum.Type type)
        {
            MediaDetails details = await GetData(type);
                        

            Description = details.DescriptionFullFormat;
            if (details.BannerImage != null)
                BannerImageURL = details.BannerImage;
            else if (details.CoverImage.Color != null)
                banner.BackgroundColor = Color.FromHex(details.CoverImage.Color);
            CoverImageURL = details.CoverImage.ExtraLarge;
            if (details.CoverImage.Color != null)
                CoverImageColor = Color.FromHex(details.CoverImage.Color);
            else
                CoverImageColor = Color.FromHex("#262524");
            if (details.Title.English != null)
                Title = details.Title.English;
            else if (details.Title.Romaji != null)
                Title = details.Title.Romaji;
            else
                Title = details.Title.Romaji;

            Favorites = "" + details.Favorites;
            Id = details.Id;
            SiteUrl = details.SiteUrl;

            RegionInfo rg = new RegionInfo(details.CountryOfOrigin);
            switch (type)
            {
                case TypeEnum.Type.Anime:
                    Popularity = "" + details.Popularity;
                    AverageScore = "" + details.AverageScore;
                    Origin = rg.EnglishName;
                    StartDate = details.StartDate.ToString();
                    EndDate = details.EndDate.ToString();
                    Season = UpperToCapitalize(details.Season) + " " + details.SeasonYear;
                    Format = details.GetFormat;
                    Status = details.GetStatus;

                    Episodes = "" + details.Episodes;
                    Duration = "" + details.Duration;

                    TitleRomaji = details.Title.Romaji;
                    TitleEnglish = details.Title.English;
                    TitleNative = details.Title.Native;
                    break;
                case TypeEnum.Type.Manga:
                    Popularity = "" + details.Popularity;
                    AverageScore = "" + details.AverageScore;
                    Origin = rg.EnglishName;
                    StartDate = details.StartDate.ToString();
                    EndDate = details.EndDate.ToString();
                    Format = details.GetFormat;
                    Status = details.GetStatus;

                    Chapters = "" + details.Chapters;
                    Volumes = "" + details.Volumes;

                    TitleRomaji = details.Title.Romaji;
                    TitleEnglish = details.Title.English;
                    TitleNative = details.Title.Native;
                    break;
            }



            // create genres
            foreach (var genre in details.Genres)
            {
                AddTag(genre, "genreFrame");
            }
            
            // create tags
            foreach (var tag in details.Tags)
            {
                AddTag("" + tag.Name + " " + tag.Rank + "%", "tagFrame");
            }

            // relations list
            relatedMediaList.ItemsSource = details.Relations.Edge;
            relatedCharacterList.ItemsSource = details.Characters.Edge;
        }

        private void AddTag(string text, string styleClass)
        {
            StackLayout s = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };
            Frame f = new Frame()
            {
                StyleClass = new List<string>() { styleClass }
            };
            Label l = new Label()
            {
                Text = text,
                StyleClass = new List<string>() { "info" },
            };
            f.Content = l;
            //genresFlex.Children.Add(f);
            BoxView b = new BoxView()
            {
                BackgroundColor = Color.Transparent,
                WidthRequest = 15
            };
            s.Children.Add(f);
            s.Children.Add(b);
            genresFlex.Children.Add(s);
            //genresFlex.Children.Add(f);
            //genresFlex.Children.Add(b);
        }

        private FormattedString description;
        public FormattedString Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        private string bannerImageURL;
        public string BannerImageURL
        {
            get { return bannerImageURL; }
            set
            {
                bannerImageURL = value;
                OnPropertyChanged(nameof(BannerImageURL));
            }
        }

        private string coverImageURL;
        public string CoverImageURL
        {
            get { return coverImageURL; }
            set
            {
                coverImageURL = value;
                OnPropertyChanged(nameof(CoverImageURL));
            }
        }

        private Color coverImageColor;
        public Color CoverImageColor
        {
            get { return coverImageColor; }
            set
            {
                coverImageColor = value;
                OnPropertyChanged(nameof(CoverImageColor));
            }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string favorites;
        public string Favorites
        {
            get { return favorites; }
            set
            {
                favorites = value;
                OnPropertyChanged(nameof(Favorites));
            }
        }

        private string popularity;
        public string Popularity
        {
            get { return popularity; }
            set
            {
                popularity = value;
                OnPropertyChanged(nameof(Popularity));
            }
        }

        private string averageScore;
        public string AverageScore
        {
            get { return averageScore; }
            set
            {
                if (value == "")
                    averageScore = "0";
                else
                    averageScore = value;
                OnPropertyChanged(nameof(AverageScore));
            }
        }

        private string origin;
        public string Origin
        {
            get { return origin; }
            set
            {
                origin = value;
                OnPropertyChanged(nameof(Origin));
            }
        }

        private int id;
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        private string startDate;
        public string StartDate
        {
            get { return startDate; }
            set
            {
                startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        private string endDate;
        public string EndDate
        {
            get { return endDate; }
            set
            {
                endDate = value;
                OnPropertyChanged(nameof(EndDate));
            }
        }

        private string season;
        public string Season
        {
            get { return season; }
            set
            {
                season = value;
                OnPropertyChanged(nameof(Season));
            }
        }

        private string format;
        public string Format
        {
            get { return format; }
            set
            {
                format = value;
                OnPropertyChanged(nameof(Format));
            }
        }

        private string status;
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        private string episodes;
        public string Episodes
        {
            get { return episodes; }
            set
            {
                episodes = value;
                OnPropertyChanged(nameof(Episodes));
            }
        }

        private string duration;
        public string Duration
        {
            get { return duration; }
            set
            {
                duration = value;
                OnPropertyChanged(nameof(Duration));
            }
        }

        private string SiteUrl;

        private string chapters;
        public string Chapters
        {
            get { return chapters; }
            set
            {
                chapters = value;
                OnPropertyChanged(nameof(Chapters));
            }
        }

        private string volumes;
        public string Volumes
        {
            get { return volumes; }
            set
            {
                volumes = value;
                OnPropertyChanged(nameof(Volumes));
            }
        }

        private string titleRomaji;
        public string TitleRomaji
        {
            get { return titleRomaji; }
            set
            {
                titleRomaji = value;
                OnPropertyChanged(nameof(TitleRomaji));
            }
        }

        private string titleEnglish;
        public string TitleEnglish
        {
            get { return titleEnglish; }
            set
            {
                titleEnglish = value;
                OnPropertyChanged(nameof(TitleEnglish));
            }
        }

        private string titleNative;
        public string TitleNative
        {
            get { return titleNative; }
            set
            {
                titleNative = value;
                OnPropertyChanged(nameof(TitleNative));
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var title = ((Label)sender).Text;
            Clipboard.SetTextAsync(title);
        }

        private async void CharacterTapped(object sender, EventArgs e)
        {
            return;
        }

        private async void RelatedTapped(object sender, EventArgs e)
        {
            var (id, type) = ((int,TypeEnum.Type))((TappedEventArgs)e).Parameter;
            Console.WriteLine("" + id);
            Console.WriteLine("" + (int)type);
            await Navigation.PushAsync(new MediaPage(id, type));
        }

        private async void menuIcon_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayActionSheet("Options", "Cancel", null, "Copy Link", "Open in WebView");
            switch (result)
            {
                case "Cancel":
                    break;
                case "Copy Link":
                    await Clipboard.SetTextAsync(SiteUrl);
                    break;
                case "Open in WebView":
                    await Navigation.PushAsync(new WebViewPage(SiteUrl));
                    break;
            }
        }
    }
}
