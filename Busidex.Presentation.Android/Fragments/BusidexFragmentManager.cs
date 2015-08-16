using System;
using Android.App;

namespace Busidex.Presentation.Android
{
	public class BusidexFragmentManager
	{
		readonly FragmentManager manager;
		Activity context;
		public BusidexFragmentManager(Activity ctx){
			context = ctx;
			manager = context.FragmentManager;
		}


	}
}

