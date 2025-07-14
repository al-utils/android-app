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
        string query = $@"query ($id: Int) {{
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
    favourites
    averageScore
    popularity
    countryOfOrigin
    title {{
      romaji
      english
      native
    }}
  }}
}}";

        private int mediaID;
        private Dictionary<string, object> BuildVariables()
        {
            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables.Add("id", this.mediaID);
            return variables;
        }

        public MediaPage(int mediaID)
        {
            this.mediaID = mediaID;
            InitializeComponent();
            BindingContext = this;
            backIcon.Source = ImageSource.FromResource("al_utils_app.Images.arrow-left.png");

            FillData();
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

        private async Task<MediaDetails> GetData()
        {
            Response data = await Request.RequestDataAsync(query, BuildVariables());
            var dict = data.Data.Media;
            return dict;
        }

        private string UpperToCapitalize(string s)
        {
            return s[0] + s.Substring(1).ToLower();
        }

        private async Task FillData()
        {
            MediaDetails details = await GetData();
                        

            Description = details.DescriptionFullFormat;
            if (details.BannerImage != null)
                BannerImageURL = details.BannerImage;
            else
                banner.BackgroundColor = Color.FromHex(details.Image.Color);
            CoverImageURL = details.Image.ExtraLarge;
            CoverImageColor = Color.FromHex(details.Image.Color);
            if (details.Title.English != null)
                Title = details.Title.English;
            else if (details.Title.Romaji != null)
                Title = details.Title.Romaji;
            else
                Title = details.Title.Romaji;

                Favorites = "" + details.Favorites;
            Popularity = "" + details.Popularity;
            AverageScore = "" + details.AverageScore;

            RegionInfo rg = new RegionInfo(details.CountryOfOrigin);
            Id = details.Id;
            Origin = rg.EnglishName;
            StartDate = details.StartDate.ToString();
            EndDate = details.EndDate.ToString();
            Season = UpperToCapitalize(details.Season) + " " + details.SeasonYear;
            Format = details.GetFormat();
            Status = details.GetStatus();
            Episodes = "" + details.Episodes;
            Duration = "" + details.Duration;

            TitleRomaji = details.Title.Romaji;
            TitleEnglish = details.Title.English;
            TitleNative = details.Title.Native;


            // create genres
            foreach (var genre in details.Genres)
            {
                Console.WriteLine("HERE: " + genre);
                Frame f = new Frame()
                {
                    StyleClass = new List<string>() { "tagFrame" }
                };
                Label l = new Label()
                {
                    Text = genre,
                    StyleClass = new List<string>() { "info" }
                };
                f.Content = l;
                genresFlex.Children.Add(f);
            }
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
    }
}
