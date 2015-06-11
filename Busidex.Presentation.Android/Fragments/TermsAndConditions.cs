
using Android.App;
using Android.OS;
using Android.Views;
using Android.Webkit;

namespace Busidex.Presentation.Android
{
	public class TermsAndConditions : DialogFragment
	{

		WebView vwTerms;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Use this to return your custom view for this Fragment
			View TCFragment = inflater.Inflate(Resource.Layout.TermsAndConditions, container, false);

			return TCFragment;
		}
		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			vwTerms = view.FindViewById<WebView> (Resource.Id.vwTerms);

			vwTerms.LoadUrl("https://www.busidex.com/#/account/terms");
		}
	}
}

