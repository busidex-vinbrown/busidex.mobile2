using System.IO;
using Busidex3.Analytics;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;
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

		public CardImageView (ref UserCard uc)
		{
			//InitializeComponent ();

		    NavigationPage.SetHasNavigationBar (this, false);

		    uc.DisplaySettings = new UserCardDisplay(
		        UserCardDisplay.DisplaySetting.FullScreen,
		        uc.Card.FrontOrientation == "H"
		            ? UserCardDisplay.CardOrientation.Horizontal
		            : UserCardDisplay.CardOrientation.Vertical,
		        uc.Card.FrontFileName);
		    BindingContext = uc;
		    _currentCard = uc;
		    
            //CardImage.OnCardImageClicked += CardImage_OnCardImageClicked;
		    App.AnalyticsManager.TrackEvent(EventCategory.UserInteractWithCard, EventAction.CardImageViewed, uc.Card.Name ?? uc.Card.CompanyName);
		}

        private async void CardImage_OnCardImageClicked(UserCard uc)
        {
            if (Taps == 0)
            {
                Taps++;
                if (_currentCard.Card.HasBackImage)
                {
                    _currentCard.SetDisplay(
                        UserCardDisplay.DisplaySetting.FullScreen,
                        UserCardDisplay.CardSide.Back,
                        _currentCard.Card.BackFileName
                    );
                    return;
                }
            }
            await Navigation.PopAsync();
        }

        protected override void OnDisappearing()
	    {
	        base.OnDisappearing();
	        NavigationPage.SetHasNavigationBar (this, true);
	        _currentCard.SetDisplay(UserCardDisplay.DisplaySetting.Detail, UserCardDisplay.CardSide.Front, _currentCard.Card.FrontFileName);
            App.DisplayManager.SetOrientation(UserCardDisplay.CardOrientation.Vertical);
	    }
	}
}