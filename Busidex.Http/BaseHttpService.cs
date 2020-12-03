using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Busidex.Http.Utils;
using Busidex.Models.Exceptions;
using Busidex.Resources.ContentTypes;
using Newtonsoft.Json;
using Plugin.Connectivity;

namespace Busidex.Http
{
    public class BaseHttpService
    {
        public static async Task<T> MakeRequestAsync<T>(string url, string method, object data = null) where T : new()
        {
            var response = new T();

            if (!IsConnected())
            {
                throw new ConnectionNotAvailableException();
            }

            try
            {

                var request = new HttpRequestMessage(new HttpMethod(method), url);
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                ServicePointManager.ServerCertificateValidationCallback += (sender, certificatePolicy, chain, sslPolicyErrors) => true;
                request.Method = new HttpMethod(method);
                HttpContent content;

                request.Headers.Add("x-authorization-token", Security.AuthToken);
                switch (method)
                {
                    case HttpVerb.Post:
                        content = new JsonContent(data);
                        content.Headers.Add("x-authorization-token", Security.AuthToken);
                        await httpClient.PostAsync(url, content).ContinueWith(async r =>
                        {
                            if (!r.IsFaulted)
                            {
                                using (var resp = await r)
                                {
                                    var responseContent = await resp.Content?.ReadAsStringAsync();
                                    response = string.IsNullOrEmpty(responseContent)
                                        ? new T()
                                        : JsonConvert.DeserializeObject<T>(responseContent);
                                }
                            }
                            else
                            {
                                throw new RequestFaultedException("Request to " + url + " faulted: " + r.Exception?.Message);
                            }
                        });
                        break;
                    case HttpVerb.Put:
                        content = new JsonContent(data);
                        content.Headers.Add("x-authorization-token", Security.AuthToken);
                        await httpClient.PutAsync(url, content).ContinueWith(async r =>
                        {
                            if (!r.IsFaulted)
                            {
                                using (var resp = await r)
                                {
                                    var responseContent = await resp.Content.ReadAsStringAsync();
                                    response = JsonConvert.DeserializeObject<T>(responseContent);
                                }
                            }
                            else
                            {
                                throw new RequestFaultedException("Request to " + url + " faulted: " + r.Exception?.Message);
                            }
                        });
                        break;
                    case HttpVerb.Delete:
                        httpClient.DefaultRequestHeaders.Add("x-authorization-token", Security.AuthToken);
                        await httpClient.DeleteAsync(url).ContinueWith(async r =>
                        {
                            if (!r.IsFaulted)
                            {
                                using (var resp = await r)
                                {
                                    var responseContent = await resp.Content.ReadAsStringAsync();
                                    response = string.IsNullOrEmpty(responseContent)
                                        ? new T()
                                        : JsonConvert.DeserializeObject<T>(responseContent);
                                }
                            }
                            else
                            {
                                throw new RequestFaultedException("Request to " + url + " faulted: " + r.Exception?.Message);
                            }
                        });
                        break;
                    default:
                        await httpClient.SendAsync(request).ContinueWith(async r =>
                        {
                            if (!r.IsFaulted)
                            {
                                using (var resp = await r)
                                {
                                    var responseContent = await resp.Content.ReadAsStringAsync();
                                    response = JsonConvert.DeserializeObject<T>(responseContent);
                                }
                            }
                            else
                            {
                                throw new RequestFaultedException("Request to " + url + " faulted: " + r.Exception?.Message);
                            }
                        });
                        break;
                }
            }
            catch (Exception e)
            {
                throw new HttpRequestException("Request Failed. See inner exception for details.", e);
            }

            return response;
        }

        private static bool IsConnected()
        {
            return CrossConnectivity.Current.IsConnected;
        }
    }
}

