using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace al_utils_app
{
    internal class MediaDetails
    {
        private string format;

        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("title")]
        public MediaTitle Title { get; set; }
        [JsonPropertyName("episodes")]
        public int? Episodes { get; set; }

        [JsonPropertyName("coverImage")]
        public MediaImage Image { get; set; }
        [JsonPropertyName("nextAiringEpisode")]
        public MediaAiringSchedule Airing { get; set; }







        [JsonPropertyName("description")]
        public string Description { get; set; }
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
    }
}