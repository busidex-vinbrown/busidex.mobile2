using System;
using Foundation;
using UIKit;
using System.IO;
using System.Linq;
using Busidex.Mobile.Models;
using Busidex.Mobile;

namespace Busidex.Presentation.iOS
{
	public partial class BaseController : UIViewController
	{

		protected LoadingOverlay Overlay;
		protected string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

		public BaseController (IntPtr handle) : base (handle)
		{
		}

		public BaseController ()
		{
		}

		protected NSHttpCookie SetAuthCookie(long userId){
			var nCookie = new System.Net.Cookie();
			nCookie.Name = Resources.AUTHENTICATION_COOKIE_NAME;
			DateTime expiration = DateTime.Now.AddYears(1);
			nCookie.Expires = expiration;
			nCookie.Value = EncodeUserId(userId);
			var cookie = new NSHttpCookie(nCookie);

			NSHttpCookieStorage.SharedStorage.SetCookie(cookie);
			return cookie;
		}

		protected NSHttpCookie GetAuthCookie(){
			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.SingleOrDefault (c => c.Name == Resources.AUTHENTICATION_COOKIE_NAME);
			return cookie;
		}

		protected virtual void StartSearch(){
			InvokeOnMainThread (() => {
				Overlay = Overlay ?? new LoadingOverlay (UIScreen.MainScreen.Bounds);
				Overlay.RemoveFromSuperview ();
				View.Add (Overlay);
			});

		}

		protected void GoToMain ()
		{
			NavigationController.SetNavigationBarHidden (true, true);

			var dataViewController = Storyboard.InstantiateViewController ("DataViewController") as DataViewController;

			if (dataViewController != null) {
				NavigationController.PushViewController (dataViewController, true);
			}
		}

		protected void ShareCard(UserCard seletcedCard){

			try{
				UIStoryboard board = UIStoryboard.FromName ("MainStoryboard_iPhone", null);

				var sharedCardController = board.InstantiateViewController ("SharedCardController") as SharedCardController;
				sharedCardController.UserCard = seletcedCard;

				if (sharedCardController != null) {
					NavigationController.PushViewController (sharedCardController, true);
				}
			}catch(Exception ex){
				new UIAlertView("Row Selected", ex.Message, null, "OK", null).Show();
			}
		}

		protected virtual void DoSearch(){

			Overlay.Hide ();
		}

		protected void AddCardToMyBusidex(UserCard userCard){
			var fullFilePath = Path.Combine (documentsPath, Resources.MY_BUSIDEX_FILE);

			string file;
			if (File.Exists (fullFilePath)) {
				using (var myBusidexFile = File.OpenText (fullFilePath)) {
					var myBusidexJson = myBusidexFile.ReadToEnd ();
					MyBusidexResponse myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (myBusidexJson);
					myBusidexResponse.MyBusidex.Busidex.Add (userCard);
					file = Newtonsoft.Json.JsonConvert.SerializeObject(myBusidexResponse);
				}

				File.WriteAllText (fullFilePath, file);
			}
		}

		protected void RemoveCardFromMyBusidex(UserCard userCard){
			var fullFilePath = Path.Combine (documentsPath, Resources.MY_BUSIDEX_FILE);

			string file;
			if (File.Exists (fullFilePath)) {
				using (var myBusidexFile = File.OpenText (fullFilePath)) {
					var myBusidexJson = myBusidexFile.ReadToEnd ();
					MyBusidexResponse myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (myBusidexJson);
					myBusidexResponse.MyBusidex.Busidex.RemoveAll (uc => uc.CardId == userCard.CardId);
					file = Newtonsoft.Json.JsonConvert.SerializeObject(myBusidexResponse);
				}

				File.WriteAllText (fullFilePath, file);
			}
		}

		protected bool isProgressFinished(float processed, float total){
			return processed.Equals (total);
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

		static string EncodeUserId(long userId){

			byte[] toEncodeAsBytes = System.Text.Encoding.ASCII.GetBytes(userId.ToString());
			string returnValue = Convert.ToBase64String(toEncodeAsBytes);
			return returnValue;
		}
	}
}

