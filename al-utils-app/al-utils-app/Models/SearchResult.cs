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
        //public int Id { get; set; }



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
        //public MediaTitle Title { get; set; }

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
        //public MediaImage Image { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
