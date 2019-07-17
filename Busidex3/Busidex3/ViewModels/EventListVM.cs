﻿using Busidex3.DomainModels;
using System.Collections.Generic;
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

        public ImageSource BackgroundImage
        {
            get
            {
                return ImageSource.FromResource("Busidex3.Resources.cards_back2.png",
                    typeof(SearchVM).Assembly);
            }
        }
    }
}