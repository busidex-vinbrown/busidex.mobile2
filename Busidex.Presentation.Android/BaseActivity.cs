
using System;

using Android.App;
using Java.Net;
using System.Linq;
using Android.Content;
using Xamarin.Auth;


namespace Busidex.Presentation.Android
{
	[Activity (Label = "BaseActivity")]			
	public class BaseActivity : Activity
	{

		protected void RedirectToMainIfLoggedIn(){

			var cookie = GetAuthCookie ();
			if(cookie != null){
				var intent = new Intent(this, typeof(MainActivity));
				StartActivity(intent);
			}
		}

		protected Account GetAuthCookie(){

			return AccountStore.Create (this).FindAccountsForService (Busidex.Mobile.Resources.AUTHENTICATION_COOKIE_NAME).FirstOrDefault();
		}

		protected void SetAuthCookie(long userId, int expires = 1){

			var cookieVal = Busidex.Mobile.Utils.EncodeUserId (userId);
			var cookie = new System.Net.Cookie(Busidex.Mobile.Resources.AUTHENTICATION_COOKIE_NAME, cookieVal);
			cookie.Expires = DateTime.Now.AddYears (expires);
			cookie.Value = Busidex.Mobile.Utils.EncodeUserId(userId);

			var container = new System.Net.CookieContainer ();
			container.SetCookies (new Uri(Busidex.Mobile.Resources.COOKIE_URI), cookie.ToString ());
			//container.Add (cookie);

			var account = new Account (userId.ToString (), container);

			AccountStore.Create (this).Save(account, Busidex.Mobile.Resources.AUTHENTICATION_COOKIE_NAME);
		}

		protected void RemoveAuthCookie(){
			var account = GetAuthCookie ();
			if(account != null && account.Cookies != null){
				var cookies = account.Cookies.GetCookies (new Uri(Busidex.Mobile.Resources.COOKIE_URI));
				if(cookies != null){
					var userId = Busidex.Mobile.Utils.DecodeUserId (cookies [Busidex.Mobile.Resources.AUTHENTICATION_COOKIE_NAME].Value);
					SetAuthCookie (userId, -1);
				}
			}
		}
	}
}

