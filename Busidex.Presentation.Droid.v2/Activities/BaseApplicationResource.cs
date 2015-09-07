using System;
using Busidex.Mobile;
using Xamarin.Auth;
using System.Linq;
using Android.Content;

namespace Busidex.Presentation.Droid.v2
{
	public class BaseApplicationResource
	{
		readonly Context context;

		public BaseApplicationResource (Context _context)
		{
			context = _context;
		}

		#region Authentication
		public string GetAuthCookie(){
			try{
				var account = GetAuthAccount ();
				if(account == null){
					return null;
				}
				var cookies = account.Cookies.GetCookies(new Uri(Resources.COOKIE_URI));
				var cookie = cookies [Resources.AUTHENTICATION_COOKIE_NAME];

				return cookie.Value;
			}
			catch(Exception ex){
				return string.Empty;
			}
		}

		public Account GetAuthAccount(){
			
			return AccountStore.Create (context).FindAccountsForService (Resources.AUTHENTICATION_COOKIE_NAME).FirstOrDefault();
		}

		public void SetAuthCookie(long userId, int expires = 1){

			var cookieVal = Utils.EncodeUserId (userId);
			var cookie = new System.Net.Cookie(Resources.AUTHENTICATION_COOKIE_NAME, cookieVal);
			cookie.Expires = DateTime.Now.AddYears (expires);
			cookie.Value = Utils.EncodeUserId(userId);

			var container = new System.Net.CookieContainer ();
			container.SetCookies (new Uri(Resources.COOKIE_URI), cookie.ToString ());

			var account = new Account (userId.ToString (), container);

			AccountStore.Create (context).Save(account, Resources.AUTHENTICATION_COOKIE_NAME);
		}

		public void SetRefreshCookie(string prop){
			var account = GetAuthAccount ();
			if(account != null && account.Cookies != null){

				var today = DateTime.Now;

				var expireDate = new DateTime(today.Year, today.Month, today.Day, 0, 0, 1).AddDays(1);

				if(!account.Properties.ContainsKey(prop)){
					account.Properties.Add (prop, expireDate.ToString ());
				}else{
					account.Properties [prop] = expireDate.ToString ();
				}

				AccountStore.Create (context).Save(account, Resources.AUTHENTICATION_COOKIE_NAME);
			}
		}

		public bool CheckRefreshDate(string prop){
			var account = GetAuthAccount ();
			if(account != null && account.Cookies != null){

				DateTime expireDate;

				if(account.Properties.ContainsKey(prop) && 
					DateTime.TryParse(account.Properties [prop], out expireDate)){

					return expireDate > DateTime.Now;
				}
			}
			return false;
		}

		public void RemoveAuthCookie(){
			var account = GetAuthAccount ();
			if(account != null && account.Cookies != null){
				AccountStore.Create (context).Delete (account, Resources.AUTHENTICATION_COOKIE_NAME);
			}
		}

		#endregion
	}
}

