using Android.Widget;
using Android.Graphics.Drawables;
using Android.Net;
using System.IO;
using Busidex.Mobile.Models;
using Android.Views;
using Android.OS;
using Busidex.Mobile;
using System.Threading.Tasks;

namespace Busidex.Presentation.Droid.v2
{
	public class CardDetailFragment : GenericViewPagerFragment
	{

		string FrontFileName{ get; set; }

		string BackFileName{ get; set; }

		public UserCard SelectedCard;

		ImageButton btnCard { get; set; }

		CardViewState ViewState { get; set; }

		enum CardViewState
		{
			Loading = 1,
			Front = 2,
			Back = 3
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.Card, container, false);

			btnCard = view.FindViewById<ImageButton> (Resource.Id.imgCardDetail);

			var frontFileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, SelectedCard.Card.FrontFileName);
			var frontUri = Uri.Parse (frontFileName);

			if (File.Exists (frontFileName)) {
				OnImageDownloadCompleted (frontUri);
			} else {
				Utils.DownloadImage (Busidex.Mobile.Resources.CARD_PATH + SelectedCard.Card.FrontFileName, Busidex.Mobile.Resources.DocumentsPath, SelectedCard.Card.FrontFileName).ContinueWith (t => {
					Activity.RunOnUiThread (() => OnImageDownloadCompleted (frontUri));
				});
			}

			btnCard.SetScaleType (ImageView.ScaleType.FitXy);

			btnCard.Click += async delegate {
				await ToggleImage ();
			};

			ViewState = CardViewState.Loading;

			return view;
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);
			ToggleImage ().ContinueWith (r => {
				
			});
		}

		public override void OnDestroy ()
		{
			if (btnCard != null) {
				var bd = (BitmapDrawable)btnCard.Drawable;
				if (bd != null) {
					bd.Bitmap.Recycle ();
				}
				btnCard.SetImageURI (null);
			}
			base.OnDestroy ();
		}

		void unload ()
		{
			var panel = new ButtonPanelFragment ();
			panel.SelectedCard = SelectedCard;
			((MainActivity)Activity).UnloadFragment (panel);
			Activity.RequestedOrientation = global::Android.Content.PM.ScreenOrientation.Portrait;	
		}

		async Task<bool> ToggleImage ()
		{

			var frontFileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, SelectedCard.Card.FrontFileName);
			var backFileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, SelectedCard.Card.BackFileName);
			var frontUri = Uri.Parse (frontFileName);
			var backUri = Uri.Parse (backFileName);

			switch (ViewState) {
			case CardViewState.Loading:
				{
					if (File.Exists (frontFileName)) {
						OnImageDownloadCompleted (frontUri);
					} else {

						var imagePath = Busidex.Mobile.Resources.CARD_PATH + SelectedCard.Card.FrontFileName;

						await Utils.DownloadImage (imagePath, Busidex.Mobile.Resources.DocumentsPath, SelectedCard.Card.FrontFileName).ContinueWith (t => {
							Activity.RunOnUiThread (() => OnImageDownloadCompleted (frontUri));
						});
					}
					ViewState = CardViewState.Front;
					break;
				}
			case CardViewState.Front:
				{

					if (SelectedCard.Card.BackFileId == System.Guid.Empty ||
					    SelectedCard.Card.BackFileId.ToString ().Equals (Busidex.Mobile.Resources.EMPTY_CARD_ID) ||
					    SelectedCard.Card.BackFileId.ToString ().Equals (Busidex.Mobile.Resources.NULL_CARD_ID)) {
						unload ();
					} else {

						if (File.Exists (backFileName)) {
							OnImageDownloadCompleted (backUri);
						} else {

							var imagePath = Busidex.Mobile.Resources.CARD_PATH + SelectedCard.Card.BackFileName;

							await Utils.DownloadImage (imagePath, Busidex.Mobile.Resources.DocumentsPath, SelectedCard.Card.BackFileName).ContinueWith (t => {
								Activity.RunOnUiThread (() => OnImageDownloadCompleted (backUri));
							});
						}
					}
					ViewState = CardViewState.Back;
					break;
				}
			case CardViewState.Back:
				{
					unload ();
					break;
				}
			}

			return true;
		}

		protected void OnImageDownloadCompleted (Uri uri)
		{
			
			btnCard.SetImageURI (uri);

			Activity.RequestedOrientation = SelectedCard.Card.FrontOrientation == "H" 
				? global::Android.Content.PM.ScreenOrientation.Landscape
				: global::Android.Content.PM.ScreenOrientation.Portrait;
		}
	}
}

