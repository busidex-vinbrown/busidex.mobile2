using System;
using GoogleAnalytics.iOS;
using UIKit;
using Busidex.Mobile.Models;
using System.IO;
using Busidex.Mobile;
using Foundation;

namespace Busidex.Presentation.iOS
{
	public partial class QuickShareController : BaseCardViewController
	{
		QuickShareLink Link;

		public QuickShareController (IntPtr handle) : base (handle){
			documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			LoadCard ();

		}

		public void SetCardSharingInfo(QuickShareLink link){
			Link = link;
		}

		public void LoadCard(){

			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (true, true);
			}

			var cookie = GetAuthCookie ();
			string token = string.Empty;

			if (cookie != null) {
				token = cookie.Value;
			}

			// Perform any additional setup after loading the view, typically from a nib.
			var result = CardController.GetCardById(token, Link.CardId);
			if(!string.IsNullOrEmpty(result)){
				var cardResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<CardDetailResponse> (result);
				var card = cardResponse.Model;
				var fileName = Path.Combine (documentsPath, Resources.THUMBNAIL_FILE_NAME_PREFIX + card.FrontFileName);
				if (File.Exists (fileName)) {
					imgSharedCard.Image = UIImage.FromFile (fileName);
				}
			}

		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "QuickShare");

			base.ViewDidAppear (animated);

			lblMessage.Text = string.Format (lblMessage.Text, Link.DisplayName);
			lblPersonalMessage.Text = Link.PersonalMessage;
		}

		public void SaveFromUrl(){


			var sharedCardController = new Busidex.Mobile.SharedCardController ();
			var cookie = GetAuthCookie ();

			string token = cookie.Value;
			var result = CardController.GetCardById (token, Link.CardId);
			if (!string.IsNullOrEmpty (result)) {
				var storyBoard = UIStoryboard.FromName ("MainStoryboard_iPhone", null);
				var busidexController = storyBoard.InstantiateViewController ("MyBusidexController") as MyBusidexController;

				var cardResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<CardDetailResponse> (result);
				var card = new Card (cardResponse.Model);
				var user = NSUserDefaults.StandardUserDefaults;
				var email = user.StringForKey (Resources.USER_SETTING_EMAIL);

				var userCard = new UserCard {
					Card = card,
					CardId = Link.CardId,
					ExistsInMyBusidex = true,
					OwnerId = cardResponse.Model.OwnerId,
					UserId = cardResponse.Model.OwnerId.GetValueOrDefault (),
					Notes = string.Empty
				};
				busidexController.AddCardToMyBusidexCache (userCard);
				var myBusidexController = new Busidex.Mobile.MyBusidexController ();
				myBusidexController.AddToMyBusidex (Link.CardId, token);

				sharedCardController.AcceptQuickShare (card, email, Link.From, token);
				Utils.RemoveQuickShareLink ();
			}

		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}


