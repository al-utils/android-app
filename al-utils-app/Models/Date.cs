using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace al_utils_app
{
    internal class Date
    {
        [JsonPropertyName("year")]
        public int? Year { get; set; }
        [JsonPropertyName("month")]
        public int? Month { get; set; }
        [JsonPropertyName("day")]
        public int? Day { get; set; }

        private enum MonthAbbr
        {
            Jan = 1,
            Feb = 2,
            Mar = 3,
            Apr = 4,
            May = 5,
            Jun = 6,
            Jul = 7,
            Aug = 8,
            Sep = 9,
            Oct = 10,
            Nov = 11,
            Dec = 12
        }

        public override string ToString()
        {
            MonthAbbr? month;
            if (Month != null)
                month = (MonthAbbr)Month;
            else
                month = null;


            if (Day != null)
            {
                return month.ToString() + " " + Day + " " + Year;
            }
            else if (month != null)
            {
                return month.ToString() + " " + Year;
            }
            else if (Year != null)
            {
                return "" + Year;
            }
            else
            {
                return "?";
            }
        }
    }
}
