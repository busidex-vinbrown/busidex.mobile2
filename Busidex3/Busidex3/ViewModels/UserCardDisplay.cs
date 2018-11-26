using System.ComponentModel;
using System.Runtime.CompilerServices;
using Busidex3.Annotations;
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

        public UserCardDisplay(DisplaySetting display = DisplaySetting.Detail)
        {

            var scale = display == DisplaySetting.Thumbnail 
                ? .5 
                : 1;
            Init(scale);
        }

        private void Init(double scale)
        {
            VFrameHeight = 300 * scale;
            VFrameWidth = 183 * scale;
            VImageHeight = 250 * scale;
            VImageWidth = 152 * scale;

            HFrameHeight = 183 * scale;
            HFrameWidth = 300 * scale;
            HImageHeight = 152 * scale;
            HImageWidth = 250 * scale;

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
