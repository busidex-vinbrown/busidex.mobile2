using System;
using Foundation;
using UIKit;
using System.IO;
using System.Linq;
using Busidex.Mobile.Models;
using Busidex.Mobile;
using System.Threading.Tasks;
using GoogleAnalytics.iOS;
using CoreAnimation;
using CoreGraphics;

namespace Busidex.Presentation.iOS
{
	public partial class BaseController : UIViewController
	{

		protected LoadingOverlay Overlay;
		protected string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

		public static UIStoryboard board;
		public static UIStoryboard orgBoard;
		public static EventListController eventListController;
		public static EventCardsController eventCardsController;
		public static MyBusidexController myBusidexController;
		public static SearchController searchController;
		public static OrganizationsController organizationsController;
		public static OrgMembersController orgMembersController;
		public static DataViewController dataViewController;
		public static QuickShareController quickShareController;
		public static ButtonPanelController buttonPanelController;
		public static SharedCardController sharedCardController;
		public static OrganizationDetailController orgDetailController;

		public BaseController (IntPtr handle) : base (handle)
		{
			
		}

		public BaseController ()
		{

		}

		static void init(){
			board = board ?? UIStoryboard.FromName ("MainStoryboard_iPhone", null);
			orgBoard = orgBoard ?? UIStoryboard.FromName("OrganizationStoryBoard_iPhone", null);

			eventListController = eventListController ?? board.InstantiateViewController ("EventListController") as EventListController;
			eventCardsController = eventCardsController ?? board.InstantiateViewController ("EventCardsController") as EventCardsController;
			myBusidexController = myBusidexController ?? board.InstantiateViewController ("MyBusidexController") as MyBusidexController;
			searchController = searchController ?? board.InstantiateViewController ("SearchController") as SearchController;
			organizationsController = organizationsController ?? board.InstantiateViewController ("OrganizationsController") as OrganizationsController;
			orgMembersController = orgMembersController ?? orgBoard.InstantiateViewController ("OrgMembersController") as OrgMembersController;
			orgDetailController = orgDetailController ?? orgBoard.InstantiateViewController ("OrganizationDetailController") as OrganizationDetailController;
			dataViewController = dataViewController ?? board.InstantiateViewController ("DataViewController") as DataViewController;
			quickShareController = quickShareController ?? board.InstantiateViewController ("QuickShareController") as QuickShareController;
			buttonPanelController = buttonPanelController ?? board.InstantiateViewController ("ButtonPanelController") as ButtonPanelController;
			sharedCardController = sharedCardController ?? board.InstantiateViewController ("SharedCardController") as SharedCardController;
		}

		protected void ShowOverlay(){
			Overlay = new CardLoadingOverlay (View.Bounds);
			Overlay.MessageText = "Loading Your Card";
			View.AddSubview (Overlay);
		}

		protected CALayer GetBorder(CGRect frame, CGColor color, float offset = 0f, float borderWidth = 1f ){
			var layer = new CALayer ();
			layer.Bounds = new CGRect (frame.X, frame.Y, frame.Width + offset, frame.Height + offset);
			layer.Position = new CGPoint ((frame.Width / 2f) + offset, (frame.Height / 2f) + offset);
			layer.ContentsGravity = CALayer.GravityResize;
			layer.BorderWidth = borderWidth;
			layer.BorderColor = color;

			return layer;
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
			init ();

		}

		protected static void SetRefreshCookie(string name){

			try{
				var user = NSUserDefaults.StandardUserDefaults;
				DateTime nextRefresh = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1).AddDays(1);
				user.SetString(nextRefresh.ToString(), name);

			}catch(Exception ex){
				Xamarin.Insights.Report (ex);
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

			UISubscriptionService.AuthToken = cookie.Value;

			return cookie;
		}

		public NSHttpCookie GetAuthCookie(){

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

			if(NavigationController.ViewControllers.Any(c => c as DataViewController != null)){
				NavigationController.PopToViewController (dataViewController, true);
			}else{
				NavigationController.PushViewController (dataViewController, true);
			}
		}

		protected void GoToQuickShare ()
		{
			if (UISubscriptionService.AppQuickShareLink != null) {
				quickShareController = quickShareController ?? Storyboard.InstantiateViewController ("QuickShareController") as QuickShareController;
				quickShareController.SetCardSharingInfo (new QuickShareLink {
					CardId = UISubscriptionService.AppQuickShareLink.CardId,
					PersonalMessage = UISubscriptionService.AppQuickShareLink.PersonalMessage,
					From = UISubscriptionService.AppQuickShareLink.From,
					DisplayName = UISubscriptionService.AppQuickShareLink.DisplayName
				});
				quickShareController.SaveFromUrl ();

				if(NavigationController.ViewControllers.Any(c => c as QuickShareController != null)){
					NavigationController.PopToViewController (quickShareController, true);
				}else{
					NavigationController.PushViewController (quickShareController, true);
				}
			}
		}

