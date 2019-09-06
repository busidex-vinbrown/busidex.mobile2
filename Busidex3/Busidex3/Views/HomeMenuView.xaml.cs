using Busidex3.DomainModels;
using Busidex3.Services.Utils;
using Busidex3.ViewModels;
using System;
using System.IO;
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

            imgBackground.HeightRequest = imgBackgroundProf.HeightRequest = DeviceDisplay.MainDisplayInfo.Height;
            imgBackground.WidthRequest = imgBackgroundProf.WidthRequest = DeviceDisplay.MainDisplayInfo.Width;
            imgBackground.Margin = imgBackgroundProf.Margin = _viewModel.IsProfessional
            ? new Thickness(0)
                : new Thickness(-60, 0,0,0);

            BindingContext = _viewModel;
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

            var orientation = myCard.Card.FrontOrientation == "H"
                ? UserCardDisplay.CardOrientation.Horizontal
                : UserCardDisplay.CardOrientation.Vertical;

            myCard.DisplaySettings = new UserCardDisplay(
                UserCardDisplay.DisplaySetting.Detail, 
                orientation, 
                myCard.Card.FrontFileName);
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
    }
}