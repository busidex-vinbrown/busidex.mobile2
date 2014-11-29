using System.Drawing;
using UIKit;

namespace Busidex.Presentation.iOS
{
	public class LoadingOverlay : UIView {
		// control declarations
		UIActivityIndicatorView activitySpinner;
		protected UILabel LoadingLabel;

		protected float centerX;
		protected float centerY;
		protected float labelWidth;
		protected const float LABEL_HEIGHT = 22;

		public string MessageText{ get; set; }

		public LoadingOverlay (CoreGraphics.CGRect frame) : base (frame)
		{
			init ();
		}
			
		void init(){
			BackgroundColor = UIColor.Black;
			Alpha = 0.75f;
			AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;


			labelWidth = (float)Frame.Width - 20;

			// derive the center x and y
			centerX = (float)Frame.Width / 2;
			centerY = (float)Frame.Height / 2;

			// create the activity spinner, center it horizontall and put it 5 points above center x
			activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
			activitySpinner.Frame = new RectangleF (
				centerX - ((float)activitySpinner.Frame.Width / 2) ,
				centerY - (float)activitySpinner.Frame.Height - 20 ,
				(float)activitySpinner.Frame.Width ,
				(float)activitySpinner.Frame.Height);
			activitySpinner.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
			AddSubview (activitySpinner);
			activitySpinner.StartAnimating ();

			// create and configure the "Loading Data" label
			LoadingLabel = new UILabel(new RectangleF (
				centerX - (labelWidth / 2),
				centerY + 20 ,
				labelWidth ,
				LABEL_HEIGHT
			));
			LoadingLabel.BackgroundColor = UIColor.Clear;
			LoadingLabel.TextColor = UIColor.White;
			LoadingLabel.Text = MessageText;
			LoadingLabel.TextAlignment = UITextAlignment.Center;
			LoadingLabel.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
			AddSubview (LoadingLabel);
		}

		/// <summary>
		/// Fades out the control and then removes it from the super view
		/// </summary>
		public void Hide ()
		{
			UIView.Animate (
				0.5, // duration
				() => {
					Alpha = 0;
				},
				RemoveFromSuperview
			);
		}
	};
}

