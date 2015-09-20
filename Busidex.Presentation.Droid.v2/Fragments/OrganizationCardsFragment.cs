
using System.Collections.Generic;

using Android.Support.V4.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.Droid.v2
{
	public class OrganizationCardsFragment : Fragment
	{
		readonly List<UserCard> OrganizationCards;
		readonly string logoPath;

		public OrganizationCardsFragment(){
			
		}

		public OrganizationCardsFragment(List<UserCard> cards, string logo){
			OrganizationCards = cards;
			logoPath = logo;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.OrganizationCards, container, false);

			var adapter = new UserCardAdapter (Activity, Resource.Id.lstCards, OrganizationCards);

			adapter.Redirect += ((MainActivity)Activity).ShowCard;
			adapter.ShowButtonPanel += ((MainActivity)Activity).ShowButtonPanel;

			var lstOrganizationCards = view.FindViewById<ListView> (Resource.Id.lstOrganizationCards);
			lstOrganizationCards.Adapter = adapter;

			var state = lstOrganizationCards.OnSaveInstanceState ();


			if(state != null){
				lstOrganizationCards.OnRestoreInstanceState (state);
			}

			const int IMAGE_HEIGHT = 82;

			var img = view.FindViewById<ImageView> (Resource.Id.imgOrganizationHeaderImage);
			var bm = AndroidUtils.DecodeSampledBitmapFromFile (logoPath, Resources.DisplayMetrics.WidthPixels, IMAGE_HEIGHT);
			img.SetImageBitmap (bm);

			return view;
		}
	}
}

