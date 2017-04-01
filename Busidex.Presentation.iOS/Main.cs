using System;
using System.Linq;
using System.Threading.Tasks;
using Busidex.Mobile;
using Foundation;
using UIKit;

namespace Busidex.Presentation.iOS
{
	public class Application
	{
		public static UINavigationController MainController { get; set; }
		public static bool LoadingSharedCard { get; set; }
		public static LoadingOverlay Overlay;

		public const int APP_VERSION = 554;

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

		public static void SetRefreshCookie (string name)
		{
			try {
				var user = NSUserDefaults.StandardUserDefaults;
				DateTime nextRefresh = new DateTime (DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1).AddDays (1);
				user.SetString (nextRefresh.ToString (), name);

			} catch (Exception ex) {
				Xamarin.Insights.Report (ex);
			}
		}

		public static bool CheckRefreshCookie (string name)
		{
			var user = NSUserDefaults.StandardUserDefaults;
			var val = user.StringForKey (name);
			if (string.IsNullOrEmpty (val)) {
				SetRefreshCookie (name);
				return false;
			} else {
				DateTime lastRefresh;
				DateTime.TryParse (val, out lastRefresh);
				if (lastRefresh <= DateTime.Now) {
					SetRefreshCookie (name);
					return false;
				}
			}
			return true;
		}

		public static DateTime NSDateToDateTime (NSDate date)
		{
			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime (
									 new DateTime (2001, 1, 1, 0, 0, 0));
			return reference.AddSeconds (date.SecondsSinceReferenceDate);
		}

		/// <summary>
		/// Shows the alert.
		/// int button = await ShowAlert ("Foo", "Bar", "Ok", "Cancel", "Maybe");
		/// </summary>
		/// <returns>The alert.</returns>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		/// <param name="buttons">Buttons.</param>
		public static Task<int> ShowAlert (string title, string message, params string [] buttons)
		{
			var tcs = new TaskCompletionSource<int> ();
			var alert = new UIAlertView {
				Title = title,
				Message = message
			};
			foreach (var button in buttons) {
				alert.AddButton (button);
			}
			alert.Clicked += (s, e) => tcs.TrySetResult ((int)e.ButtonIndex);
			alert.Show ();
			return tcs.Task;
		}
	}
}
