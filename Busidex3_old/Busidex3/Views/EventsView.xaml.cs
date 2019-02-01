using Busidex3.Analytics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EventsView : ContentPage
	{
		public EventsView ()
		{
			InitializeComponent ();
		    App.AnalyticsManager.TrackScreen(ScreenName.Events);
		}
	}
}