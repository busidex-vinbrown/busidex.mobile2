using System;
using UIKit;

namespace Busidex.Presentation.iOS
{
	public class ButtonPanel : UIView
	{
		const float ANIMATION_SPEED = 0.5f;
		bool visible;

		public ButtonPanel(CoreGraphics.CGRect frame): base(frame){

		}

		public void Toggle(){

			UIView.Animate (ANIMATION_SPEED, () => {

				nfloat x = visible ? Frame.Width : 0;
				visible = !visible;
				Frame = new CoreGraphics.CGRect(x, Frame.Location.Y, Frame.Size.Width, Frame.Size.Height);
			});
		}

		public void Hide(){

			if (!visible)
				return;

			UIView.Animate (ANIMATION_SPEED, () => {
				Frame = new CoreGraphics.CGRect(Frame.Width, Frame.Location.Y, Frame.Size.Width, Frame.Size.Height);
				visible = false;
			});
		}
	}
}

