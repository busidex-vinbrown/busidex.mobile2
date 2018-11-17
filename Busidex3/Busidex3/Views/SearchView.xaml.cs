using Busidex3.Analytics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SearchView : ContentPage
	{
		public SearchView ()
		{
			InitializeComponent ();
		    App.AnalyticsManager.TrackScreen(ScreenName.Search);
		}
	}
}