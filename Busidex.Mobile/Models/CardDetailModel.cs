using System;
using System.Collections.Generic;

namespace Busidex.Mobile.Models
{
	public class CardDetailModel
	{
		public long CardId { get; set; }
		        public string Name { get; set; }
		        public string Title { get; set; }
		        public int? BusinessId { get; set; }
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
		        public Guid OwnerToken { get; set; }
		        public string TagList { get; set; }
		        public List<Tag> Tags { get; set; }
		        public DisplayType Display { get; set; }
		        public string Markup { get; set; }
		        public long CreatedBy { get; set; }
		        public CardType CardType { get; set; }
		        public List<Address> Addresses { get; set; }
	}
}

