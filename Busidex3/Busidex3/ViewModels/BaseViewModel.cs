using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Busidex3.ViewModels
{
    public class BaseViewModel
    {
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
                Xamarin.Insights.Report (new Exception ("Error loading " + imagePath, ex));
            } finally {
                semaphore.Release ();
            }

            return jpgFilename;
        }

        protected void SaveResponse (string response, string fileName)
        {
            var fullFilePath = Path.Combine (Resources.DocumentsPath, fileName);
            try {
                if (File.Exists (fullFilePath)) {
                    if (!IsFileInUse (new FileInfo (fullFilePath))) {
                        File.WriteAllText (fullFilePath, response);
                    }
                } else {
                    File.WriteAllText (fullFilePath, response);
                }
            } catch (Exception ex) {
                Xamarin.Insights.Report (new Exception("Error in SaveResponse saving " + fileName + " with response " + response, ex));
            }
        }

        protected T GetCachedResult<T> (string fileName) where T : new()
        {
            try {
                if (!File.Exists (fileName)) {
                    return new T ();
                }

                using (var file = File.OpenText (fileName)) {
                    var fileJson = file.ReadToEnd ();
                    file.Close ();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T> (fileJson);
                }
            } catch (IOException) {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return new T ();
            }
        }

        private static bool IsFileInUse (FileInfo file)
        {
            FileStream stream = null;

            try {
                if (!File.Exists (file.FullName)) {
                    return false;
                }

                stream = file.Open (FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            } catch (IOException) {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            } finally {
                if (stream != null)
                    stream.Close ();
            }
            return false;
        }
    }
}
