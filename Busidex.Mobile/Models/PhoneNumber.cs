using System;

namespace Busidex.Mobile.Models
{
	public class PhoneNumber
	{
		public PhoneNumber ()
		{
		}

		public int PhoneNumberId{ get; set; }
		public PhoneNumberType PhoneNumberType{ get; set;}
		public int PhoneNumberTypeId{ get; set; }
		public long CardId{ get; set; }
		public string Number{ get; set; }
		public string Extension{ get; set; }
		public DateTime Created{ get; set; }
		public DateTime Updated{ get; set; }
		public bool Deleted{ get; set; }
	}
}

