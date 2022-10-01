﻿using Busidex.Http;
using Busidex.Http.Utils;
using Busidex.Models.Domain;
using Busidex.Resources.String;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Busidex.Professional.ViewModels
{
    public class HomeMenuVM : BaseViewModel
    {
        public ImageSource MyBusidexItem { get; set; }
        public ImageSource SearchItem { get; set; }
        public ImageSource ProfileItem { get; set; }
        public ImageSource FaqItem { get; set; }
        public ImageSource ManageCardItem { get; set; }
        public ImageSource PresentationLinkItem { get; set; }
        public ImageSource ShareItem { get; set; }
        public ImageSource OrganizationsItem { get; set; }

        //private MainMenuMenuItem _shareItem;
        //public MainMenuMenuItem ShareItem
        //{
        //    get => _shareItem;
        //    set
        //    {
        //        _shareItem = value;
        //        OnPropertyChanged(nameof(ShareItem));
        //    }
        //}

        //private MainMenuMenuItem _organizationsItem;
        //public MainMenuMenuItem OrganizationsItem
        //{
        //    get => _organizationsItem;
        //    set
        //    {
        //        _organizationsItem = value;
        //        OnPropertyChanged(nameof(OrganizationsItem));
        //    }
        //}

        //private MainMenuMenuItem _eventsItem;
        //public MainMenuMenuItem EventsItem
        //{
        //    get => _eventsItem;
        //    set
        //    {
        //        _eventsItem = value;
        //        OnPropertyChanged(nameof(EventsItem));
        //    }
        //}

        private bool _showEvents;
        public bool ShowEvents {
            get => _showEvents;
            set {
                _showEvents = value;
                OnPropertyChanged(nameof(ShowEvents));
            }
        }

        private bool _showOrganizations;
        public bool ShowOrganizations {
            get => _showOrganizations;
            set {
                _showOrganizations = value;
                OnPropertyChanged(nameof(ShowOrganizations));
            }
        }

        private ImageSource _backgroundImage;
        public ImageSource BackgroundImage {
            get => _backgroundImage;
            set {
                _backgroundImage = value;
                OnPropertyChanged(nameof(BackgroundImage));
            }
        }

        public async Task<bool> LaunchBrowser(string _url)
        {
            string url;
            if (string.IsNullOrEmpty(_url)) return false;

            url = !_url.StartsWith("http", StringComparison.Ordinal)
                ? "http://" + _url
                : _url;
            try
            {
                await Launcher.OpenAsync(new Uri(url));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> LaunchPresentation()
        {
            var settings = await SettingsHttpService.GetSystemSettings();
            var setting = settings.FirstOrDefault(s => s.Setting == "PresentationLink");
            if (setting != null)
            {
                var url = setting.Value;
                await Launcher.OpenAsync(new Uri(url));
                return true;
            }
            return false;
        }

        public async void SendContactUsEmail()
        {
            var settings = await SettingsHttpService.GetSystemSettings();
            var email = settings.FirstOrDefault(s => s.Setting == "ContactEmail");
            if (email != null)
            {
                await Launcher.OpenAsync(new Uri($"mailto:{email.Value}"));
            }
        }

        public HomeMenuVM()
        {
            App.OnOrganizationsLoaded += CheckShowOrganizations;

            App.OnEventsLoaded += CheckShowEvents;

            MyBusidexItem = ImageSource.FromResource("Busidex.Resources.Images.mybusidexicon.png",
                    typeof(Resources.Images.ImageLoader).GetTypeInfo().Assembly);

            SearchItem = ImageSource.FromResource("Busidex.Resources.Images.searchicon.png",
                    typeof(Resources.Images.ImageLoader).GetTypeInfo().Assembly);

            PresentationLinkItem = ImageSource.FromResource("Busidex.Resources.Images.presentationicon.png",
                    typeof(Resources.Images.ImageLoader).GetTypeInfo().Assembly);

            ProfileItem = ImageSource.FromResource("Busidex.Resources.Images.manageaccounticon.png",
                    typeof(Resources.Images.ImageLoader).GetTypeInfo().Assembly);

            ManageCardItem = ImageSource.FromResource("Busidex.Resources.Images.editcardmenuicon.png",
                    typeof(Resources.Images.ImageLoader).GetTypeInfo().Assembly);

            FaqItem = ImageSource.FromResource("Busidex.Resources.Images.faqicon.png",
                    typeof(Resources.Images.ImageLoader).GetTypeInfo().Assembly);

            ShareItem = ImageSource.FromResource("Busidex.Resources.Images.shareicon.png",
                    typeof(Resources.Images.ImageLoader).GetTypeInfo().Assembly);

            OrganizationsItem = ImageSource.FromResource("Busidex.Resources.Images.organizationsicon.png",
                    typeof(Resources.Images.ImageLoader).GetTypeInfo().Assembly);

            var path = Path.Combine(Serialization.LocalStorageFolder, StringResources.OWNED_CARD_FILE);
            var ownedCard = Serialization.LoadData<Card>(path);

            var src = "Busidex.Resources.Images.home_professional.png";

            BackgroundImage = ImageSource.FromResource(src, typeof(Resources.Images.ImageLoader).GetTypeInfo().Assembly);

            if (ownedCard == null)
            {
                Task.Factory.StartNew(async () =>
                {
                    ownedCard = await App.LoadOwnedCard();
                    //if (ownedCard != null)
                    //{
                    //    Device.BeginInvokeOnMainThread(() =>
                    //    {
                    //        CheckHasCard(ownedCard);
                    //    });
                    //}
                });
            }
            //else
            //{
            //    //CheckHasCard(ownedCard);
            //}

            CheckShowOrganizations();
            CheckShowEvents();
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
