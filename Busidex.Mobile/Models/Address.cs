using System;

namespace Busidex.Mobile.Models
{
	public class Address
	{
		public Address ()
		{
		}

		public long CardAddressId{ get; set; }
		public long CardId{ get; set; }
		public string Address1{ get; set; }
		public string Address2{ get; set; }
		public string City{ get; set; }
		public State State{ get; set; }
		public int StateCodeId{ get; set; }
		public string ZipCode{ get; set; }
		public string Region{ get; set; }
		public string Country{ get; set; }
		public bool Deleted{ get; set; }
		public double Latitude{ get; set; }
		public double Longitude{ get; set; }

	}
}

