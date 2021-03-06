﻿using Android.Widget;
using Android.Views;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using Android.App;
using Android.Net;
using System.IO;
using Android.Content;
using Android.Views.Animations;
using Java.Lang;
using System.Linq;
using System.Threading.Tasks;

namespace Busidex.Presentation.Android
{
	public delegate void RedirectToCardHandler(CardDetailFragment fragment);
	public delegate void SendEmailHandler(Intent intent);
	public delegate void OpenMapHandler(Intent intent);
	public delegate void OpenBrowserHandler(Intent intent);
	public delegate void CardAddedToMyBusidexHandler(Intent intent);
	public delegate void CardRemovedFromMyBusidexHandler(Intent intent);

	public class UserCardAdapter : ArrayAdapter<UserCard>, IFilterable
	{
		public event RedirectToCardHandler Redirect;
		public event SendEmailHandler SendEmail;
		public event OpenMapHandler OpenMap;
		public event OpenBrowserHandler OpenBrowser;
		public event CardAddedToMyBusidexHandler CardAddedToMyBusidex;
		public event CardRemovedFromMyBusidexHandler CardRemovedFromMyBusidex;

		public bool ShowNotes{ get; set; }

		protected const int CARD_HEIGHT_VERTICAL = 170;
		protected const int CARD_HEIGHT_HORIZONTAL = 120;
		protected const int CARD_WIDTH_VERTICAL = 110;
		protected const int CARD_WIDTH_HORIZONTAL = 180;

		List<UserCard> Cards { get; set; }
		List<UserCard> _originalItems { get; set; }
		List<int> PanelReferences {get;set;}

		readonly Activity context;

		Intent PhoneIntent { get; set; }
		Intent NotesIntent { get; set; }
		Intent ShareCardIntent { get; set; }
		Intent CardDetailIntent{ get; set; }
		Intent OpenMapIntent{ get; set; }
		Intent SendEmailIntent{ get; set; }
		Intent OpenBrowserIntent{ get; set; }
		Intent AddToMyBusidexIntent{ get; set; }
		Intent RemoveFromMyBusidexIntent{ get; set; }

		async Task<bool> doRedirect(CardDetailFragment fragment){
			if(Redirect != null){
				Redirect (fragment);
			}
			return true;
		}

		public override int Count
		{
			get { return Cards.Count; }
		}

		public Filter Filter { get; private set; }

		public UserCardAdapter (Activity ctx, int id, List<UserCard> cards) : base(ctx, id, cards)
		{
			Cards = _originalItems = cards;
			context = ctx;
			PanelReferences = new List<int> ();
			Filter = new UserCardFilter (this);
		}

		public UserCard this[int position]{ 
			get{ 
				if(Count == 0){
					return null;
				}
				if(position > Count){
					return Cards [Count - 1];
				}
				return Cards [position]; 
			}
		}


		void OnPhoneButtonClicked(object sender, System.EventArgs e){
			//doRedirect(PhoneIntent);
		}

		void OnNotesButtonClicked(object sender, System.EventArgs e){
			//doRedirect(NotesIntent);
		}

		void OnShareCardButtonClicked(object sender, System.EventArgs e){
			//doRedirect(ShareCardIntent);
		}

		void OnMapButtonClicked(object sender, System.EventArgs e){
			if(OpenMap != null){
				OpenMap (OpenMapIntent);
			}
		}

		void OnEmailButtonClicked(object sender, System.EventArgs e){
			if(SendEmail != null){
				SendEmail (SendEmailIntent);
			}
		}

		void OnBrowserButtonClicked(object sender, System.EventArgs e){
			if(OpenBrowser != null){
				OpenBrowser (OpenBrowserIntent);
			}
		}

		void OnAddToMyBusidexClicked(object sender, System.EventArgs e){
			if (CardAddedToMyBusidex != null){
				CardAddedToMyBusidex (AddToMyBusidexIntent);
			}
			var btnAddToMyBusidex = (ImageButton)sender;
			btnAddToMyBusidex.Visibility = ViewStates.Gone;
			var panel = btnAddToMyBusidex.Parent as View;
			if (panel != null) {
				var btnRemoveFromMyBusidex = panel.FindViewById<ImageButton> (Resource.Id.btnPanelRemove);
				btnRemoveFromMyBusidex.Visibility = ViewStates.Visible;
			}
		}

