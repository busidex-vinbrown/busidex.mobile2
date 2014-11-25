using System;

namespace Busidex.Mobile.Models
{
	public class Organization
	{
		public Organization ()
		{
		}

		public long OrganizationId{ get; set; }
		public long UserId{ get; set; }
		public string Logo{ get; set; }
		public string LogoType{ get; set; }
		public string LogoFileName{ get; set; }
		public string LogoFilePath{ get; set; }
		public string Name{ get; set; }
		public string Description{ get; set; }
		public string Email{ get; set; }
		public string AdminEmail{ get; set; }
		public string Url{ get; set; }
		public string Contacts{ get; set; }
		public string Phone1{ get; set; }
		public string Extension1{ get; set; }
		public string Phone2{ get; set; }
		public string Extension2{ get; set; }
		public string Twitter{ get; set; }
		public string Facebook{ get; set; }
		public string HomePage{ get; set; }
		public DateTime Created{ get; set; }
		public DateTime Updated { get; set; }
		public bool Deleted { get; set; }
		public object Groups { get; set; }
	}
}

