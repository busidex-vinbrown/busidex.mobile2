using System;

namespace Busidex3.DomainModels
{
	public class UserDevice
	{
		public long UserId { get; set; }
		public int DeviceTypeId { get; set; }
		public int? Version { get; set; }
		public DateTime Created { get; set; }
		public bool Deleted { get; set; }
	}
}

