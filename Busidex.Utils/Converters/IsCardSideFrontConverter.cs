using System;
using Busidex.Models.Constants;
using Xamarin.Forms;

namespace Busidex.Utils.Converters
{
    public class IsCardSideFrontConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(CardSide.Front);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
}