		void OnRemoveFromMyBusidexClicked(object sender, System.EventArgs e){
			if(CardRemovedFromMyBusidex != null){
				CardRemovedFromMyBusidex (RemoveFromMyBusidexIntent);
			}
			var btnRemoveFromMyBusidex = (ImageButton)sender;
			btnRemoveFromMyBusidex.Visibility = ViewStates.Gone;
			var panel = btnRemoveFromMyBusidex.Parent as View;
			if (panel != null) {
				var btnAddToMyBusidex = panel.FindViewById<ImageButton> (Resource.Id.btnPanelAdd);
				btnAddToMyBusidex.Visibility = ViewStates.Visible;
			}
		}

		void OnCardDetailButtonClicked(object sender, System.EventArgs e){

			var position = System.Convert.ToInt32(((ImageButton)sender).Tag);
			var fragment = ((SplashActivity)Context).fragments ["CardDetailFragment"] as CardDetailFragment;
			fragment.UserCard = Cards [position];
			doRedirect (fragment).ContinueWith (delegate (Task<bool> result) {
			});
		}

		View SetButtonPanel (ref RelativeLayout layout, View view, View parent, int position){
		
			var panel = view.FindViewById<View> (Resource.Layout.ButtonPanel);
			if (panel == null) {

				panel = layout.FindViewById<View> (Resource.Layout.ButtonPanel);
				if (panel == null) {
					panel = context.LayoutInflater.Inflate (Resource.Layout.ButtonPanel, null);
					panel.Id = Resource.Layout.ButtonPanel;

					var layoutParams = panel.LayoutParameters;
					if(layoutParams == null){
						layoutParams = new ViewGroup.LayoutParams (parent.Width, view.Height);
					}else{
						layoutParams.Width = parent.Width;
						layoutParams.Height = view.Height;
					}
					panel.LayoutParameters = layoutParams;

					var btnInfo = view.FindViewById<ImageButton> (Resource.Id.btnInfo);
					var btnHideInfo = panel.FindViewById<ImageButton> (Resource.Id.btnHideInfo);
					btnInfo.Visibility = ViewStates.Visible;

					btnHideInfo.Click += (sender, e) => {
						var leftAndOut = AnimationUtils.LoadAnimation (context, Resource.Animation.SlideOutAnimation);
						panel.StartAnimation (leftAndOut);
						panel.Visibility = ViewStates.Gone;
						btnInfo.Visibility = ViewStates.Visible;
					};

					layout.AddView (panel);
				}
			}
			panel.Visibility = ViewStates.Visible;

			var userCard = this [position];// Cards [position];
			PhoneIntent = new Intent(context, typeof(PhoneActivity));
			NotesIntent = new Intent(context, typeof(NotesActivity));
			ShareCardIntent = new Intent(context, typeof(ShareCardActivity));
			SendEmailIntent = new Intent(Intent.ActionSend);
			AddToMyBusidexIntent = new Intent (context, context.GetType ());
			RemoveFromMyBusidexIntent = new Intent (context, context.GetType ());

			OpenBrowserIntent = new Intent (Intent.ActionView);

			var data = Newtonsoft.Json.JsonConvert.SerializeObject(userCard);
			PhoneIntent.PutExtra("Card", data);
			NotesIntent.PutExtra("Card", data);
			ShareCardIntent.PutExtra("Card", data);
			AddToMyBusidexIntent.PutExtra ("Card", data);
			RemoveFromMyBusidexIntent.PutExtra ("Card", data);
			SendEmailIntent.PutExtra ("Card", data);

			SendEmailIntent.PutExtra (Intent.ExtraEmail, new []{userCard.Card.Email} );
			SendEmailIntent.SetType ("message/rfc822");

			var geoString = "geo:0,0";
			if(userCard.Card.Addresses != null && userCard.Card.Addresses.Count > 0){
				var address = userCard.Card.Addresses [0];
				geoString = string.Format ("geo:0,0?q={0}", address);
			}

			var geoUri = Uri.Parse (geoString);
			OpenMapIntent = new Intent (Intent.ActionView, geoUri);
			OpenMapIntent.PutExtra ("Card", data);

			var url = string.Empty;
			if(!string.IsNullOrEmpty(userCard.Card.Url)){
				url = !userCard.Card.Url.StartsWith ("http", System.StringComparison.Ordinal) ? "http://" + userCard.Card.Url : userCard.Card.Url;
			}
			var uri = Uri.Parse (url);
			OpenBrowserIntent.SetData (uri);
			OpenBrowserIntent.PutExtra ("Card", data);

			var btnPhone = panel.FindViewById<ImageButton> (Resource.Id.btnPanelPhone);
			var btnNotes = panel.FindViewById<ImageButton> (Resource.Id.btnPanelNotes);
			var btnShareCard = panel.FindViewById<ImageButton> (Resource.Id.btnPanelShare);
			var btnEmail = panel.FindViewById<ImageButton> (Resource.Id.btnPanelEmail);
			var btnMap = panel.FindViewById<ImageButton> (Resource.Id.btnPanelMap);
			var btnBrowser = panel.FindViewById<ImageButton> (Resource.Id.btnPanelBrowser);
			var btnAddToMyBusidex = panel.FindViewById<ImageButton> (Resource.Id.btnPanelAdd);
			var btnRemoveFromMyBusidex = panel.FindViewById<ImageButton> (Resource.Id.btnPanelRemove);

			const float ENABLED = 1f;
			const float DISABLED = .2f;

			btnPhone.Enabled = (userCard.Card.PhoneNumbers != null && userCard.Card.PhoneNumbers.Count > 0);
			btnPhone.Alpha = btnPhone.Enabled ? ENABLED : DISABLED;

			btnEmail.Enabled = !string.IsNullOrEmpty (userCard.Card.Email);
			btnEmail.Alpha = btnEmail.Enabled ? ENABLED : DISABLED;

			btnNotes.Enabled = ShowNotes;
			btnNotes.Alpha = btnNotes.Enabled ? ENABLED : DISABLED;

			btnBrowser.Enabled = !string.IsNullOrEmpty (userCard.Card.Url);
			btnBrowser.Alpha = btnBrowser.Enabled ? ENABLED : DISABLED;

			btnMap.Enabled = (userCard.Card.Addresses != null && userCard.Card.Addresses.Count > 0 && userCard.Card.Addresses [0].HasAddress);
			btnMap.Alpha = btnMap.Enabled ? ENABLED : DISABLED;

			btnAddToMyBusidex.Visibility = userCard.Card.ExistsInMyBusidex ? ViewStates.Gone : ViewStates.Visible;
			btnRemoveFromMyBusidex.Visibility = userCard.Card.ExistsInMyBusidex ? ViewStates.Visible : ViewStates.Gone;

			btnPhone.Click -= OnPhoneButtonClicked;
			btnPhone.Click += OnPhoneButtonClicked;

			btnNotes.Click -= OnNotesButtonClicked;
			btnNotes.Click += OnNotesButtonClicked;

			btnShareCard.Click -= OnShareCardButtonClicked;
			btnShareCard.Click += OnShareCardButtonClicked;

			btnEmail.Click -= OnEmailButtonClicked;
			btnEmail.Click += OnEmailButtonClicked;

			btnBrowser.Click -= OnBrowserButtonClicked;
			btnBrowser.Click += OnBrowserButtonClicked;

			btnAddToMyBusidex.Click -= OnAddToMyBusidexClicked;
			btnAddToMyBusidex.Click += OnAddToMyBusidexClicked;

			btnRemoveFromMyBusidex.Click -= OnRemoveFromMyBusidexClicked;
			btnRemoveFromMyBusidex.Click += OnRemoveFromMyBusidexClicked;

			btnMap.Click -= OnMapButtonClicked;
			btnMap.Click += OnMapButtonClicked;


			return panel;
		}


		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = convertView ?? context.LayoutInflater.Inflate (Resource.Layout.UserCardListItem, null);

