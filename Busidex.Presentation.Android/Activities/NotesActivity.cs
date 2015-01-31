
using Android.App;
using Android.OS;
using Android.Widget;
using Busidex.Mobile.Models;
using Android.Net;
using System.IO;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "Notes")]			
	public class NotesActivity : BaseCardActivity
	{

		AutoCompleteTextView txtNotes;
		ImageView imgNotesSaved;

		protected override void OnImageDownloadCompleted (Uri uri)
		{
			base.OnImageDownloadCompleted (uri);

			var imgNotesCardHorizontal = FindViewById<ImageView> (Resource.Id.imgNotesCardHorizontal);
			var imgNotesCardVertical = FindViewById<ImageView> (Resource.Id.imgNotesCardVertical);
			var isHorizontal = UserCard.Card.FrontOrientation == "H";
			var imgDisplay = isHorizontal ? imgNotesCardHorizontal : imgNotesCardVertical;
			imgNotesCardHorizontal.Visibility = isHorizontal ? global::Android.Views.ViewStates.Visible : global::Android.Views.ViewStates.Gone;
			imgNotesCardVertical.Visibility = isHorizontal ? global::Android.Views.ViewStates.Gone : global::Android.Views.ViewStates.Visible;

			imgDisplay.SetImageURI (uri);
		}

		void SaveNotes(){

			var token = GetAuthCookie ();

			var controller = new Busidex.Mobile.NotesController ();
			controller.SaveNotes (UserCard.UserCardId, txtNotes.Text.Trim (), token).ContinueWith (response => {
				var result = response.Result;
				if(!string.IsNullOrEmpty(result)){

					RunOnUiThread(() =>{
						SaveNotesResponse obj = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveNotesResponse> (result);
						if(obj.Success){

							UpdateLocalCardNotes ();

							// need to sync the notes with the local user card
							imgNotesSaved.Visibility = global::Android.Views.ViewStates.Visible;
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
						if(uc.UserCardId == UserCard.UserCardId){
							uc.Notes = txtNotes.Text.Trim ();
							break;
						}
					}
					file = Newtonsoft.Json.JsonConvert.SerializeObject(myBusidexResponse);
				}

				File.WriteAllText (fullFilePath, file);
			}

		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetContentView (Resource.Layout.Notes);

			TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_NOTES, 0);

			base.OnCreate (savedInstanceState);

			imgNotesSaved = FindViewById<ImageView> (Resource.Id.imgNotesSaved);

			var btnSaveNotes = FindViewById<Button> (Resource.Id.btnSaveNotes);
			txtNotes = FindViewById<AutoCompleteTextView> (Resource.Id.txtNotes);

			txtNotes.Text = UserCard.Notes;

			imgNotesSaved.Visibility = global::Android.Views.ViewStates.Invisible;

			txtNotes.AfterTextChanged += delegate {
				imgNotesSaved.Visibility = global::Android.Views.ViewStates.Invisible;
			};

			btnSaveNotes.Click += delegate {
				SaveNotes();
			};
		}
	}
}

