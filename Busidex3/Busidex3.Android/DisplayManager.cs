using Busidex3.Droid;
using Busidex3.ViewModels;
using Xamarin.Forms;

[assembly: Dependency(typeof(DisplayManager))]
namespace Busidex3.Droid
{
    public class DisplayManager : IDisplayManager
    {
        public IDisplayManager GetInstance()
        {
            return this;
        }

        public void SetOrientation(UserCardDisplay.CardOrientation orientation)
        {
            var main = Forms.Context as MainActivity;
            main.SetOrientation(orientation);
        }
    }
}