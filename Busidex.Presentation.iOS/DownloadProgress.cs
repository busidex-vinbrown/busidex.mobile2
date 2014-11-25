using System;

namespace Busidex.Presentation.iOS
{
	public class DownloadProgress
	{
		public DownloadProgress(string fileName, float bytesReceived, float totalBytes)
		{
			ItemsReceived = bytesReceived;
			Total = totalBytes;
		}

		public float Total { get; private set; }

		public float ItemsReceived { get; private set; }

		public float PercentComplete { get { return (float)ItemsReceived / Total; } }

		public bool IsFinished { get { return ItemsReceived == Total; } }
	}
}

