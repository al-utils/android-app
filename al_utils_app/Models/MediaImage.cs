using System;
using System.Text.Json.Serialization;

namespace al_utils_app.Models
{
    internal class MediaImage
    {
        [JsonPropertyName("extraLarge")]
        public string ExtraLarge { get; set; }
        [JsonPropertyName("color")]
        public string Color { get; set; }

        public override string ToString()
        {
            return ExtraLarge;
        }
    }
}