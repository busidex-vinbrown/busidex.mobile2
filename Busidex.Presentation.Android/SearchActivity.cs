

using Android.App;
using Android.OS;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "Search")]			
	public class SearchActivity : BaseActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.Search);

			// Create your application here
		}
	}
}