			var txtName = view.FindViewById<TextView> (Resource.Id.txtName);
			var txtCompanyName = view.FindViewById<TextView> (Resource.Id.txtCompanyName);
			var btnCardH = view.FindViewById<ImageButton> (Resource.Id.imgCardHorizontal);
			var btnCardV =  view.FindViewById<ImageButton> (Resource.Id.imgCardVertical);
			var btnInfo = view.FindViewById<ImageButton> (Resource.Id.btnInfo);

			btnInfo.Click += (sender, e) => {
			
				var layout = view.FindViewById<RelativeLayout> (Resource.Id.listItemLayout);
				var panel = SetButtonPanel (ref layout, view, parent, position);

				var leftAndIn = AnimationUtils.LoadAnimation (context, Resource.Animation.SlideAnimation);
				btnInfo.Visibility = ViewStates.Invisible;
				panel.Visibility = ViewStates.Visible;
				panel.StartAnimation (leftAndIn);
			};

			var card = Cards [position];

			// for these buttons, need to set the tag property to the cardID because the
			// buttons are reused

//			btnCardH.Touch += async delegate(object sender, View.TouchEventArgs e) {
//				if(e.Event.Action == MotionEventActions.Up){
//					await OnCardDetailButtonClicked(sender, e);
//				}
//			};
//
//			btnCardV.Touch += async delegate(object sender, View.TouchEventArgs e) {
//				if(e.Event.Action == MotionEventActions.Up){
//					await OnCardDetailButtonClicked(sender, e);
//				}
//			};
			btnCardH.Click -= OnCardDetailButtonClicked;
			btnCardH.Click += OnCardDetailButtonClicked;
			btnCardH.Tag = position;

