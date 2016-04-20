using System;
using System.Collections.Generic;
using System.Linq;
using Busidex.Mobile.Models;
using Foundation;
using UIKit;

namespace Busidex.Presentation.iOS
{
	public class Application
	{
		//public static List<UserCard> MyBusidex { get; set; }
		public static UINavigationController MainController { get; set; }
		public static bool MyBusidexInvalidated { get; set; } // Seems a little hacky, but oh well

		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			Xamarin.Insights.Initialize (XamarinInsights.ApiKey);
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}
