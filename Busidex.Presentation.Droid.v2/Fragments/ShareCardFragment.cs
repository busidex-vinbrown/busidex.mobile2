using Android.OS;
using Android.Views;
using Android.Widget;
using System.IO;
using Android.Net;
using Busidex.Mobile;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.Droid.v2
{
	public class ShareCardFragment : GenericViewPagerFragment
	{
		TextView lblShareError;
		TextView txtShareDisplayName;
		TextView txtShareEmail;
		ImageView imgCheckShared;
		readonly UserCard SelectedCard;

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
			var displayName = txtShareDisplayName.Text;

			if(string.IsNullOrEmpty(email)){
				//ShowAlert("Missing Information", "Please enter an email address");
				return;
			}

			if(string.IsNullOrEmpty(displayName)){
				//ShowAlert("Missing Information", "Please enter a display name");
				return;
			}

			var ctrl = new SharedCardController();
			var response = ctrl.ShareCard (SelectedCard.Card, email, token);

			if( !string.IsNullOrEmpty(response) && response.Contains("true")){
				imgCheckShared.Visibility = ViewStates.Visible;
			}else{
				lblShareError.Visibility = ViewStates.Invisible;
				imgCheckShared.Visibility = ViewStates.Visible;
			}

			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_SHARE, 0);
		}

		void HideFeedbackLabels(){
			lblShareError.Visibility = imgCheckShared.Visibility = ViewStates.Invisible;
		}
	}
}

