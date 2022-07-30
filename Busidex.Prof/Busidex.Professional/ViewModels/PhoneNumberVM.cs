using Busidex.Models.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace Busidex.Professional.ViewModels
{
    public class PhoneNumberVM : BaseViewModel
    {
        private string _selectedPhoneNumberType;
        public string SelectedPhoneNumberType {
            get => _selectedPhoneNumberType;
            set {
                if (value != null)
                {
                    _selectedPhoneNumberType = value;
                    OnPropertyChanged(nameof(SelectedPhoneNumberType));
                }
            }
        }

        public int PhoneNumberId { get; set; }
        public bool Deleted { get; set; }

        private string _number;
        public string Number {
            get => _number;
            set {
                _number = value;
                OnPropertyChanged(nameof(Number));
            }
        }

        public ImageSource SmsImageSource { get; }
        public ImageSource PhoneImageSource { get; }

        private ImageSource _deletePhoneImage { get; set; }
        public ImageSource DeletePhoneImage {
            get => _deletePhoneImage;
            set {
                _deletePhoneImage = value;
                OnPropertyChanged(nameof(DeletePhoneImage));
            }
        }

        private ObservableCollection<string> _phoneNumberTypeNames;
        public ObservableCollection<string> PhoneNumberTypeNames {
            get => _phoneNumberTypeNames;
            set {
                _phoneNumberTypeNames = value;
                OnPropertyChanged(nameof(PhoneNumberTypeNames));
            }
        }

        public PhoneNumberVM(PhoneNumber p)
        {
            SmsImageSource = ImageSource.FromResource("Busidex.Resources.Images.textmessage.png", typeof(Resources.Images.ImageLoader).GetTypeInfo().Assembly);
            PhoneImageSource = ImageSource.FromResource("Busidex.Resources.Images.phone.png", typeof(Resources.Images.ImageLoader).GetTypeInfo().Assembly);

            PhoneNumberId = p.PhoneNumberId;
            Number = p.Number;
            var typeNames = getPhoneNumberTypes().Select(t => t.Name).ToList();
            PhoneNumberTypeNames = new ObservableCollection<string>(typeNames);
            SelectedPhoneNumberType = p.PhoneNumberType?.Name;
            DeletePhoneImage = ImageSource.FromResource("Busidex.Resources.Images.red_minus.png",
                typeof(Resources.Images.ImageLoader).GetTypeInfo().Assembly);
        }

        public PhoneNumberType GetSelectedPhoneNumberType()
        {
            return getPhoneNumberTypes().SingleOrDefault(t => t.Name == SelectedPhoneNumberType);
        }

        private List<PhoneNumberType> getPhoneNumberTypes()
        {
            return new List<PhoneNumberType> {
                new PhoneNumberType {
                    PhoneNumberTypeId = 1,
                    Name = "Business",
                    Deleted = false
                },
                new PhoneNumberType {
                    PhoneNumberTypeId = 2,
                    Name = "Home",
                    Deleted = false
                },
                new PhoneNumberType {
                    PhoneNumberTypeId = 3,
                    Name = "Mobile",
                    Deleted = false
                },
                new PhoneNumberType {
                    PhoneNumberTypeId = 4,
                    Name = "Fax",
                    Deleted = false
                },
                new PhoneNumberType {
                    PhoneNumberTypeId = 5,
                    Name = "Toll Free",
                    Deleted = false
                },
                new PhoneNumberType {
                    PhoneNumberTypeId = 6,
                    Name = "eFax",
                    Deleted = false
                },
                new PhoneNumberType {
                    PhoneNumberTypeId = 7,
                    Name = "Other",
                    Deleted = false
                },
                new PhoneNumberType {
                    PhoneNumberTypeId = 8,
                    Name = "Direct",
                    Deleted = false
                },
                new PhoneNumberType {
                    PhoneNumberTypeId = 9,
                    Name = "Voice Mail",
                    Deleted = false
                },
                new PhoneNumberType {
                    PhoneNumberTypeId = 10,
                    Name = "Business 2",
                    Deleted = false
                }
            };
        }
    }
}
