using System;
using System.Collections.Generic;
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
        [JsonPropertyName("type")]
        public string Type { get; set; }
        public TypeEnum.Type GetMediaType
        {
            get
            {
                switch (Type)
                {
                    case "ANIME":
                        return TypeEnum.Type.Anime;
                    case "MANGA":
                        return TypeEnum.Type.Manga;
                    default:
                        return TypeEnum.Type.Anime;
                }
            }
        }
        [JsonPropertyName("episodes")]
        public int? Episodes { get; set; }

        [JsonPropertyName("duration")]
        public int? Duration { get; set; }

        private MediaImage coverImage;
        [JsonPropertyName("coverImage")]
        public MediaImage CoverImage
        {
            get { return coverImage; }
            set 
            { 
                coverImage = value;
                OnPropertyChanged();
            }
        }
        [JsonPropertyName("nextAiringEpisode")]
        public MediaAiringSchedule Airing { get; set; }
        public string GetNextEpisodeString
        {
            get
            {
                return "Ep " + Airing.Episode + 
                    ": " + SecondsToString((int)Airing.TimeUntilAiring);
            }
        }
        public string GetEpisodes
        {
            get
            {
                return (Episodes == null) ? "?" : "" + Episodes;
            }
        }
        private string SecondsToString(int s)
        {
            int d = s / (3600 * 24);
            int h = s % (3600 * 24) / 3600;
            return "" + d + "d" + h + "h";
        }


        public (int, TypeEnum.Type) OpenInfo
        {
            get
            {
                return (Id, GetMediaType);
            }
        }






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
        public string GetStatus
        {
            get
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
        }
        [JsonPropertyName("format")]
        public string Format { get; set; }
        public string GetFormat
        {
            get
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
        }
        [JsonPropertyName("bannerImage")]
        public string BannerImage { get; set; }

        [JsonPropertyName("genres")]
        public List<string> Genres { get; set; }
        [JsonPropertyName("tags")]
        public List<MediaTag> Tags { get; set; }
        [JsonPropertyName("favourites")]
        public int Favorites { get; set; }
        [JsonPropertyName("averageScore")]
        public int? AverageScore { get; set; }
        [JsonPropertyName("popularity")]
        public int? Popularity { get; set; }
        [JsonPropertyName("countryOfOrigin")]
        public string CountryOfOrigin { get; set; }
        [JsonPropertyName("siteUrl")]
        public string SiteUrl { get; set; }




        [JsonPropertyName("relations")]
        public MediaRelation Relations { get; set; }
        [JsonPropertyName("characters")]
        public MediaRelation Characters { get; set; }




        // characters
        [JsonPropertyName("name")]
        public CharacterName Name { get; set; }
        [JsonPropertyName("image")]
        public CharacterImage Image { get; set; }
        
        // manga
        [JsonPropertyName("chapters")]
        public int? Chapters { get; set; }
        [JsonPropertyName("volumes")]
        public int? Volumes { get; set; }




        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}