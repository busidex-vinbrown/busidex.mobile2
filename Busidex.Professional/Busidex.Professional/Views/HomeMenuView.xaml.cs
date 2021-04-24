using Busidex.Http;
using Busidex.Http.Utils;
using Busidex.Models.Domain;
using Busidex.Professional.ViewModels;
using Busidex.Professional.Views.EditCard;
using Busidex.Resources.String;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex.Professional.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeMenuView
    {
        private readonly HomeMenuVM _viewModel = new HomeMenuVM();

        public HomeMenuView()
        {
            InitializeComponent();

            Shell.SetNavBarIsVisible(this, false);

            imgBackground.HeightRequest = DeviceDisplay.MainDisplayInfo.Height;
            imgBackground.WidthRequest = DeviceDisplay.MainDisplayInfo.Width;
            
            imgBackground.Margin = new Thickness(-60, 0, 0, 0);
            BindingContext = _viewModel;

            var quickSharePath = Path.Combine(Serialization.LocalStorageFolder, StringResources.QUICKSHARE_LINK);

            if (File.Exists(quickSharePath))
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

        private async void stkShare_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"//home/share");
        }

        private async void stkSearch_Tapped(object sender, EventArgs e)
        {
            //await Shell.Current.GoToAsync($"//home/search");
            await Navigation.PushAsync(new SearchView());
        }

        private async void stkMyBusidex_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"/home/mybusidex");
        }

        private async void stkOrganizations_Tapped(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new OrganizationsView());
        }

        private async void stkEvents_Tapped(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new EventsView());
        }

        protected override bool OnBackButtonPressed()
        {
            //App.LoadHomePage();
            return true;
        }

        private async void stkManageAccount_Tapped(object sender, EventArgs e)
        {            
            await Shell.Current.GoToAsync("account");
        }

        private async void stkManageCard_Tapped(object sender, EventArgs e)
        {
            var card = await App.LoadOwnedCard();
            var uc = new UserCard(card);
            var page = new EditCardMenuView(ref uc);
            await Shell.Current.Navigation.PushAsync(page);
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