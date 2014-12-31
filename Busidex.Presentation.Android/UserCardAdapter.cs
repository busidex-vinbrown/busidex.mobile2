using Android.Widget;
using Android.Views;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using Android.App;
using Android.Net;
using System.IO;
using Android.Content;
using Android.Views.Animations;

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
		readonly Activity context;

		void doRedirect(Intent intent){

			if(Redirect != null){
				Redirect (intent);
			}
		}

		public UserCardAdapter (Activity ctx, int id, List<UserCard> cards) : base(ctx, id, cards)
		{
			Cards = cards;
			context = ctx;
		}

		View SetButtonPanel (ref RelativeLayout layout, View view, ViewGroup parent, int position){

			var panel = layout.FindViewById<View> (Resource.Layout.ButtonPanel);
			if (panel == null) {
				panel = context.LayoutInflater.Inflate (Resource.Layout.ButtonPanel, null);
				layout.AddView (panel);
			}
			panel.Visibility = ViewStates.Visible;

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
			};

			var phoneIntent = new Intent(context, typeof(PhoneActivity));
			var notesIntent = new Intent(context, typeof(NotesActivity));
			var shareCardIntent = new Intent(context, typeof(ShareCardActivity));

			var data = Newtonsoft.Json.JsonConvert.SerializeObject(Cards[position]);
			phoneIntent.PutExtra("Card", data);
			notesIntent.PutExtra("Card", data);
			shareCardIntent.PutExtra("Card", data);

			var btnPhone = panel.FindViewById<ImageButton> (Resource.Id.btnPanelPhone);
			var btnNotes = panel.FindViewById<ImageButton> (Resource.Id.btnPanelNotes);
			var btnShareCard = panel.FindViewById<ImageButton> (Resource.Id.btnPanelShare);

			btnPhone.Click += delegate {
				doRedirect(phoneIntent);
			};
			btnNotes.Click += delegate {
				doRedirect(notesIntent);
			};
			btnShareCard.Click += delegate {
				doRedirect(shareCardIntent);
			};

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

			btnInfo.Click += (object sender, System.EventArgs e) => {

				var layout = view.FindViewById<RelativeLayout>(Resource.Id.listItemLayout);
				var panel = SetButtonPanel(ref layout, view, parent, position);

				var leftAndIn = AnimationUtils.LoadAnimation(context, Resource.Animation.SlideAnimation);
				btnInfo.Visibility = ViewStates.Invisible;
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

