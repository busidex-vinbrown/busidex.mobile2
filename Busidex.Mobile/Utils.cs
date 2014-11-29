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


	}
}

