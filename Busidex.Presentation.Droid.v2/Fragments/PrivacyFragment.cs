
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Webkit;
using Busidex.Mobile;

namespace Busidex.Presentation.Droid.v2
{
	public class PrivacyFragment : GenericViewPagerFragment
	{
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.PrivacyPolicy, container, false);

			var webView = view.FindViewById<WebView> (Resource.Id.vwPrivacy);
			webView.LoadUrl (Busidex.Mobile.Resources.PRIVACY_URL);

			var btnTermsAndConditions = view.FindViewById<Button> (Resource.Id.btnTermsAndConditions);
			btnTermsAndConditions.Click += delegate {

				var fragment = new TermsAndConditionsFragment ();

				((MainActivity)Activity).ShowTerms (fragment);
			};

			var btnBack = view.FindViewById<Button> (Resource.Id.btnBack);
			btnBack.Click += delegate {
				((MainActivity)Activity).LoadFragment (new ProfileFragment (UISubscriptionService.CurrentUser));
			};
			return view;
		}
	}
}

