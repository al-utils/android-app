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

    internal class MediaAiringScheduleComparer : IComparer<MediaAiringSchedule>
    {
        public int Compare(MediaAiringSchedule x, MediaAiringSchedule y)
        {
            if (x.TimeUntilAiring == y.TimeUntilAiring)
                return 0;
            if (x == null)
                return 1;
            if (y == null)
                return -1;
            return x.TimeUntilAiring < y.TimeUntilAiring ? 1 : -1;
        }
    }
}