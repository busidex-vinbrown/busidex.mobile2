using System;
using GoogleAnalytics.iOS;
using UIKit;
using Busidex.Mobile.Models;
using System.IO;
using Busidex.Mobile;

namespace Busidex.Presentation.iOS
{
	public partial class QuickShareController : BaseCardViewController
	{
		long CardId;

		public QuickShareController (IntPtr handle) : base (handle){
			documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			LoadCard ();

		}

		public void SetCardId(long cardId){
			CardId = cardId;	
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
			var result = CardController.GetCardById(token, CardId);
			if(!string.IsNullOrEmpty(result)){
				var cardResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<CardDetailResponse> (result);
				var card = cardResponse.Model;
				var fileName = Path.Combine (documentsPath, Resources.THUMBNAIL_FILE_NAME_PREFIX + card.FrontFileName);
				if (File.Exists (fileName)) {
					//imgSharedCard.Image = UIImage.FromFile (fileName);
				}
			}

		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "QuickShare");

			base.ViewDidAppear (animated);


		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}