			btnCardV.Click -= OnCardDetailButtonClicked;
			btnCardV.Click += OnCardDetailButtonClicked;
			btnCardV.Tag = position;

			if(card != null){

				txtName.Text = card.Card.Name;
				txtCompanyName.Text = card.Card.CompanyName;
				txtCompanyName.Visibility = string.IsNullOrEmpty (card.Card.CompanyName) ? ViewStates.Gone : ViewStates.Visible;

				var fileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + card.Card.FrontFileName);
				var uri = Uri.Parse (fileName);

				if (card.Card.FrontOrientation == "H") {
					btnCardH.SetImageURI (uri);
					btnCardH.SetScaleType (ImageView.ScaleType.FitXy);
					btnCardH.Visibility = ViewStates.Visible;
					btnCardV.Visibility = ViewStates.Gone;
				}else{
					btnCardV.SetImageURI (uri);
					btnCardV.SetScaleType (ImageView.ScaleType.FitXy);
					btnCardV.Visibility = ViewStates.Visible;
					btnCardH.Visibility = ViewStates.Gone;
				}
			}

			return view;
		}

		public class UserCardFilter : Filter
		{
			readonly UserCardAdapter _adapter;

			public UserCardFilter (UserCardAdapter adapter)
			{
				_adapter = adapter;
			}

			protected override FilterResults PerformFiltering(ICharSequence constraint)
			{
				var returnObj = new FilterResults();
				var results = new List<UserCard>();
				if (_adapter.Cards == null || constraint == null || string.IsNullOrWhiteSpace (constraint.ToString ())) {
					_adapter.Cards = _adapter._originalItems;
					results.AddRange (_adapter.Cards);
				} else {

					//if (constraint == null) return returnObj;

					if (_adapter.Cards != null && _adapter.Cards.Any ()) {
						// Compare constraint to all names lowercased. 
						// It they are contained they are added to results.
						results.AddRange (
							_adapter.Cards.Where (
								card => 
							(!string.IsNullOrEmpty (card.Card.Name) && card.Card.Name.ToLower ().Contains (constraint.ToString ())) ||
								(!string.IsNullOrEmpty (card.Card.CompanyName) && card.Card.CompanyName.ToLower ().Contains (constraint.ToString ())) ||
								(!string.IsNullOrEmpty (card.Card.Email) && card.Card.Email.ToLower ().Contains (constraint.ToString ())) ||
								(!string.IsNullOrEmpty (card.Card.Url) && card.Card.Url.ToLower ().Contains (constraint.ToString ()))
							) 
						);
					}
				}

				// Nasty piece of .NET to Java wrapping, be careful with this!
				returnObj.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
				returnObj.Count = results.Count;

				constraint.Dispose();

				return returnObj;
			}

			protected override void PublishResults(ICharSequence constraint, FilterResults results)
			{
				using (var values = results.Values)
					_adapter.Cards = values.ToArray<Object>()
						.Select(r => r.ToNetObject<UserCard>()).ToList();

				_adapter.NotifyDataSetChanged();

				// Don't do this and see GREF counts rising
				constraint.Dispose();
				results.Dispose();
			}
		}
	}
}

