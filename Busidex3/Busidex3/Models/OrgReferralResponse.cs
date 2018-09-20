using System.Collections.Generic;

namespace Busidex3.Models
{
	public class OrgReferralResponse
	{
		public bool Success{ get; set; }
		public List<UserCard> Model{ get; set; }

		public OrgReferralResponse ()
		{
		}
	}
}

