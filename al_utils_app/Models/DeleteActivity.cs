using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace al_utils_app.Models
{
    internal class DeleteActivity
    {
        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }
    }
}
