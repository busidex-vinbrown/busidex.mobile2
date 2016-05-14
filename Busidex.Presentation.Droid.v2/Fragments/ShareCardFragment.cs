using Android.OS;
using Android.Views;
using Android.Widget;
using System.IO;
using Android.Net;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using Plugin.Messaging;

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
		readonly Xamarin.Contacts.Phone SelectedPhone;
		readonly string SelectedMessage;

		string currentDisplayName = string.Empty;

		public ShareCardFragment ()
		{
			
		}

		public ShareCardFragment (UserCard selectedCard, Xamarin.Contacts.Phone selectedPhone = null, string selectedMessage = null)
		{
			SelectedCard = selectedCard;
			SelectedPhone = selectedPhone;
			SelectedMessage = selectedMessage;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			var view = inflater.Inflate (Resource.Layout.SharedCard, container, false);
			
			var imgCardHorizontal = view.FindViewById<ImageView> (Resource.Id.imgShareHorizontal);
			var imgCardVertical = view.FindViewById<ImageView> (Resource.Id.imgShareVertical);

			imgCardHorizontal.Visibility = imgCardVertical.Visibility = ViewStates.Invisible;

			lblShareError = view.FindViewById<TextView> (Resource.Id.lblShareError);
			imgCheckShared = view.FindViewById<ImageView> (Resource.Id.imgCheckShared);

			HideFeedbackLabels ();

			var btnShareCard = view.FindViewById<Button> (Resource.Id.btnShareCard);
			btnShareCard.Click += delegate {
				ShareCard ();
			};

			txtShareDisplayName = view.FindViewById<TextView> (Resource.Id.txtShareDisplayName);
			txtShareEmail = view.FindViewById<TextView> (Resource.Id.txtShareEmail);
			txtSharePhoneNumber = view.FindViewById<TextView> (Resource.Id.txtSharePhoneNumber);
			txtShareMessage = view.FindViewById<TextView> (Resource.Id.txtShareMessage);

			if (!string.IsNullOrEmpty (SelectedMessage)) {
				txtShareMessage.Text = SelectedMessage;
			}

			txtShareDisplayName.Text = currentDisplayName = UISubscriptionService.CurrentUser.UserAccount.DisplayName;
			var imgShareHorizontal = view.FindViewById<ImageView> (Resource.Id.imgShareHorizontal);
			var imgShareVertical = view.FindViewById<ImageView> (Resource.Id.imgShareVertical);

			if (SelectedPhone != null) {
				txtSharePhoneNumber.Text = SelectedPhone.Number;
			}

			if (SelectedCard != null) {
				var fileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + SelectedCard.Card.FrontFileName);
				var uri = Uri.Parse (fileName);

				var isHorizontal = SelectedCard.Card.FrontOrientation == "H";
				var imgDisplay = isHorizontal ? imgShareHorizontal : imgShareVertical;
				imgShareHorizontal.Visibility = !isHorizontal ? ViewStates.Gone : ViewStates.Visible;
				imgShareVertical.Visibility = isHorizontal ? ViewStates.Gone : ViewStates.Visible;

				imgDisplay.SetImageURI (uri);
			} else {
				imgShareHorizontal.Visibility = imgShareVertical.Visibility = ViewStates.Gone;
			}

			var btnClose = view.FindViewById<ImageButton> (Resource.Id.btnClose);
			btnClose.Click += delegate {
				var panel = new ButtonPanelFragment ();
				panel.SelectedCard = SelectedCard;
				((MainActivity)Activity).UnloadFragment (panel);
			};

			var btnContacts = view.FindViewById (Resource.Id.btnContacts);
			btnContacts.Click += delegate {
				var contactsAdapter = new ContactsAdapter (Activity, MainActivity.Contacts, SelectedCard, txtShareMessage.Text);
				((MainActivity)Activity).LoadFragment (new ContactsFragment (contactsAdapter, SelectedCard, txtShareMessage.Text));
			};
	
			return view;
		}

		void ShareCard ()
		{

			HideFeedbackLabels ();

			var email = txtShareEmail.Text;
			var phoneNumber = txtSharePhoneNumber.Text;
			var displayName = txtShareDisplayName.Text;
			var personalMessage = txtShareMessage.Text;

			if (string.IsNullOrEmpty (email) && string.IsNullOrEmpty (phoneNumber)) {
				
				return;
			}

			if (string.IsNullOrEmpty (displayName)) {
				
				return;
			}

			// normalize the phone number if there is one
			if (!string.IsNullOrEmpty (phoneNumber)) {
				phoneNumber = phoneNumber.Replace ("(", "").Replace (")", "").Replace (".", "").Replace ("-", "").Replace (" ", "");
				var smsTask = MessagingPlugin.SmsMessenger;
				EmailTemplateController.GetTemplate (EmailTemplateCode.SharedCardSMS, UISubscriptionService.AuthToken).ContinueWith (r => {

					var template = Newtonsoft.Json.JsonConvert.DeserializeObject<EmailTemplateResponse> (r.Result);
					if (template != null) {
						string message = string.Format (template.Template.Subject, displayName) + System.Environment.NewLine + System.Environment.NewLine +
						                 template.Template.Body;
						var userId = Utils.DecodeUserId (UISubscriptionService.AuthToken);
						var parameters = new QuickShareLink {
							CardId = SelectedCard.Card.CardId,
							From = userId,
							DisplayName = displayName,
							PersonalMessage = personalMessage
						};
						var resp = BranchApiController.GetBranchUrl (parameters);
						string shortendUrl = resp;
						if (shortendUrl != null && !shortendUrl.Contains ("error")) {

							var branchUrl = Newtonsoft.Json.JsonConvert.DeserializeObject<BranchUrl> (shortendUrl);
							message = message + branchUrl.url;

							Activity.RunOnUiThread (() => smsTask.SendSms (phoneNumber, message));
						} else {
							Activity.RunOnUiThread (() => ShowAlert (
								"Application Error", 
								"There was a problem contacting the service that creates the text message. Please try again when you have a better internet connection.", 
								"Ok", null)
							);
						}

//						Activity.RunOnUiThread( ()=> {
//						Intent sendIntent = new Intent(Intent.ActionSend);
//						//sendIntent.SetClassName("com.android.mms", "com.android.mms.ui.ComposeMessageActivity");
//						sendIntent.PutExtra("address", phoneNumber);
//						sendIntent.PutExtra("sms_body", message);
//
//						File file1 = new File("mFileName");
//						if(file1.Exists())
//						{
//							//File Exist
//						}
//						//Uri uri = Uri.FromFile(file1);
//						//sendIntent.PutExtra(Intent.ExtraStream, uri);
//						sendIntent.SetType("image/*");
//						StartActivity(sendIntent);
//						});
					}
				});

				lblShareError.Visibility = ViewStates.Invisible;
				imgCheckShared.Visibility = ViewStates.Visible;
			} else {
				var ctrl = new SharedCardController ();
				var response = ctrl.ShareCard (SelectedCard.Card, email, phoneNumber, UISubscriptionService.AuthToken);
				if (!string.IsNullOrEmpty (response) && response.Contains ("true")) {
					imgCheckShared.Visibility = ViewStates.Visible;
				} else {
					lblShareError.Visibility = ViewStates.Invisible;
					imgCheckShared.Visibility = ViewStates.Visible;
				}
			}

			if (!currentDisplayName.Equals (UISubscriptionService.CurrentUser.UserAccount.DisplayName, System.StringComparison.Ordinal)) {
				AccountController.UpdateDisplayName (txtShareDisplayName.Text, UISubscriptionService.AuthToken);	
				UISubscriptionService.CurrentUser.UserAccount.DisplayName = txtShareDisplayName.Text;
			}

			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_SHARE, 0);
		}

		void HideFeedbackLabels ()
		{
			lblShareError.Visibility = imgCheckShared.Visibility = ViewStates.Invisible;
		}
	}
}

