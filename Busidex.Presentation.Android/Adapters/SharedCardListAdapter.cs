﻿using Android.Widget;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using System.IO;
using Android.Net;
using System.Linq;

namespace Busidex.Presentation.Android
{
	public delegate void SharingCardHandler(SharedCard card);

	public class SharedCardListAdapter : ArrayAdapter<SharedCard>
	{
		List<SharedCard> Cards { get; set; }
		List<View> ViewCache{ get; set; }
		readonly Activity context;

		public event SharingCardHandler SharingCard;

		public SharedCardListAdapter (Activity ctx, int id, List<SharedCard> cards) : base(ctx, id, cards)
		{
			Cards = cards;
			context = ctx;
			ViewCache = new List<View>();
		}

		protected void AcceptCard(object sender, System.EventArgs e){

			var position = System.Convert.ToInt32(((Button)sender).Tag);
			var card = Cards [position];

			card.Accepted = true;
			card.Declined = false;

			SaveSharedCard (card);
			UpdateSharingUI (position, true);
		}

		protected void DeclineCard(object sender, System.EventArgs e){

			var position = System.Convert.ToInt32(((Button)sender).Tag);
			var card = Cards [position];

			card.Accepted = false;
			card.Declined = true;

			SaveSharedCard (card);
			UpdateSharingUI (position, false);
		}

		void UpdateSharingUI(int position, bool accepted){

			var view = ViewCache.SingleOrDefault(v=> System.Convert.ToInt32(v.Tag) == position);
			if(view != null){
				var btnAccept = view.FindViewById<TextView> (Resource.Id.btnAccept);
				var btnDecline = view.FindViewById<TextView> (Resource.Id.btnDecline);
				var imgResults = view.FindViewById<ImageView> (Resource.Id.imgResults);

				btnAccept.Visibility = btnDecline.Visibility = ViewStates.Invisible;
				imgResults.SetImageResource (accepted ? Resource.Drawable.checkmark : Resource.Drawable.red_minus);
				imgResults.Visibility = ViewStates.Visible;
			}
		}

		void SaveSharedCard(SharedCard card){
			if(SharingCard != null){
				SharingCard (card);
			}
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = convertView ?? context.LayoutInflater.Inflate (Resource.Layout.SharedCardListItem, null);

			var card = Cards [position];

			var txtSharedCardName = view.FindViewById<TextView> (Resource.Id.txtSharedCardName);
			var txtSharedCardCompanyName = view.FindViewById<TextView> (Resource.Id.txtSharedCardCompanyName);
			var imgSharedCardHorizontal = view.FindViewById<ImageView> (Resource.Id.imgSharedCardHorizontal);
			var imgSharedCardVertical =  view.FindViewById<ImageView> (Resource.Id.imgSharedCardVertical);
			var btnAccept = view.FindViewById<TextView> (Resource.Id.btnAccept);
			var btnDecline = view.FindViewById<TextView> (Resource.Id.btnDecline);
			var imgResults = view.FindViewById<ImageView> (Resource.Id.imgResults);

			imgResults.Visibility = ViewStates.Gone;

			txtSharedCardName.Text = card.Card.Name;
			txtSharedCardCompanyName.Text = card.Card.CompanyName;
			txtSharedCardCompanyName.Visibility = string.IsNullOrEmpty (card.Card.CompanyName) ? ViewStates.Gone : ViewStates.Visible;

			var fileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, card.Card.FrontFileName);
			var uri = Uri.Parse (fileName);

			if (card.Card.FrontOrientation == "H") {
				imgSharedCardHorizontal.SetImageURI (uri);
				imgSharedCardHorizontal.SetScaleType (ImageView.ScaleType.FitXy);
				imgSharedCardHorizontal.Visibility = ViewStates.Visible;
				imgSharedCardVertical.Visibility = ViewStates.Gone;
			}else{
				imgSharedCardVertical.SetImageURI (uri);
				imgSharedCardVertical.SetScaleType (ImageView.ScaleType.FitXy);
				imgSharedCardVertical.Visibility = ViewStates.Visible;
				imgSharedCardHorizontal.Visibility = ViewStates.Gone;
			}

			btnAccept.Click -= AcceptCard;
			btnAccept.Click += AcceptCard;

			btnDecline.Click -= DeclineCard;
			btnDecline.Click += DeclineCard;

			btnAccept.Tag = btnDecline.Tag = view.Tag = position;

			if(ViewCache.SingleOrDefault(v=> System.Convert.ToInt32(v.Tag) == position) == null){
				ViewCache.Add (view);
			}

			return view;
		}
	}
}

