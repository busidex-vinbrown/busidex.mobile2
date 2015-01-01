using Android.App;
using Android.OS;
using Android.Widget;
using System.IO;
using Busidex.Mobile;
using Android.Net;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "ShareCardActivity")]			
	public class ShareCardActivity : BaseCardActivity
	{
		TextView lblShareError;
		ImageView imgCheckShared;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.SharedCard);

			lblShareError = FindViewById<TextView> (Resource.Id.lblShareError);
			imgCheckShared = FindViewById<ImageView> (Resource.Id.imgCheckShared);

			HideFeedbackLabels ();

			var imgCardHorizontal = FindViewById<ImageView> (Resource.Id.imgShareHorizontal);
			var imgCardVertical = FindViewById<ImageView> (Resource.Id.imgShareVertical);
			var isHorizontal = Card.Card.FrontOrientation == "H";
			var imgDisplay = isHorizontal ? imgCardHorizontal : imgCardVertical;
			imgCardHorizontal.Visibility = isHorizontal ? global::Android.Views.ViewStates.Visible : global::Android.Views.ViewStates.Gone;
			imgCardVertical.Visibility = isHorizontal ? global::Android.Views.ViewStates.Gone : global::Android.Views.ViewStates.Visible;

			var frontFileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Card.Card.FrontFileName);
			var frontUri = Uri.Parse (frontFileName);

			if (File.Exists (frontFileName)) {
				imgDisplay.SetImageURI (frontUri);
			}else{

				//ShowOverlay ();

				Utils.DownloadImage (Busidex.Mobile.Resources.CARD_PATH + Card.Card.FrontFileName, Busidex.Mobile.Resources.DocumentsPath, Card.Card.FrontFileName).ContinueWith (t => {
					RunOnUiThread (() => {
						imgDisplay.SetImageURI (frontUri);
						//Overlay.Hide();
					});
				});
			}


			var btnShareCard = FindViewById<Button> (Resource.Id.btnShareCard);
			btnShareCard.Click += delegate {

				ShareCard();

			};
//			Button saveButton = new Button (this);
//			saveButton.SetTextColor (global::Android.Graphics.Color.ParseColor("#ff0582f1"));
//			saveButton.SetText (Resource.String.Share_ShareCard);
//			saveButton.SetBackgroundColor(global::Android.Graphics.Color.ParseColor("#ffffffff"));
//
//			ActionBar.CustomView = saveButton;
//			ActionBar.SetDisplayShowCustomEnabled (true);
		}

		void HideFeedbackLabels(){
			lblShareError.Visibility = imgCheckShared.Visibility = global::Android.Views.ViewStates.Invisible;
		}

		void ShareCard(){

			var token = GetAuthCookie ();
			var txtEmail = FindViewById<TextView> (Resource.Id.txtShareEmail);

			HideFeedbackLabels ();

			var email = txtEmail.Text;

			var ctrl = new SharedCardController();
			var response = ctrl.ShareCard (Card.Card, email, token);

			if( !string.IsNullOrEmpty(response) && response.Contains("true")){
				imgCheckShared.Visibility = global::Android.Views.ViewStates.Visible;
			}else{
				lblShareError.Visibility = global::Android.Views.ViewStates.Invisible;
				imgCheckShared.Visibility = global::Android.Views.ViewStates.Visible;
			}
		}
	}
}

