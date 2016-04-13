using Busidex.Mobile.Models;
using System.IO;
using Android.Widget;
using Android.Views;
using Android.OS;
using Android.Net;
using Android.Text.Method;

namespace Busidex.Presentation.Droid.v2
{
	public class NotesFragment : GenericViewPagerFragment
	{
		readonly UserCard SelectedCard;
		EditText txtNotes;
		ImageView imgNotesSaved;

		public NotesFragment () : base()
		{
		}

		public NotesFragment(UserCard selectedCard) : base(){
			SelectedCard = selectedCard;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.Notes, container, false);
		
			imgNotesSaved = view.FindViewById<ImageView> (Resource.Id.imgNotesSaved);

			var btnSaveNotes = view.FindViewById<Button> (Resource.Id.btnSaveNotes);
			txtNotes = view.FindViewById<EditText> (Resource.Id.txtNotes);

			txtNotes.Text = SelectedCard.Notes;
			txtNotes.MovementMethod = LinkMovementMethod.Instance; // for making links clickable
			imgNotesSaved.Visibility = ViewStates.Invisible;

			txtNotes.AfterTextChanged += delegate {
				imgNotesSaved.Visibility = ViewStates.Invisible;
			};

			btnSaveNotes.Click += delegate {
				((MainActivity)Activity).SaveNotes (SelectedCard.UserCardId, txtNotes.Text.Trim ());
				imgNotesSaved.Visibility = ViewStates.Visible;
			};

			var fileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + SelectedCard.Card.FrontFileName);
			var uri = Uri.Parse (fileName);

			var imgNotesCardHorizontal = view.FindViewById<ImageView> (Resource.Id.imgNotesCardHorizontal);
			var imgNotesCardVertical = view.FindViewById<ImageView> (Resource.Id.imgNotesCardVertical);
			var isHorizontal = SelectedCard.Card.FrontOrientation == "H";
			var imgDisplay = isHorizontal ? imgNotesCardHorizontal : imgNotesCardVertical;
			imgNotesCardHorizontal.Visibility = !isHorizontal ? ViewStates.Gone : ViewStates.Visible;
			imgNotesCardVertical.Visibility = isHorizontal ? ViewStates.Gone : ViewStates.Visible;

			imgDisplay.SetImageURI (uri);

			var btnClose = view.FindViewById<ImageButton> (Resource.Id.btnClose);
			btnClose.Click += delegate {
				var panel = new ButtonPanelFragment();
				panel.SelectedCard = SelectedCard;
				((MainActivity)Activity).UnloadFragment(panel);
			};

			return view;
		}
	}
}

