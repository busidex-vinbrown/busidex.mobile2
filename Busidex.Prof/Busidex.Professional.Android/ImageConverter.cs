using Busidex.Professional.Droid;
using Busidex.Utils;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(ImageConverter))]
namespace Busidex.Professional.Droid
{
    public class ImageConverter : IImageConverter
    {
        public ImageConverter()
        {

        }
        public Stream GetContactImage(string imgPath)
        {
            var uri = Android.Net.Uri.Parse(imgPath);//content://com.android.contacts/contacts/8379/photo
            var stream = Android.App.Application.Context.ContentResolver.OpenInputStream(uri);

            return stream;
        }
    }
}