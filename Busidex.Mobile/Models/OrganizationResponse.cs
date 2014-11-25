using System;
using System.Collections.Generic;

namespace Busidex.Mobile.Models
{
	public class OrganizationResponse
	{
		public OrganizationResponse ()
		{
		}
		public bool Success{ get; set; }
		public List<Organization> Model {get;set;}
	}
}

