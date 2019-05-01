using System.Reflection;
using Busidex3.DomainModels;
using Xamarin.Forms;

namespace Busidex3.ViewModels
{
    public class CardMenuVM : BaseViewModel
    {
        public CardMenuVM()
        {
            EditCardImage = ImageSource.FromResource("Busidex3.Resources.editimage.png",
                typeof(CardMenuVM).GetTypeInfo().Assembly);
            VisibilityImage = ImageSource.FromResource("Busidex3.Resources.visibility_64x64.png",
                typeof(CardMenuVM).GetTypeInfo().Assembly);
            ContactInfoImage = ImageSource.FromResource("Busidex3.Resources.contacts_64x64.png",
                typeof(CardMenuVM).GetTypeInfo().Assembly);
            SearchInfoImage = ImageSource.FromResource("Busidex3.Resources.search_64x64.png",
                typeof(CardMenuVM).GetTypeInfo().Assembly);
            TagsImage = ImageSource.FromResource("Busidex3.Resources.tags_64x64.png",
                typeof(CardMenuVM).GetTypeInfo().Assembly);
            AddressInfoImage = ImageSource.FromResource("Busidex3.Resources.maps.png",
                typeof(CardMenuVM).GetTypeInfo().Assembly);
        }

        public UserCard SelectedCard { get; set; }

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
