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

        public override string ToString()
        {
            return "page";
        }
    }
}