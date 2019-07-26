using Busidex3.DomainModels;
using Busidex3.Services.Utils;
using Busidex3.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Busidex3.ViewModels
{
    public class HomeMenuVM : BaseViewModel
    {
        private MainMenuMenuItem _myBusidexItem;
        public MainMenuMenuItem MyBusidexItem
        {
            get
            {
                return _myBusidexItem;
            }
            set
            {
                _myBusidexItem = value;
                OnPropertyChanged(nameof(MyBusidexItem));
            }
        }

        private MainMenuMenuItem _searchItem;
        public MainMenuMenuItem SearchItem
        {
            get
            {
                return _searchItem;
            }
            set
            {
                _searchItem = value;
                OnPropertyChanged(nameof(SearchItem));
            }
        }

        public double? RowHeight { get; set; }
        public double? RowWidth { get; set; }

        private MainMenuMenuItem _shareItem;
        public MainMenuMenuItem ShareItem
        {
            get
            {
                return _shareItem;
            }
            set
            {
                _shareItem = value;
                OnPropertyChanged(nameof(ShareItem));
            }
        }

        private MainMenuMenuItem _organizationsItem;
        public MainMenuMenuItem OrganizationsItem
        {
            get
            {
                return _organizationsItem;
            }
            set
            {
                _organizationsItem = value;
                OnPropertyChanged(nameof(OrganizationsItem));
            }
        }

        private MainMenuMenuItem _eventsItem;
        public MainMenuMenuItem EventsItem
        {
            get
            {
                return _eventsItem;
            }
            set
            {
                _eventsItem = value;
                OnPropertyChanged(nameof(EventsItem));
            }
        }

        private bool _showEvents;
        public bool ShowEvents
        {
            get { return _showEvents; }
            set
            {
                _showEvents = value;
                OnPropertyChanged(nameof(ShowEvents));
            }
        }

        private bool _showOrganizations;
        public bool ShowOrganizations
        {
            get { return _showOrganizations; }
            set
            {
                _showOrganizations = value;
                OnPropertyChanged(nameof(ShowOrganizations));
            }
        }

        private bool _hasCard;
        public bool HasCard
        {
            get { return _hasCard; }
            set
            {
                _hasCard = value;
                OnPropertyChanged(nameof(HasCard));
            }
        }

        public HomeMenuVM()
        {
            MyBusidexItem = new MainMenuMenuItem
            {
                Id = 0,
                Title = ViewNames.MyBusidex,
                TargetType = typeof(MyBusidexView),
                Image = ImageSource.FromResource("Busidex3.Resources.mybusidexicon.png",
                    typeof(HomeMenuVM).GetTypeInfo().Assembly),
                IsClickable = true
            };

            SearchItem = new MainMenuMenuItem
            {
                Id = 1,
                Title = ViewNames.Search,
                TargetType = typeof(SearchView),
                Image = ImageSource.FromResource("Busidex3.Resources.searchicon.png",
                    typeof(HomeMenuVM).GetTypeInfo().Assembly),
                IsClickable = true
            };

            ShareItem = new MainMenuMenuItem
            {
                Id = 2,
                Title = ViewNames.Share + " My Card",
                TargetType = typeof(ShareView),
                Image = ImageSource.FromResource("Busidex3.Resources.shareicon.png",
                    typeof(HomeMenuVM).GetTypeInfo().Assembly),
                IsClickable = true
            };

            OrganizationsItem = new MainMenuMenuItem
            {
                Id = 3,
                Title = ViewNames.Organizations,
                TargetType = typeof(OrganizationsView),
                Image = ImageSource.FromResource("Busidex3.Resources.organizationsicon.png",
                    typeof(HomeMenuVM).GetTypeInfo().Assembly),
                IsClickable = true
            };

            EventsItem = new MainMenuMenuItem
            {
                Id = 4,
                Title = ViewNames.Events,
                TargetType = typeof(EventsView),
                Image = ImageSource.FromResource("Busidex3.Resources.eventicon.png",
                    typeof(HomeMenuVM).GetTypeInfo().Assembly),
                IsClickable = true
            };

            var path = Path.Combine(Serialization.LocalStorageFolder, StringResources.OWNED_CARD_FILE);
            var ownedCard = Serialization.LoadData<Card>(path);
            HasCard = false;
            if (ownedCard == null)
            {
                Task.Factory.StartNew(async () =>
                {
                    ownedCard = await App.LoadOwnedCard();
                    if(ownedCard != null)
                    {
                        HasCard = ownedCard.FrontFileId != Guid.Empty && ownedCard.FrontFileId != null;
                    }
                });
            }
        }
    }
}
