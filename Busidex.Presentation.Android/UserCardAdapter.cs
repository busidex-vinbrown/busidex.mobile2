using System;
using Android.Widget;
using Android.Views;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using Android.App;
using Android.Net;

namespace Busidex.Presentation.Android
{
	public class UserCardAdapter : ArrayAdapter<UserCard>
	{
		List<UserCard> Cards { get; set; }
		readonly Activity context;

		public UserCardAdapter (Activity ctx, int id, List<UserCard> cards) : base(ctx, id, cards)
		{
			Cards = cards;
			context = ctx;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = convertView;
			if(view == null) {
				view = context.LayoutInflater.Inflate (Resource.Layout.UserCardListItem, null);
			}

			var txtName = view.FindViewById<TextView> (Resource.Id.txtName);
			var txtCompanyName = view.FindViewById<TextView> (Resource.Id.txtCompanyName);
			var imgCard = view.FindViewById<ImageButton> (Resource.Id.imgCard);
			 
			var card = Cards [position];
			if(card != null){
				txtName.Text = card.Card.Name;
				txtCompanyName.Text = card.Card.CompanyName;
				var uri = global::Android.Net.Uri.Parse (Busidex.Mobile.Resources.CARD_PATH + card.Card.FrontFileName);
				imgCard.SetImageURI (uri);
			}

			return view;
		}
	}
}

