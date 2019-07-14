﻿using System;
using System.IO;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services;
using Busidex3.Services.Utils;
using Busidex3.ViewModels;
using Busidex3.Views.EditCard;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMenu
    {
        public MainMenu(bool quickShare = false)
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
            MasterPage.OnLogout += RedirectToLogin;
            MasterPage.OnShareClicked += MasterPage_OnShareClicked;
            MasterPage.OnCardEditClicked += MasterPage_OnCardEditClicked;
            MasterPage.OnProfileClicked += MasterPage_OnProfileClicked;
            this.IsPresentedChanged += MainMenu_IsPresentedChanged;

            var quickSharePath = Path.Combine(Serialization.LocalStorageFolder, StringResources.QUICKSHARE_LINK);


            if (string.IsNullOrEmpty(Security.AuthToken))
            {
                RedirectToStartup();
            }
            else if (File.Exists(quickSharePath))
            {
                var quickShareLink =  Serialization.LoadData<QuickShareLink>(quickSharePath);
                var uc = SaveFromUrl(quickShareLink).ContinueWith(response =>
                {
                    var page = new QuickShareView(response.Result, quickShareLink.DisplayName, quickShareLink.PersonalMessage);

                    Detail = page;
                    IsPresented = false;
                    IsGestureEnabled = false;
                    NavigationPage.SetHasNavigationBar(Detail, false);
                });                
            }
            else
            {
                var page = (Page)Activator.CreateInstance(typeof(MyBusidexView));
                page.Title = ViewNames.MyBusidex;

                Detail = new NavigationPage(page);
                IsPresented = false;
            }
        }

        private async Task<UserCard> SaveFromUrl(QuickShareLink link)
        {
            var cardService = new CardHttpService();
            var result = await cardService.GetCardById(link.CardId);
            if (result.Success)
            {
                var card = new Card(result.Model);

                var myBusidexService = new MyBusidexHttpService();
                await myBusidexService.AddToMyBusidex(card.CardId);

                var sharedCardService = new SharedCardHttpService();
                await sharedCardService.AcceptQuickShare(card, Security.CurrentUser.Email, link.From, link.PersonalMessage);
                Serialization.RemoveQuickShareLink();

                var orientation = card.FrontOrientation == "H" ? UserCardDisplay.CardOrientation.Horizontal : UserCardDisplay.CardOrientation.Vertical;
                var userCard = new UserCard
                {
                    Card = card,
                    CardId = card.CardId,
                    ExistsInMyBusidex = true,
                    OwnerId = card.OwnerId,
                    UserId = card.OwnerId.GetValueOrDefault(),
                    Notes = string.Empty,
                    DisplaySettings = new UserCardDisplay(UserCardDisplay.DisplaySetting.Detail, orientation, card.FrontFileName)
                };
                return userCard;
            }
            else
            {
                return null;
            }
        }

        private void MasterPage_OnProfileClicked()
        {
            App.LoadProfilePage();
        }

        private void MasterPage_OnCardEditClicked(ref UserCard card)
        {
            var page = new EditCardMenuView(ref card) {Title = "Edit My Card"};
            page.Title = "Edit My Card";

            Detail = new NavigationPage(page);
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }

        private void MasterPage_OnShareClicked(ref UserCard card)
        {
            var page = new ShareView(ref card) {Title = "Share My Card"};

            Detail = new NavigationPage(page);
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }

        private void MainMenu_IsPresentedChanged(object sender, EventArgs e)
        {
            if (IsPresented)
            {
                MasterPage.RefreshProfile();
            }
        }

        private void RedirectToStartup()
        {
            var page = (Page)Activator.CreateInstance(typeof(Startup));
            Detail = page;
            IsPresented = false;
            NavigationPage.SetHasNavigationBar (Detail, false);
            IsGestureEnabled = false;
        }

        private void RedirectToLogin()
        {
            var page = (Page)Activator.CreateInstance(typeof(Login));
            Detail = page;
            IsPresented = false;
            NavigationPage.SetHasNavigationBar(Detail, false);
            IsGestureEnabled = false;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (!(e.SelectedItem is MainMenuMenuItem item))
                return;

            var page = (Page)Activator.CreateInstance(item.TargetType);
            page.Title = item.Title;

            Detail = new NavigationPage(page);
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }
    }
}