using System;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Collections.Concurrent;

namespace Busidex.Mobile
{
	public static class Utils
	{
		static readonly ConcurrentDictionary<string, SemaphoreSlim> locks = new ConcurrentDictionary<string, SemaphoreSlim> ();

		public static long DecodeUserId (string id)
		{

			long userId = 0;

			try {
				byte[] raw = Convert.FromBase64String (id); 
				string s = System.Text.Encoding.UTF8.GetString (raw);
				long.TryParse (s, out userId);

			} catch (Exception ex) {
				LoggingController.LogError (ex, id);
			}

			return userId;
		}

		public static async Task<string> DownloadImage (string imagePath, string documentsPath, string fileName)
		{
			if (imagePath.Contains ("00000000-0000-0000-0000-000000000000")) {
				return string.Empty;
			}

			var semaphore = locks.GetOrAdd (imagePath, new SemaphoreSlim (1, 1));
			await semaphore.WaitAsync ();

			string jpgFilename = Path.Combine (documentsPath, fileName);

			try {
				using (var webClient = new WebClient ()) {

					var imageData = webClient.DownloadDataTaskAsync (new Uri (imagePath));

					string localPath = Path.Combine (documentsPath, fileName);
					if (await imageData != null) {
						File.WriteAllBytes (localPath, imageData.Result); // writes to local storage  
					}
				}
			} catch (Exception ex) {
				Xamarin.Insights.Report (new Exception ("Error loading " + imagePath, ex));
			} finally {
				semaphore.Release ();
			}

			return jpgFilename;
		}

		public static string EncodeUserId (long userId)
		{

			byte[] toEncodeAsBytes = System.Text.Encoding.ASCII.GetBytes (userId.ToString (CultureInfo.InvariantCulture));
			string returnValue = Convert.ToBase64String (toEncodeAsBytes);
			return returnValue;
		}

		public static void SaveResponse (string response, string fileName)
		{
			var fullFilePath = Path.Combine (Resources.DocumentsPath, fileName);
			try {
				if (File.Exists (fullFilePath)) {
					if (!IsFileInUse (new FileInfo (fullFilePath))) {
						File.WriteAllText (fullFilePath, response);
					}
				} else {
					File.WriteAllText (fullFilePath, response);
				}
			} catch (Exception ex) {
				Xamarin.Insights.Report (ex);
			}
		}

		public static void RemoveQuickShareLink ()
		{
			var quickShareFile = Path.Combine (Resources.DocumentsPath, Resources.QUICKSHARE_LINK);
			if (File.Exists (quickShareFile)) {
				File.Delete (quickShareFile);
			}
		}

		public static QuickShareLink GetQuickShareLink ()
		{

			try {
				string fullFileName = Path.Combine (Resources.DocumentsPath, Resources.QUICKSHARE_LINK);
				if (!File.Exists (fullFileName)) {
					return null;
				}

				using (var file = File.OpenText (fullFileName)) {
					var fileJson = file.ReadToEnd ();
					file.Close ();
					var quickShareLink = Newtonsoft.Json.JsonConvert.DeserializeObject<QuickShareLink> (fileJson);
					UISubscriptionService.AppQuickShareLink = quickShareLink;
					return quickShareLink;
				}
			} catch (IOException) {
				//the file is unavailable because it is:
				//still being written to
				//or being processed by another thread
				//or does not exist (has already been processed)
				return null;
			}
		}

		public static T GetCachedResult<T> (string fileName) where T : new()
		{
			try {
				if (!File.Exists (fileName)) {
					return new T ();
				}

				using (var file = File.OpenText (fileName)) {
					var fileJson = file.ReadToEnd ();
					file.Close ();
					return Newtonsoft.Json.JsonConvert.DeserializeObject<T> (fileJson);
				}
			} catch (IOException) {
				//the file is unavailable because it is:
				//still being written to
				//or being processed by another thread
				//or does not exist (has already been processed)
				return new T ();
			}		
		}

		static bool IsFileInUse (FileInfo file)
		{
			FileStream stream = null;

			try {
				if (!File.Exists (file.FullName)) {
					return false;
				}

				stream = file.Open (FileMode.Open, FileAccess.ReadWrite, FileShare.None);
			} catch (IOException) {
				//the file is unavailable because it is:
				//still being written to
				//or being processed by another thread
				//or does not exist (has already been processed)
				return true;
			} finally {
				if (stream != null)
					stream.Close ();
			}
			return false; 
		}

		public static void RemoveCacheFiles ()
		{
			var myBusidexLocalFile = Path.Combine (Resources.DocumentsPath, Resources.MY_BUSIDEX_FILE);
			if (File.Exists (myBusidexLocalFile)) {
				File.Delete (myBusidexLocalFile);
			}

			var mySharedCardsFile = Path.Combine (Resources.DocumentsPath, Resources.SHARED_CARDS_FILE);
			if (File.Exists (mySharedCardsFile)) {
				File.Delete (mySharedCardsFile);
			}

			var myOrganizationsLocalFile = Path.Combine (Resources.DocumentsPath, Resources.MY_ORGANIZATIONS_FILE);
			if (File.Exists (myOrganizationsLocalFile)) {
				File.Delete (myOrganizationsLocalFile);
			}

			var organizationMembersLocalFile = Path.Combine (Resources.DocumentsPath, Resources.ORGANIZATION_MEMBERS_FILE);
			if (File.Exists (organizationMembersLocalFile)) {
				File.Delete (organizationMembersLocalFile);
			}

			RemoveQuickShareLink ();
		}
	}
}

