using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace al_utils_app.Models
{
    internal class Response
    {
        [JsonPropertyName("data")]
        public ResponseData Data { get; set; }

        public override string ToString()
        {
            return "response";
        }
    }
}
