using System;
using UIKit;
using CoreGraphics;
using System.Drawing;

namespace Busidex.Presentation.iOS
{
	public static class UIImageExtension
	{
		public static UIImage Crop (this UIImage sourceImage, float crop_x, float crop_y, float width, float height)
		{
			var imgSize = sourceImage.Size;
			UIGraphics.BeginImageContext (new SizeF (width, height));
			var context = UIGraphics.GetCurrentContext ();
			var clippedRect = new RectangleF (0, 0, width, height);
			context.ClipToRect (clippedRect);
			var drawRect = new RectangleF (-crop_x, -crop_y, (float)imgSize.Width, (float)imgSize.Height);
			sourceImage.Draw (drawRect);
			var modifiedImage = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return modifiedImage;
		}

		public static UIImage ScaleAndRotateImage (this UIImage image)
		{
			int kMaxResolution = 1024 / 4; // Or whatever

			CGImage imgRef = image.CGImage;
			float width = imgRef.Width;
			float height = imgRef.Height;
			CGAffineTransform transform = CGAffineTransform.MakeIdentity ();
			RectangleF bounds = new RectangleF (0, 0, width, height);

			if (width > kMaxResolution || height > kMaxResolution) {
				float ratio = width / height;

				if (ratio > 1) {
					bounds.Size = new SizeF (kMaxResolution, bounds.Size.Width / ratio);
				} else {
					bounds.Size = new SizeF (bounds.Size.Height * ratio, kMaxResolution);
				}
			}

			float scaleRatio = bounds.Size.Width / width;
			SizeF imageSize = new SizeF (imgRef.Width, imgRef.Height);
			UIImageOrientation orient = image.Orientation;
			float boundHeight;

			switch (orient) {
			case UIImageOrientation.Up:                                        //EXIF = 1
				transform = CGAffineTransform.MakeIdentity ();
				break;
			// TODO: Add other Orientations
			case UIImageOrientation.Right:                                     //EXIF = 8
				boundHeight = bounds.Size.Height;
				bounds.Size = new SizeF (boundHeight, bounds.Size.Width);
				transform = CGAffineTransform.MakeTranslation (imageSize.Height, 0);
				transform = CGAffineTransform.Rotate (transform, (float)Math.PI / 2.0f);
				break;
			default:
				throw new Exception ("Invalid image orientation");                        

			}

			UIGraphics.BeginImageContext (bounds.Size);

			CGContext context = UIGraphics.GetCurrentContext ();

			if (orient == UIImageOrientation.Right || orient == UIImageOrientation.Left) {
				context.ScaleCTM (-scaleRatio, scaleRatio);
				context.TranslateCTM (-height, 0);
			} else {
				context.ScaleCTM (scaleRatio, -scaleRatio);
				context.TranslateCTM (0, -height);
			}

			context.ConcatCTM (transform);

			context.DrawImage (new RectangleF (0, 0, width, height), imgRef);
			UIImage imageCopy = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();

			return imageCopy;
		}
	}
}

