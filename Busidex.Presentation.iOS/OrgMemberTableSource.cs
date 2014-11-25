using System;
using System.Collections.Generic;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.iOS
{
	public class OrgMemberTableSource : TableSource
	{
		public OrgMemberTableSource (List<UserCard> items) : base(items)
		{

		}
	}
}

