using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace al_utils_app
{
    internal class Request
    {
        private const string URL = "https://graphql.anilist.co";
        private static readonly HttpClient client = new HttpClient();

        public Request()
        {
        }

        public static async Task<Response> RequestDataAsync(string query, Dictionary<string, object> variables)
        {
            Dictionary<string, object> json = new Dictionary<string, object>();

            json.Add("query", query);
            json.Add("variables", JsonSerializer.Serialize(variables));
            string jsonString = JsonSerializer.Serialize(json);

            // request
            var response = await client.PostAsync(URL, new StringContent(jsonString, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            jsonString = await response.Content.ReadAsStringAsync();

            Response data = JsonSerializer.Deserialize<Response>(jsonString);
            return data;
        }
    }
}
