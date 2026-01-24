using al_utils_app.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace al_utils_app.ViewModels
{
    internal class API
    {
        private const int maxPages = 10; // idk
        public static string BuildQuery()
        {
            var s = "query ($name: String) {";
            for (int i = 1; i <= maxPages; i++)
            {
                s += $@"
  page{i}: Page(page: {i}, perPage: 50) {{
    mediaList(userName: $name, status_in: [CURRENT, PLANNING], type: ANIME) {{
      progress
      media {{
        id
        title {{
          english
          romaji
        }}
        status
        episodes
        coverImage {{
          extraLarge
        }}
        nextAiringEpisode {{
          id
          timeUntilAiring
          episode
        }}
      }}
    }}
  }}
";
            }
            return s + "}";
        }
        private static string currentUser = Preferences.Get("currentUser", "");
        private static Dictionary<string, object> BuildVariables()
        {
            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables.Add("name", currentUser);
            return variables;
        }

        public static async Task<List<MediaListEntry>> GetData(string status = null)
        {
            Response data = await Request.RequestDataAsync(API.BuildQuery(), API.BuildVariables());

            var dict = data.Data.Pages;
            List<MediaListEntry> mediaList = new List<MediaListEntry>();

            // combine into one list
            foreach (KeyValuePair<string, object> page in dict)
            {
                var jsonElement = page.Value;
                var jsonString2 = jsonElement.ToString();
                ResponsePage data2 = JsonSerializer.Deserialize<ResponsePage>(jsonString2);
                mediaList.AddRange(data2.MediaList);
                //Console.Out.WriteLine(data2.MediaList.Count);
            }
            //Console.Out.WriteLine("MediaList Count: " + mediaList.Count);

            // filter
            mediaList = mediaList.Where(x => x.Details.Airing != null)
                                 .OrderBy(x => x.Details.Airing.TimeUntilAiring)
                                 .ToList();

            Console.Out.WriteLine("MediaList Count: " + mediaList.Count);

            if (status == "RELEASING" || status == "NOT_YET_RELEASED")
                return mediaList.Where(x => x.Details.Status == status).ToList();

            return mediaList;
        }
    }
}
