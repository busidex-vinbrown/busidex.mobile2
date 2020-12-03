using System;
using System.Reflection;
using Busidex.Models.Domain;
using Xamarin.Forms;

namespace Busidex3.ViewModels
{
    public class CardMenuVM : BaseViewModel
    {
        public CardMenuVM()
        {
            EditCardImage = ImageSource.FromResource("Busidex.Resources.Images.editimage.png",
                typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
            VisibilityImage = ImageSource.FromResource("Busidex.Resources.Images.visibility_64x64.png",
                typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
            ContactInfoImage = ImageSource.FromResource("Busidex.Resources.Images.contacts_64x64.png",
                typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
            SearchInfoImage = ImageSource.FromResource("Busidex.Resources.Images.search_64x64.png",
                typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
            TagsImage = ImageSource.FromResource("Busidex.Resources.Images.tags_64x64.png",
                typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
            AddressInfoImage = ImageSource.FromResource("Busidex.Resources.Images.maps.png",
                typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
            ExternalLinksImage = ImageSource.FromResource("Busidex.Resources.Images.externallink.png",
                typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
        }
        
        public void CheckHasCard()
        {
            ShowCardImage = SelectedCard?.Card?.FrontFileId != Guid.Empty && SelectedCard?.Card?.FrontFileId != null;            
        }

        public UserCard SelectedCard { get; set; }

        private bool _showCardImage;
        public bool ShowCardImage
        {
            get => _showCardImage;
            set
            {
                _showCardImage = value;
                OnPropertyChanged(nameof(ShowCardImage));
            }
        }

        private int _imageSize { get; set; }
        public int ImageSize
        {
            get { return _imageSize; }
            set
            {
                _imageSize = value;
                OnPropertyChanged(nameof(ImageSize));
            }
        }

        private ImageSource _externalLinksImage { get; set; }
        public ImageSource ExternalLinksImage {
            get => _externalLinksImage;
            set {
                _externalLinksImage = value;
                OnPropertyChanged(nameof(ExternalLinksImage));
            }
        }

        private ImageSource _editCardImage { get; set; }
        public ImageSource EditCardImage { get => _editCardImage;
            set
            {
                _editCardImage = value;
                OnPropertyChanged(nameof(EditCardImage));
            }
        }

        private ImageSource _visibilityImage { get; set; }
        public ImageSource VisibilityImage { get => _visibilityImage;
            set
            {
                _visibilityImage = value;
                OnPropertyChanged(nameof(VisibilityImage));
            }
        }

        private ImageSource _contactInfoImage { get; set; }
        public ImageSource ContactInfoImage { get => _contactInfoImage;
            set
            {
                _contactInfoImage = value;
                OnPropertyChanged(nameof(ContactInfoImage));
            }
        }

        private ImageSource _searchInfoImage { get; set; }
        public ImageSource SearchInfoImage { get => _searchInfoImage;
            set
            {
                _searchInfoImage = value;
                OnPropertyChanged(nameof(SearchInfoImage));
            }
        }

        private ImageSource _tagsImage { get; set; }
        public ImageSource TagsImage { get => _tagsImage;
            set
            {
                _tagsImage = value;
                OnPropertyChanged(nameof(TagsImage));
            }
        }

        private ImageSource _addressInfoImage { get; set; }
        public ImageSource AddressInfoImage { get => _addressInfoImage;
            set
            {
                _addressInfoImage = value;
                OnPropertyChanged(nameof(AddressInfoImage));
            }
        }
    }
}
