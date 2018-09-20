using System;
using System.Collections.Generic;
using Busidex.Mobile.Models;

namespace Busidex3.Models
{
	public class EventListResponse
	{
		public bool Success { get; set; }
		public List<EventTag> Model { get; set; }
		public DateTime LastRefresh { get; set; }
	}
}