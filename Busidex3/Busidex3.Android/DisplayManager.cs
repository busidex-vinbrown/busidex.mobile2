using Busidex3.Droid;
using Busidex3.ViewModels;
using Plugin.CurrentActivity;
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
            var main = CrossCurrentActivity.Current.Activity as MainActivity;

            main?.SetOrientation(orientation);
        }
    }
}