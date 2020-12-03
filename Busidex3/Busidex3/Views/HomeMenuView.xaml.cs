using Busidex.Http;
using Busidex.Http.Utils;
using Busidex.Models.Domain;
using Busidex.Resources.String;
using Busidex3.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeMenuView
    {
        private readonly HomeMenuVM _viewModel = new HomeMenuVM();

        public HomeMenuView()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            //imgBackground.HeightRequest = imgBackgroundProf.HeightRequest = DeviceDisplay.MainDisplayInfo.Height;
            //imgBackground.WidthRequest = imgBackgroundProf.WidthRequest = DeviceDisplay.MainDisplayInfo.Width;
            //imgBackground.Margin = imgBackgroundProf.Margin = _viewModel.IsProfessional
            //? new Thickness(0)
            //    : new Thickness(-60, 0,0,0);
            imgBackground.HeightRequest = DeviceDisplay.MainDisplayInfo.Height;
            imgBackground.WidthRequest = DeviceDisplay.MainDisplayInfo.Width;
            
            imgBackground.Margin = new Thickness(-60, 0, 0, 0);
            BindingContext = _viewModel;

            var quickSharePath = Path.Combine(Serialization.LocalStorageFolder, StringResources.QUICKSHARE_LINK);

            if (string.IsNullOrEmpty(Security.AuthToken))
            {
                RedirectToLogin();
            }
            else if (File.Exists(quickSharePath))
            {
                var quickShareLink = Serialization.LoadData<QuickShareLink>(quickSharePath);
                Task.Factory.StartNew(async () =>
                {
                    var uc = await SaveFromUrl(quickShareLink);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var page = uc.Card?.FrontFileId != Guid.Empty && uc.Card?.FrontFileId != null
                            ? new QuickShareView(uc, quickShareLink.DisplayName, quickShareLink.PersonalMessage) as Page
                            : new ConfirmCardOwnerView(uc, quickShareLink.DisplayName, quickShareLink.PersonalMessage) as Page;

                        Navigation.PushAsync(page);
                        NavigationPage.SetHasNavigationBar(page, false);
                    });
                });
            }
        }

        private void RedirectToLogin()
        {
            var page = (Page)Activator.CreateInstance(typeof(Login));
            NavigationPage.SetHasNavigationBar(page, false);
            Navigation.PushAsync(page);
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
                if (card.OwnerId.HasValue)
                {
                    await sharedCardService.AcceptQuickShare(card, Security.CurrentUser.Email, link.From, link.PersonalMessage);
                }

                Serialization.RemoveQuickShareLink();

                var userCard = new UserCard
                {
                    Card = card,
                    CardId = card.CardId,
                    ExistsInMyBusidex = true,
                    OwnerId = card.OwnerId,
                    UserId = Security.CurrentUser.UserId,
                    Notes = string.Empty,
                };
                return userCard;
            }
            else
            {
                return null;
            }
        }

        //private void AppLoaded()
        //{
        //    stkMenu.IsVisible = imgBackground.IsVisible = true;
        //    stkLoading.IsVisible = false;
        //    App.OnAppLoaded -= AppLoaded;
        //}

        private async void stkShare_Tapped(object sender, EventArgs e)
        {
            var path = Path.Combine(Serialization.LocalStorageFolder, StringResources.OWNED_CARD_FILE);
            var ownedCard = Serialization.LoadData<Card>(path);
            if (ownedCard == null)
            {
                ownedCard = await App.LoadOwnedCard();
            }
            var myCard = new UserCard(ownedCard);

            //var orientation = myCard.Card.FrontOrientation == "H"
            //    ? UserCardDisplay.CardOrientation.Horizontal
            //    : UserCardDisplay.CardOrientation.Vertical;

            //myCard.DisplaySettings = new UserCardDisplay(
            //    UserCardDisplay.DisplaySetting.Detail, 
            //    orientation, 
            //    myCard.Card.FrontFileName);
            var page = new ShareView(ref myCard);
            page.Title = ViewNames.Share + " My Card";
            await Navigation.PushAsync(page);
        }

        private async void stkSearch_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SearchView());
        }

        private async void stkMyBusidex_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyBusidexView());
        }

        private async void stkOrganizations_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new OrganizationsView());
        }

        private async void stkEvents_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EventsView());
        }

        protected override bool OnBackButtonPressed()
        {
            //App.LoadHomePage();
            return true;
        }

        private void stkManageAccount_Tapped(object sender, EventArgs e)
        {            
            var needsSetup = string.IsNullOrEmpty(Security.AuthToken);
            var page = new MyProfileView();
            Navigation.PushAsync(page);
            NavigationPage.SetHasNavigationBar(page, !needsSetup);            
        }

        private void stkContactUs_Tapped(object sender, EventArgs e)
        {
            _viewModel.SendContactUsEmail();
        }

        private async void stkFaq_Tapped(object sender, EventArgs e)
        {
            await _viewModel.LaunchBrowser(StringResources.FAQ_URL);
        }

        private async void stkPresentation_Tapped(object sender, EventArgs e)
        {
            await _viewModel.LaunchPresentation();
        }
    }
}