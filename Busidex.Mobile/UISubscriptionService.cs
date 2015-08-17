using System;
using Busidex.Mobile.Models;
using System.Collections.Generic;

namespace Busidex.Mobile
{
	public static class UISubscriptionService
	{
		static UISubscriptionService(){
			UserCards = UserCards ?? new List<UserCard>();
			EventList = EventList ?? new List<EventTag> ();
			EventCards = EventCards ?? new Dictionary<string, List<UserCard>> ();
		}

		public static List<UserCard> UserCards { get; set; }
		public static List<EventTag> EventList { get; set; }
		public static Dictionary<string, List<UserCard>> EventCards { get; set; }
	}
}

