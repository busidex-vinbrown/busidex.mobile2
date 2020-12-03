using Busidex.Models.Analytics;
using Busidex.Models.Constants;
using Busidex.Models.Domain;
using Busidex.SharedUI;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CardImageView : ContentPage
	{
	    private int Taps = 0;
	    private readonly UserCard _currentCard;
		private CardImageVM _viewModel;

		public CardImageView (ref UserCard uc)
		{
			InitializeComponent ();

		    NavigationPage.SetHasNavigationBar (this, false);

			_viewModel.DisplaySettings = new UserCardDisplay(
		        DisplaySetting.FullScreen,
		        uc.Card.FrontOrientation == "H"
		            ? CardOrientation.Horizontal
		            : CardOrientation.Vertical,
		        uc.Card.FrontFileName,
				uc.Card.FrontOrientation);

		    BindingContext = uc;
		    _currentCard = uc;
		    
            CardImage.OnCardImageClicked += CardImage_OnCardImageClicked;
		    App.AnalyticsManager.TrackEvent(EventCategory.UserInteractWithCard, EventAction.CardImageViewed, uc.Card.Name ?? uc.Card.CompanyName);
		}

        private async void CardImage_OnCardImageClicked(IUserCardDisplay ucd)
        {
            if (Taps == 0)
            {
                Taps++;
				if (_currentCard.Card.HasBackImage)
				{
					_viewModel.DisplaySettings = new UserCardDisplay(
						DisplaySetting.FullScreen,
						_currentCard.Card.BackOrientation == "H"
							? CardOrientation.Horizontal
							: CardOrientation.Vertical,
						_currentCard.Card.BackFileName);
					return;
				}
            }
            await Navigation.PopAsync();
        }

        protected override void OnDisappearing()
	    {
	        base.OnDisappearing();
	        NavigationPage.SetHasNavigationBar (this, true);
	        // _currentCard.SetDisplay(UserCardDisplay.DisplaySetting.Detail, UserCardDisplay.CardSide.Front, _currentCard.Card.FrontFileName);
            App.DisplayManager.SetOrientation(CardOrientation.Vertical);
	    }
	}
}