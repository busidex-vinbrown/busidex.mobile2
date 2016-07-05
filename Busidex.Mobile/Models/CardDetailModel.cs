using System;
using System.Collections.Generic;

namespace Busidex.Mobile.Models
{
	public class CardDetailModel
	{
		public CardDetailModel ()
		{

		}

		public CardDetailModel (Card card)
		{
			CardId = card.CardId;
			Name = card.Name;
			Title = card.Title;
			BusinessId = card.BusinessId;
			Searchable = card.Searchable;
			Email = card.Email;
			Url = card.Url;
			CompanyName = card.CompanyName;

			PhoneNumbers = new List<PhoneNumber> ();
			if (card.PhoneNumbers != null) {
				PhoneNumbers.AddRange (card.PhoneNumbers);
			}
			OwnerId = card.OwnerId;
			IsMyCard = card.IsMyCard;
			IsUserLoggedIn = true;
			FrontFileId = card.FrontFileId;
			FrontFileType = FrontType = card.FrontType;
			BackFileId = card.BackFileId;
			BackFileType = BackType = card.BackType;
			FrontOrientation = card.FrontOrientation;
			BackOrientation = card.BackOrientation;
			ExistsInMyBusidex = card.ExistsInMyBusidex;
			TagList = card.TagList;
			Tags = new List<Tag> ();
			if (card.Tags != null) {
				Tags.AddRange (card.Tags);
			}
			Display = (DisplayType)Enum.Parse (typeof (DisplayType), card.Display.ToString ());
			CreatedBy = card.CreatedBy;
			Visibility = card.Visibility;
			CardType = (CardType)Enum.Parse (typeof (CardType), card.CardType.ToString ());
			Addresses = new List<Address> ();
			if (card.Addresses != null) {
				Addresses.AddRange (card.Addresses);
			}
		}

		public long CardId { get; set; }

		public string Name { get; set; }

		public string Title { get; set; }

		public long? BusinessId { get; set; }

		public bool Searchable { get; set; }

		public string Email { get; set; }

		public string Url { get; set; }

		public string CompanyName { get; set; }

		public List<PhoneNumber> PhoneNumbers { get; set; }

		public Dictionary<long, long> CardRelations { get; set; }

		public long? OwnerId { get; set; }

		public bool IsMyCard { get; set; }

		public bool HasBackImage { get; set; }

		public bool IsUserLoggedIn { get; set; }

		public AccountType UserAccountType { get; set; }

		public Guid? FrontFileId { get; set; }

		public string FrontFileType { get; set; }

		public string FrontType { get; set; }

		public Guid? BackFileId { get; set; }

		public string BackFileType { get; set; }

		public string BackType { get; set; }

		public string FrontOrientation { get; set; }

		public string BackOrientation { get; set; }

		public bool ExistsInMyBusidex { get; set; }

		public string BasicRelationsJSON { get; set; }

		public Guid? OwnerToken { get; set; }

		public string TagList { get; set; }

		public List<Tag> Tags { get; set; }

		public DisplayType Display { get; set; }

		public string Markup { get; set; }

		public long? CreatedBy { get; set; }

		public byte Visibility { get; set; }

		public CardType CardType { get; set; }

		public List<Address> Addresses { get; set; }

		const string fileName = "{0}.{1}";

		public string FrontFileName { get { return string.Format (fileName, FrontFileId, FrontType); } }
	}
}

