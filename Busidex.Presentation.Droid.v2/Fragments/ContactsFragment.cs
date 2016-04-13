
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
using Xamarin.Contacts;
using Busidex.Mobile.Models;


namespace Busidex.Presentation.Droid.v2
{
	public class ContactsFragment : GenericViewPagerFragment
	{
		readonly ContactsAdapter contactsAdapter;
		readonly UserCard SelectedCard;

		public ContactsFragment(){
			
		}

		public ContactsFragment(ContactsAdapter adapter, UserCard card){
			contactsAdapter = adapter;
			SelectedCard = card;
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			var view = inflater.Inflate(Resource.Layout.Contacts, container, false);
			var listView = view.FindViewById<ExpandableListView> (Resource.Id.lstContacts);
			listView.SetAdapter (contactsAdapter);

			var btnContactListHideInfo = view.FindViewById<ImageButton> (Resource.Id.btnContactListHideInfo);
			btnContactListHideInfo.Click += delegate {
				var card = SelectedCard;
				var fragment = new ShareCardFragment(card, null);
				((MainActivity)Activity).UnloadFragment (fragment);
			};

			return view;
		}
	}
}

