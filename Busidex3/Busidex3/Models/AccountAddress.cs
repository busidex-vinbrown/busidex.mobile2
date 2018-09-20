
namespace Busidex3.Models
{
	public class AccountAddress
	{
		public long CardAddressId{ get; set; }
		public long CardId{ get; set; }
		public string Address1{ get; set; }
		public string Address2{ get; set; }
		public string City{ get; set; }
		public string State{ get; set; }
		public string ZipCode{ get; set; }
		public string Region{ get; set; }
		public string Country{ get; set; }
		public double Latitude{ get; set; }
		public double Longitude{ get; set; }
	}
}

