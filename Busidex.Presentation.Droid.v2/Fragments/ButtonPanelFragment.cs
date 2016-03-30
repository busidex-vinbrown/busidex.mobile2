
using Android.Support.V4.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile.Models;
using Android.Net;
using System.IO;

namespace Busidex.Presentation.Droid.v2
{
	public class ButtonPanelFragment : Fragment
	{
		public UserCard SelectedCard { get; set; }
		View view;

		public override void OnResume ()
		{
			base.OnResume ();

			try{
				if(SelectedCard == null){
					if (Activity != null) {
						((MainActivity)Activity).UnloadFragment (null, Resource.Animation.SlideUpAnimation, Resource.Animation.SlideDownAnimation);
					}
					return;
				}

				var btnHideInfo = view.FindViewById<ImageButton> (Resource.Id.btnHideInfo);

				btnHideInfo.Click += (sender, e) => ((MainActivity)Activity).UnloadFragment (null, Resource.Animation.SlideUpAnimation, Resource.Animation.SlideDownAnimation);

				var fileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + SelectedCard.Card.FrontFileName);
				var uri = Uri.Parse (fileName);

				var imgH = view.FindViewById<ImageButton> (Resource.Id.imgPanelCardPreviewH);
				var imgV = view.FindViewById<ImageButton> (Resource.Id.imgPanelCardPreviewV);
				ImageButton currentImageControl;

				if (SelectedCard.Card.FrontOrientation == "H") {
					currentImageControl = imgH;
					imgH.Visibility = ViewStates.Visible;
					imgV.Visibility = ViewStates.Gone;
				}else{
					currentImageControl = imgV;
					imgV.Visibility = ViewStates.Visible;
					imgH.Visibility = ViewStates.Gone;
				}
				currentImageControl.SetImageURI (uri);
				currentImageControl.Click += delegate {
					var fragment = new CardDetailFragment();
					fragment.SelectedCard = SelectedCard;
					((MainActivity)Activity).ShowCard (fragment);
				};

				var txtName = view.FindViewById<TextView> (Resource.Id.txtName);
				var txtCompanyName = view.FindViewById<TextView> (Resource.Id.txtCompanyName);

				txtName.Text = SelectedCard.Card.Name;
				txtCompanyName.Text = SelectedCard.Card.CompanyName;

				var OpenBrowserIntent = new Intent (Intent.ActionView);

				var SendEmailIntent = new Intent(Intent.ActionSend);
				var data = Newtonsoft.Json.JsonConvert.SerializeObject(SelectedCard);
				SendEmailIntent.PutExtra ("Card", data);

				SendEmailIntent.PutExtra (Intent.ExtraEmail, new []{SelectedCard.Card.Email} );
				SendEmailIntent.SetType ("message/rfc822");

				var geoString = "geo:0,0";
				if(SelectedCard.Card.Addresses != null && SelectedCard.Card.Addresses.Count > 0){
					var address = SelectedCard.Card.Addresses [0];
					geoString = string.Format ("geo:0,0?q={0}", address);
				}

				var geoUri = Uri.Parse (geoString);
				var OpenMapIntent = new Intent (Intent.ActionView, geoUri);
				OpenMapIntent.PutExtra ("Card", data);

				var url = string.Empty;
				if(!string.IsNullOrEmpty(SelectedCard.Card.Url)){
					url = !SelectedCard.Card.Url.StartsWith ("http", System.StringComparison.Ordinal) ? "http://" + SelectedCard.Card.Url : SelectedCard.Card.Url;
				}
				uri = Uri.Parse (url);
				OpenBrowserIntent.SetData (uri);
				OpenBrowserIntent.PutExtra ("Card", data);

				var btnPhone = view.FindViewById<ImageButton> (Resource.Id.btnPanelPhone);
				var btnNotes = view.FindViewById<ImageButton> (Resource.Id.btnPanelNotes);

				var btnPanelShare = view.FindViewById<ImageButton> (Resource.Id.btnPanelShare);
				var btnEmail = view.FindViewById<ImageButton> (Resource.Id.btnPanelEmail);
				var btnMap = view.FindViewById<ImageButton> (Resource.Id.btnPanelMap);
				var btnBrowser = view.FindViewById<ImageButton> (Resource.Id.btnPanelBrowser);
				var btnAddToMyBusidex = view.FindViewById<ImageButton> (Resource.Id.btnPanelAdd);
				var btnRemoveFromMyBusidex = view.FindViewById<ImageButton> (Resource.Id.btnPanelRemove);

				const float ENABLED = 1f;
				const float DISABLED = .2f;

				btnPhone.Enabled = (SelectedCard.Card.PhoneNumbers != null && SelectedCard.Card.PhoneNumbers.Count > 0);
				btnPhone.Alpha = btnPhone.Enabled ? ENABLED : DISABLED;

				btnEmail.Enabled = !string.IsNullOrEmpty (SelectedCard.Card.Email);
				btnEmail.Alpha = btnEmail.Enabled ? ENABLED : DISABLED;

				//btnNotes.Enabled = ShowNotes;
				btnNotes.Alpha = btnNotes.Enabled ? ENABLED : DISABLED;

				btnBrowser.Enabled = !string.IsNullOrEmpty (SelectedCard.Card.Url);
				btnBrowser.Alpha = btnBrowser.Enabled ? ENABLED : DISABLED;

				btnMap.Enabled = (SelectedCard.Card.Addresses != null && SelectedCard.Card.Addresses.Count > 0 && SelectedCard.Card.Addresses [0].HasAddress);
				btnMap.Alpha = btnMap.Enabled ? ENABLED : DISABLED;

				btnAddToMyBusidex.Visibility = SelectedCard.Card.ExistsInMyBusidex ? ViewStates.Gone : ViewStates.Visible;
				btnRemoveFromMyBusidex.Visibility = SelectedCard.Card.ExistsInMyBusidex ? ViewStates.Visible : ViewStates.Gone;

				btnPhone.Click += delegate {
					var fragment = new PhoneFragment(SelectedCard);
					((MainActivity)Activity).ShowPhoneDialer (fragment);
				};

				btnNotes.Click += delegate {
					var fragment = new NotesFragment(SelectedCard);
					((MainActivity)Activity).ShowNotes (fragment);
				};

				btnEmail.Click += delegate{
					((MainActivity)Activity).SendEmail(SendEmailIntent, SelectedCard.CardId);
				};

				btnPanelShare.Click += delegate {
					var fragment = new ShareCardFragment(SelectedCard);
					((MainActivity)Activity).ShareCard(fragment);
				};

				btnBrowser.Click += delegate {
					((MainActivity)Activity).OpenBrowser(OpenBrowserIntent);
				};

				btnAddToMyBusidex.Click += delegate {

					((MainActivity)Activity).AddToMyBusidex(SelectedCard);

					btnAddToMyBusidex.Visibility = ViewStates.Gone;
					btnRemoveFromMyBusidex.Visibility = ViewStates.Visible;
				};

				btnRemoveFromMyBusidex.Click += delegate {

					((MainActivity)Activity).RemoveFromMyBusidex(SelectedCard);

					btnRemoveFromMyBusidex.Visibility = ViewStates.Gone;
					btnAddToMyBusidex.Visibility = ViewStates.Visible;
				};

				btnMap.Click += delegate {
					((MainActivity)Activity).OpenMap(OpenMapIntent);
				};
			}
			catch(System.Exception ex){
				Xamarin.Insights.Report (ex);
				if (Activity != null) {
					((MainActivity)Activity).UnloadFragment (null, Resource.Animation.SlideUpAnimation, Resource.Animation.SlideDownAnimation);
				}
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			view = inflater.Inflate(Resource.Layout.ButtonPanel, container, false);



			return view;
		}
	}
}

