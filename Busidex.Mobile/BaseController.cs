using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Busidex.Mobile
{
	public class BaseController
	{
		const string ERROR_MESSAGE = "Error";


		protected static async Task<string> MakeRequestAsync(string url, string method, string token, string data = null){
			var request = (HttpWebRequest)WebRequest.Create(url);
			string response = string.Empty;

			request.Method = method;

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


			try {
				await request.GetResponseAsync().ContinueWith(r => {
					using (var webStream = r.Result.GetResponseStream()) {
						var responseReader = new StreamReader (webStream);
						response = responseReader.ReadToEnd();

						responseReader.Close();

						return response;
					}
				});
			} 

			catch(System.AggregateException e){
				response = Newtonsoft.Json.JsonConvert.SerializeObject (new CheckAccountResult {
					Success = false,
					UserId = -1,
					ReasonPhrase = e.InnerException.Message
				});
			}
			catch (Exception e) {
				//NewRelic.NRLogger.Log ((uint)NewRelic.NRLogLevels.Error, e.Source, 14, "MakeRequest", e.Message);
				LoggingController.LogError (e, token);
				response = Newtonsoft.Json.JsonConvert.SerializeObject (new CheckAccountResult {
					Success = false,
					UserId = -1,
					ReasonPhrase = e.Message
				});
			}
			return response;
		}

		protected static string MakeRequest(string url, string method, string token, string data = null){
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = method;

//			if(!NetworkInterface.GetIsNetworkAvailable()){
//				return ERROR_MESSAGE;
//			}
				
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
					if (e.Message.Contains ("NameResolutionFailure")) {
						response = ERROR_MESSAGE;
					} else {
						throw new Exception (e.Message);
					}
				}
			}
			catch (Exception e) {
				//NewRelic.NRLogger.Log ((uint)NewRelic.NRLogLevels.Error, e.Source, 14, "MakeRequest", e.Message);
				LoggingController.LogError (e, token);
				response = ERROR_MESSAGE;
			}
			return response;
		}
	}
}

