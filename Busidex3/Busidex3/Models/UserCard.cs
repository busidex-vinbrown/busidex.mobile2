using System;
using System.Collections.Generic;
using System.Linq;

namespace Busidex3.Models
{
	public class UserCard
	{

		public UserCard ()
		{
		}

		public UserCard (CardDetailModel cardDetail)
		{
			UserCardId = 0;
			CardId = cardDetail.CardId;
			UserId = 0;
			OwnerId = cardDetail.OwnerId;
			Card = new Card {
				CardId = cardDetail.CardId,
				Name = cardDetail.Name,
				FrontOrientation = cardDetail.FrontOrientation,
				BackOrientation = cardDetail.BackOrientation,
				BusinessId = cardDetail.BusinessId,
				Searchable = cardDetail.Searchable,
				Email = cardDetail.Email,
				Url = cardDetail.Url,
				PhoneNumber1 = cardDetail.PhoneNumbers.FirstOrDefault (),
				CreatedBy = cardDetail.CreatedBy,
				OwnerId = cardDetail.OwnerId,
				CompanyName = cardDetail.CompanyName,
				OwnerToken = cardDetail.OwnerToken,
				FrontFileId = cardDetail.FrontFileId.GetValueOrDefault (),
				BackFileId = cardDetail.BackFileId.GetValueOrDefault (),
				Title = cardDetail.Title,
				Markup = cardDetail.Markup,
				Display = (int)cardDetail.Display,
				FrontType = cardDetail.FrontType,
				BackType = cardDetail.BackType,
				IsMyCard = cardDetail.IsMyCard,
				CardType = (int)cardDetail.CardType,
				ExistsInMyBusidex = cardDetail.ExistsInMyBusidex
			};
			Deleted = false;
			Notes = string.Empty;
			ExistsInMyBusidex = cardDetail.ExistsInMyBusidex;
		}

		public UserCard (Card card)
		{
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

