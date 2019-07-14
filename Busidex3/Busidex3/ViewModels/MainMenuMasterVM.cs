﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;
using Busidex3.Views;
using Xamarin.Forms;

namespace Busidex3.ViewModels
{
    public class MainMenuMasterVM : INotifyPropertyChanged
    {
        public ObservableCollection<MainMenuMenuItem> MenuItems { get; set; }
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

            EditTitle = HasCard ? ViewNames.Edit : ViewNames.Add;

            MenuItems = new ObservableCollection<MainMenuMenuItem>(new[]
            {
                new MainMenuMenuItem { 
                    Id = -1, 
                    Title = string.Empty, 
                    TargetType = null, 
                    Image = null,
                    IsSeparator = true
                },
                new MainMenuMenuItem { 
                    Id = 0, 
                    Title = ViewNames.MyBusidex, 
                    TargetType = typeof(MyBusidexView), 
                    Image = ImageSource.FromResource("Busidex3.Resources.mybusidexicon.png",
                    typeof(MainMenuMenuItem).GetTypeInfo().Assembly),
                    IsClickable = true
                },
                new MainMenuMenuItem { 
                    Id = 1, 
                    Title = ViewNames.Search, 
                    TargetType = typeof(SearchView), 
                    Image = ImageSource.FromResource("Busidex3.Resources.searchicon.png",
                    typeof(MainMenuMenuItem).GetTypeInfo().Assembly),
                    IsClickable = true
                },
                new MainMenuMenuItem { 
                    Id = 2, 
                    Title = ViewNames.Events, 
                    TargetType = typeof(EventsView), 
                    Image = ImageSource.FromResource("Busidex3.Resources.eventicon.png",
                    typeof(MainMenuMenuItem).GetTypeInfo().Assembly),
                    IsClickable = true
                },
                new MainMenuMenuItem {
                    Id = 3, 
                    Title = ViewNames.Organizations, 
                    TargetType = typeof(OrganizationsView), 
                    Image = ImageSource.FromResource("Busidex3.Resources.organizationsicon.png",
                    typeof(MainMenuMenuItem).GetTypeInfo().Assembly),
                    IsClickable = true
                },
            });
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
