using System;
using System.Text.Json.Serialization;

namespace al_utils_app.Models
{
    internal class MediaListEntry
    {
        [JsonPropertyName("progress")]
        public int Progress { get; set; }
        [JsonPropertyName("media")]
        public MediaDetails Details { get; set; }
    }
}