using al_utils_app.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace al_utils_app.Models
{
    internal class MediaEdge
    {
        [JsonPropertyName("node")]
        public MediaDetails Node { get; set; }
        // for media
        [JsonPropertyName("relationType")]
        public string RelationType { get; set; }
        public string GetRelationType
        {
            get
            {
                switch (RelationType)
                {
                    case "ADAPTATION":
                        return "Adaptation";
                    case "PREQUEL":
                        return "Prequel";
                    case "SEQUEL":
                        return "Sequel";
                    case "PARENT":
                        return "Parent";
                    case "SIDE_STORY":
                        return "Side Story";
                    case "CHARACTER":
                        return "Character";
                    case "SUMMARY":
                        return "Summary";
                    case "ALTERNATIVE":
                        return "Alternative";
                    case "SPIN_OFF":
                        return "Spin Off";
                    case "OTHER":
                        return "Other";
                    case "SOURCE":
                        return "Source";
                    case "COMPILATION":
                        return "Compilation";
                    case "CONTAINS":
                        return "Contains";
                    default:
                        return "";
                }
            }
        }
        // for character
        [JsonPropertyName("role")]
        public string Role { get; set; }
        public string GetRole
        {
            get
            {
                switch (Role)
                {
                    case "MAIN":
                        return "Main";
                    case "SUPPORTING":
                        return "Supporting";
                    case "BACKGROUND":
                        return "Background";
                    default:
                        return "";
                }
            }
        }
    }
}
