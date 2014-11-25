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
		public const string MY_BUSIDEX_FILE = "mybusidex.json";
		public static List<UserCard> MyBusidex{ get; set; } 

		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}
