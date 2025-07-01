using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using Xamarin.Forms;

namespace al_utils_app.Models
{
    internal class SearchResult : INotifyPropertyChanged
    {

        private int id;
        [JsonPropertyName("id")]
        public int Id
        {
            get { return id; }
            set 
            { 
                id = value;
                OnPropertyChanged();
            }
        }

        private MediaTitle title;
        [JsonPropertyName("title")]
        public MediaTitle Title
        {
            get { return title; }
            set 
            { 
                title = value;
                OnPropertyChanged();
            }
        }

        private MediaImage image;
        [JsonPropertyName("coverImage")]
        public MediaImage Image
        {
            get { return image; }
            set 
            { 
                image = value;
                OnPropertyChanged();
            }
        }

        private string description;
        [JsonPropertyName("description")]
        public string Description
        {
            get
            {
                if (description == null || description == "")
                    return "(no description)";
                var s = description.Replace("</", "<");
                s = s.Replace("<b>", "<i>");

                Console.WriteLine(s);
                s = string.Join( "", s.Split( new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries ));
                s = string.Join( "<br>", s.Split( new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries ));
                s = s.Replace("<br>", "\n");
                s = s.Replace("<i>", "");
                return s;
            }
            set 
            { 
                description = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
