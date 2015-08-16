
using System;

using Android.App;
using Android.OS;
using Android.Widget;
using Busidex.Mobile.Models;
using System.IO;
using System.Collections.Generic;
using Android.Content;
using Android.Views;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "My Busidex")]			
	public class MyBusidexActivity : BaseCardActivity
	{
		List<UserCard> Cards { get; set; }
		static UserCardAdapter MyBusidexAdapter { get; set; }
		static SearchView txtFilter { get; set; }


		public override bool OnOptionsItemSelected(IMenuItem item) {
			Finish ();
			return base.OnOptionsItemSelected(item);
		}

		void SetActionBarTabs(Context ctx){
			ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
			var searchTab = ActionBar.NewTab ();
			var myBusidexTab = ActionBar.NewTab ();
			var myOrganizationTab = ActionBar.NewTab ();

			var searchView = new ImageView (ctx);
			searchView.SetImageResource (Resource.Drawable.spotlight_icon);
			searchTab.SetCustomView(searchView);

			var myBusidexView = new ImageView (ctx);
			myBusidexView.SetImageResource (Resource.Drawable.icon);
			myBusidexTab.SetCustomView(myBusidexView);

			var myOrganizationView = new ImageView (ctx);
			myOrganizationView.SetImageResource (Resource.Drawable.people);
			myOrganizationTab.SetCustomView(myOrganizationView);
		}
			
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			//TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_LIST, 0);

			SetContentView (Resource.Layout.MyBusidex);

			Cards = new List<UserCard> ();

			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.MY_BUSIDEX_FILE);
			//LoadFromFile (fullFilePath);
		}

		public override void OnBackPressed ()
		{
			//Redirect(new Intent(this, typeof(MainActivity)));
		}

		static void DoFilter(string filter){
			if(string.IsNullOrEmpty(filter)){
				MyBusidexAdapter.Filter.InvokeFilter ("");
			}else{
				MyBusidexAdapter.Filter.InvokeFilter(filter);
			}
		}

		protected override void ProcessFile(string data){

			var myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (data);
			myBusidexResponse.MyBusidex.Busidex.ForEach (c => c.ExistsInMyBusidex = true);

			Cards = myBusidexResponse.MyBusidex.Busidex;

			var lstCards = FindViewById<ListView> (Resource.Id.lstCards);
			txtFilter = FindViewById<SearchView> (Resource.Id.txtFilter);

			MyBusidexAdapter = new UserCardAdapter (this, Resource.Id.lstCards, myBusidexResponse.MyBusidex.Busidex);

			var lblNoCardsMessage = FindViewById<TextView> (Resource.Id.lblNoCardsMessage);
			lblNoCardsMessage.Text = GetString (Resource.String.MyBusidex_NoCards);

			lblNoCardsMessage.Visibility = Cards.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
			txtFilter.Visibility = Cards.Count == 0 ? ViewStates.Gone : ViewStates.Visible;

//			MyBusidexAdapter.Redirect += ShowCard;
//			MyBusidexAdapter.SendEmail += SendEmail;
//			MyBusidexAdapter.OpenBrowser += OpenBrowser;
//			MyBusidexAdapter.CardAddedToMyBusidex += AddCardToMyBusidex;
//			MyBusidexAdapter.CardRemovedFromMyBusidex += RemoveCardFromMyBusidex;
//			MyBusidexAdapter.OpenMap += OpenMap;
//
			MyBusidexAdapter.ShowNotes = true;

			lstCards.Adapter = MyBusidexAdapter;

			txtFilter.QueryTextChange += delegate {
				DoFilter(txtFilter.Query);
			};

			txtFilter.Iconified = false;
			lstCards.RequestFocus (FocusSearchDirection.Down);
			//DismissKeyboard (txtFilter.WindowToken);

			txtFilter.Touch += delegate {
				txtFilter.Focusable = true;
				txtFilter.RequestFocus();
			};
				
		}
	}
}

