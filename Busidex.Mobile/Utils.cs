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
			string jpgFilename = System.IO.Path.Combine (documentsPath, fileName);

			using (var webClient = new WebClient ()) {

				var imageData = webClient.DownloadDataTaskAsync (new Uri (imagePath));

				string localPath = Path.Combine (documentsPath, fileName);
				if (await imageData != null) {
					File.WriteAllBytes (localPath, imageData.Result); // writes to local storage  
				}

				return jpgFilename;
			}
		}

		public static string EncodeUserId(long userId){

			byte[] toEncodeAsBytes = System.Text.Encoding.ASCII.GetBytes(userId.ToString());
			string returnValue = Convert.ToBase64String(toEncodeAsBytes);
			return returnValue;
		}

		public static void SaveResponse(string response, string fileName){
			var fullFilePath = Path.Combine (Resources.DocumentsPath, fileName);
			File.WriteAllText (fullFilePath, response);
		}

		public static void RemoveCacheFiles(){
			var myBusidexLocalFile = Path.Combine (Resources.DocumentsPath, Resources.MY_BUSIDEX_FILE);
			if (File.Exists (myBusidexLocalFile)) {
				File.Delete (myBusidexLocalFile);
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

