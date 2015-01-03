
using Android.App;
using System.Linq;
using Android.Content;
using Xamarin.Auth;
using System.IO;
using Busidex.Mobile.Models;
using Busidex.Mobile;
using Android.Net;

namespace Busidex.Presentation.Android
{
	 
	[Activity (Label = "BaseActivity")]			
	public class BaseActivity : Activity
	{

		protected void RedirectToMainIfLoggedIn(){

			var cookie = GetAuthCookie ();
			if(cookie != null){
				var intent = new Intent(this, typeof(MainActivity));
				Redirect(intent);
			}
		}

		#region Authentication
		protected string GetAuthCookie(){
			var account = GetAuthAccount ();
			var cookies = account.Cookies.GetCookies(new System.Uri(Busidex.Mobile.Resources.COOKIE_URI));
			var cookie = cookies [Busidex.Mobile.Resources.AUTHENTICATION_COOKIE_NAME];
			return cookie.Value;
		}

		protected Account GetAuthAccount(){

			return AccountStore.Create (this).FindAccountsForService (Busidex.Mobile.Resources.AUTHENTICATION_COOKIE_NAME).FirstOrDefault();
		}

		protected void SetAuthCookie(long userId, int expires = 1){

			var cookieVal = Utils.EncodeUserId (userId);
			var cookie = new System.Net.Cookie(Busidex.Mobile.Resources.AUTHENTICATION_COOKIE_NAME, cookieVal);
			cookie.Expires = System.DateTime.Now.AddYears (expires);
			cookie.Value = Utils.EncodeUserId(userId);

			var container = new System.Net.CookieContainer ();
			container.SetCookies (new System.Uri(Busidex.Mobile.Resources.COOKIE_URI), cookie.ToString ());
			//container.Add (cookie);

			var account = new Account (userId.ToString (), container);

			AccountStore.Create (this).Save(account, Busidex.Mobile.Resources.AUTHENTICATION_COOKIE_NAME);
		}

		protected void RemoveAuthCookie(){
			var account = GetAuthAccount ();
			if(account != null && account.Cookies != null){
				var cookies = account.Cookies.GetCookies (new System.Uri(Busidex.Mobile.Resources.COOKIE_URI));
				if(cookies != null){
					var userId = Utils.DecodeUserId (cookies [Busidex.Mobile.Resources.AUTHENTICATION_COOKIE_NAME].Value);
					SetAuthCookie (userId, -1);
				}
			}
		}
		#endregion

		protected void Redirect(Intent intent){

			StartActivity(intent);
		}

		protected void ShowCard(Intent intent){

			var userCard = GetUserCardFromIntent (intent);
			var token = GetAuthCookie ();
			ActivityController.SaveActivity ((long)EventSources.Details, userCard.CardId, token);
			Redirect(intent);
		}

		protected void SendEmail(Intent intent){

			var userCard = GetUserCardFromIntent (intent);
			var token = GetAuthCookie ();
			ActivityController.SaveActivity ((long)EventSources.Email, userCard.CardId, token);

			StartActivity(intent);
		}

		protected void OpenBrowser(Intent intent){

			var userCard = GetUserCardFromIntent (intent);
			var token = GetAuthCookie ();
			ActivityController.SaveActivity ((long)EventSources.Website, userCard.CardId, token);

			var browserIntent = Intent.CreateChooser(intent, "Open with");
			StartActivity (browserIntent);
		}

		protected void OpenMap(Intent intent){
			StartActivity (intent);
		}

		static UserCard GetUserCardFromIntent(Intent intent){

			var data = intent.GetStringExtra ("Card");
			return !string.IsNullOrEmpty (data) ? Newtonsoft.Json.JsonConvert.DeserializeObject<UserCard> (data) : null;
		}

		protected void AddCardToMyBusidex(Intent intent){

			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.MY_BUSIDEX_FILE);
			var userCard = GetUserCardFromIntent (intent);
			if (userCard!= null) {

				string file;
				if (File.Exists (fullFilePath)) {
					using (var myBusidexFile = File.OpenText (fullFilePath)) {
						var myBusidexJson = myBusidexFile.ReadToEnd ();
						MyBusidexResponse myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (myBusidexJson);
						myBusidexResponse.MyBusidex.Busidex.Add (userCard);
						file = Newtonsoft.Json.JsonConvert.SerializeObject(myBusidexResponse);

						var token = GetAuthCookie ();
						ActivityController.SaveActivity ((long)EventSources.Add, userCard.CardId, token);
					}

					File.WriteAllText (fullFilePath, file);
				}
			}
		}

		protected void RemoveCardFromMyBusidex(Intent intent){

			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.MY_BUSIDEX_FILE);
			var userCard = GetUserCardFromIntent (intent);

			if (userCard!= null) {

				string file;
				if (File.Exists (fullFilePath)) {
					using (var myBusidexFile = File.OpenText (fullFilePath)) {
						var myBusidexJson = myBusidexFile.ReadToEnd ();
						MyBusidexResponse myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (myBusidexJson);
						myBusidexResponse.MyBusidex.Busidex.RemoveAll (uc => uc.CardId == userCard.CardId);
						file = Newtonsoft.Json.JsonConvert.SerializeObject (myBusidexResponse);
					}

					File.WriteAllText (fullFilePath, file);
				}
			}
		}

		protected virtual void ProcessCards(string data){

		}

		protected void LoadCardsFromFile(string fullFilePath){

			if(File.Exists(fullFilePath)){
				var file = File.OpenText (fullFilePath);
				var fileJson = file.ReadToEnd ();
				ProcessCards (fileJson);
			}
		}
	}
}

