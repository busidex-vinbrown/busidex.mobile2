using System.Collections.Generic;

namespace Busidex3.DomainModels
{
	public class OrgMemberResponse
	{
		public bool Success{ get; set; }
		public List<Card> Model{ get; set; }

		public OrgMemberResponse ()
		{
		}
	}
}

