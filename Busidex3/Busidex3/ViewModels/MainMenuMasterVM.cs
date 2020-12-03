using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Busidex.Http.Utils;
using Busidex.Models.Domain;
using Busidex.Resources.String;
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

        private MainMenuMenuItem _homeItem;
        public MainMenuMenuItem HomeItem
        {
            get
            {
                return _homeItem;
            }
            set
            {
                _homeItem = value;
                OnPropertyChanged(nameof(HomeItem));
            }
        }

        public ImageSource ProfileImage {
            get {
                return ImageSource.FromResource("Busidex.Resources.Images.defaultprofile.png",
                    typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
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

        private UserCardDisplay _displaySettings { get; set; }
        public UserCardDisplay DisplaySettings { 
            get => _displaySettings; 
            set {
                _displaySettings = value;
                OnPropertyChanged(nameof(DisplaySettings));
            }
        }

        public MainMenuMasterVM()
        {
            ShareImage = new MainMenuMenuItem
            {
                Id = 0,
                Title = ViewNames.Share + " My Card",
                TargetType = typeof(ShareView),
                Image = ImageSource.FromResource("Busidex.Resources.Images.share.png",
                    typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly)
            };
            
            EditImage = new MainMenuMenuItem
            {
                Id = 0,
                Title = HasCard ? ViewNames.Edit : ViewNames.Add,
                TargetType = typeof(ShareView),
                Image = ImageSource.FromResource("Busidex.Resources.Images.editicon.png",
                    typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly)
            };

            AdminItem = new MainMenuMenuItem
            {
                Id = 4,
                Title = ViewNames.Admin,
                TargetType = typeof(AdminView),
                Image = ImageSource.FromResource("Busidex.Resources.Images.admin.png",
                    typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly),
                IsClickable = true
            };

            HomeItem = new MainMenuMenuItem
            {
                Id = 5,
                Title = ViewNames.Home,
                TargetType = typeof(AdminView),
                Image = ImageSource.FromResource("Busidex.Resources.Images.home.png",
                    typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly),
                IsClickable = true
            };

            EditTitle = HasCard ? ViewNames.Edit : ViewNames.Add;

            ShowProfileImage = true;

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
                // MyCard?.SetDisplay(UserCardDisplay.DisplaySetting.Thumbnail, UserCardDisplay.CardSide.Front, MyCard.Card.FrontFileName);
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
