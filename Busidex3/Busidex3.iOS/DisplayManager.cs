using Busidex.Models.Constants;
using Busidex3.iOS;
using Busidex3.ViewModels;
using Xamarin.Forms;

[assembly: Dependency(typeof(DisplayManager))]
namespace Busidex3.iOS
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