using Xamarin.Contacts;
using Android.Widget;
using Android.App;
using System.Collections.Generic;
using Android.Views;
using System;
using System.Linq;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.Droid.v2
{
	public class ContactsAdapter : BaseExpandableListAdapter {
		
		readonly Activity Context;
		readonly List<Contact> ContactList;
		List<Contact>[] ContactGroups;

		public ContactsAdapter(Activity newContext, List<Contact> contactList)
		{
			Context = newContext;
			ContactList = contactList;
			ContactGroups = new List<Contact>[26];

			for(var i=0; i < 26; i++){
				var letter = ((char)(65 + i)).ToString ();
				var newGroup = new List<Contact> ();
				newGroup.AddRange (filterList (letter));
				ContactGroups[i] =newGroup;
			}
		}

		//protected List<Contact> DataList { get; set; }

		List<Contact> filterList(string letter){
			return ContactList.FindAll ((Contact obj) => {
				if(!string.IsNullOrEmpty(obj.LastName)){
					return obj.LastName.ToLowerInvariant().StartsWith (letter.ToLowerInvariant(), StringComparison.Ordinal);
				}
				if(!string.IsNullOrEmpty(obj.DisplayName)){
					return obj.DisplayName.ToLowerInvariant().StartsWith (letter.ToLowerInvariant(), StringComparison.Ordinal);;
				}

				return false;
			});
		}

		public override View GetGroupView (int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
		{
			View header = convertView ?? Context.LayoutInflater.Inflate (Resource.Layout.ListGroup, null);
			var letter = ((char)(65 + groupPosition)).ToString ();
			var title = ContactGroups [groupPosition].Count;
			header.FindViewById<TextView> (Resource.Id.txtContactListHeader).Text = letter + string.Format(" ({0})", title);

			return header;
		}

		public override View GetChildView (int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
		{
			View row = convertView ?? Context.LayoutInflater.Inflate (Resource.Layout.ContactListItem, null);
			string newId =string.Empty, newValue = string.Empty;
			GetChildViewHelper (groupPosition, childPosition, out newId, out newValue);
			var thisGroup = ContactGroups [groupPosition];
			var email =  thisGroup[childPosition].Emails.FirstOrDefault ();
			var name = thisGroup [childPosition].DisplayName;
			var first = thisGroup [childPosition].FirstName;
			var last = thisGroup [childPosition].LastName;
			row.FindViewById<TextView> (Resource.Id.txtContactDisplayName).Text = string.IsNullOrEmpty(name) ? email.Address : name;
			var thumbnail = row.FindViewById<ImageView> (Resource.Id.imgContact);
			if(thisGroup[childPosition].GetThumbnail() == null){
				thumbnail.Visibility = ViewStates.Gone;
			}else{
				thumbnail.SetImageBitmap (thisGroup [childPosition].GetThumbnail ());
				thumbnail.Visibility = ViewStates.Visible;
			}

			row.Click += delegate {
				((MainActivity)Context).LoadFragment(new ContactProfileFragment(thisGroup [childPosition]));
			};

			return row;
		}

		public override int GetChildrenCount (int groupPosition)
		{
			var letter = ((char)(65 + groupPosition)).ToString ();
			var results = filterList (letter);

			return results.Count;
		}

		public override int GroupCount {
			get {
				return 26;
			}
		}

		void GetChildViewHelper (int groupPosition, int childPosition, out string Id, out string Value)
		{
			var letter = ((char)(65 + groupPosition)).ToString ();
			var results = filterList (letter);

			Id = results [childPosition].Id;
			Value = results [childPosition].DisplayName;
		}

		#region implemented abstract members of BaseExpandableListAdapter

		public override Java.Lang.Object GetChild (int groupPosition, int childPosition)
		{
			throw new NotImplementedException ();
		}

		public override long GetChildId (int groupPosition, int childPosition)
		{
			return childPosition;
		}

		public override Java.Lang.Object GetGroup (int groupPosition)
		{
			throw new NotImplementedException ();
		}

		public override long GetGroupId (int groupPosition)
		{
			return groupPosition;
		}

		public override bool IsChildSelectable (int groupPosition, int childPosition)
		{
			//throw new NotImplementedException ();
			return groupPosition > 0 && groupPosition < ContactList.Count;
		}

		public override bool HasStableIds {
			get {
				return true;
			}
		}

		#endregion
	}
}

