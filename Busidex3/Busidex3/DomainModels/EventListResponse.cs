using System;
using System.Collections.Generic;

namespace Busidex3.DomainModels
{
	public class EventListResponse
	{
		public bool Success { get; set; }
		public List<EventTag> Model { get; set; }
		public DateTime LastRefresh { get; set; }
	}
}