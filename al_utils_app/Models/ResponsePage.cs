using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static Xamarin.Essentials.Permissions;

namespace al_utils_app.Models
{
    internal class ResponsePage
    {
        [JsonPropertyName("mediaList")]
        public List<MediaListEntry> MediaList { get; set; }

        [JsonPropertyName("media")]
        public List<MediaDetails> Results { get; set; }

        [JsonPropertyName("activities")]
        public List<Activity> Activities { get; set; }

        public override string ToString()
        {
            return "page";
        }
    }
}