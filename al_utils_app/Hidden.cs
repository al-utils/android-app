using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Xamarin.Essentials;

namespace al_utils_app
{
    internal class Hidden
    {
        private static List<int> hiddenIds = new List<int>();
        public static void Hide(int id)
        {
            hiddenIds.Add(id);
        }

        public static void Show(int id)
        {
            hiddenIds.Remove(id);
        }

        public static bool IsHidden(int id)
        {
            return hiddenIds.Contains(id);
        }

        public static void LoadHiddenList()
        {
            var value = Preferences.Get("hiddenIds", "");
            if (value != null && value != "")
                hiddenIds = JsonSerializer.Deserialize<List<int>>(value);
        }

        public static void SaveHiddenList()
        {
            Preferences.Set("hiddenIds", JsonSerializer.Serialize(hiddenIds));
        }

        public static void Reset()
        {
            hiddenIds = new List<int>();
            Preferences.Set("hiddenIds", JsonSerializer.Serialize(hiddenIds));
        }

        public static List<int> GetHiddenIds()
        {
            return hiddenIds;
        }
    }
}
