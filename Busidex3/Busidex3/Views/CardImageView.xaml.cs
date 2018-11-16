using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Busidex3.Analytics;
using Busidex3.DomainModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CardImageView : ContentPage
	{
		public CardImageView (UserCard uc)
		{
			InitializeComponent ();

		    BindingContext = uc;

		    var analyticsManager = DependencyService.Get<IAnalyticsManager>();
		    analyticsManager.InitWithId();
		    analyticsManager.TrackEvent(EventCategory.UserInteractWithCard, EventAction.CardImageViewed, uc.Card.Name ?? uc.Card.CompanyName);

		}

	    private void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
	    {
	        
	    }
	}
}