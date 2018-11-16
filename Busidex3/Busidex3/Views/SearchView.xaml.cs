using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		    var analyticsManager = DependencyService.Get<IAnalyticsManager>();
		    analyticsManager.InitWithId();
		    analyticsManager.TrackScreen(ScreenName.Search);
		}
	}
}