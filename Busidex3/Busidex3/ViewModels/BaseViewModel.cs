using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Busidex3.ViewModels
{
    public class BaseViewModel
    {

        public virtual async Task<bool> Init()
        {
            return await Task.FromResult(true);
        }
        
        protected async Task<string> DownloadImage (string imagePath, string documentsPath, string fileName)
        {
            ServicePointManager.Expect100Continue = false;

            if (imagePath.Contains (Resources.NULL_CARD_ID)) {
                return string.Empty;
            }

            var semaphore = new SemaphoreSlim (1, 1);
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
                //Xamarin.Insights.Report (new Exception ("Error loading " + imagePath, ex));
            } finally {
                semaphore.Release ();
            }

            return jpgFilename;
        }
        
        
        
    }
}
