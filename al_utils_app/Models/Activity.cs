using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace al_utils_app.Models
{
    internal class Activity
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("progress")]
        public string Progress { get; set; }
        public string GetActivityMessage
        {
            get
            {
                var status = Status.ToCharArray();
                status[0] = char.ToUpper(status[0]);
                string message = new string(status);
                if (Progress != null)
                    message += " " + Progress + " of";
                message += " " + Media.Title.GetTitle;
                return message;
            }
        }

        [JsonPropertyName("createdAt")]
        public int CreatedAt { get; set; }
        [JsonPropertyName("media")]
        public MediaDetails Media { get; set; }
        [JsonPropertyName("likes")]
        public List<User> Likes { get; set; }

    }
}
