using System;
using System.Text.Json.Serialization;

namespace al_utils_app
{
    internal class MediaTitle
    {
        [JsonPropertyName("english")]
        public string English { get; set; }
        [JsonPropertyName("romaji")]
        public string Romaji { get; set; }
    }
}