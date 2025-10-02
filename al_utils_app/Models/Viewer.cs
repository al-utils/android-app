using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace al_utils_app.Models
{
    internal class Viewer
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}
