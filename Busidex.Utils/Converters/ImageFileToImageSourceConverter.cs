using Busidex.Http.Utils;
using System;
using System.Globalization;
using System.IO;
using Xamarin.Forms;

namespace Busidex.Utils.Converters
{
    public class ImageFileToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            var fileName = Path.Combine(Serialization.LocalStorageFolder, value.ToString());
            if (!File.Exists(fileName))
            {
                return null;
            }
            return ImageSource.FromFile(fileName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
