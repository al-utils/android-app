using System.Text.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;

namespace al_utils_app
{
    internal class MediaAiringSchedule
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [JsonPropertyName("timeUntilAiring")]
        public int? TimeUntilAiring { get; set; }
        [JsonPropertyName("episode")]
        public int? Episode { get; set; }
    }
}