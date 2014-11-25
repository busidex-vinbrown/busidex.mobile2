using System;
using System.Collections.Generic;
using Busidex.Mobile.Models;

namespace Busidex.Mobile
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

