using Busidex3.DomainModels;
using Busidex3.Services.Utils;
using Busidex3.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Busidex3.ViewModels
{
    public class HomeMenuVM : BaseViewModel
    {        
        private MainMenuMenuItem _myBusidexItem;
        public MainMenuMenuItem MyBusidexItem
        {
            get => _myBusidexItem;
            set
            {
                _myBusidexItem = value;
                OnPropertyChanged(nameof(MyBusidexItem));
            }
        }

        private MainMenuMenuItem _searchItem;
        public MainMenuMenuItem SearchItem
        {
            get => _searchItem;
            set
            {
                _searchItem = value;
                OnPropertyChanged(nameof(SearchItem));
            }
        }

        private MainMenuMenuItem _shareItem;
        public MainMenuMenuItem ShareItem
        {
            get => _shareItem;
            set
            {
                _shareItem = value;
                OnPropertyChanged(nameof(ShareItem));
            }
        }

        private MainMenuMenuItem _organizationsItem;
        public MainMenuMenuItem OrganizationsItem
        {
            get => _organizationsItem;
            set
            {
                _organizationsItem = value;
                OnPropertyChanged(nameof(OrganizationsItem));
            }
        }

        private MainMenuMenuItem _eventsItem;
        public MainMenuMenuItem EventsItem
        {
            get => _eventsItem;
            set
            {
                _eventsItem = value;
                OnPropertyChanged(nameof(EventsItem));
            }
        }

        private bool _showEvents;
        public bool ShowEvents
        {
            get => _showEvents;
            set
            {
                _showEvents = value;
                OnPropertyChanged(nameof(ShowEvents));
            }
        }

        private bool _showOrganizations;
        public bool ShowOrganizations
        {
            get => _showOrganizations;
            set
            {
                _showOrganizations = value;
                OnPropertyChanged(nameof(ShowOrganizations));
            }
        }

        private bool _isProfessional;
        public bool IsProfessional
        {
            get => _isProfessional;
            set
            {
                _isProfessional = value;
                OnPropertyChanged(nameof(IsProfessional));
            }
        }

        private bool _isConsumer;
        public bool IsConsumer
        {
            get => _isConsumer;
            set
            {
                _isConsumer = value;
                OnPropertyChanged(nameof(IsConsumer));
            }
        }

        private ImageSource _backgroundImage;
        public ImageSource BackgroundImage
        {
            get => _backgroundImage;
            set
            {
                _backgroundImage = value;
                OnPropertyChanged(nameof(BackgroundImage));
            }
        }

        private ImageSource _backgroundImageProf;
        public ImageSource BackgroundImageProf
        {
            get => _backgroundImageProf;
            set
            {
                _backgroundImageProf = value;
                OnPropertyChanged(nameof(BackgroundImageProf));
            }
        }

        public HomeMenuVM()
        {
            App.OnOrganizationsLoaded += CheckShowOrganizations;

            App.OnEventsLoaded += CheckShowEvents;

            
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
            
            var src = "Busidex3.Resources.home_consumer.png";
            var srcProf = "Busidex3.Resources.home_professional.png";

            BackgroundImage = ImageSource.FromResource(src, typeof(HomeMenuVM).GetTypeInfo().Assembly);
            BackgroundImageProf = ImageSource.FromResource(srcProf, typeof(HomeMenuVM).GetTypeInfo().Assembly);

            IsProfessional = App.IsProfessional;
            IsConsumer = !IsProfessional;

            if (ownedCard == null)
            {
                Task.Factory.StartNew(async () =>
                {
                    ownedCard = await App.LoadOwnedCard();
                    if(ownedCard != null)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            CheckHasCard(ownedCard);
                        });
                    }
                });
            }
            else
            {
                CheckHasCard(ownedCard);
            }

            CheckShowOrganizations();
            CheckShowEvents();
        }

        private void CheckHasCard(Card ownedCard)
        {
            IsProfessional = ownedCard?.FrontFileId != Guid.Empty && ownedCard?.FrontFileId != null;
            IsConsumer = !IsProfessional;
        }

        private void CheckShowOrganizations()
        {
            var organizations = Serialization.GetCachedResult<List<Organization>>(Path.Combine(Serialization.LocalStorageFolder, StringResources.MY_ORGANIZATIONS_FILE))
                ?? new List<Organization>();
            ShowOrganizations = organizations.Any();
        }

        private void CheckShowEvents()
        {
            var events = Serialization.GetCachedResult<List<EventTag>>(Path.Combine(Serialization.LocalStorageFolder, StringResources.MY_ORGANIZATIONS_FILE))
                ?? new List<EventTag>();
            ShowEvents = events.Any();
        }
    }
}
