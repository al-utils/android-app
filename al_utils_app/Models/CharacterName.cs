using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace al_utils_app.Models
{
    internal class CharacterName
    {
        [JsonPropertyName("full")]
        public string Full { get; set; }
    }
}
