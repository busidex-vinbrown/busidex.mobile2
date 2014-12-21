using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace Busidex.Mobile
{
	public class BaseController
	{


		protected static async Task<string> MakeRequestAsync(string url, string method, string token, string data = null){
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = method;

//			if(data != null){
//				var writer = new StreamWriter (request.GetRequestStream (), System.Text.Encoding.ASCII);
//				writer.Write (data);
//				request.ContentType = "application/json";
//				writer.Close ();
//			}
			request.Headers.Add ("X-Authorization-Token", token);

			if (method == "POST" || method == "PUT") {
				var requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
				if (data != null) {
					requestWriter.Write (data);
				} else {
					requestWriter.Write ("{}");
				}
				request.ContentType = "application/json";
				requestWriter.Close();
			}

			string response = string.Empty;
			try {
				await request.GetResponseAsync().ContinueWith(async r => {
					using (var webStream = r.Result.GetResponseStream()) {
						var responseReader = new StreamReader (webStream);
						response = responseReader.ReadToEnd();

						responseReader.Close();

						return response;
					}
				});
			} 
			catch(WebException e){
				if(e.Status == WebExceptionStatus.ProtocolError){
					response = e.Message;
				}else{
					throw new Exception (e.Message);
				}
			}
			catch (Exception e) {
				//NewRelic.NRLogger.Log ((uint)NewRelic.NRLogLevels.Error, e.Source, 14, "MakeRequest", e.Message);
				LoggingController.LogError (e, token);
				response = "Error";
			}
			return response;
		}

		protected static string MakeRequest(string url, string method, string token, string data = null){
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = method;

			if(data != null){
				var writer = new StreamWriter (request.GetRequestStream (), System.Text.Encoding.ASCII);
				writer.Write (data);
				request.ContentType = "application/json";
				writer.Close ();
			}
			request.Headers.Add ("X-Authorization-Token", token);

//			if (method == "POST") {
//				StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
//				requestWriter.Write("{}");
//				request.ContentType = "application/json";
//				requestWriter.Close();
//			}

			string response = string.Empty;
			try {
				var webResponse = request.GetResponse();

				using (var webStream = webResponse.GetResponseStream()) {
					var responseReader = new StreamReader (webStream);
					response = responseReader.ReadToEnd();

					responseReader.Close();

					return response;
				}
			} 
			catch(WebException e){
				if(e.Status == WebExceptionStatus.ProtocolError){
					response = e.Message;
				}else{
					throw new Exception (e.Message);
				}
			}
			catch (Exception e) {
				//NewRelic.NRLogger.Log ((uint)NewRelic.NRLogLevels.Error, e.Source, 14, "MakeRequest", e.Message);
				LoggingController.LogError (e, token);
				response = "Error";
			}
			return response;
		}
	}
}

