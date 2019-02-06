using System;
using Busidex3.Analytics;
using Busidex3.DomainModels;
using Busidex3.ViewModels;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views.EditCard
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditCardMenuView
	{
	    public CardMenuVM _viewModel { get; set; }

		public EditCardMenuView (ref UserCard card)
		{
			InitializeComponent ();
            _viewModel = new CardMenuVM
            {
                SelectedCard = card, 
                ImageSize = 65
            };
            var fileName = card.DisplaySettings.CurrentFileName;
            card.DisplaySettings = new UserCardDisplay(fileName: fileName);
            BindingContext = _viewModel;

		    App.AnalyticsManager.TrackScreen(ScreenName.MyCard);
		}

        private async void EditCardImageTapped(object sender, EventArgs e)
        {
            var sc = _viewModel.SelectedCard;
            await Navigation.PushAsync(new EditCardImageView(ref sc));
        }

        private async void VisibilityTapped(object sender, EventArgs e)
        {
            var sc = _viewModel.SelectedCard;
            await Navigation.PushAsync(new EditVisibilityView(ref sc));
        }

        private async void EditContactInfoTapped(object sender, EventArgs e)
        {
            var sc = _viewModel.SelectedCard;
            await Navigation.PushAsync(new EditContactInfoView(ref sc));
        }

        private async void SearchInfoTapped(object sender, EventArgs e)
        {
            var sc = _viewModel.SelectedCard;
            await Navigation.PushAsync(new EditSearchInfoView(ref sc));
        }

        private async void TagsTapped(object sender, EventArgs e)
        {
            var sc = _viewModel.SelectedCard;
            await Navigation.PushAsync(new EditTagsView(ref sc));
        }

        private async void AddressInfoTapped(object sender, EventArgs e)
        {
            var sc = _viewModel.SelectedCard;
            await Navigation.PushAsync(new EditAddressView(ref sc));
        }
    }
}