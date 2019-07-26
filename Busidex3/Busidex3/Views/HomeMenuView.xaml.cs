using Busidex3.DomainModels;
using Busidex3.Services.Utils;
using Busidex3.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeMenuView : ContentPage
    {
        private readonly HomeMenuVM _viewModel = new HomeMenuVM();

        public HomeMenuView()
        {
            InitializeComponent();
            _viewModel.RowWidth = 500;
            _viewModel.RowHeight = 500;

            var events = Serialization.GetCachedResult<List<EventTag>>(Path.Combine(Serialization.LocalStorageFolder, StringResources.EVENT_LIST_FILE))
                ?? new List<EventTag>();
            _viewModel.ShowEvents = events.Any();

            var organizations = Serialization.GetCachedResult<List<Organization>>(Path.Combine(Serialization.LocalStorageFolder, StringResources.MY_ORGANIZATIONS_FILE))
                ?? new List<Organization>();
            _viewModel.ShowOrganizations = organizations.Any();

            BindingContext = _viewModel;
        }

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
    }
}