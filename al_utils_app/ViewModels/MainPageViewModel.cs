using al_utils_app.Models;
using al_utils_app.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace al_utils_app.ViewModels
{
    internal class MainPageViewModel : INotifyPropertyChanged
    {
        private static List<MediaListEntry> mediaList = new List<MediaListEntry>();

        private ObservableCollection<MediaListEntry> _releasingList;
        public ObservableCollection<MediaListEntry> ReleasingList
        {
            get { return _releasingList; }
            set
            {
                if (_releasingList != value)
                {
                    _releasingList = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("ReleasingList"));
                }
            }
        }
        private ObservableCollection<MediaListEntry> _notYetReleasedList;
        public ObservableCollection<MediaListEntry> NotYetReleasedList
        {
            get { return _notYetReleasedList; }
            set
            {
                if (_notYetReleasedList != value)
                {
                    _notYetReleasedList = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("NotYetReleasedList"));
                }
            }
        }

        private MainPage instance;
        public MainPageViewModel(MainPage instance)
        {
            this.instance = instance;
            LoadData();
        }

        public async Task LoadData()
        {
            mediaList = await API.GetData();
            ReleasingList = new ObservableCollection<MediaListEntry>(
                mediaList.Where(x => x.Details.Status == "RELEASING")
                         .Where(x => x.Details.Airing != null)
                         .Where(x => !Hidden.IsHidden(x.Details.Id))
                         .ToList());
            NotYetReleasedList = new ObservableCollection<MediaListEntry>(
                mediaList.Where(x => x.Details.Status == "NOT_YET_RELEASED")
                         .Where(x => x.Details.Airing != null)
                         .Where(x => !Hidden.IsHidden(x.Details.Id))
                         .ToList());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

        public async Task OnAppearing()
        {
            await LoadData();
        }

        public ICommand LongPressCommand
        {
            get
            {
                return new Command((x) =>
                {
                    MediaListEntry media = (MediaListEntry)x;
                    instance.LongPressMenu(media);
                });

            }
        }

        public ICommand TapCommand
        {
            get
            {
                return new Command((x) =>
                {
                    Debug.WriteLine("OEFIJEOIFJE");
                    MediaListEntry media = (MediaListEntry)x;
                    Application.Current.MainPage.Navigation.PushAsync(new MediaPage(media.Details.Id, TypeEnum.Type.Anime));
                });
            }
        }

        public void RemoveMedia(MediaListEntry media)
        {
            ReleasingList.Remove(media);
            instance.ReleasingCount = ReleasingList.Count;
        }
    }
}
