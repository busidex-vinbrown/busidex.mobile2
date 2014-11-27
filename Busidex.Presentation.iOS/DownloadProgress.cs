
namespace Busidex.Presentation.iOS
{
	public class DownloadProgress
	{
		public DownloadProgress(float bytesReceived, float totalBytes)
		{
			ItemsReceived = bytesReceived;
			Total = totalBytes;
		}

		public float Total { get; private set; }

		public float ItemsReceived { get; private set; }

		public float PercentComplete { get { return Total.Equals(0f) ? 100f : ItemsReceived / Total; } }

		public bool IsFinished { get { return ItemsReceived.Equals(Total); } }
	}
}