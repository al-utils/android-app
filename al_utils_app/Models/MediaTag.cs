using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace al_utils_app.Models
{
    internal class MediaTag
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("rank")]
        public int Rank { get; set; }
    }
}
