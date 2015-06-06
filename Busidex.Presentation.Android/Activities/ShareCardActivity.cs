using Android.App;
using Android.OS;
using Android.Widget;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using Android.Net;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "Share A Card")]			
	public class ShareCardActivity : BaseCardActivity
	{
		TextView lblShareError;
		ImageView imgCheckShared;

		protected override void OnImageDownloadCompleted (Uri uri)
		{
			base.OnImageDownloadCompleted (uri);

			var imgCardHorizontal = FindViewById<ImageView> (Resource.Id.imgShareHorizontal);
			var imgCardVertical = FindViewById<ImageView> (Resource.Id.imgShareVertical);
			var isHorizontal = UserCard.Card.FrontOrientation == "H";
			var imgDisplay = isHorizontal ? imgCardHorizontal : imgCardVertical;
			imgCardHorizontal.Visibility = isHorizontal ? global::Android.Views.ViewStates.Visible : global::Android.Views.ViewStates.Gone;
			imgCardVertical.Visibility = isHorizontal ? global::Android.Views.ViewStates.Gone : global::Android.Views.ViewStates.Visible;

			imgDisplay.SetImageURI (uri);

		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetContentView (Resource.Layout.SharedCard);

			var imgCardHorizontal = FindViewById<ImageView> (Resource.Id.imgShareHorizontal);
			var imgCardVertical = FindViewById<ImageView> (Resource.Id.imgShareVertical);

			imgCardHorizontal.Visibility = imgCardVertical.Visibility = global::Android.Views.ViewStates.Invisible;

			base.OnCreate (savedInstanceState);

			TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_SHARE, 0);

			lblShareError = FindViewById<TextView> (Resource.Id.lblShareError);
			imgCheckShared = FindViewById<ImageView> (Resource.Id.imgCheckShared);

			HideFeedbackLabels ();

			var btnShareCard = FindViewById<Button> (Resource.Id.btnShareCard);
			btnShareCard.Click += delegate {
				ShareCard();
			};

			var txtShareDisplayName = FindViewById<TextView> (Resource.Id.txtShareDisplayName);
			txtShareDisplayName.Text = GetStringPreference (Busidex.Mobile.Resources.USER_SETTING_DISPLAYNAME);



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

		void UpdateDisplayName(string token){

			var txtShareDisplayName = FindViewById<TextView> (Resource.Id.txtShareDisplayName);

			var displayName = txtShareDisplayName.Text;
			var savedDisplayName = GetStringPreference (Busidex.Mobile.Resources.USER_SETTING_DISPLAYNAME);

			if(string.IsNullOrEmpty(savedDisplayName)){
				var accountJSON = AccountController.GetAccount (token);
				var account = Newtonsoft.Json.JsonConvert.DeserializeObject<BusidexUser> (accountJSON);
				savedDisplayName = account.UserAccount.DisplayName;
			}

			if(!displayName.Equals(savedDisplayName)){
				AccountController.UpdateDisplayName (displayName, token);
				SaveStringPreference (Busidex.Mobile.Resources.USER_SETTING_DISPLAYNAME, displayName);
			}
		}

		void ShareCard(){

			var token = applicationResource.GetAuthCookie ();
			var txtShareEmail = FindViewById<TextView> (Resource.Id.txtShareEmail);
			var txtShareDisplayName = FindViewById<TextView> (Resource.Id.txtShareDisplayName);

			HideFeedbackLabels ();

			var email = txtShareEmail.Text;
			var displayName = txtShareDisplayName.Text;

			if(string.IsNullOrEmpty(email)){
				ShowAlert("Missing Information", "Please enter an email address");
				return;
			}

			if(string.IsNullOrEmpty(displayName)){
				ShowAlert("Missing Information", "Please enter a display name");
				return;
			}

			UpdateDisplayName (token);

			var ctrl = new SharedCardController();
			var response = ctrl.ShareCard (UserCard.Card, email, token);

			if( !string.IsNullOrEmpty(response) && response.Contains("true")){
				imgCheckShared.Visibility = global::Android.Views.ViewStates.Visible;
			}else{
				lblShareError.Visibility = global::Android.Views.ViewStates.Invisible;
				imgCheckShared.Visibility = global::Android.Views.ViewStates.Visible;
			}
		}
	}
}

