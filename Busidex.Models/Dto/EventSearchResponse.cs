using Busidex.Models.Domain;
using System;
namespace Busidex.Models.Dto
{
    public class EventSearchResponse : SearchResponse
    {
        public DateTime LastRefreshDate { get; set; }
    }
}