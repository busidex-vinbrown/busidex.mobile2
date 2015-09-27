using System;
using Android.Content;
using Android.Util;
using Android.Widget;

namespace Busidex.Presentation.Droid.v2
{
	public class OverscrollListView : ListView, ListView.IOnScrollListener
	{
		public event Action<int> OverScrolled;
		public event Action OverScrollCanceled;

		public OverscrollListView (Context context) :
		base (context)
		{
			Initialize ();
		}

		public OverscrollListView (Context context, IAttributeSet attrs) :
		base (context, attrs)
		{
			Initialize ();
		}

		public OverscrollListView (Context context, IAttributeSet attrs, int defStyle) :
		base (context, attrs, defStyle)
		{
			Initialize ();
		}

		void Initialize ()
		{
			SetOnScrollListener (this);
		}

		protected override bool OverScrollBy (int deltaX, int deltaY, int scrollX, int scrollY, int scrollRangeX, int scrollRangeY, int maxOverScrollX, int maxOverScrollY, bool isTouchEvent)
		{
			if (OverScrolled != null) {
				OverScrolled (deltaY);
			}

			return base.OverScrollBy (deltaX, deltaY, scrollX, scrollY, scrollRangeX, scrollRangeY, maxOverScrollX, maxOverScrollY, isTouchEvent);
		}

		public void OnScroll (AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
		{
			if (OverScrollCanceled != null)
				OverScrollCanceled ();
		}

		public void OnScrollStateChanged (AbsListView view, ScrollState scrollState)
		{
			if (OverScrollCanceled != null
				&& (scrollState == ScrollState.Idle || scrollState == ScrollState.Fling))
				OverScrollCanceled ();
		}
	}
}

