
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Busidex.Presentation.Android
{
	public class StartUpFragment : BaseFragment
	{
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			base.OnCreateView (inflater, container, savedInstanceState);

			var view = inflater.Inflate (Resource.Layout.StartUp, container, false);

			var cookie = applicationResource.GetAuthCookie ();
			var loaderView = view.FindViewById (Resource.Id.fragment_holder);
			loaderView.Visibility = cookie == null ? global::Android.Views.ViewStates.Gone : global::Android.Views.ViewStates.Visible;
//			if (this.ActionBar != null) {
//				this.ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
//			}

			var btnLogin = view.FindViewById<Button> (Resource.Id.btnConnect);
			var btnStart = view.FindViewById<Button> (Resource.Id.btnStart);

			btnLogin.Click += delegate {
				//StartActivity(new Intent(this, typeof(LoginActivity)));
				Redirect(new LoginFragment());
			};

			btnStart.Click += delegate {
				//StartActivity(new Intent(this.Activity, typeof(ProfileActivity)));	
				Redirect(new LoginFragment());
			};

			return view;
		}
	}
}

