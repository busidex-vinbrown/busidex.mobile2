using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using Xamarin;

namespace Busidex.Mobile
{
	public class BaseController
	{
		const string ERROR_MESSAGE = "Error";

		protected static async Task<string> MakeRequestAsync (string url, string method, string token, object data = null, HttpMessageHandler handler = null)
		{
            
			string response = string.Empty;

			try {
				var request = new HttpRequestMessage (new HttpMethod (method), url);
				var httpClient = handler == null ? new HttpClient () : new HttpClient (handler);
				httpClient.DefaultRequestHeaders.Accept.Add (new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue ("application/json"));

				ServicePointManager.ServerCertificateValidationCallback += (sender, certificatePolicy, chain, sslPolicyErrors) => true;
				request.Method = new HttpMethod (method);
				HttpContent content;

				request.Headers.Add ("x-authorization-token", token);
				switch (method) {
				case "POST":
					content = new JsonContent (data);
					content.Headers.Add ("x-authorization-token", token);
					await httpClient.PostAsync (url, content).ContinueWith (async r => {
						if (!r.IsFaulted) {
						    using (var resp = await r)
						    {
						        response = await resp.Content.ReadAsStringAsync ();
						    }
						}
					});
					break;
				case "PUT":
					content = new JsonContent (data);
					content.Headers.Add ("x-authorization-token", token);
					await httpClient.PutAsync (url, content).ContinueWith (async r => {
						if (!r.IsFaulted) {
						    using (var resp = await r)
						    {
						        response = await resp.Content.ReadAsStringAsync ();
						    }
						}
					});
					break;
				case "DELETE":
					httpClient.DefaultRequestHeaders.Add ("x-authorization-token", token);
					await httpClient.DeleteAsync (url).ContinueWith (async r => {
						if (!r.IsFaulted) {
						    using (var resp = await r)
						    {
						        response = await resp.Content.ReadAsStringAsync ();
						    }
						}
					});
					break;
				default:
					await httpClient.SendAsync (request).ContinueWith (async r => {
						if (!r.IsFaulted) {
						    using (var resp = await r)
						    {
						        response = await resp.Content.ReadAsStringAsync ();
						    }
						}
					});
					break;
				}
			} catch (AggregateException e) {
				response = Newtonsoft.Json.JsonConvert.SerializeObject (new CheckAccountResult {
					Success = false,
					UserId = -1,
					ReasonPhrase = e.InnerException?.Message
				});
				Insights.Report (e);
			} catch (Exception e) {
				LoggingController.LogError (e, token);
				response = Newtonsoft.Json.JsonConvert.SerializeObject (new CheckAccountResult {
					Success = false,
					UserId = -1,
					ReasonPhrase = e.Message
				});
				Insights.Report (e);
			}
			return response;
		}

		protected static string MakeRequest (string url, string method, string token, string data = null, HttpMessageHandler handler = null)
		{


			url = url.Replace ("https", "http");
			//var httpClient = handler == null ? new HttpClient() : new HttpClient(handler);


			var request = (HttpWebRequest)WebRequest.Create (url);//new HttpRequestMessage (new HttpMethod (method), url);

			request.Method = method;

			//			if(!NetworkInterface.GetIsNetworkAvailable()){
			//				return ERROR_MESSAGE;
			//			}

			if (data != null) {
				var writer = new StreamWriter (request.GetRequestStream (), System.Text.Encoding.ASCII);
				writer.Write (data);
				request.ContentType = "application/json";
				//request.Headers.Add ("ContentType", "application/json");
				//request.Content = new JsonContent(data);
				writer.Close ();
			}
			request.Headers.Add ("x-authorization-token", token);

			//			if (method == "POST") {
			//				StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
			//				requestWriter.Write("{}");
			//				request.ContentType = "application/json";
			//				requestWriter.Close();
			//			}



			string response = string.Empty;
			string results = string.Empty;
			try {
				//				await httpClient.SendAsync (request).ContinueWith(async r => {
				//					response = await r;
				//
				//					await response.Content.ReadAsStringAsync().ContinueWith(rr => {
				//						return rr.Result;
				//					});
				//
				//				});

				var webResponse = request.GetResponse ();

				using (var webStream = webResponse.GetResponseStream ()) {
					var responseReader = new StreamReader (webStream);
					response = responseReader.ReadToEnd ();

					responseReader.Close ();

					return response;
				}
			} catch (WebException e) {
				if (e.Status == WebExceptionStatus.ProtocolError) {
					results = e.Message;
				} else {
					if (e.Message.Contains ("NameResolutionFailure")) {
						results = ERROR_MESSAGE;
					} else if (e.Message.Contains ("ConnectFailure")) {
						results = ERROR_MESSAGE;
					} else {
						throw new Exception (e.Message);
					}
				}
				Insights.Report (e);
				return results;
			} catch (Exception e) {
				//NewRelic.NRLogger.Log ((uint)NewRelic.NRLogLevels.Error, e.Source, 14, "MakeRequest", e.Message);
				//LoggingController.LogError (e, token);
				results = ERROR_MESSAGE;
				Insights.Report (e);
				return results;
			}

			//return results;
		}

		protected static async Task<string> MakeExternalReequest (string url, string method, object data = null)
		{
			var request = new HttpRequestMessage (new HttpMethod (method), url);
			var httpClient = new HttpClient ();

			string response = string.Empty;

			request.Method = new HttpMethod (method);

			try {

				await httpClient.SendAsync (request).ContinueWith (async r => {
					var _response = await r;

					response = await _response.Content.ReadAsStringAsync ();
				});
			} catch (Exception e) {
				response = Newtonsoft.Json.JsonConvert.SerializeObject (new CheckAccountResult {
					Success = false,
					UserId = -1,
					ReasonPhrase = e.Message
				});
				Insights.Report (e);
			}
			return response;
		}
	}
}

