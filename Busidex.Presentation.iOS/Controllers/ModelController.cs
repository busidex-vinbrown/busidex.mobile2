﻿using System;
using System.Collections.Generic;

using Foundation;
using UIKit;

namespace Busidex.Presentation.iOS
{
	public class ModelController : UIPageViewControllerDataSource
	{
		readonly List<string> pageData;

		public ModelController ()
		{
			var formatter = new NSDateFormatter ();
			pageData = new List<string> (formatter.MonthSymbols);
		}

		public HomeController GetViewController (int index, UIStoryboard storyboard)
		{
			if (index >= pageData.Count)
				return null;

			// Create a new view controller and pass suitable data.
			var dataViewController = (HomeController)storyboard.InstantiateViewController ("DataViewController");
			dataViewController.DataObject = pageData [index];

			return dataViewController;
		}

		public int IndexOf (HomeController viewController)
		{
			return pageData.IndexOf (viewController.DataObject);
		}

		public override UIViewController GetNextViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			int index = IndexOf ((HomeController)referenceViewController);

			if (index == -1 || index == pageData.Count - 1)
				return null;

			return GetViewController (index + 1, referenceViewController.Storyboard);
		}

		public override UIViewController GetPreviousViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			int index = IndexOf ((HomeController)referenceViewController);

			if (index == -1 || index == 0)
				return null;

			return GetViewController (index - 1, referenceViewController.Storyboard);
		}


	}
}

