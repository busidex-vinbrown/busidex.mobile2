using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Webkit;
using Busidex.Mobile;

namespace Busidex.Presentation.Droid.v2
{
	public class TermsAndConditionsFragment : GenericViewPagerFragment
	{
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.TermsAndConditions, container, false);

			var webView = view.FindViewById<WebView> (Resource.Id.vwTerms);
			webView.LoadUrl (Busidex.Mobile.Resources.TERMS_AND_CONDITIONS_URL);

			var btnPrivacy = view.FindViewById<Button> (Resource.Id.btnPrivacy);
			btnPrivacy.Click += delegate {
				var fragment = new PrivacyFragment ();
				((MainActivity)Activity).ShowPrivacy (fragment);
			};

			var btnBack = view.FindViewById<Button> (Resource.Id.btnBack);
			btnBack.Click += delegate {
				((MainActivity)Activity).LoadFragment (new ProfileFragment (UISubscriptionService.CurrentUser));
			};

			return view;
		}
	}
}

