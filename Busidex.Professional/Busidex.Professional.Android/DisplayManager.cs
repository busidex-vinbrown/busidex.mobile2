using Busidex.Models.Constants;
using Busidex.Professional.Droid;
using Plugin.CurrentActivity;
using Xamarin.Forms;

[assembly: Dependency(typeof(DisplayManager))]
namespace Busidex.Professional.Droid
{
    public class DisplayManager : IDisplayManager
    {
        public IDisplayManager GetInstance()
        {
            return this;
        }

        public void SetOrientation(CardOrientation orientation)
        {
            var main = CrossCurrentActivity.Current.Activity as MainActivity;

            main?.SetOrientation(orientation);
        }
    }
}