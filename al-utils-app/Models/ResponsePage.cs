using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static Xamarin.Essentials.Permissions;

namespace al_utils_app
{
    internal class ResponsePage
    {
        [JsonPropertyName("mediaList")]
        public List<Media> MediaList { get; set; }

        public override string ToString()
        {
            return "page";
        }
    }
}