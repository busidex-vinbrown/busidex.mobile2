using Busidex.Models.Domain;
using System;
using System.Collections.Generic;

namespace Busidex.Models.Dto
{
    public class EventListResponse
    {
        public bool Success { get; set; }
        public List<EventTag> Model { get; set; }
        public DateTime LastRefresh { get; set; }
    }
}