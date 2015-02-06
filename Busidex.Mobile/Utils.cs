using System;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace Busidex.Mobile
{
	public static class Utils
	{
		public static long DecodeUserId(string id){

			long userId = 0;

			try{
				byte[] raw = Convert.FromBase64String(id); 
				string s = System.Text.Encoding.UTF8.GetString(raw);
				long.TryParse(s, out userId);

			}catch(Exception ex){
				LoggingController.LogError (ex, id);
			}

			return userId;
		}

		public static async Task<string> DownloadImage(string imagePath, string documentsPath, string fileName)
		{
			string jpgFilename = Path.Combine (documentsPath, fileName);

			try{
				using (var webClient = new WebClient ()) {

					var imageData = webClient.DownloadDataTaskAsync (new Uri (imagePath));

					string localPath = Path.Combine (documentsPath, fileName);
					if (await imageData != null) {
						File.WriteAllBytes (localPath, imageData.Result); // writes to local storage  
					}
				}
			}
			catch(Exception ex){
				//return jpgFilename; 
			}
			return jpgFilename;
		}

		public static string EncodeUserId(long userId){

			byte[] toEncodeAsBytes = System.Text.Encoding.ASCII.GetBytes(userId.ToString());
			string returnValue = Convert.ToBase64String(toEncodeAsBytes);
			return returnValue;
		}

		public static void SaveResponse(string response, string fileName){
			var fullFilePath = Path.Combine (Resources.DocumentsPath, fileName);
			try{
				if(!IsFileInUse(new FileInfo(fullFilePath))){
					File.WriteAllText (fullFilePath, response);
				}
			}catch(Exception ex){
			}
		}

		static bool IsFileInUse(FileInfo file){
			FileStream stream = null;

			try
			{
				if(!File.Exists(file.FullName)){
					return false;
				}

				stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
			}
			catch (IOException ioEx)
			{
				//the file is unavailable because it is:
				//still being written to
				//or being processed by another thread
				//or does not exist (has already been processed)
				return true;
			}
			finally
			{
				if (stream != null)
					stream.Close();
			}
			return false; 
		}

		public static void RemoveCacheFiles(){
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
		}
	}
}

