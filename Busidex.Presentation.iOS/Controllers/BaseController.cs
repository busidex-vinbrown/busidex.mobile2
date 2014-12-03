using System;
using Foundation;
using UIKit;
using System.IO;
using System.Linq;
using Busidex.Mobile.Models;

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

		protected NSHttpCookie GetAuthCookie(){
			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.SingleOrDefault (c => c.Name == Busidex.Mobile.Resources.AUTHENTICATION_COOKIE_NAME);
			return cookie;
		}

		protected virtual void StartSearch(){
			this.InvokeOnMainThread (() => {
				Overlay = Overlay ?? new LoadingOverlay (UIScreen.MainScreen.Bounds);
				Overlay.RemoveFromSuperview();
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

		protected virtual void DoSearch(){

			Overlay.Hide ();
		}

		protected void AddCardToMyBusidex(UserCard userCard){
			var fullFilePath = Path.Combine (documentsPath, Application.MY_BUSIDEX_FILE);
			// we only need to update the file if they've gotten their busidex. If they haven't, the new card will
			// come along with all the others
			var file = string.Empty;
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

	}
}

