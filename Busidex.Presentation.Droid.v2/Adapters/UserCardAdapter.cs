using Android.Widget;
using Android.Views;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using Android.App;
using Android.Net;
using System.IO;
using Android.Content;
using System.Threading.Tasks;
using System.Threading;

namespace Busidex.Presentation.Droid.v2
{
	public delegate void RedirectToCardHandler(CardDetailFragment fragment);
	public delegate void ShowButtonPanelHandler(ButtonPanelFragment fragment, Uri uri, string orientation);
	public delegate void SendEmailHandler(Intent intent);
	public delegate void OpenMapHandler(Intent intent);
	public delegate void OpenBrowserHandler(Intent intent);
	public delegate void CardAddedToMyBusidexHandler(Intent intent);
	public delegate void CardRemovedFromMyBusidexHandler(Intent intent);

	public class UserCardAdapter : ArrayAdapter<UserCard>//, IFilterable
	{
		public event RedirectToCardHandler Redirect;
		public event ShowButtonPanelHandler ShowButtonPanel;
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

		public List<UserCard> Cards { get; set; }
		public List<UserCard> _originalItems { get; set; }
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

		public Filter CardFilter { get; private set; }

		public void UpdateData(List<UserCard> cards){
			Cards = cards;
			NotifyDataSetChanged ();
		}

		public UserCardAdapter (Activity ctx, int id, List<UserCard> cards) : base(ctx, id, cards)
		{
			Cards = _originalItems = cards;
			context = ctx;
			PanelReferences = new List<int> ();
			CardFilter = new UserCardFilter (this);
		}

		public UserCard this[int position]{ 
			get{ 
				if(Count == 0){
					return null;
				}
				return position > Count ? Cards [Count - 1] : Cards [position];
			}
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

//			var position = System.Convert.ToInt32(((ImageButton)sender).Tag);
//			var fragment = new CardDetailFragment();
//			fragment.UserCard = Cards [position];
//			Redirect (fragment);

			var position = System.Convert.ToInt32(((ImageButton)sender).Tag);
			var fragment = new ButtonPanelFragment();
			fragment.SelectedCard = Cards [position];

			var fileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + Cards [position].Card.FrontFileName);
			var uri = Uri.Parse (fileName);

			ShowButtonPanel (fragment, uri, Cards [position].Card.FrontOrientation);
		}

//		void OnButtonPanelButtonClicked(object sender, System.EventArgs e){
//
//			var position = System.Convert.ToInt32(((ImageButton)sender).Tag);
//			var fragment = new ButtonPanelFragment();
//			fragment.SelectedCard = Cards [position];
//
//			var fileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + Cards [position].Card.FrontFileName);
//			var uri = Uri.Parse (fileName);
//
//			ShowButtonPanel (fragment, uri, Cards [position].Card.FrontOrientation);
//		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = convertView ?? context.LayoutInflater.Inflate (Resource.Layout.UserCardListItem, null);

			var txtName = view.FindViewById<TextView> (Resource.Id.txtName);
			var txtCompanyName = view.FindViewById<TextView> (Resource.Id.txtCompanyName);
			var btnCardH = view.FindViewById<ImageButton> (Resource.Id.imgCardHorizontal);
			var btnCardV =  view.FindViewById<ImageButton> (Resource.Id.imgCardVertical);

			var btnInfo = view.FindViewById<ImageButton> (Resource.Id.btnInfo);
			btnInfo.Visibility = ViewStates.Gone;

			//btnInfo.Click -= OnButtonPanelButtonClicked;
			//btnInfo.Click += OnButtonPanelButtonClicked;
			//btnInfo.Tag = position;

			var card = Cards [position];

			// for these buttons, need to set the tag property to the cardID because the buttons are reused
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

				ProgressBar progress1 = view.FindViewById<ProgressBar> (Resource.Id.progressBar1);
				progress1.Visibility = !File.Exists (fileName) ? ViewStates.Visible : ViewStates.Gone;

				if (card.Card.FrontOrientation == "H") {
					btnCardH.SetImageURI (uri);
					btnCardH.SetScaleType (ImageView.ScaleType.FitXy);
					btnCardH.Visibility = ViewStates.Visible;
					btnCardV.Visibility = ViewStates.Gone;
				} else {
					btnCardV.SetImageURI (uri);
					btnCardV.SetScaleType (ImageView.ScaleType.FitXy);
					btnCardV.Visibility = ViewStates.Visible;
					btnCardH.Visibility = ViewStates.Gone;
				}

				// If the image file doesn't exist yet, queue up a thread to wait for it
				if (!File.Exists (fileName)) {
					ThreadPool.QueueUserWorkItem (
						token => {
							while (!File.Exists (fileName)) {
								Thread.Sleep (3000);
							}

							context.RunOnUiThread (() => {
								progress1.Visibility = File.Exists (fileName) ? ViewStates.Gone : ViewStates.Visible;
								if (card.Card.FrontOrientation == "H") {
									btnCardH.SetImageURI (uri);
									btnCardH.SetScaleType (ImageView.ScaleType.FitXy);
									btnCardH.Visibility = ViewStates.Visible;
									btnCardV.Visibility = ViewStates.Gone;
								} else {
									btnCardV.SetImageURI (uri);
									btnCardV.SetScaleType (ImageView.ScaleType.FitXy);
									btnCardV.Visibility = ViewStates.Visible;
									btnCardH.Visibility = ViewStates.Gone;
								}
							});
						}
					);
				}
			}

			return view;
		}
	}
}

