using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Busidex3.Annotations;
using Busidex3.DomainModels;
using Plugin.InputKit.Shared.Controls;
using Xamarin.Forms;

namespace Busidex3.ViewModels
{
    public class PhoneNumberVM : INotifyPropertyChanged
    {
        private string _selectedPhoneNumberType;
        public string SelectedPhoneNumberType { 
            get => _selectedPhoneNumberType;
            set
            {
                _selectedPhoneNumberType = value;
                OnPropertyChanged(nameof(SelectedPhoneNumberType));
            }
        }

        public int PhoneNumberId { get; set; }
        public bool Deleted { get; set; }

        private string _number;
        public string Number { 
            get => _number;
            set
            {
                _number = value;
                OnPropertyChanged(nameof(Number));
            }
        }

        public ImageSource SmsImageSource { get; }
        public ImageSource PhoneImageSource { get; }
        
        private ImageSource _deletePhoneImage { get; set; }
        public ImageSource DeletePhoneImage { get => _deletePhoneImage;
            set
            {
                _deletePhoneImage = value;
                OnPropertyChanged(nameof(DeletePhoneImage));
            }
        }

        private List<string> _phoneNumberTypeNames;
        public List<string> PhoneNumberTypeNames
        {
            get => _phoneNumberTypeNames;
            set
            {
                _phoneNumberTypeNames = value;
                OnPropertyChanged(nameof(PhoneNumberTypeNames));
            }
        }

        public PhoneNumberVM(PhoneNumber p)
        {
            SmsImageSource = ImageSource.FromResource("Busidex3.Resources.textmessage.png");
            PhoneImageSource = ImageSource.FromResource("Busidex3.Resources.phone.png");

            PhoneNumberId = p.PhoneNumberId;
            Number = p.Number;
            SelectedPhoneNumberType = p.PhoneNumberType?.Name;
            PhoneNumberTypeNames = getPhoneNumberTypes().Select(t => t.Name).ToList();
            
            DeletePhoneImage = ImageSource.FromResource("Busidex3.Resources.red_minus.png",
                typeof(ShareVM).GetTypeInfo().Assembly);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));  
        }

        public PhoneNumberType GetSelectedPhoneNumberType()
        {
            return getPhoneNumberTypes().SingleOrDefault(t => t.Name == SelectedPhoneNumberType);
        }

        private List<PhoneNumberType> getPhoneNumberTypes ()
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
