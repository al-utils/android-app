using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace al_utils_app.Models
{
    internal class CharacterImage
    {
        [JsonPropertyName("large")]
        public string Large { get; set; }
    }
}
