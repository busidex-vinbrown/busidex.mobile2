
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
	[Activity (Label = "SharedCardsActivity")]			
	public class SharedCardsActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetContentView (Resource.Layout.SharedCardList);

			base.OnCreate (savedInstanceState);

			// Create your application here
		}
	}
}

