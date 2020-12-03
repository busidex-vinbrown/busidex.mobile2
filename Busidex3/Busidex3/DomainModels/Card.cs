using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Busidex3.Annotations;

namespace Busidex3.DomainModels
{
	public class Card : INotifyPropertyChanged
	{
		public Card (Card model)
		{
			Tags = new List<Tag> ();
			Addresses = new List<Address> ();
			PhoneNumbers = new List<PhoneNumber>();

			if (model == null) {
				return;
			}

			CardId = model.CardId;
			Name = model.Name;
			FrontOrientation = model.FrontOrientation;
			BackOrientation = model.BackOrientation;
			Searchable = model.Searchable;
			Email = model.Email;
			Url = model.Url;

			CreatedBy = model.CreatedBy;
			OwnerId = model.OwnerId;
			CompanyName = model.CompanyName;
			OwnerToken = model.OwnerToken;
			FrontFileId = model.FrontFileId.GetValueOrDefault ();
			BackFileId = model.BackFileId.GetValueOrDefault ();
			Title = model.Title;
			Markup = model.Markup;
			Display = model.Display;
			FrontType = model.FrontType;
			BackType = model.BackType;
			Visibility = model.Visibility;
			ExistsInMyBusidex = model.ExistsInMyBusidex;

			if (model.Tags != null) {
				Tags.AddRange (model.Tags);
			}
			if(model.ExternalLinks != null)
            {
				ExternalLinks.AddRange(model.ExternalLinks);
            }
			if (model.Addresses != null) {
				Addresses.AddRange (model.Addresses);
			}
			if (model.PhoneNumbers != null) {
				PhoneNumbers.AddRange (model.PhoneNumbers);
			}
            
		}

		public Card (CardDetailModel model)
		{
			Tags = new List<Tag> ();// model.Tags;
			Addresses = new List<Address> ();// model.Addresses;
			PhoneNumbers = new List<PhoneNumber> ();

			if (model == null) {
				return;
			}

			CardId = model.CardId;
			Name = model.Name;
			FrontOrientation = model.FrontOrientation;
			BackOrientation = model.BackOrientation;
			Searchable = model.Searchable;
			Email = model.Email;
			Url = model.Url;
			CreatedBy = model.CreatedBy;
			OwnerId = model.OwnerId;
			CompanyName = model.CompanyName;
			OwnerToken = model.OwnerToken;
			FrontFileId = model.FrontFileId.GetValueOrDefault ();
			BackFileId = model.BackFileId.GetValueOrDefault ();
			Title = model.Title;
			Markup = model.Markup;
			Display = (int)model.Display;
			FrontType = model.FrontFileType ?? model.FrontType;
			BackType = model.BackFileType ?? model.BackType;
			Visibility = model.Visibility;
			ExistsInMyBusidex = model.ExistsInMyBusidex;

			if (model.Tags != null) {
				Tags.AddRange (model.Tags);
			}
			if(model.ExternalLinks != null)
            {
				ExternalLinks.AddRange(model.ExternalLinks);
            }
			if (model.Addresses != null) {
				Addresses.AddRange (model.Addresses);
			}
			if (model.PhoneNumbers != null) {
				PhoneNumbers.AddRange (model.PhoneNumbers);
			}
		}

		public Card ()
		{
			PhoneNumbers = PhoneNumbers ?? new List<PhoneNumber> ();
			Tags = Tags ?? new List<Tag> ();
			Addresses = Addresses ?? new List<Address> ();
			ExternalLinks = new List<ExternalLink>();
		}

		public long CardId { get; set; }

		public string Name { get; set; }

		public string FrontOrientation { get; set; }

		public string BackOrientation { get; set; }

		public long? BusinessId { get; set; }

		public bool Searchable { get; set; }

        private string _email;
		public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private string _url;
        public string Url
        {
            get => _url;
            set
            {
                _url = value;
                OnPropertyChanged(nameof(Url));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));  
        }
   

		public PhoneNumber PhoneNumber1 { get; set; }

		public DateTime Created { get; set; }

		public long? CreatedBy { get; set; }

		public long? OwnerId { get; set; }

		public DateTime Updated { get; set; }

		public bool Deleted { get; set; }

		public string CompanyName { get; set; }

		public Guid? OwnerToken { get; set; }

		public Guid? FrontFileId { get; set; }

		public Guid? BackFileId { get; set; }

		public string Title { get; set; }

		public string Markup { get; set; }

        private CardVisibility _visibility { get; set; }
        public CardVisibility Visibility { get => _visibility;
            set
            {
                _visibility = value;
                OnPropertyChanged(nameof(Visibility));
            }
        }

		public int Display { get; set; }

		public byte [] FrontImage { get; set; }

		public byte [] BackImage { get; set; }

		public string FrontType { get; set; }

		public string BackType { get; set; }

		public bool IsMyCard { get; set; }

		public int CardType { get; set; }

        private List<PhoneNumber> _phoneNumbers;

        public List<PhoneNumber> PhoneNumbers
        {
            get => _phoneNumbers;
            set
            {
                _phoneNumbers = value;
                OnPropertyChanged(nameof(PhoneNumbers));
            }
        }

        public List<Tag> Tags { get; set; }

		public List<ExternalLink> ExternalLinks { get; set; } = new List<ExternalLink>();

		public List<Address> Addresses { get; set; }

		public string BackImageString { get; set; }

		public string FrontImageString { get; set; }

		public string TagList { get; set; }

		public bool ExistsInMyBusidex { get; set; }

		const string fileName = "{0}.{1}";

		public string FrontFileName => string.Format (fileName, FrontFileId, FrontType);
        public string FrontThumbnailName => string.Format (StringResources.THUMBNAIL_FILE_NAME_PREFIX + fileName, FrontFileId, FrontType);

        public string BackFileName => string.Format (fileName, BackFileId, BackType);
        public string BackThumbnailName => string.Format (StringResources.THUMBNAIL_FILE_NAME_PREFIX + fileName, BackFileId, BackType);

        public bool HasBackImage =>
            BackFileId != Guid.Empty &&
            BackFileId != null &&
            BackFileId.ToString() != StringResources.EMPTY_CARD_ID;

	}
}

