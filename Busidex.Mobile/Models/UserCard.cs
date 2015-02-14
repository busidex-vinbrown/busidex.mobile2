using System;
using System.Collections.Generic;

namespace Busidex.Mobile.Models
{
	public class UserCard
	{
		public UserCard ()
		{
		}

		public UserCard(Card card){
			UserCardId = 0;
			CardId = card.CardId;
			UserId = 0;
			OwnerId = card.OwnerId;
			Card = card;
			Created = card.Created;
			Deleted = card.Deleted;
			Notes = string.Empty;
			ExistsInMyBusidex = card.ExistsInMyBusidex;
		}

		public long UserCardId{ get; set; }
		public long CardId{ get; set; }
		public long UserId{ get; set; }
		public long? OwnerId{ get; set; }
		public DateTime Created{ get; set; }
		public bool Deleted{ get; set; }
		public string Notes{ get; set; }
		public long? SharedById{ get; set; }
		public bool Selected{ get; set; }
		public bool MobileView{ get; set; }
		public Card Card{ get; set; }
		public List<Card> RelatedCards{ get; set; }
		public bool ExistsInMyBusidex{ get; set; }
	}
}

