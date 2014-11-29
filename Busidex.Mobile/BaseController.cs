using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace Busidex.Mobile
{
	public class BaseController
	{
		public BaseController ()
		{
		}


		protected static async Task<string> MakeRequest(string url, string method, string token, string data = null){
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = method;

			if(data != null){
				var writer = new StreamWriter (request.GetRequestStream (), System.Text.Encoding.ASCII);
				writer.Write (data);
				request.ContentType = "application/json";
				writer.Close ();
			}
			request.Headers.Add ("X-Authorization-Token", token);

			if (method == "POST") {
				StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
				requestWriter.Write("{}");
				request.ContentType = "application/json";
				requestWriter.Close();
			}

			string response = string.Empty;
			try {
				var webResponse = await request.GetResponseAsync();

				using (var webStream = webResponse.GetResponseStream()) {
					var responseReader = new StreamReader (webStream);
					response = responseReader.ReadToEnd();

					responseReader.Close();

					return response;
				}


			} 
			catch(System.Net.WebException e){
				if(e.Status == WebExceptionStatus.ProtocolError){
					response = e.Message;
				}else{
					throw new Exception (e.Message);
				}
			}
			catch (Exception e) {
				//NewRelic.NRLogger.Log ((uint)NewRelic.NRLogLevels.Error, e.Source, 14, "MakeRequest", e.Message);
				response = "Error";
			}
			return response;
		}
	}
}

