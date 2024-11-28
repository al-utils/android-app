﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace al_utils_app
{
    internal class ResponseData
    {
        //[JsonPropertyName("page0")]
        [JsonExtensionData]
        public Dictionary<string, object> Pages { get; set; }
        //public ResponsePage Page { get; set; }
        public override string ToString()
        {
            return "data";
        }
    }
}