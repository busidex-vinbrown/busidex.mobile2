using Android.App;
using Android.OS;
using Android.Widget;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "Busidex")]			
	public class ButtonPanelActivity : BaseActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.ButtonPanel);

			var btnMaps = FindViewById<ImageButton> (Resource.Id.btnPanelMap);
			var btnNotes = FindViewById<ImageButton> (Resource.Id.btnPanelNotes);
			var btnPhone = FindViewById<ImageButton> (Resource.Id.btnPanelPhone);
			var btnEmail = FindViewById<ImageButton> (Resource.Id.btnPanelEmail);
			var btnBrowser = FindViewById<ImageButton> (Resource.Id.btnPanelBrowser);
			var btnShare = FindViewById<ImageButton> (Resource.Id.btnPanelShare);
			var btnAdd = FindViewById<ImageButton> (Resource.Id.btnPanelAdd);
			var btnRemove = FindViewById<ImageButton> (Resource.Id.btnPanelRemove);

		}
	}
}

