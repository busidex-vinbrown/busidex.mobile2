using System;
using System.Globalization;
using System.IO;
using Busidex3.Services.Utils;
using Xamarin.Forms;

namespace Busidex3.Converters
{
    public class ImageFileToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fileName = Path.Combine (Serialization.LocalStorageFolder, value.ToString());
            return ImageSource.FromFile(fileName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
