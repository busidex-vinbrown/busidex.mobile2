using Busidex.Mobile.Models;
using System.IO;
using Android.Widget;
using Android.Views;
using Android.OS;
using Android.Net;

namespace Busidex.Presentation.Droid.v2
{
	public class NotesFragment : GenericViewPagerFragment
	{
		readonly UserCard SelectedCard;
		AutoCompleteTextView txtNotes;
		ImageView imgNotesSaved;

		public NotesFragment ()
		{
		}

		public NotesFragment(UserCard selectedCard){
			SelectedCard = selectedCard;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.Notes, container, false);
		
			imgNotesSaved = view.FindViewById<ImageView> (Resource.Id.imgNotesSaved);

			var btnSaveNotes = view.FindViewById<Button> (Resource.Id.btnSaveNotes);
			txtNotes = view.FindViewById<AutoCompleteTextView> (Resource.Id.txtNotes);

			txtNotes.Text = SelectedCard.Notes;

			imgNotesSaved.Visibility = ViewStates.Invisible;

			txtNotes.AfterTextChanged += delegate {
				imgNotesSaved.Visibility = ViewStates.Invisible;
			};

			btnSaveNotes.Click += delegate {
				SaveNotes();
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

			return view;
		}

		void SaveNotes(){

			var token = applicationResource.GetAuthCookie ();

			var controller = new Busidex.Mobile.NotesController ();
			controller.SaveNotes (SelectedCard.UserCardId, txtNotes.Text.Trim (), token).ContinueWith (response => {
				var result = response.Result;
				if(!string.IsNullOrEmpty(result)){

					Activity.RunOnUiThread(() =>{
						SaveNotesResponse obj = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveNotesResponse> (result);
						if(obj.Success){

							UpdateLocalCardNotes ();

							// need to sync the notes with the local user card
							imgNotesSaved.Visibility = ViewStates.Visible;
						}
					});
				}
			});
		}

		void UpdateLocalCardNotes(){

			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.MY_BUSIDEX_FILE);
			// we only need to update the file if they've gotten their busidex. If they haven't, the new card will
			// come along with all the others
			var file = string.Empty;
			if (File.Exists (fullFilePath)) {
				using (var myBusidexFile = File.OpenText (fullFilePath)) {
					var myBusidexJson = myBusidexFile.ReadToEnd ();
					MyBusidexResponse myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (myBusidexJson);
					foreach(var uc in myBusidexResponse.MyBusidex.Busidex){
						if(uc.UserCardId == SelectedCard.UserCardId){
							uc.Notes = txtNotes.Text.Trim ();
							break;
						}
					}
					file = Newtonsoft.Json.JsonConvert.SerializeObject(myBusidexResponse);
				}

				File.WriteAllText (fullFilePath, file);
			}

		}
	}
}

