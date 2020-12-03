using System.IO;

namespace Busidex.Utils
{
    public interface IImageConverter
    {
        Stream GetContactImage(string imgPath);
    }
}
