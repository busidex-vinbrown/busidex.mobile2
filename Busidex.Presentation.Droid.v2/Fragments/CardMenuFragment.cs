
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Busidex.Mobile;

namespace Busidex.Presentation.Droid.v2
{
	public class CardMenuFragment : GenericViewPagerFragment
	{
		View cardMenu;

		public override void OnResume ()
		{
			base.OnResume ();

			if (IsVisible) {
				BaseApplicationResource.TrackScreenView (Mobile.Resources.GA_SCREEN_CARD_MENU);
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			cardMenu = inflater.Inflate (Resource.Layout.CardMenu, container, false);

			var btnCardImage = cardMenu.FindViewById<Button> (Resource.Id.btnCardImage);
			var btnVisibility = cardMenu.FindViewById<Button> (Resource.Id.btnVisibility);
			var btnContactInfo = cardMenu.FindViewById<Button> (Resource.Id.btnContactInfo);
			var btnSearchInfo = cardMenu.FindViewById<Button> (Resource.Id.btnSearchInfo);
			var btnTags = cardMenu.FindViewById<Button> (Resource.Id.btnTags);
			var btnAddressInfo = cardMenu.FindViewById<Button> (Resource.Id.btnAddressInfo);
			var btnBack = cardMenu.FindViewById<ImageButton> (Resource.Id.btnBack);

			btnCardImage.Click += delegate {
				var fragment = new CardImageEditFragment ();
				((MainActivity)Activity).ShowCardEditFragment (fragment);
			};
			btnVisibility.Click += delegate {
				var fragment = new CardVisibilityFragment ();
				((MainActivity)Activity).ShowCardEditFragment (fragment);
			};
			btnContactInfo.Click += delegate {
				var fragment = new CardContactInfoFragment ();
				((MainActivity)Activity).ShowCardEditFragment (fragment);
			};
			btnSearchInfo.Click += delegate {
				var fragment = new CardSearchInfoFragment ();
				((MainActivity)Activity).ShowCardEditFragment (fragment);
			};
			btnTags.Click += delegate {
				var fragment = new CardTagsFragment ();
				((MainActivity)Activity).ShowCardEditFragment (fragment);
			};
			btnAddressInfo.Click += delegate {
				var fragment = new CardAddressInfoFragment ();
				((MainActivity)Activity).ShowCardEditFragment (fragment);
			};

			btnBack.Click += delegate {
				((MainActivity)Activity).UnloadFragment (null);
			};
			return cardMenu;
		}

		public override void OnDestroyView ()
		{
			base.OnDestroyView ();
			cardMenu = null;
		}
	}
}

