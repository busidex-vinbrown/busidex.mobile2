using Android.Content;
using Busidex3.Services.Utils;
using System;
using System.Globalization;
using System.IO;
using Xamarin.Forms;

namespace Busidex3.Converters
{
    public class UriToImageSourceConverter : IValueConverter
    {      
        const string DEFAULT_PROFILE = "defaultprofile.png";
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            var image = value.ToString();
            if(!string.IsNullOrEmpty(image) && Serialization.IsImageNameGuid(image))
            {
                var fileName = Path.Combine(Serialization.LocalStorageFolder, StringResources.THUMBNAIL_FILE_NAME_PREFIX + value.ToString());
                if (!File.Exists(fileName))
                {
                    return null;
                }
                return ImageSource.FromFile(fileName);
            }
            
            if (Device.RuntimePlatform == Device.Android && image != DEFAULT_PROFILE)
            {
                var uri = Android.Net.Uri.Parse(image);
                var stream = Android.App.Application.Context.ContentResolver.OpenInputStream(uri);

                return ImageSource.FromStream(() => stream);
            }
            if (Device.RuntimePlatform == Device.iOS && image != DEFAULT_PROFILE)
            {
                return ImageSource.FromUri(new Uri(image));
            }
            return ImageSource.FromResource("Busidex3.Resources." + DEFAULT_PROFILE);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }        
    }
}
