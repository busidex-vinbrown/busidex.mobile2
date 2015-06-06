using System;
using Android.Views;

namespace Busidex.Presentation.Android
{
	public class SwipeGestureListener : GestureDetector.SimpleOnGestureListener
	{
		public SwipeGestureListener ()
		{
		}



		public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
		{
			
			return true;
		}
	}

}

