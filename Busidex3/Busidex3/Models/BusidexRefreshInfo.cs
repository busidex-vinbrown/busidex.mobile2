using System;

namespace Busidex3.Models
{
    public enum RefreshItem
    {
        MyBusidex,
        OrganizationList,
        Events
    }

    public class BusidexRefreshInfo
    {
        public DateTime? LastMyBusidexRefresh { get; set; }
        public DateTime? LastOrganizationListRefresh { get; set; }
        public DateTime? LastEventsRefresh { get; set; }
    }
}
