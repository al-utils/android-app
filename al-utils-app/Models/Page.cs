using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace al_utils_app.Models
{
    internal class Page
    {
        [JsonPropertyName("media")]
        public List<SearchResult> Results { get; set; }
    }
}
