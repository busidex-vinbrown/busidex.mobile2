using System;
using Foundation;
using UIKit;
using System.IO;
using System.Linq;
using Busidex.Mobile.Models;
using Busidex.Mobile;
using System.Threading.Tasks;
using GoogleAnalytics.iOS;

namespace Busidex.Presentation.iOS
{
	public partial class BaseController : UIViewController
	{

		protected LoadingOverlay Overlay;
		protected string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

		public static UIStoryboard board;
		public static EventListController eventListController;
		public static MyBusidexController myBusidexController;
		public static SearchController searchController;
		public static OrganizationsController organizationsController;
		public static DataViewController dataViewController;

		public BaseController (IntPtr handle) : base (handle)
		{
		}

		public BaseController ()
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			NavigationController.SetToolbarHidden (true, false);
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Send (GAIDictionaryBuilder.CreateScreenView ().Build ());

			base.ViewDidAppear (animated);

			board = UIStoryboard.FromName ("MainStoryboard_iPhone", null);
			eventListController = board.InstantiateViewController ("EventListController") as EventListController;
			myBusidexController = board.InstantiateViewController ("MyBusidexController") as MyBusidexController;
			searchController = board.InstantiateViewController ("SearchController") as SearchController;
			organizationsController = board.InstantiateViewController ("OrganizationsController") as OrganizationsController;
			dataViewController = board.InstantiateViewController ("DataViewController") as DataViewController;
		}

		protected static void SetRefreshCookie(string name){

			try{

				var user = NSUserDefaults.StandardUserDefaults;
				DateTime nextRefresh = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1).AddDays(1);
				user.SetString(nextRefresh.ToString(), name);

			}catch(Exception ignore){
				
			}
		}

		protected static bool CheckRefreshCookie(string name){

			var user = NSUserDefaults.StandardUserDefaults;
			String val = user.StringForKey (name);
			if(string.IsNullOrEmpty(val)){
				SetRefreshCookie (name);
				return false;
			}else{
				DateTime lastRefresh;
				DateTime.TryParse (val, out lastRefresh);
				if(lastRefresh <= DateTime.Now){
					SetRefreshCookie (name);
					return false;
				}
			}
			return true;
		}

		public static DateTime NSDateToDateTime(NSDate date)
		{
			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime( 
				new DateTime(2001, 1, 1, 0, 0, 0) );
			return reference.AddSeconds(date.SecondsSinceReferenceDate);
		}

		protected NSHttpCookie SetAuthCookie(long userId){
			var nCookie = new System.Net.Cookie();
			nCookie.Name = Resources.AUTHENTICATION_COOKIE_NAME;
			DateTime expiration = DateTime.Now.AddYears(1);
			nCookie.Expires = expiration;
			nCookie.Value = Utils.EncodeUserId(userId);
			var cookie = new NSHttpCookie(nCookie);

			NSHttpCookieStorage.SharedStorage.SetCookie(cookie);
			return cookie;
		}

		protected NSHttpCookie GetAuthCookie(){

			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.SingleOrDefault (c => c.Name == Resources.AUTHENTICATION_COOKIE_NAME);

			if(cookie == null){
				return null;
			}
			var expireDate = NSDateToDateTime (cookie.ExpiresDate);

			return (expireDate > DateTime.Now) ? cookie : null;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			var shouldAllowOtherOrientation = ShouldAllowLandscape (); // same here
			return shouldAllowOtherOrientation ? UIInterfaceOrientationMask.AllButUpsideDown : UIInterfaceOrientationMask.Portrait; 

		}

		protected bool ShouldAllowLandscape ()
		{
			return false; // implement this to return true when u want it
		}

		protected void RemoveAuthCookie(){

			var nCookie = new System.Net.Cookie();
			nCookie.Name = Resources.AUTHENTICATION_COOKIE_NAME;
			nCookie.Expires = DateTime.Now.AddDays (-2);
			var cookie = new NSHttpCookie(nCookie);
			NSHttpCookieStorage.SharedStorage.SetCookie(cookie);

			Utils.RemoveCacheFiles ();
		}

		protected virtual void StartSearch(){
			InvokeOnMainThread (() => {
				Overlay = new LoadingOverlay (UIScreen.MainScreen.Bounds);
				//Overlay.RemoveFromSuperview ();
				View.Add (Overlay);
			});

		}

		protected void GoToMain ()
		{
			NavigationController.SetNavigationBarHidden (true, true);

			dataViewController = dataViewController ?? Storyboard.InstantiateViewController ("DataViewController") as DataViewController;

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

				string name = Resources.GA_LABEL_SHARE;
				if(sharedCardController.UserCard != null && sharedCardController.UserCard.Card != null){
					name = string.IsNullOrEmpty(sharedCardController.UserCard.Card.Name) ? sharedCardController.UserCard.Card.CompanyName : sharedCardController.UserCard.Card.Name;
				}

				AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_SHARE, name, 0);

			}catch(Exception ex){
				new UIAlertView("Row Selected", ex.Message, null, "OK", null).Show();
			}
		}

		protected virtual async Task<int> DoSearch(){

			Overlay.Hide ();

			return 1;
		}

		protected void AddCardToMyBusidexCache(UserCard userCard){
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

			string name = Resources.GA_LABEL_ADD;
			if(userCard != null && userCard.Card != null){
				name = string.IsNullOrEmpty(userCard.Card.Name) ? userCard.Card.CompanyName : userCard.Card.Name;
			}

			AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_ADD, name, 0);
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
				file.Close ();
				InvokeInBackground (() => ProcessCards (fileJson));
			}
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

