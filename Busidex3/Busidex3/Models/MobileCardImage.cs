using System;

namespace Busidex3.Models
{
	public class MobileCardImage
	{
		public enum DisplayMode
		{
			Front = 0,
			Back = 1
		}

		public Guid? FrontFileId { get; set; }
		public Guid? BackFileId { get; set; }
		public string Orientation { get; set; }
		public string EncodedCardImage { get; set; }
		public DisplayMode Side { get; set; }
	}
}

