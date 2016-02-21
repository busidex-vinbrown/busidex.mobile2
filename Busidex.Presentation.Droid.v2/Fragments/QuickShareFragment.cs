using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile.Models;
using System.IO;
using Android.Net;

namespace Busidex.Presentation.Droid.v2
{
	public class QuickShareFragment : GenericViewPagerFragment
	{
		readonly UserCard SelectedCard;
		TextView txtQuickShareMessage;
		ImageView imgHCard;
		ImageView imgVCard;
		readonly string DisplayName;
		readonly string PersonalMessage;
		ProgressBar progress3;

		public QuickShareFragment () : base()
		{
		}

		public QuickShareFragment(UserCard selectedCard, string displayName, string message) : base(){
			SelectedCard = selectedCard;
			DisplayName = displayName;
			PersonalMessage = message;
		}
			
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.QuickShare, container, false);

			txtQuickShareMessage = view.FindViewById<TextView> (Resource.Id.txtQuickShareMessage);
			imgHCard = view.FindViewById<ImageView> (Resource.Id.imgQuickShareCardHorizontal);
			imgVCard = view.FindViewById<ImageView> (Resource.Id.imgQuickShareCardVertical);

			txtQuickShareMessage.Text = string.Format (GetString (Resource.String.QuickShareMessage), 
				DisplayName, System.Environment.NewLine + System.Environment.NewLine, PersonalMessage);

			progress3 = view.FindViewById<ProgressBar> (Resource.Id.progressBar3);
			progress3.Visibility = ViewStates.Gone;

			SetImage ();

			var btnClose = view.FindViewById<ImageButton> (Resource.Id.btnClose);
			btnClose.Click += delegate {
				((MainActivity)Activity).UnloadFragment(null);
			};
			return view;
		}

		void SetImage(){

			var fImageUrl = Busidex.Mobile.Resources.THUMBNAIL_PATH + SelectedCard.Card.FrontFileName;
			var fileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + SelectedCard.Card.FrontFileName);
			var isHorizontal = SelectedCard.Card.FrontOrientation == "H";
			var imgDisplay = isHorizontal ? imgHCard : imgVCard;
			imgHCard.Visibility = !isHorizontal ? ViewStates.Gone : ViewStates.Visible;
			imgVCard.Visibility = isHorizontal ? ViewStates.Gone : ViewStates.Visible;

			if (!File.Exists (fileName)) {
				progress3.Visibility = ViewStates.Visible;
				Busidex.Mobile.Utils.DownloadImage (fImageUrl, Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + SelectedCard.Card.FrontFileName).ContinueWith (
					r => {
						Activity.RunOnUiThread( () => {
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

