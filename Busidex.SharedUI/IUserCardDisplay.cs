using System.ComponentModel;
using Xamarin.Forms;

namespace Busidex.SharedUI
{
    public interface IUserCardDisplay
    {
        string CurrentFileName { get; set; }
        string FrontOrientation { get; set; }
        double HFrameHeight { get; set; }
        double HFrameWidth { get; set; }
        double HImageHeight { get; set; }
        double HImageWidth { get; set; }
        bool ShowCard { get; set; }
        Color ThumbnailBackground { get; set; }
        double VFrameHeight { get; set; }
        double VFrameWidth { get; set; }
        double VImageHeight { get; set; }
        double VImageWidth { get; set; }
        bool IsVisible { get; set; }
        event PropertyChangedEventHandler PropertyChanged;
    }
}