using Busidex3.DomainModels;
using Xamarin.Forms;

namespace Busidex3.ViewModels
{
    public class PhoneNumberVM
    {
        public PhoneNumberType PhoneNumberType { get; set; }
        public string Number { get; set; }

        public ImageSource SmsImageSource { get; }
        public ImageSource PhoneImageSource { get; }

        public PhoneNumberVM(PhoneNumber p)
        {
            SmsImageSource = ImageSource.FromResource("Busidex3.Resources.textmessage.png");
            PhoneImageSource = ImageSource.FromResource("Busidex3.Resources.phone.png");
            
            Number = p.Number;
            PhoneNumberType = p.PhoneNumberType;
        }
    }
}
