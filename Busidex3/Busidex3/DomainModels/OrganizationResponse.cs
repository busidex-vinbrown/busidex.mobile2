using System.Collections.Generic;

namespace Busidex3.DomainModels
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

