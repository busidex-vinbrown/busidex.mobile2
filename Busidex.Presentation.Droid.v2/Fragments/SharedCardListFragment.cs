
using System.Collections.Generic;

using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.Droid.v2
{

//	public class SharedCardListFragment : GenericViewPagerFragment
//	{
//		List<SharedCard> sharedCards;
//		SharedCardListAdapter adapter;
//
//
//		public SharedCardListFragment() : base(){
//
//		}
//
//		public SharedCardListFragment(List<SharedCard> cards){
//			sharedCards = cards;
//		}
//
//		public void UpdateData(List<SharedCard> cards){
//			sharedCards = cards;
//			adapter.UpdateData (sharedCards);
//		}
//
//		void SaveSharedCard(SharedCard card){
//
//			((MainActivity)Activity).SaveSharedCard (card);
//		}
//
//		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
//		{
//			var view = inflater.Inflate(Resource.Layout.SharedCardList, container, false);
//
//			var lstSharedCards = view.FindViewById<ListView>(Resource.Id.lstSharedCards);
//
//			adapter = new SharedCardListAdapter(Activity, Resource.Id.lstSharedCards, sharedCards);
//			lstSharedCards.Adapter = adapter;
//
//			adapter.SharingCard -= SaveSharedCard;
//			adapter.SharingCard += SaveSharedCard;
//
//			return view;
//		}
//	}
}

