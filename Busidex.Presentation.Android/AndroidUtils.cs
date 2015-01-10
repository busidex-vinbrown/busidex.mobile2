using Android.Graphics;

namespace Busidex.Presentation.Android
{
	public static class AndroidUtils
	{

		static int calculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight) {
			// Raw height and width of image
			int height = options.OutHeight;
			int width = options.OutWidth;
			int inSampleSize = 2;

			if (height > reqHeight || width > reqWidth) {

				int halfHeight = height / 2;
				int halfWidth = width / 2;

				// Calculate the largest inSampleSize value that is a power of 2 and keeps both
				// height and width larger than the requested height and width.
				while ((halfHeight / inSampleSize) > reqHeight
					&& (halfWidth / inSampleSize) > reqWidth) {
					inSampleSize *= 2;
				}
			}

			return inSampleSize;
		}

		public static Bitmap DecodeSampledBitmapFromFile(string fileName,
			int reqWidth, int reqHeight) {

			// First decode with inJustDecodeBounds=true to check dimensions
			var options = new BitmapFactory.Options();
			options.InScaled = true;
			options.InJustDecodeBounds = true;
			BitmapFactory.DecodeFile(fileName, options);

			// Calculate inSampleSize
			options.InSampleSize = calculateInSampleSize(options, reqWidth, reqHeight);

			// Decode bitmap with inSampleSize set
			options.InJustDecodeBounds = false;
			options.InScaled = true;
			return BitmapFactory.DecodeFile(fileName, options);
		}
	}
}

