﻿using System;
using System.Text.Json.Serialization;

namespace al_utils_app
{
    internal class MediaImage
    {
        [JsonPropertyName("extraLarge")]
        public string ExtraLarge { get; set; }
        [JsonPropertyName("color")]
        public string Color { get; set; }
    }
}