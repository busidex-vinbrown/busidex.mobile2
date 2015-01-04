
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Busidex.Mobile.Models;
using Busidex.Mobile;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "Members")]			
	public class OrganizationMembersActivity : BaseCardActivity
	{
		List<UserCard> Cards { get; set; }

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetContentView (Resource.Layout.OrganizationMembers);

			base.OnCreate (savedInstanceState);

		}


		protected override void ProcessFile(string data){

			var orgMembersResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgMemberResponse> (data);
			orgMembersResponse.Model.ForEach (c => c.ExistsInMyBusidex = true);

			Cards = new List<UserCard> ();

			foreach (var item in orgMembersResponse.Model) {
				if (item != null) {

					var userCard = new UserCard ();

					userCard.ExistsInMyBusidex = item.ExistsInMyBusidex;
					userCard.Card = item;
					userCard.CardId = item.CardId;

					Cards.Add (userCard);
				}
			}

			var lstCards = FindViewById<ListView> (Resource.Id.lstCards);
			var adapter = new UserCardAdapter (this, Resource.Id.lstCards, Cards);

			adapter.Redirect += ShowCard;
			adapter.SendEmail += SendEmail;
			adapter.OpenBrowser += OpenBrowser;
			adapter.CardAddedToMyBusidex += AddCardToMyBusidex;
			adapter.CardRemovedFromMyBusidex += RemoveCardFromMyBusidex;
			adapter.OpenMap += OpenMap;

			adapter.ShowNotes = true;

			lstCards.Adapter = adapter;
		}
	}
}

