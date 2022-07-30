using Busidex.Professional.iOS;
using Xamarin.Forms;
using Busidex.Models.Constants;

[assembly: Dependency(typeof(DisplayManager))]
namespace Busidex.Professional.iOS
{
    public class DisplayManager : IDisplayManager
    {
        public IDisplayManager GetInstance()
        {
            return this;
        }

        public void SetOrientation(CardOrientation orientation)
        {
           
        }
    }
}