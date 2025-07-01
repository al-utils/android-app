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
        [JsonPropertyName("native")]
        public string Native { get; set; }

        public string GetTitle
        {
            get
            {
                if (!string.IsNullOrEmpty(English) && !string.IsNullOrWhiteSpace(English)) return English;
                if (!string.IsNullOrEmpty(Romaji) && !string.IsNullOrWhiteSpace(Romaji)) return Romaji;
                return Native;
            }
        }
    }
}