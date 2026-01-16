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
        public static DateTime UnixTimeStampToDateTime( double unixTimeStamp )
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
            return dateTime;
        }
        public string GetRelativeTime
        {
            get
            {
                string result = string.Empty;
                var timeSpan = DateTime.Now.Subtract(UnixTimeStampToDateTime(CreatedAt));
            
                if (timeSpan <= TimeSpan.FromSeconds(60))
                {
                    result = string.Format("{0} seconds ago", timeSpan.Seconds);
                }
                else if (timeSpan <= TimeSpan.FromMinutes(60))
                {
                    result = timeSpan.Minutes > 1 ? 
                        String.Format("{0} minutes ago", timeSpan.Minutes) :
                        "a minute ago";
                }
                else if (timeSpan <= TimeSpan.FromHours(24))
                {
                    result = timeSpan.Hours > 1 ? 
                        String.Format("{0} hours ago", timeSpan.Hours) : 
                        "an hour ago";
                }
                else if (timeSpan <= TimeSpan.FromDays(30))
                {
                    result = timeSpan.Days > 1 ? 
                        String.Format("{0} days ago", timeSpan.Days) : 
                        "yesterday";
                }
                else if (timeSpan <= TimeSpan.FromDays(365))
                {
                    result = timeSpan.Days > 30 ? 
                        String.Format("{0} months ago", timeSpan.Days / 30) : 
                        "a month ago";
                }
                else
                {
                    result = timeSpan.Days > 365 ? 
                        String.Format("{0} years ago", timeSpan.Days / 365) : 
                        "a year ago";
                }

                return result;
            }
        }
        [JsonPropertyName("media")]
        public MediaDetails Media { get; set; }
        [JsonPropertyName("likes")]
        public List<User> Likes { get; set; }

    }
}
