
using System;
using Android.App;
using Android.Content;

namespace Busidex.Presentation.Android
{
	public class BaseFragment : Fragment
	{
		protected BaseApplicationResource applicationResource;

//		public override void OnCreate (Bundle savedInstanceState)
//		{
//			base.OnCreate (savedInstanceState);
//
//			// Create your fragment here
//		}
//
//		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
//		{
//			// Use this to return your custom view for this Fragment
//			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);
//
//			return base.OnCreateView (inflater, container, savedInstanceState);
//		}

		protected void RedirectToMainIfLoggedIn(){

			var cookie = applicationResource.GetAuthCookie ();
			if(cookie != null){
				var intent = new Intent(Activity.BaseContext, typeof(MainActivity));
				Redirect(intent);
			}
		}

		protected void Redirect(Intent intent){

			StartActivity(intent);

		}
	}
}

