using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace al_utils_app.Models
{
    internal class Character
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public CharacterName Name { get; set; }
        [JsonPropertyName("image")]
        public CharacterImage Image { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("gender")]
        public string Gender { get; set; }
        [JsonPropertyName("dateOfBirth")]
        public Date DateOfBirth { get; set; }
        [JsonPropertyName("age")]
        public string Age { get; set; }
        [JsonPropertyName("bloodType")]
        public string BloodType { get; set; }
        [JsonPropertyName("siteUrl")]
        public string SiteUrl { get; set; }
    }
}
