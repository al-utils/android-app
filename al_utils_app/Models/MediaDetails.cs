using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xamarin.Forms;

namespace al_utils_app.Models
{
    internal class MediaDetails
    {
        private int id;
        [JsonPropertyName("id")]
        public int Id
        {
            get { return id; }
            set 
            { 
                id = value;
                OnPropertyChanged();
            }
        }
        private MediaTitle title;
        [JsonPropertyName("title")]
        public MediaTitle Title
        {
            get { return title; }
            set 
            { 
                title = value;
                OnPropertyChanged();
            }
        }
        [JsonPropertyName("episodes")]
        public int? Episodes { get; set; }

        private MediaImage image;
        [JsonPropertyName("coverImage")]
        public MediaImage Image
        {
            get { return image; }
            set 
            { 
                image = value;
                OnPropertyChanged();
            }
        }
        [JsonPropertyName("nextAiringEpisode")]
        public MediaAiringSchedule Airing { get; set; }







        private string description;
        [JsonPropertyName("description")]
        public string Description
        {
            get { return description; }
            set 
            { 
                description = value;
                OnPropertyChanged();
            }
        }
        public string DescriptionSearchFormat
        {
            get
            {
                if (description == null || description == "")
                    return "(no description)";
                var s = description.Replace("</", "<");
                s = s.Replace("<b>", "<i>");

                s = string.Join( "", s.Split( new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries ));
                s = string.Join( "<br>", s.Split( new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries ));
                s = s.Replace("<br>", "\n");
                s = s.Replace("<i>", "");
                return s;
            }
        }
        public FormattedString DescriptionFullFormat
        {
            get
            {
                if (description == null || description == "")
                    return "(no description)";
                var s = description;
                s = s.Replace("</", "<");
                s = s.Replace("<b>", "<i>");

                s = string.Join( "", s.Split( new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries ));
                s = string.Join( "<br>", s.Split( new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries ));
                s = s.Replace("<br>", "\n\n");

                var parts = s.Split(new string[] { "<i>" }, StringSplitOptions.RemoveEmptyEntries);

                FormattedString fs = new FormattedString();
                for (var i = 0; i < parts.Length; i++)
                {
                    if (i % 2 == 0)
                        fs.Spans.Add(new Span { Text = parts[i] });
                    else
                        fs.Spans.Add(new Span
                        {
                            Text = parts[i],
                            FontAttributes = FontAttributes.Italic,
                            FontFamily = "JostItalic",
                            FontSize = 16
                        });
                }

                return fs;
            }
        }



        [JsonPropertyName("startDate")]
        public Date StartDate { get; set; }
        [JsonPropertyName("endDate")]
        public Date EndDate { get; set; }
        [JsonPropertyName("season")]
        public string Season { get; set; }
        [JsonPropertyName("seasonYear")]
        public int? SeasonYear { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        public string GetStatus()
        {
            switch (Status)
            {
                case "FINISHED":
                    return "Finished";
                case "RELEASING":
                    return "Releasing";
                case "NOT_YET_RELEASED":
                    return "Not Yet Released";
                case "CANCELLED":
                    return "Cancelled";
                case "HIATUS":
                    return "Hiatus";
                default:
                    return "Unknown";
            }
        }
        [JsonPropertyName("format")]
        public string Format { get; set; }
        public string GetFormat()
        {
            switch (Format)
            {
                case "TV":
                    return "TV";
                case "TV_SHORT":
                    return "TV Short";
                case "MOVIE":
                    return "Movie";
                case "SPECIAL":
                    return "Special";
                case "OVA":
                    return "OVA";
                case "ONA":
                    return "ONA";
                case "MUSIC":
                    return "Music";
                case "MANGA":
                    return "Manga";
                case "NOVEL":
                    return "Novel";
                case "ONE_SHOT":
                    return "One Shot";
                default:
                    return "Unknown";
            }
        }
        [JsonPropertyName("bannerImage")]
        public string BannerImage { get; set; }
        [JsonPropertyName("favourites")]
        public int Favorites { get; set; }
        [JsonPropertyName("averageScore")]
        public int? AverageScore { get; set; }
        [JsonPropertyName("popularity")]
        public int? Popularity { get; set; }
        [JsonPropertyName("countryOfOrigin")]
        public string CountryOfOrigin { get; set; }




        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}