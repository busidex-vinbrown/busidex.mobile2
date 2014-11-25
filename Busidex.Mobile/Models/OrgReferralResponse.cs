using System;
using Busidex.Mobile.Models;
using System.Collections.Generic;

namespace Busidex.Mobile
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

