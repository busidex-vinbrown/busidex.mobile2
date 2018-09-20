using System;
namespace Busidex3.Models
{
	public class EventSearchResponse : SearchResponse
	{
		public DateTime LastRefreshDate { get; set; }
	}
}