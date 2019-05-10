using System;
using Busidex3.ViewModels;
using Xamarin.Forms;

namespace Busidex3.Converters
{
    public class IsCardSideFrontConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(UserCardDisplay.CardSide.Front);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
}
