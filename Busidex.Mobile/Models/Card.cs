using System;
using System.Collections.Generic;

namespace Busidex.Mobile.Models
{
	public class Card
	{
		public Card ()
		{
		}

		public long CardId{ get; set; }
		public string Name{ get; set; }
		public string FrontOrientation{ get; set; }
		public string BackOrientation{ get; set; }
		public long? BusinessId{ get; set; }
		public bool Searchable{ get; set; }
		public string Email{ get; set; }
		public string Url{ get; set; }
		public PhoneNumber PhoneNumber1{ get; set; }
		public DateTime Created { get; set; }
		public long CreatedBy{ get; set; }
		public long? OwnerId{ get; set; }
		public DateTime Updated { get; set; }
		public bool Deleted{ get; set; }
		public string CompanyName{ get; set; }
		public Guid? OwnerToken{ get; set; }
		public Guid FrontFileId{ get; set; }
		public Guid BackFileId{ get; set; }
		public string Title{ get; set; }
		public string Markup{ get; set; }
		public int Visibility{ get; set; }
		public int Display{ get; set; }
		public byte[] FrontImage{ get; set; }
		public byte[] BackImage { get; set; }
		public string FrontType{ get; set; }
		public string BackType{ get; set; }
		public bool IsMyCard{ get; set; }
		public int CardType{ get; set; }
		public List<PhoneNumber> PhoneNumbers { get; set; }
		public List<Tag> Tags{ get; set; }
		public List<Address> Addresses{ get; set; }
		public string BackImageString{ get; set; }
		public string FrontImageString{ get; set; }
		public string TagList{ get; set; }
		public bool ExistsInMyBusidex{ get; set; }
	}
}

