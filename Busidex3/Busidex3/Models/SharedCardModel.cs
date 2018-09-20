
namespace Busidex3.Models
{
	public class SharedCardModel
	{
		public long UserId { get; set; }
		public long[] AcceptedCardIdList { get; set; }
		public long[] DeclinedCardIdList { get; set; }
		public long[] CardIdList { get; set; }
		public string SharedWith { get; set; }
		public bool Accepted { get; set; }
		public bool Declined { get; set; }
		public string PersonalMessage { get; set; }
	}
}

