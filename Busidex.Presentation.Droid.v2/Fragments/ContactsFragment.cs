
using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.Droid.v2
{
	public class ContactsFragment : GenericViewPagerFragment
	{
		readonly ContactsAdapter contactsAdapter;
		readonly UserCard SelectedCard;
		readonly string SelectedMessage;

		public ContactsFragment ()
		{
			
		}

		public ContactsFragment (ContactsAdapter adapter, UserCard card, string selectedMessage = null)
		{
			contactsAdapter = adapter;
			SelectedCard = card;
			SelectedMessage = selectedMessage;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			var view = inflater.Inflate (Resource.Layout.Contacts, container, false);
			var listView = view.FindViewById<ExpandableListView> (Resource.Id.lstContacts);
			listView.SetAdapter (contactsAdapter);

			var btnContactListHideInfo = view.FindViewById<ImageButton> (Resource.Id.btnContactListHideInfo);
			btnContactListHideInfo.Click += delegate {
				var card = SelectedCard; 
				var fragment = new ShareCardFragment (card, null, SelectedMessage);
				((MainActivity)Activity).UnloadFragment (fragment);
			};

			return view;
		}
	}
}

