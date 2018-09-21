
using Busidex.Mobile.Models;

namespace Busidex3.DomainModels
{
	public class Address
	{

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

		public override string ToString ()
		{
			return string.Format ("{0} {1} {2}, {3} {4} {5} {6}", Address1, Address2, City, StateCode, ZipCode, Region, Country);
		}

		string StateCode{
			get{
				return State != null ? State.Code : string.Empty;
			}
		}
		public bool HasAddress{get {
				var addressString = string.Format ("{0}{1}{2}{3}{4}{5}{6}", Address1, Address2, City, StateCode, ZipCode, Region, Country);
				return !string.IsNullOrEmpty (addressString);
			}
		}
	}
}

