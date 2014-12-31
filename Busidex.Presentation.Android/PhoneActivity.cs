using Android.App;
using Android.OS;
using Android.Widget;
using System.IO;
using Android.Net;
using Busidex.Mobile.Models;
using Busidex.Mobile;
using Android.Content;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "PhoneActivity")]			
	public class PhoneActivity : BaseCardActivity
	{

		void DialPhoneNumber(PhoneNumber number){
			var userToken = GetAuthCookie ();
			var uri = Uri.Parse ("tel:" + number.Number);
			var intent = new Intent (Intent.ActionView, uri); 
			ActivityController.SaveActivity ((long)EventSources.Call, Card.Card.CardId, userToken);
			StartActivity (intent); 
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Phone);

			var imgPhoneCard = FindViewById<ImageView> (Resource.Id.imgPhoneCard);
			var frontFileName = System.IO.Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Card.Card.FrontFileName);
			var frontUri = Uri.Parse (frontFileName);

			if (File.Exists (frontFileName)) {
				imgPhoneCard.SetImageURI (frontUri);
			}else{

				//ShowOverlay ();

				Utils.DownloadImage (Busidex.Mobile.Resources.CARD_PATH + Card.Card.FrontFileName, Busidex.Mobile.Resources.DocumentsPath, Card.Card.FrontFileName).ContinueWith (t => {
					RunOnUiThread (() => {
						imgPhoneCard.SetImageURI (frontUri);
						//Overlay.Hide();
					});
				});
			}

			if (Card != null && Card.Card.PhoneNumbers != null) {

				var lstPhoneNumbers = FindViewById<ListView> (Resource.Id.lstPhoneNumbers);
				var adapter = new PhoneNumberEntryAdapter (this, Resource.Id.lstPhoneNumbers, Card.Card.PhoneNumbers);
				adapter.PhoneNumberDialed += DialPhoneNumber;

				lstPhoneNumbers.Adapter = adapter;
			}
		}
	}
}