		protected void ShareCard(UserCard seletcedCard){

			try{
				UIStoryboard board = UIStoryboard.FromName ("MainStoryboard_iPhone", null);

				sharedCardController = sharedCardController ?? board.InstantiateViewController ("SharedCardController") as SharedCardController;
				sharedCardController.SelectedCard = seletcedCard;

				if(NavigationController.ViewControllers.Any(c => c as SharedCardController != null)){
					NavigationController.PopToViewController (sharedCardController, true);
				}else{
					NavigationController.PushViewController (sharedCardController, true);
				}

				string name = Resources.GA_LABEL_SHARE;
				if(sharedCardController.SelectedCard != null && sharedCardController.SelectedCard.Card != null){
					name = string.IsNullOrEmpty(sharedCardController.SelectedCard.Card.Name) ? sharedCardController.SelectedCard.Card.CompanyName : sharedCardController.SelectedCard.Card.Name;
				}

				AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_SHARE, name, 0);

			}catch(Exception ex){
				Xamarin.Insights.Report (ex);
			}
		}

//		protected virtual async Task<int> DoSearch(){
//
//			Overlay.Hide ();
//
//			return 1;
//		}

//		public void AddCardToMyBusidexCache(UserCard userCard){
//			var fullFilePath = Path.Combine (documentsPath, Resources.MY_BUSIDEX_FILE);
//
//			string file;
//			if (File.Exists (fullFilePath)) {
//				using (var myBusidexFile = File.OpenText (fullFilePath)) {
//					var myBusidexJson = myBusidexFile.ReadToEnd ();
//					MyBusidexResponse myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (myBusidexJson);
//					if (myBusidexResponse.MyBusidex.Busidex.All (uc => uc.Card.CardId != userCard.Card.CardId)) {
//						myBusidexResponse.MyBusidex.Busidex.Add (userCard);
//
//						myBusidexResponse.MyBusidex.Busidex = myBusidexResponse.MyBusidex.Busidex.OrderByDescending (c => c.Card != null && c.Card.OwnerId.GetValueOrDefault () > 0 ? 1 : 0)
//							.ThenBy (c => c.Card != null ? c.Card.Name : "")
//							.ThenBy (c => c.Card != null ? c.Card.CompanyName : "")
//							.ToList ();
//						
//					}
//					file = Newtonsoft.Json.JsonConvert.SerializeObject (myBusidexResponse);
//				}
//				Utils.SaveResponse (file, fullFilePath);
//
//				if (Application.MyBusidex.All (c => c.CardId != userCard.CardId)) {
//					Application.MyBusidex.Add (userCard);
//					Application.MyBusidex = Application.MyBusidex.OrderByDescending (c => c.Card != null && c.Card.OwnerId.GetValueOrDefault () > 0 ? 1 : 0)
//						.ThenBy (c => c.Card != null ? c.Card.Name : "")
//						.ThenBy (c => c.Card != null ? c.Card.CompanyName : "")
//						.ToList ();
//				}
//			}

//			string name = Resources.GA_LABEL_ADD;
//			if(userCard != null && userCard.Card != null){
//				name = string.IsNullOrEmpty(userCard.Card.Name) ? userCard.Card.CompanyName : userCard.Card.Name;
//			}
//
//			AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_ADD, name, 0);
//		}

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

		protected BusinessCardDimensions GetCardDimensions(string orientation){

			/*
				Business cards have an aspect ratio of 1.75 (Canada and US).
			*/
			const float ASPECT_RATIO = 1.75f;
			const float hBase = 165f;
			const float vBase = 115f;

			float height;
			float width;
			float leftMargin;

			if(orientation == "H"){
				height = hBase;
				width = hBase * ASPECT_RATIO;
			}else{
				height = vBase * ASPECT_RATIO;
				width = vBase;
			}
			leftMargin = ((float)UIScreen.MainScreen.Bounds.Width - width) / 2f;

			return new BusinessCardDimensions (height, width, leftMargin);
		}

		protected struct BusinessCardDimensions{

			public BusinessCardDimensions(float h, float w, float m){
				Height = h;
				Width = w;
				MarginLeft = m;
			}

			public float Height;
			public float Width;
			public float MarginLeft;
		}
	}
}

