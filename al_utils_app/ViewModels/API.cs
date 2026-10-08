using al_utils_app.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static string BuildActivityQuery(int page, int userId)
        {
            var s = $@"query {{
  Page(page: {page}, perPage: 50) {{
    activities(userId: {userId}, sort: ID_DESC) {{
      __typename
      ... on ListActivity{{
        id
        status
        progress
        createdAt
        media {{
          id
          coverImage {{
            extraLarge
          }}
          title {{
            romaji
            english
            native
          }}
        }}
        likes {{
          id
        }}
      }}
    }}
  }}
}}
";
            return s;
        }
        private static string currentUser = Preferences.Get("currentUser", "");
        private static int userId = Preferences.Get("userId", -1);
        private static Dictionary<string, object> BuildVariables(string user)
        {
            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables.Add("name", user);
            return variables;
        }


        private static readonly int activityPageNum = 1;
        public static async Task<(List<MediaListEntry>, List<Activity>, User)> GetData(User user = null)
        {
            if (user == null)
                user = new User(currentUser, userId);

            Response data = await Request.RequestDataAsync(API.BuildQuery(), API.BuildVariables(user.Name));

            var dict = data.Data.Pages;
            List<MediaListEntry> mediaList = new List<MediaListEntry>();

            // combine into one list
            foreach (KeyValuePair<string, object> page in dict)
            {
                var jsonElement = page.Value;
                var jsonString2 = jsonElement.ToString();
                ResponsePage data2 = JsonSerializer.Deserialize<ResponsePage>(jsonString2);
                mediaList.AddRange(data2.MediaList);
            }

            // filter
            mediaList = mediaList.Where(x => x.Details.Airing != null)
                                 .OrderBy(x => x.Details.Airing.TimeUntilAiring)
                                 .ToList();


            // load activities
            data = await Request.RequestDataAsync(API.BuildActivityQuery(activityPageNum, user.ID), new Dictionary<string, object>());
            // only care about list updates, ignore other msg types
            List<Activity> activityList = data.Data.Page.Activities;

            activityList = activityList.Where(x => x.TypeName == "ListActivity").ToList();

            Debug.WriteLine(activityList.Count);

            return (mediaList, activityList, user);
        }
    }
}
