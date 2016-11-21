using System.IO;

using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile.Models;
using Android.Net;
using Newtonsoft.Json;
using Android.Content;

namespace Busidex.Presentation.Droid.v2
{
	[Activity (Label = "QuickShareActivity")]
	public class QuickShareActivity : Activity
	{
		UserCard SelectedCard;
		TextView txtQuickShareMessage;
		ImageView imgHCard;
		ImageView imgVCard;
		string DisplayName;
		string PersonalMessage;
		ProgressBar progress3;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SelectedCard = JsonConvert.DeserializeObject<UserCard> (Intent.GetStringExtra ("SelectedCard"));
			DisplayName = Intent.GetStringExtra ("DisplayName");
			PersonalMessage = Intent.GetStringExtra ("PersonalMessage");

			SetContentView (Resource.Layout.QuickShare);

			ActionBar.Hide ();

			RequestedOrientation = ScreenOrientation.Portrait;

			txtQuickShareMessage = FindViewById<TextView> (Resource.Id.txtQuickShareMessage);
			txtQuickShareMessage.Visibility = ViewStates.Visible;

			imgHCard = FindViewById<ImageView> (Resource.Id.imgQuickShareCardHorizontal);
			imgVCard = FindViewById<ImageView> (Resource.Id.imgQuickShareCardVertical);

			txtQuickShareMessage.Text = string.Format (GetString (Resource.String.QuickShareMessage),
				DisplayName, System.Environment.NewLine + System.Environment.NewLine, PersonalMessage);

			progress3 = FindViewById<ProgressBar> (Resource.Id.progressBar3);
			progress3.Visibility = ViewStates.Gone;

			SetImage ();

			var btnClose = FindViewById<ImageButton> (Resource.Id.btnClose);
			btnClose.Click += delegate {
				var intent = new Intent (this, typeof (MainActivity));
				StartActivity (intent);
				Finish ();
			};
		}

		void SetImage ()
		{

			var fImageUrl = Mobile.Resources.THUMBNAIL_PATH + SelectedCard.Card.FrontFileName;
			var fileName = Path.Combine (Mobile.Resources.DocumentsPath, Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + SelectedCard.Card.FrontFileName);
			var isHorizontal = SelectedCard.Card.FrontOrientation == "H";
			var imgDisplay = isHorizontal ? imgHCard : imgVCard;
			imgHCard.Visibility = !isHorizontal ? ViewStates.Gone : ViewStates.Visible;
			imgVCard.Visibility = isHorizontal ? ViewStates.Gone : ViewStates.Visible;

			if (!File.Exists (fileName)) {
				progress3.Visibility = ViewStates.Visible;
				Mobile.Utils.DownloadImage (fImageUrl, Mobile.Resources.DocumentsPath, Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + SelectedCard.Card.FrontFileName).ContinueWith (
					r => {
						RunOnUiThread (() => {
							var uri = Uri.Parse (fileName);
							imgDisplay.SetImageURI (uri);
							progress3.Visibility = ViewStates.Gone;
						});
					});
			} else {
				var uri = Uri.Parse (fileName);
				imgDisplay.SetImageURI (uri);
			}
		}
	}
}
