
//using System;

//using Android.App;
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

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			var view = inflater.Inflate(Resource.Layout.ButtonPanel, container, false);

			var btnHideInfo = view.FindViewById<ImageButton> (Resource.Id.btnHideInfo);

			btnHideInfo.Click += (sender, e) => ((MainActivity)Activity).UnloadFragment (this, Resource.Animation.SlideUpAnimation, Resource.Animation.SlideDownAnimation);
				

			var fileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + SelectedCard.Card.FrontFileName);
			var uri = Uri.Parse (fileName);

			var imgH = view.FindViewById<ImageView> (Resource.Id.imgPanelCardPreviewH);
			var imgV = view.FindViewById<ImageView> (Resource.Id.imgPanelCardPreviewV);
			
			if (SelectedCard.Card.FrontOrientation == "H") {
				imgH.SetImageURI (uri);
				imgH.Visibility = ViewStates.Visible;
				imgV.Visibility = ViewStates.Gone;
			}else{
				imgV.SetImageURI (uri);
				imgV.Visibility = ViewStates.Visible;
				imgH.Visibility = ViewStates.Gone;
			}

			var txtName = view.FindViewById<TextView> (Resource.Id.txtName);
			var txtCompanyName = view.FindViewById<TextView> (Resource.Id.txtCompanyName);

			txtName.Text = SelectedCard.Card.Name;
			txtCompanyName.Text = SelectedCard.Card.CompanyName;

			//var userCard = this [position];// Cards [position];
//			var PhoneIntent = new Intent(Activity, typeof(PhoneActivity));
//			var NotesIntent = new Intent(Activity, typeof(NotesActivity));
//			var ShareCardIntent = new Intent(Activity, typeof(ShareCardActivity));
			var SendEmailIntent = new Intent(Intent.ActionSend);
			//var AddToMyBusidexIntent = new Intent (Context, Activity.GetType ());
			//var RemoveFromMyBusidexIntent = new Intent (Activity, Activity.GetType ());

			var OpenBrowserIntent = new Intent (Intent.ActionView);

			var data = Newtonsoft.Json.JsonConvert.SerializeObject(SelectedCard);
//			PhoneIntent.PutExtra("Card", data);
//			NotesIntent.PutExtra("Card", data);
//			ShareCardIntent.PutExtra("Card", data);
//			AddToMyBusidexIntent.PutExtra ("Card", data);
//			RemoveFromMyBusidexIntent.PutExtra ("Card", data);
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

//			btnPhone.Click -= OnPhoneButtonClicked;
//			btnPhone.Click += OnPhoneButtonClicked;
//
//			btnNotes.Click -= OnNotesButtonClicked;
//			btnNotes.Click += OnNotesButtonClicked;
//
//			btnShareCard.Click -= OnShareCardButtonClicked;
//			btnShareCard.Click += OnShareCardButtonClicked;
//
//			btnEmail.Click -= OnEmailButtonClicked;
//			btnEmail.Click += OnEmailButtonClicked;
//
//			btnBrowser.Click -= OnBrowserButtonClicked;
//			btnBrowser.Click += OnBrowserButtonClicked;
//
//			btnAddToMyBusidex.Click -= OnAddToMyBusidexClicked;
//			btnAddToMyBusidex.Click += OnAddToMyBusidexClicked;
//
//			btnRemoveFromMyBusidex.Click -= OnRemoveFromMyBusidexClicked;
//			btnRemoveFromMyBusidex.Click += OnRemoveFromMyBusidexClicked;
//
//			btnMap.Click -= OnMapButtonClicked;
//			btnMap.Click += OnMapButtonClicked;

			return view;
		}
	}
}

