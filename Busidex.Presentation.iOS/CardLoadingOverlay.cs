using System;
using System.Drawing;
using UIKit;

namespace Busidex.Presentation.iOS
{
	public class CardLoadingOverlay : LoadingOverlay
	{
		public CardLoadingOverlay  (CoreGraphics.CGRect frame) : base (frame)
		{
			init ();
		}

		public int TotalItems{ get; set; }

		float GetProgress(float current){
			if(current.Equals(0f)){
				return current;
			}
			return TotalItems.Equals (0f) ? 100f : (float)Math.Round (current / TotalItems, 2);
		}

		void init(){

			BackgroundColor = UIColor.LightGray;
			LoadingLabel.Text = MessageText;

		}
	}
}