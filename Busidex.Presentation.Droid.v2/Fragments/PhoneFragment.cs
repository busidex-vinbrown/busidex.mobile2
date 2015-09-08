﻿using Android.Content;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using System.IO;

namespace Busidex.Presentation.Droid.v2
{
	public class PhoneFragment : GenericViewPagerFragment
	{
		private readonly UserCard SelectedCard;

		public PhoneFragment () : base()
		{
		}

		public PhoneFragment(UserCard selectedCard) : base(){
			SelectedCard = selectedCard;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.Phone, container, false);

			if (SelectedCard != null && SelectedCard.Card.PhoneNumbers != null) {

				var lstPhoneNumbers = view.FindViewById<ListView> (Resource.Id.lstPhoneNumbers);
				var adapter = new PhoneNumberEntryAdapter (Activity, Resource.Id.lstPhoneNumbers, SelectedCard.Card.PhoneNumbers);
				adapter.PhoneNumberDialed += DialPhoneNumber;

				lstPhoneNumbers.Adapter = adapter;

				var fileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + SelectedCard.Card.FrontFileName);
				var uri = Uri.Parse (fileName);

				var imgPhoneCardHorizontal = view.FindViewById<ImageView> (Resource.Id.imgPhoneCardHorizontal);
				var imgPhoneCardVertical = view.FindViewById<ImageView> (Resource.Id.imgPhoneCardVertical);
				var isHorizontal = SelectedCard.Card.FrontOrientation == "H";
				var imgDisplay = isHorizontal ? imgPhoneCardHorizontal : imgPhoneCardVertical;
				imgPhoneCardHorizontal.Visibility = !isHorizontal ? ViewStates.Gone : ViewStates.Visible;
				imgPhoneCardVertical.Visibility = isHorizontal ? ViewStates.Gone : ViewStates.Visible;

				imgDisplay.SetImageURI (uri);

			}
			return view;
		}

		void DialPhoneNumber(PhoneNumber number){
			var userToken = applicationResource.GetAuthCookie ();
			var uri = Uri.Parse ("tel:" + number.Number);
			var intent = new Intent (Intent.ActionView, uri); 
			ActivityController.SaveActivity ((long)EventSources.Call, SelectedCard.Card.CardId, userToken);
			StartActivity (intent); 
		}
	}
}
