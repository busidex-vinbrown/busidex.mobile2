using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Busidex.Mobile.Models;
using System.Drawing;
using System.Linq;
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

			var webClient = new WebClient ();
//			webClient.DownloadDataCompleted += (s, e) => {
//				var bytes = e.Result; // get the downloaded data
//
//				string localPath = Path.Combine (documentsPath, fileName);
//				if(bytes != null){
//					File.WriteAllBytes (localPath, bytes); // writes to local storage  
//				}
//			};

			var imageData = webClient.DownloadData (new Uri (imagePath));

			string localPath = Path.Combine (documentsPath, fileName);
			if(imageData != null){
				File.WriteAllBytes (localPath, imageData); // writes to local storage  
			}

			return jpgFilename;
		}

		public const string CARD_PATH =  "https://busidexcdn.blob.core.windows.net/cards/";//"https://az381524.vo.msecnd.net/cards/";
	}
}

