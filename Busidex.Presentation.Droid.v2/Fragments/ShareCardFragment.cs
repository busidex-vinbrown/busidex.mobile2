using Android.OS;
using Android.Views;
using Android.Widget;
using System.IO;
using Android.Net;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using Plugin.Messaging;
using System.Net;
using Android.Telephony;
using System.Collections.Generic;
using Android.Content;

namespace Busidex.Presentation.Droid.v2
{
	public class ShareCardFragment : GenericViewPagerFragment
	{
		TextView lblShareError;
		TextView txtShareDisplayName;
		TextView txtShareEmail;
		TextView txtSharePhoneNumber;
		TextView txtShareMessage;

		ImageView imgCheckShared;
		readonly UserCard SelectedCard;

		string currentDisplayName = string.Empty;

		public ShareCardFragment()
		{
			
		}

		public ShareCardFragment(UserCard selectedCard)
		{
			SelectedCard = selectedCard;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			var view = inflater.Inflate(Resource.Layout.SharedCard, container, false);

			var imgCardHorizontal = view.FindViewById<ImageView> (Resource.Id.imgShareHorizontal);
			var imgCardVertical = view.FindViewById<ImageView> (Resource.Id.imgShareVertical);

			imgCardHorizontal.Visibility = imgCardVertical.Visibility = ViewStates.Invisible;

			lblShareError = view.FindViewById<TextView> (Resource.Id.lblShareError);
			imgCheckShared = view.FindViewById<ImageView> (Resource.Id.imgCheckShared);

			HideFeedbackLabels ();

			var btnShareCard = view.FindViewById<Button> (Resource.Id.btnShareCard);
			btnShareCard.Click += delegate {
				ShareCard();
			};

			txtShareDisplayName = view.FindViewById<TextView> (Resource.Id.txtShareDisplayName);
			txtShareEmail = view.FindViewById<TextView> (Resource.Id.txtShareEmail);
			txtSharePhoneNumber = view.FindViewById<TextView> (Resource.Id.txtSharePhoneNumber);
			txtShareMessage = view.FindViewById<TextView> (Resource.Id.txtShareMessage);

			txtShareDisplayName.Text = currentDisplayName = UISubscriptionService.CurrentUser.UserAccount.DisplayName;

			var fileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + SelectedCard.Card.FrontFileName);
			var uri = Uri.Parse (fileName);

			var imgShareHorizontal = view.FindViewById<ImageView> (Resource.Id.imgShareHorizontal);
			var imgShareVertical = view.FindViewById<ImageView> (Resource.Id.imgShareVertical);
			var isHorizontal = SelectedCard.Card.FrontOrientation == "H";
			var imgDisplay = isHorizontal ? imgShareHorizontal : imgShareVertical;
			imgShareHorizontal.Visibility = !isHorizontal ? ViewStates.Gone : ViewStates.Visible;
			imgShareVertical.Visibility = isHorizontal ? ViewStates.Gone : ViewStates.Visible;

			imgDisplay.SetImageURI (uri);

			var btnClose = view.FindViewById<ImageButton> (Resource.Id.btnClose);
			btnClose.Click += delegate {
				var panel = new ButtonPanelFragment();
				panel.SelectedCard = SelectedCard;
				((MainActivity)Activity).UnloadFragment(panel);
			};

			return view;
		}

		void ShareCard(){

			var token = BaseApplicationResource.GetAuthCookie ();

			HideFeedbackLabels ();

			var email = txtShareEmail.Text;
			var phoneNumber = txtSharePhoneNumber.Text;
			var displayName = txtShareDisplayName.Text;
			var personalMessage = txtShareMessage.Text;

			if(string.IsNullOrEmpty(email) && string.IsNullOrEmpty(phoneNumber)){
				
				return;
			}

			if(string.IsNullOrEmpty(displayName)){
				
				return;
			}

			// normalize the phone number if there is one
			if(!string.IsNullOrEmpty(phoneNumber)){
				phoneNumber = phoneNumber.Replace ("(", "").Replace (")", "").Replace (".", "").Replace ("-", "").Replace(" ", "");
				var smsTask = MessagingPlugin.SmsMessenger;
				EmailTemplateController.GetTemplate (EmailTemplateCode.SharedCardSMS, token).ContinueWith (r => {

					var template = Newtonsoft.Json.JsonConvert.DeserializeObject<EmailTemplateResponse> (r.Result);
					if(template != null){
						string message = string.Format(template.Template.Subject, displayName) + System.Environment.NewLine + System.Environment.NewLine + 
							string.Format(template.Template.Body, SelectedCard.Card.CardId, Utils.DecodeUserId(token), displayName,
								personalMessage, System.Environment.NewLine);

						int startIdx = message.IndexOf('[');
						int endIdx = message.IndexOf(']');
						string originalUrl = message.Substring(startIdx + 1, endIdx - startIdx - 2);
						string url = WebUtility.UrlEncode(originalUrl);

						UrlShortenerController.ShortenUrl(url).ContinueWith(resp => {

							var bitlyResponse = resp.Result;
							if(bitlyResponse != null){
								message = message.Replace(originalUrl, bitlyResponse).Replace("[", "").Replace("]", "");

								Activity.RunOnUiThread( ()=> smsTask.SendSms (phoneNumber, message));
							}
						});

//						Activity.RunOnUiThread( ()=> {
//						Intent sendIntent = new Intent(Intent.ActionSend);
//						//sendIntent.SetClassName("com.android.mms", "com.android.mms.ui.ComposeMessageActivity");
//						sendIntent.PutExtra("address", phoneNumber);
//						sendIntent.PutExtra("sms_body", message);
//
////						File file1 = new File("mFileName");
////						if(file1.Exists())
////						{
////							//File Exist
////						}
//						//Uri uri = Uri.FromFile(file1);
//						//sendIntent.PutExtra(Intent.ExtraStream, uri);
//						sendIntent.SetType("image/*");
//						StartActivity(sendIntent);
//						});
					}
				});

				lblShareError.Visibility = ViewStates.Invisible;
				imgCheckShared.Visibility = ViewStates.Visible;
			}else{
				var ctrl = new SharedCardController();
				var response = ctrl.ShareCard (SelectedCard.Card, email, phoneNumber, token);
				if( !string.IsNullOrEmpty(response) && response.Contains("true")){
					imgCheckShared.Visibility = ViewStates.Visible;
				}else{
					lblShareError.Visibility = ViewStates.Invisible;
					imgCheckShared.Visibility = ViewStates.Visible;
				}
			}

			if(!currentDisplayName.Equals(UISubscriptionService.CurrentUser.UserAccount.DisplayName, System.StringComparison.Ordinal)){
				AccountController.UpdateDisplayName (txtShareDisplayName.Text, token);	
				UISubscriptionService.CurrentUser.UserAccount.DisplayName = txtShareDisplayName.Text;
			}

			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_SHARE, 0);
		}

		void HideFeedbackLabels(){
			lblShareError.Visibility = imgCheckShared.Visibility = ViewStates.Invisible;
		}
	}
}

