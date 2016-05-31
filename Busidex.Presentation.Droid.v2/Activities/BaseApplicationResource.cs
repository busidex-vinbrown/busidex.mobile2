using System;
using Busidex.Mobile;
using Xamarin.Auth;
using System.Linq;
using Android.Content;
using Android.Gms.Analytics;
using System.Collections.Generic;
using Android.App;

namespace Busidex.Presentation.Droid.v2
{
	public static class BaseApplicationResource
	{
		static Context context { get; set; }

		static BaseApplicationResource ()
		{
			
		}

		public static void Init (Activity ctx)
		{
			context = ctx;
			var gai = GoogleAnalytics.GetInstance (context);
			_tracker = _tracker ?? gai.NewTracker (Resources.GOOGLE_ANALYTICS_KEY_ANDROID);

			const int DISPATCH_PERIOD = 1;

			// Optional: set Google Analytics dispatch interval to e.g. 20 seconds.
			gai.SetLocalDispatchPeriod (DISPATCH_PERIOD);
		}

		#region Authentication

		public static string GetAuthCookie ()
		{
			try {
				var account = GetAuthAccount ();
				if (account == null) {
					return null;
				}
				var cookies = account.Cookies.GetCookies (new Uri (Resources.COOKIE_URI));
				var cookie = cookies [Resources.AUTHENTICATION_COOKIE_NAME];

				return cookie.Value;
			} catch (Exception ex) {
				TrackException (ex);
				return string.Empty;
			}
		}

		public static Account GetAuthAccount ()
		{
			
			return AccountStore.Create (context).FindAccountsForService (Resources.AUTHENTICATION_COOKIE_NAME).FirstOrDefault ();
		}

		public static void SetAuthCookie (long userId, int expires = 1)
		{

			var cookieVal = Utils.EncodeUserId (userId);
			var cookie = new System.Net.Cookie (Resources.AUTHENTICATION_COOKIE_NAME, cookieVal);
			cookie.Expires = DateTime.Now.AddYears (expires);
			cookie.Value = Utils.EncodeUserId (userId);

			var container = new System.Net.CookieContainer ();
			container.SetCookies (new Uri (Resources.COOKIE_URI), cookie.ToString ());

			var account = new Account (userId.ToString (), container);

			AccountStore.Create (context).Save (account, Resources.AUTHENTICATION_COOKIE_NAME);
		}

		public static void SetRefreshCookie (string prop)
		{
			var account = GetAuthAccount ();
			if (account != null && account.Cookies != null) {

				var today = DateTime.Now;

				var expireDate = new DateTime (today.Year, today.Month, today.Day, 0, 0, 1).AddDays (1);

				if (!account.Properties.ContainsKey (prop)) {
					account.Properties.Add (prop, expireDate.ToString ());
				} else {
					account.Properties [prop] = expireDate.ToString ();
				}

				AccountStore.Create (context).Save (account, Resources.AUTHENTICATION_COOKIE_NAME);
			}
		}

		public static bool CheckRefreshDate (string prop)
		{
			var account = GetAuthAccount ();
			if (account != null && account.Cookies != null) {

				DateTime expireDate;

				if (account.Properties.ContainsKey (prop) &&
				    DateTime.TryParse (account.Properties [prop], out expireDate)) {

					return expireDate > DateTime.Now;
				}
			}
			return false;
		}

		public static void RemoveAuthCookie ()
		{
			var account = GetAuthAccount ();
			if (account != null && account.Cookies != null) {
				AccountStore.Create (context).Delete (account, Resources.AUTHENTICATION_COOKIE_NAME);
			}
		}

		#endregion

		#region Google Analytics

		public static void TrackAnalyticsEvent (string category, string label, string action, int value)
		{

			var build = new HitBuilders.EventBuilder ()
				.SetCategory (category)
				.SetLabel (label)	
				.SetAction (action)
				.SetValue (value) 
				.Build ();
			var build2 = new Dictionary<string,string> ();
			foreach (var key in build.Keys) {
				build2.Add (key, build [key]);
			}
			GATracker.Send (build2);
		}

		public static void TrackScreenView (string screen)
		{

			_tracker.SetScreenName (screen);
			_tracker.Send (new HitBuilders.ScreenViewBuilder ().Build ());
		}

		public static void TrackException (Exception ex)
		{
			try {
				var build = new HitBuilders.ExceptionBuilder ()
					.SetDescription (ex.Message)
					.SetFatal (false) // This is useful for uncaught exceptions
					.Build ();
				var build2 = new Dictionary<string,string> ();
				foreach (var key in build.Keys) {
					build2.Add (key, build [key]);
				}
				GATracker.Send (build2);
			} catch {
				Xamarin.Insights.Report (ex);
			}
		}

		static Tracker _tracker;

		static Tracker GATracker { 
			get { 
				return _tracker; 
			} 
		}

		#endregion
	}
}

