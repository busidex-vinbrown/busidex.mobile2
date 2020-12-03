using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

using Busidex.Professional.Models;
using Busidex.Professional.Services;
using System.Threading.Tasks;

namespace Busidex.Professional.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();

        private const int VIEW_HEIGHT_HORIZONTAL = 590;
        private const int VIEW_HEIGHT_VERTICAL = 710;

        public virtual async Task<bool> Init(string cachedPath)
        {
            return await Task.FromResult(true);
        }

        bool isBusy = false;
        public bool IsBusy {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private string _searchValue;
        public string SearchValue {
            get => _searchValue;
            set {
                _searchValue = value;
                OnPropertyChanged(nameof(SearchValue));
            }
        }

        public void SetViewHeightForOrientation(string orientation)
        {
            ViewHeight = orientation == "H" ? VIEW_HEIGHT_HORIZONTAL : VIEW_HEIGHT_VERTICAL;
        }

        private int _viewHeight;
        public int ViewHeight {
            get => _viewHeight;
            set {
                _viewHeight = value;
                OnPropertyChanged(nameof(ViewHeight));
            }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
