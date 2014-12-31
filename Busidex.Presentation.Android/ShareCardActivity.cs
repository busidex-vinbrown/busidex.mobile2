
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "ShareCardActivity")]			
	public class ShareCardActivity : BaseCardActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.SharedCard);
			// Create your application here
		}
	}
}

