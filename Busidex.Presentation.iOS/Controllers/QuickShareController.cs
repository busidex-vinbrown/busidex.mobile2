using System;
using GoogleAnalytics.iOS;
using UIKit;
using Busidex.Mobile.Models;
using System.IO;
using Busidex.Mobile;
using Foundation;
using System.Drawing;

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

				var leftMargin = card.FrontOrientation == "H" ? 11f : 72f;
				const float cardTop = 66f;
				var cardWidth = card.FrontOrientation == "H" ? 300f : 180f;
				var cardHeight = card.FrontOrientation == "H" ? 180f : 300f;

				const float messageLeft = 20f;
				var messageTop = cardTop + cardHeight;
				const float messageWidth = 280f;
				const float messageHeight = 90f;
				lblMessage.Frame = new RectangleF (messageLeft, messageTop, messageWidth, messageHeight);

				messageTop += messageHeight;

				lblPersonalMessage.Frame = new RectangleF (messageLeft, messageTop, messageWidth, messageHeight);

				imgSharedCard.Frame = new RectangleF (leftMargin, cardTop, cardWidth, cardHeight);
				
				if (File.Exists (fileName)) {
					imgSharedCard.Image = UIImage.FromFile (fileName);
				}else{
					ShowOverlay ();

					Utils.DownloadImage (Resources.CARD_PATH + card.FrontFileName, documentsPath, Resources.THUMBNAIL_FILE_NAME_PREFIX + card.FrontFileName).ContinueWith (t => {
						InvokeOnMainThread (() => {
							imgSharedCard.Image = UIImage.FromFile (fileName);
							Overlay.Hide();
						});
					});
				}
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "QuickShare");

			base.ViewDidAppear (animated);

			lblMessage.Text = string.Format (lblMessage.Text, Link.DisplayName);
			lblPersonalMessage.Text = Link.PersonalMessage.Trim ();//+ "\"";
		}

		public void SaveFromUrl(){

			var sharedCardController = new Busidex.Mobile.SharedCardController ();
			var cookie = GetAuthCookie ();

			string token = cookie.Value;
			var result = CardController.GetCardById (token, Link.CardId);
			if (!string.IsNullOrEmpty (result)) {

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
				UISubscriptionService.AddCardToMyBusidex(userCard);

				sharedCardController.AcceptQuickShare (card, email, Link.From, token, Link.PersonalMessage);
				Utils.RemoveQuickShareLink ();
			}

		}
	}
}


