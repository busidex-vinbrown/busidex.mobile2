using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;
using Busidex3.Views;
using Busidex3.Views.Admin;
using Xamarin.Forms;

namespace Busidex3.ViewModels
{
    public class MainMenuMasterVM : INotifyPropertyChanged
    {
        public UserCard MyCard { get; set; }
        public MainMenuMenuItem ShareImage { get; set; }

        private MainMenuMenuItem _editImage;
        public MainMenuMenuItem EditImage {
            get{
                return _editImage;
            } 
            set{
                _editImage = value; 
                OnPropertyChanged(nameof(EditImage));
                }
        }

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

        private MainMenuMenuItem _adminItem;
        public MainMenuMenuItem AdminItem
        {
            get
            {
                return _adminItem;
            }
            set
            {
                _adminItem = value;
                OnPropertyChanged(nameof(AdminItem));
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

        public ImageSource ProfileImage {
            get {
                return ImageSource.FromResource("Busidex3.Resources.defaultprofile.png",
                    typeof(MainMenuMenuItem).GetTypeInfo().Assembly);
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

        private bool? _isAdmin;
        public bool? IsAdmin
        {
            get { return _isAdmin; }
            set
            {
                _isAdmin = value;
                OnPropertyChanged(nameof(IsAdmin));
            }
        }

        private string _editTitle;
        public string EditTitle
        {
            get { return _editTitle; }
            set
            {
                _editTitle = value;
                OnPropertyChanged(nameof(EditTitle));
            }
        }

        private bool _showProfileImage;
        public bool ShowProfileImage
        {
            get { return _showProfileImage; }
            set
            {
                _showProfileImage = value;
                OnPropertyChanged(nameof(ShowProfileImage));
            }
        }

        public MainMenuMasterVM()
        {
            ShareImage = new MainMenuMenuItem
            {
                Id = 0,
                Title = ViewNames.Share,
                TargetType = typeof(ShareView),
                Image = ImageSource.FromResource("Busidex3.Resources.share.png",
                    typeof(MainMenuMenuItem).GetTypeInfo().Assembly)
            };
            
            EditImage = new MainMenuMenuItem
            {
                Id = 0,
                Title = HasCard ? ViewNames.Edit : ViewNames.Add,
                TargetType = typeof(ShareView),
                Image = ImageSource.FromResource("Busidex3.Resources.editicon.png",
                    typeof(MainMenuMenuItem).GetTypeInfo().Assembly)
            };

            EventsItem = new MainMenuMenuItem
            {
                Id = 2,
                Title = ViewNames.Events,
                TargetType = typeof(EventsView),
                Image = ImageSource.FromResource("Busidex3.Resources.eventicon.png",
                    typeof(MainMenuMenuItem).GetTypeInfo().Assembly),
                IsClickable = true
            };

            MyBusidexItem = new MainMenuMenuItem
            {
                Id = 0,
                Title = ViewNames.MyBusidex,
                TargetType = typeof(MyBusidexView),
                Image = ImageSource.FromResource("Busidex3.Resources.mybusidexicon.png",
                    typeof(MainMenuMenuItem).GetTypeInfo().Assembly),
                IsClickable = true
            };

            SearchItem = new MainMenuMenuItem
            {
                Id = 1,
                Title = ViewNames.Search,
                TargetType = typeof(SearchView),
                Image = ImageSource.FromResource("Busidex3.Resources.searchicon.png",
                    typeof(MainMenuMenuItem).GetTypeInfo().Assembly),
                IsClickable = true
            };

            OrganizationsItem = new MainMenuMenuItem
            {
                Id = 3,
                Title = ViewNames.Organizations,
                TargetType = typeof(OrganizationsView),
                Image = ImageSource.FromResource("Busidex3.Resources.organizationsicon.png",
                    typeof(MainMenuMenuItem).GetTypeInfo().Assembly),
                IsClickable = true
            };

            AdminItem = new MainMenuMenuItem
            {
                Id = 4,
                Title = ViewNames.Admin,
                TargetType = typeof(AdminView),
                Image = ImageSource.FromResource("Busidex3.Resources.admin.png",
                    typeof(MainMenuMenuItem).GetTypeInfo().Assembly),
                IsClickable = true
            };

            EditTitle = HasCard ? ViewNames.Edit : ViewNames.Add;

            RefreshProfile();
        }

        public async void RefreshProfile()
        {
            var path = Path.Combine(Serialization.LocalStorageFolder, StringResources.OWNED_CARD_FILE);
            var ownedCard = Serialization.LoadData<Card>(path);
            if(ownedCard == null)
            {
                await App.LoadOwnedCard();
            }

            if (ownedCard != null)
            {
                MyCard = new UserCard(ownedCard);
                MyCard?.SetDisplay(UserCardDisplay.DisplaySetting.Thumbnail, UserCardDisplay.CardSide.Front, MyCard.Card.FrontFileName);
                HasCard = MyCard.Card.FrontFileId != Guid.Empty && MyCard.Card.FrontFileId != null;
                ShowProfileImage = !HasCard;
                OnPropertyChanged(nameof(MyCard));
            }
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
