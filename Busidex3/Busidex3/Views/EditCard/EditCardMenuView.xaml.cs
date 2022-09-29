using System;
using System.Collections.Generic;
using System.IO;
using Busidex.Http.Utils;
using Busidex.Models.Analytics;
using Busidex.Models.Domain;
using Busidex.Resources.String;
using Busidex3.ViewModels;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views.EditCard
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditCardMenuView
	{
	    public CardMenuVM _viewModel { get; set; }

		public EditCardMenuView (ref UserCard uc)
		{
			InitializeComponent ();
            Init(uc);
        }

        private async void Init(UserCard uc)
        {
            if (uc != null && _viewModel == null)
            {
                try
                {
                    _viewModel = new CardMenuVM
                    {
                        SelectedCard = uc,
                        ImageSize = 65
                    };
                    // uc.DisplaySettings = new UserCardDisplay(fileName: uc.Card.FrontFileName);
                    _viewModel.CheckHasCard();
                    BindingContext = _viewModel;
                    _viewModel.SetViewHeightForOrientation(_viewModel.SelectedCard.Card.FrontOrientation);
                }
                catch(Exception ex)
                {
                    var err = ex;
                }
                this.ctrlCardImageHeader.IsVisible = _viewModel.ShowCardImage;
            }
            else
            {
                await Navigation.PopAsync();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var card = Serialization.LoadData<Card>(Path.Combine(Serialization.LocalStorageFolder, StringResources.OWNED_CARD_FILE));
            var uc = new UserCard
            {
                ExistsInMyBusidex = true,
                UserId = card.OwnerId.Value,
                Card = card,
                CardId = card.CardId
            };
            Init(uc);

            App.AnalyticsManager.TrackScreen(ScreenName.MyCard);
        }

        private CardVM GetViewModel()
        {
            var sc = _viewModel.SelectedCard;
            var myBusidex = Serialization.LoadData<List<UserCard>> (Path.Combine (Serialization.LocalStorageFolder, StringResources.MY_BUSIDEX_FILE));
            return new CardVM(ref sc, ref myBusidex);
        }

        private async void EditCardImageTapped(object sender, EventArgs e)
        {
            //var vm = GetViewModel();
            
            //await Navigation.PushAsync(new EditCardImageView(ref vm));
        }

        private async void VisibilityTapped(object sender, EventArgs e)
        {
            var vm = GetViewModel();
            await Navigation.PushAsync(new EditVisibilityView(ref vm));
        }

        private async void EditContactInfoTapped(object sender, EventArgs e)
        {
            var vm = GetViewModel();
            await Navigation.PushAsync(new EditContactInfoView(ref vm));
        }

        private async void SearchInfoTapped(object sender, EventArgs e)
        {
            var vm = GetViewModel();
            await Navigation.PushAsync(new EditSearchInfoView(ref vm));
        }

        private async void TagsTapped(object sender, EventArgs e)
        {
            var vm = GetViewModel();
            await Navigation.PushAsync(new EditTagsView(ref vm));
        }

        private async void AddressInfoTapped(object sender, EventArgs e)
        {
            var vm = GetViewModel();
            await Navigation.PushAsync(new EditAddressView(ref vm));
        }

        private async void ExternalLinksTapped(object sender, EventArgs e)
        {
            var vm = GetViewModel();
            await Navigation.PushAsync(new EditExternalLinksView(ref vm));
        }

        protected override bool OnBackButtonPressed()
        {
            //App.LoadHomePage();
            Navigation.PopToRootAsync();
            return true;
        }
    }
}