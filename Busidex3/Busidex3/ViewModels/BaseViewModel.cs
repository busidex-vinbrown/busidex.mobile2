using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Busidex3.Annotations;

namespace Busidex3.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private string _searchValue;

        public virtual async Task<bool> Init()
        {
            return await Task.FromResult(true);
        }

        public string SearchValue        
        {
            get => _searchValue;
            set => _searchValue = value;
        }

        protected async Task<string> DownloadImage (string imagePath, string documentsPath, string fileName)
        {
            ServicePointManager.Expect100Continue = false;

            if (imagePath.Contains (StringResources.NULL_CARD_ID)) {
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


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));          
        }
    }
}
