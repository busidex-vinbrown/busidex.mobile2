using Android.App;
using Android.OS;
using Android.Widget;
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
			ActivityController.SaveActivity ((long)EventSources.Call, UserCard.Card.CardId, userToken);
			StartActivity (intent); 
		}

		protected override void OnImageDownloadCompleted (Uri uri)
		{
			base.OnImageDownloadCompleted (uri);

			var imgPhoneCardHorizontal = FindViewById<ImageView> (Resource.Id.imgPhoneCardHorizontal);
			var imgPhoneCardVertical = FindViewById<ImageView> (Resource.Id.imgPhoneCardVertical);
			var isHorizontal = UserCard.Card.FrontOrientation == "H";
			var imgDisplay = isHorizontal ? imgPhoneCardHorizontal : imgPhoneCardVertical;
			imgPhoneCardHorizontal.Visibility = isHorizontal ? global::Android.Views.ViewStates.Visible : global::Android.Views.ViewStates.Gone;
			imgPhoneCardVertical.Visibility = isHorizontal ? global::Android.Views.ViewStates.Gone : global::Android.Views.ViewStates.Visible;

			imgDisplay.SetImageURI (uri);
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetContentView (Resource.Layout.Phone);

			base.OnCreate (savedInstanceState);

			if (UserCard != null && UserCard.Card.PhoneNumbers != null) {

				var lstPhoneNumbers = FindViewById<ListView> (Resource.Id.lstPhoneNumbers);
				var adapter = new PhoneNumberEntryAdapter (this, Resource.Id.lstPhoneNumbers, UserCard.Card.PhoneNumbers);
				adapter.PhoneNumberDialed += DialPhoneNumber;

				lstPhoneNumbers.Adapter = adapter;
			}
		}
	}
}