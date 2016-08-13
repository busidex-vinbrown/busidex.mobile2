using System;
using System.Linq;
using Busidex.Mobile;
using Foundation;
using UIKit;

namespace Busidex.Presentation.iOS
{
	public class Application
	{
		//public static List<UserCard> MyBusidex { get; set; }
		public static UINavigationController MainController { get; set; }
		public static bool LoadingSharedCard { get; set; }
		public static LoadingOverlay Overlay;

		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			Xamarin.Insights.Initialize (XamarinInsights.ApiKey);
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "AppDelegate");
		}

		public static NSHttpCookie GetAuthCookie ()
		{

			var cookie = NSHttpCookieStorage.SharedStorage.Cookies.SingleOrDefault (c => c.Name == Resources.AUTHENTICATION_COOKIE_NAME);

			if (cookie == null) {
				return null;
			}
			var expireDate = NSDateToDateTime (cookie.ExpiresDate);

			return (expireDate > DateTime.Now) ? cookie : null;
		}

		public static NSHttpCookie SetAuthCookie (long userId)
		{
			var nCookie = new System.Net.Cookie ();
			nCookie.Name = Resources.AUTHENTICATION_COOKIE_NAME;
			DateTime expiration = DateTime.Now.AddYears (1);
			nCookie.Expires = expiration;
			nCookie.Value = Utils.EncodeUserId (userId);
			var cookie = new NSHttpCookie (nCookie);

			NSHttpCookieStorage.SharedStorage.SetCookie (cookie);

			UISubscriptionService.AuthToken = cookie.Value;

			return cookie;
		}

		public static void RemoveAuthCookie ()
		{

			var nCookie = new System.Net.Cookie ();
			nCookie.Name = Resources.AUTHENTICATION_COOKIE_NAME;
			nCookie.Expires = DateTime.Now.AddDays (-2);
			var cookie = new NSHttpCookie (nCookie);
			NSHttpCookieStorage.SharedStorage.SetCookie (cookie);

			Utils.RemoveCacheFiles ();
		}

		public static DateTime NSDateToDateTime (NSDate date)
		{
			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime (
									 new DateTime (2001, 1, 1, 0, 0, 0));
			return reference.AddSeconds (date.SecondsSinceReferenceDate);
		}
	}
}
