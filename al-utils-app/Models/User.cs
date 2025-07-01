using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace al_utils_app
{
    internal class User
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("id")]
        public int ID { get; set; }
    }
}