using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Busidex3.Annotations;
using Busidex3.Services.Utils;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Busidex3.ViewModels
{
    public class UserCardDisplay : INotifyPropertyChanged
    {
        public enum DisplaySetting
        {
            Detail,
            Thumbnail,
            FullScreen
        }

        public enum CardOrientation
        {
            Horizontal,
            Vertical
        }

        public enum CardSide
        {
            Front,
            Back
        }

        public double VFrameHeight { get; set; }
        public double VFrameWidth { get; set; }
        public double VImageHeight { get; set; }
        public double VImageWidth { get; set; }


        public double HFrameHeight { get; set; }
        public double HFrameWidth { get; set; }
        public double HImageHeight { get; set; }
        public double HImageWidth { get; set; }

        public Color ThumbnailBackground { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly DisplaySetting _currentDisplaySetting;
        private readonly CardOrientation _currentOrientation;
        public string CurrentFileName { get; set; }

        public UserCardDisplay(
            DisplaySetting display = DisplaySetting.Detail, 
            CardOrientation orientation = CardOrientation.Horizontal,
            string fileName = ""
            )
        {
            _currentDisplaySetting = display;
            _currentOrientation = orientation;
            CurrentFileName = fileName;

            var filePath = Path.Combine(Serialization.LocalStorageFolder, CurrentFileName);
            if(!File.Exists(filePath))
            {
                var imageUrl = (display == DisplaySetting.FullScreen
                                   ? StringResources.CARD_PATH
                                   : StringResources.THUMBNAIL_PATH) + CurrentFileName;

                App.DownloadImage(imageUrl, Serialization.LocalStorageFolder, filePath)
                    .ContinueWith(result => { OnPropertyChanged(nameof(CurrentFileName)); });
            }
            var scale = display == DisplaySetting.Thumbnail
                ? .5
                : 1;
            Init(scale);
        }

        private void Init(double scale)
        {
            if (_currentDisplaySetting == DisplaySetting.FullScreen)
            {
                VFrameHeight = HFrameHeight = DeviceDisplay.MainDisplayInfo.Height;
                VImageHeight = HImageHeight = VFrameHeight - 10;
                VFrameWidth = HFrameWidth = DeviceDisplay.MainDisplayInfo.Width;

                App.DisplayManager.SetOrientation(_currentOrientation);
            }
            else
            {
                VFrameHeight = 300 * scale;
                VFrameWidth = 183 * scale;
                VImageHeight = 250 * scale;
                VImageWidth = 152 * scale;

                HFrameHeight = 183 * scale;
                HFrameWidth = 300 * scale;
                HImageHeight = 152 * scale;
                HImageWidth = 250 * scale;
            }
            

            OnPropertyChanged(nameof(VFrameHeight));
            OnPropertyChanged(nameof(VFrameWidth));
            OnPropertyChanged(nameof(VImageHeight));
            OnPropertyChanged(nameof(VImageWidth));
            OnPropertyChanged(nameof(HFrameHeight));
            OnPropertyChanged(nameof(HFrameWidth));
            OnPropertyChanged(nameof(HImageHeight));
            OnPropertyChanged(nameof(HImageWidth));

            ThumbnailBackground = Color.White;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
