using System;
namespace Busidex3.DomainModels
{
	public class EventSearchResponse : SearchResponse
	{
		public DateTime LastRefreshDate { get; set; }
	}
}