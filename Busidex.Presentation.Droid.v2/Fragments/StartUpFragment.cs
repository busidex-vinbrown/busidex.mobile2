
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Busidex.Presentation.Droid.v2
{
	public class StartUpFragment : GenericViewPagerFragment
	{
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.StartUp, container, false);

			//var cookie = applicationResource.GetAuthCookie ();
			//var loaderView = view.FindViewById (Resource.Id.fragment_holder);
			//loaderView.Visibility = cookie == null ? ViewStates.Gone : ViewStates.Visible;

			var btnLogin = view.FindViewById<Button> (Resource.Id.btnConnect);
			var btnStart = view.FindViewById<Button> (Resource.Id.btnStart);

			btnLogin.Click += delegate {
				((MainActivity)Activity).ShowLogin();
			};

			btnStart.Click += delegate {
				((MainActivity)Activity).ShowRegistration();
			};

			return view;
		}
	}
}

