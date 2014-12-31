using Android.Widget;
using Android.Views;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using Android.App;
using Android.Net;
using System.IO;
using Android.Content;
using Android.Views.Animations;
using System.Linq;

namespace Busidex.Presentation.Android
{
	public delegate void RedirectToCardHandler(Intent intent);

	public class UserCardAdapter : ArrayAdapter<UserCard>
	{
		public event RedirectToCardHandler Redirect;

		protected const int CARD_HEIGHT_VERTICAL = 170;
		protected const int CARD_HEIGHT_HORIZONTAL = 120;
		protected const int CARD_WIDTH_VERTICAL = 110;
		protected const int CARD_WIDTH_HORIZONTAL = 180;

		List<UserCard> Cards { get; set; }
		List<int> PanelReferences {get;set;}
		readonly Activity context;
		Intent PhoneIntent {get; set;}
		Intent NotesIntent {get; set;}
		Intent ShareCardIntent {get; set;}

		void doRedirect(Intent intent){

			if(Redirect != null){
				Redirect (intent);
			}
		}

		public UserCardAdapter (Activity ctx, int id, List<UserCard> cards) : base(ctx, id, cards)
		{
			Cards = cards;
			context = ctx;
			PanelReferences = new List<int> ();
		}

		void OnPhoneButtonClicked(object sender, System.EventArgs e){
			doRedirect(PhoneIntent);
		}

		void OnNotesButtonClicked(object sender, System.EventArgs e){
			doRedirect(NotesIntent);
		}

		void OnShareCardButtonClicked(object sender, System.EventArgs e){
			doRedirect(ShareCardIntent);
		}

		View SetButtonPanel (ref RelativeLayout layout, View view, ViewGroup parent, int position){
		
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

					btnHideInfo.Click += (object sender, System.EventArgs e) => {
						var leftAndOut = AnimationUtils.LoadAnimation(context, Resource.Animation.SlideOutAnimation);
						panel.StartAnimation(leftAndOut);
						panel.Visibility = ViewStates.Gone;
						btnInfo.Visibility = ViewStates.Visible;
					};

					layout.AddView (panel);
				}
			}
			panel.Visibility = ViewStates.Visible;

			PhoneIntent = new Intent(context, typeof(PhoneActivity));
			NotesIntent = new Intent(context, typeof(NotesActivity));
			ShareCardIntent = new Intent(context, typeof(ShareCardActivity));

			var data = Newtonsoft.Json.JsonConvert.SerializeObject(Cards[position]);
			PhoneIntent.PutExtra("Card", data);
			NotesIntent.PutExtra("Card", data);
			ShareCardIntent.PutExtra("Card", data);

			var btnPhone = panel.FindViewById<ImageButton> (Resource.Id.btnPanelPhone);
			var btnNotes = panel.FindViewById<ImageButton> (Resource.Id.btnPanelNotes);
			var btnShareCard = panel.FindViewById<ImageButton> (Resource.Id.btnPanelShare);

			btnPhone.Click -= OnPhoneButtonClicked;
			btnPhone.Click += OnPhoneButtonClicked;

			btnNotes.Click -= OnNotesButtonClicked;
			btnNotes.Click += OnNotesButtonClicked;

			btnShareCard.Click -= OnShareCardButtonClicked;
			btnShareCard.Click += OnShareCardButtonClicked;

			return panel;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = convertView ?? context.LayoutInflater.Inflate (Resource.Layout.UserCardListItem, null);

			var txtName = view.FindViewById<TextView> (Resource.Id.txtName);
			var txtCompanyName = view.FindViewById<TextView> (Resource.Id.txtCompanyName);
			var imgCardH = view.FindViewById<ImageButton> (Resource.Id.imgCardHorizontal);
			var imgCardV =  view.FindViewById<ImageButton> (Resource.Id.imgCardVertical);
			var btnInfo = view.FindViewById<ImageButton> (Resource.Id.btnInfo);
			bool alreadyAdded = convertView != null;

			btnInfo.Click += (object sender, System.EventArgs e) => {

				if(convertView != null){
					alreadyAdded = true;
				}
				var layout = view.FindViewById<RelativeLayout>(Resource.Id.listItemLayout);
				var panel = SetButtonPanel(ref layout, view, parent, position);

				if (!alreadyAdded) {
					//layout.ch
					//layout.AddView (panel);
				}

				var leftAndIn = AnimationUtils.LoadAnimation(context, Resource.Animation.SlideAnimation);
				btnInfo.Visibility = ViewStates.Invisible;
				panel.Visibility = ViewStates.Visible;
				panel.StartAnimation(leftAndIn);
			};

			var intent = new Intent(context, typeof(CardDetailActivity));
			var data = Newtonsoft.Json.JsonConvert.SerializeObject(Cards[position]);
			intent.PutExtra("Card", data);

			imgCardH.Click += delegate {
				doRedirect(intent);
			};

			imgCardV.Click += delegate {
				doRedirect(intent);
			};

			var card = Cards [position];
			if(card != null){
				txtName.Text = card.Card.Name;
				txtCompanyName.Text = card.Card.CompanyName;
				txtCompanyName.Visibility = string.IsNullOrEmpty (card.Card.CompanyName) ? ViewStates.Gone : ViewStates.Visible;

				var fileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + card.Card.FrontFileName);
				var uri = Uri.Parse (fileName);

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
			}

			return view;
		}
	}
}

