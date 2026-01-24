using al_utils_app.Views;
using System;
using System.Text.Json.Serialization;
using System.Windows.Input;
using Xamarin.Forms;

namespace al_utils_app.Models
{
    internal class MediaListEntry
    {
        [JsonPropertyName("progress")]
        public int Progress { get; set; }
        [JsonPropertyName("media")]
        public MediaDetails Details { get; set; }

        public string GetProgressString
        {
            get
            {
                return "" + Progress + "/" + Details.GetEpisodes;
            }
        }

        public bool IsBehindProgress
        {
            get
            {
                return Progress < Details.Airing.Episode - 1 && // behind on progress
                    Details.Airing.Episode > 1; // started releasing
            }
        }
    }
}