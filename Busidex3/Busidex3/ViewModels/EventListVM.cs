using Busidex.Models.Domain;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;

namespace Busidex3.ViewModels
{
    public class EventListVM : BaseViewModel
    {
        private List<EventTag> _eventList;
        public List<EventTag> EventList
        {
            get => _eventList;
            set
            {
                _eventList = value;
                OnPropertyChanged(nameof(EventList));
            }
        }

        private bool _isRefreshing;
        public bool IsRefreshing { 
            get => _isRefreshing;
            set {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public ImageSource BackgroundImage
        {
            get
            {
                return ImageSource.FromResource("Busidex.Resources.Images.cards_back2.png",
                    typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
            }
        }
    }
}
