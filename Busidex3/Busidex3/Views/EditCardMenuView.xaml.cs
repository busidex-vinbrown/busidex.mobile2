using Busidex3.Analytics;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditCardMenuView : ContentPage
	{
	    public CardVM _viewModel { get; set; }

		public EditCardMenuView ()
		{
			InitializeComponent ();
		    App.AnalyticsManager.TrackScreen(ScreenName.MyCard);
		}
	}
}