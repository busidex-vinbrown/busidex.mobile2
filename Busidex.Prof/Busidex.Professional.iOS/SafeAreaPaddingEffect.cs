
using Busidex.Professional.iOS.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("Busidex3")]
[assembly: ExportEffect(typeof(SafeAreaPaddingEffect), nameof(SafeAreaPaddingEffect))]
namespace Busidex.Professional.iOS.Effects
{
    class SafeAreaPaddingEffect : PlatformEffect
    {
        Thickness _padding;
        protected override void OnAttached()
        {
            if (Element is Layout element)
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
                {
                    _padding = element.Padding;
                    var insets = UIApplication.SharedApplication.Windows[0].SafeAreaInsets; // Can't use KeyWindow this early
                    if (insets.Top > 0) // We have a notch
                    {
                        element.Padding = new Thickness(_padding.Left + insets.Left, _padding.Top + insets.Top, _padding.Right + insets.Right, _padding.Bottom);
                        return;
                    }
                }
                // Uses a default Padding of 20. Could use an property to modify if you wanted.
                element.Padding = new Thickness(_padding.Left, _padding.Top + 20, _padding.Right, _padding.Bottom);
            }
        }

        protected override void OnDetached()
        {
            if (Element is Layout element)
            {
                element.Padding = _padding;
            }
        }
    }
}