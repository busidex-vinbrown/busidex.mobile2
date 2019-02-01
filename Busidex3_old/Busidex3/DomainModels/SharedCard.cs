using System;

namespace Busidex3.DomainModels
{
	public class SharedCard
	{
		public long SharedCardId { get; set; }
		public long CardId { get; set; }
		public long SendFrom { get; set; }
		public string SendFromEmail { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public long? ShareWith { get; set; }
		public DateTime SharedDate { get; set; }
		public bool? Accepted { get; set; }
		public bool? Declined { get; set; }
		public string Recommendation { get; set; }
		public bool UseQuickShare { get; set; }
		public Card Card { get; set; }
	}
}