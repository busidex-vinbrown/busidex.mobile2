
using Android.App;
using System.Linq;
using Android.Content;
using Xamarin.Auth;
using System.IO;
using Busidex.Mobile.Models;
using Busidex.Mobile;
using Android.Views.InputMethods;
using Android.OS;
using System.Threading.Tasks;

namespace Busidex.Presentation.Android
{
	 
	[Activity (Label = "BaseActivity")]			
	public class BaseActivity : Activity
	{

		protected static ProgressDialog progressDialog;
		Handler progressBarHandler = new Handler();

		#region Loading
		protected virtual void ProcessFile(string data){

		}

		protected bool CheckBusidexFileCache(string fullFilePath){
			if(File.Exists(fullFilePath)){
				var file = File.OpenText (fullFilePath);
				var fileJson = file.ReadToEnd ();
				file.Close ();
				return !string.IsNullOrEmpty (fileJson);
			}
			return false;
		}

		protected void LoadFromFile(string fullFilePath){

			if(File.Exists(fullFilePath)){
				var file = File.OpenText (fullFilePath);
				var fileJson = file.ReadToEnd ();
				file.Close ();
				ProcessFile (fileJson);
			}
		}

		protected void RedirectToMainIfLoggedIn(){

			var cookie = GetAuthCookie ();
			if(cookie != null){
				var intent = new Intent(this, typeof(MainActivity));
				Redirect(intent);
			}
		}
		#endregion

		#region Authentication
		protected string GetAuthCookie(){
			var account = GetAuthAccount ();
			if(account == null){
				return null;
			}
			var cookies = account.Cookies.GetCookies(new System.Uri(Busidex.Mobile.Resources.COOKIE_URI));
			var cookie = cookies [Busidex.Mobile.Resources.AUTHENTICATION_COOKIE_NAME];
			return cookie.Value;
		}

		protected Account GetAuthAccount(){

			return AccountStore.Create (this).FindAccountsForService (Busidex.Mobile.Resources.AUTHENTICATION_COOKIE_NAME).FirstOrDefault();
		}

		protected void SetAuthCookie(long userId, int expires = 1){

			var cookieVal = Busidex.Mobile.Utils.EncodeUserId (userId);
			var cookie = new System.Net.Cookie(Busidex.Mobile.Resources.AUTHENTICATION_COOKIE_NAME, cookieVal);
			cookie.Expires = System.DateTime.Now.AddYears (expires);
			cookie.Value = Utils.EncodeUserId(userId);

			var container = new System.Net.CookieContainer ();
			container.SetCookies (new System.Uri(Busidex.Mobile.Resources.COOKIE_URI), cookie.ToString ());

			var account = new Account (userId.ToString (), container);

			AccountStore.Create (this).Save(account, Busidex.Mobile.Resources.AUTHENTICATION_COOKIE_NAME);
		}

		protected void SetRefreshCookie(string prop){
			var account = GetAuthAccount ();
			if(account != null && account.Cookies != null){

				var today = System.DateTime.Now;

				var expireDate = new System.DateTime(today.Year, today.Month, today.Day, 0, 0, 1).AddDays(1);
		
				if(!account.Properties.ContainsKey(prop)){
					account.Properties.Add (prop, expireDate.ToString ());
				}else{
					account.Properties [prop] = expireDate.ToString ();
				}

				AccountStore.Create (this).Save(account, Busidex.Mobile.Resources.AUTHENTICATION_COOKIE_NAME);
			}
		}

		protected bool CheckRefreshDate(string prop){
			var account = GetAuthAccount ();
			if(account != null && account.Cookies != null){

				System.DateTime expireDate;

				if(account.Properties.ContainsKey(prop) && 
					System.DateTime.TryParse(account.Properties [prop], out expireDate)){

					return expireDate > System.DateTime.Now;
				}
			}
			return false;
		}

		protected void RemoveAuthCookie(){
			var account = GetAuthAccount ();
			if(account != null && account.Cookies != null){
				AccountStore.Create (this).Delete (account, Busidex.Mobile.Resources.AUTHENTICATION_COOKIE_NAME);
			}
		}

		#endregion

		#region Card Actions
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

			var userCard = GetUserCardFromIntent (intent);
			var token = GetAuthCookie ();
			ActivityController.SaveActivity ((long)EventSources.Map, userCard.CardId, token);

			StartActivity (intent);
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
		#endregion

		#region Keyboard
		static UserCard GetUserCardFromIntent(Intent intent){

			var data = intent.GetStringExtra ("Card");
			return !string.IsNullOrEmpty (data) ? Newtonsoft.Json.JsonConvert.DeserializeObject<UserCard> (data) : null;
		}

		protected async Task<bool> DismissKeyboard(IBinder token){
			var imm = (InputMethodManager)GetSystemService(InputMethodService); 
			return imm.HideSoftInputFromWindow(token, 0);
		}
		#endregion

		#region Progress Bar
		protected void ShowLoadingSpinner(string message = null, ProgressDialogStyle style = ProgressDialogStyle.Spinner, int max = 100){

			Context context = this;
			var loadingText = message ?? context.GetString (Resource.String.Global_OneMoment);

			progressDialog = new ProgressDialog (this);

			progressDialog.Max = max;
			progressDialog.SetProgressStyle (style);
			progressDialog.SetMessage (loadingText);
			progressDialog.Progress = 0;
			progressDialog.Show ();
			progressDialog.SetCanceledOnTouchOutside (false);

//			new Thread(new ThreadStart(delegate
//				{
//					RunOnUiThread(() => Toast.MakeText(this, messageText, ToastLength.Long).Show());
//				})
//			).Start();
		}

		protected void UpdateLoadingSpinner(decimal current, decimal total){

			var progress = total == 0 ? 0 : (System.Math.Round (current / total, 2)) * 100;
			progressBarHandler.Post( () => { progressDialog.Progress = (int)progress; });
		}

		protected void HideLoadingSpinner(){
			if (progressDialog != null) {
				progressDialog.Dismiss ();
			}
		}
		#endregion
	}
}

