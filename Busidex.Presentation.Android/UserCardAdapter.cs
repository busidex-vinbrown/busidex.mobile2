using Android.Widget;
using Android.Views;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using Android.App;
using Android.Net;
using System.IO;

namespace Busidex.Presentation.Android
{
	public class UserCardAdapter : ArrayAdapter<UserCard>
	{
		protected const int CARD_HEIGHT_VERTICAL = 170;
		protected const int CARD_HEIGHT_HORIZONTAL = 120;
		protected const int CARD_WIDTH_VERTICAL = 110;
		protected const int CARD_WIDTH_HORIZONTAL = 180;

		List<UserCard> Cards { get; set; }
		readonly Activity context;

		public UserCardAdapter (Activity ctx, int id, List<UserCard> cards) : base(ctx, id, cards)
		{
			Cards = cards;
			context = ctx;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = convertView ?? context.LayoutInflater.Inflate (Resource.Layout.UserCardListItem, null);

			var txtName = view.FindViewById<TextView> (Resource.Id.txtName);
			var txtCompanyName = view.FindViewById<TextView> (Resource.Id.txtCompanyName);
			var imgCardH = view.FindViewById<ImageButton> (Resource.Id.imgCardHorizontal);
			var imgCardV =  view.FindViewById<ImageButton> (Resource.Id.imgCardVertical);

			var card = Cards [position];
			if(card != null){
				txtName.Text = card.Card.Name;
				txtCompanyName.Text = card.Card.CompanyName;
				txtCompanyName.Visibility = string.IsNullOrEmpty (card.Card.CompanyName) ? ViewStates.Gone : ViewStates.Visible;

				var fileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + card.Card.FrontFileName);
				var uri = Uri.Parse (fileName);

				//imgCard.SetAdjustViewBounds (true);
				if (card.Card.FrontOrientation == "H") {
					imgCardH.SetImageURI (uri);
					imgCardH.SetScaleType (ImageView.ScaleType.FitXy);
					imgCardH.Visibility = ViewStates.Visible;
					imgCardV.Visibility = ViewStates.Gone;
				}else{
					imgCardV.SetImageURI (uri);
					imgCardV.SetScaleType (ImageView.ScaleType.FitXy);
					imgCardV.Visibility = ViewStates.Visible;
					imgCardH.Visibility = ViewStates.Gone;
				}
				//imgCard.SetMinimumWidth(card.Card.FrontOrientation == "H" ? CARD_WIDTH_HORIZONTAL : CARD_WIDTH_VERTICAL);

			}

			return view;
		}
	}
}

