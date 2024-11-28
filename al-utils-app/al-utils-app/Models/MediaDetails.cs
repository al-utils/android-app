using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace al_utils_app
{
    internal class MediaDetails
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("title")]
        public MediaTitle Title { get; set; }
        [JsonPropertyName("episodes")]
        public int? Episodes { get; set; }

        [JsonPropertyName("coverImage")]
        public MediaImage Image { get; set; }
        [JsonPropertyName("nextAiringEpisode")]
        public MediaAiringSchedule Airing { get; set; }
    }
}