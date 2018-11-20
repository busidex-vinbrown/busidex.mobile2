using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Busidex3.Annotations;
using Microsoft.AppCenter.Crashes;

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

            if (string.IsNullOrEmpty(imagePath) || imagePath.Contains (StringResources.NULL_CARD_ID)) {
                return string.Empty;
            }

            var semaphore = new SemaphoreSlim (1, 1);
            await semaphore.WaitAsync ();

            var jpgFilename = Path.Combine (documentsPath, fileName);

            try {
                using (var webClient = new WebClient ()) {

                    var imageData = await webClient.DownloadDataTaskAsync (new Uri (imagePath));

                    var localPath = Path.Combine (documentsPath, fileName);
                    if (imageData != null) {
                        File.WriteAllBytes (localPath, imageData); // writes to local storage  
                    }
                }
            } catch (Exception ex) {
                Crashes.TrackError(new Exception ("Error loading " + imagePath, ex));
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
