using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Busidex3.Annotations;

namespace Busidex3.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private string _searchValue;

        private const int VIEW_HEIGHT_HORIZONTAL = 590;
        private const int VIEW_HEIGHT_VERTICAL = 710;

        public virtual async Task<bool> Init(string cachedPath)
        {
            return await Task.FromResult(true);
        }

        public void SetViewHeightForOrientation(string orientation)
        {
            ViewHeight = orientation == "H" ? VIEW_HEIGHT_HORIZONTAL : VIEW_HEIGHT_VERTICAL;
        }

        private int _viewHeight;
        public int ViewHeight
        {
            get => _viewHeight;
            set
            {
                _viewHeight = value;
                OnPropertyChanged(nameof(ViewHeight));
            }
        }

        public string SearchValue        
        {
            get => _searchValue;
            set {
                _searchValue = value;
                OnPropertyChanged(nameof(SearchValue));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));          
        }
    }
}
