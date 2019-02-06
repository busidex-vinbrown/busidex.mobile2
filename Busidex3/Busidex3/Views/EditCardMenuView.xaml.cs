using Busidex3.Analytics;
using Busidex3.DomainModels;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditCardMenuView : ContentPage
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
	}
}