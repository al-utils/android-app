using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace al_utils_app.Models
{
    public class User
    {
        public User(string name, int id)
        {
            Name = name;
            ID = id;
        }

        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("id")]
        public int ID { get; set; }
        [JsonPropertyName("about")]
        public string About { get; set; }
        [JsonPropertyName("bannerImage")]
        public string BannerURL { get; set; }
    }
}