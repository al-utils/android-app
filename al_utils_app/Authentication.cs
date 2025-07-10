using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace al_utils_app
{
    internal class Authentication
    {
        public static string GetAccessToken()
        {
            return Preferences.Get("accessToken", "");
        }

        public static void SaveAccessToken(string token)
        {
            Preferences.Set("accessToken", token);
        }

        public static bool IsAuthenticated()
        {
            return !Preferences.Get("accessToken", "").Equals("");
        }
    }
}
