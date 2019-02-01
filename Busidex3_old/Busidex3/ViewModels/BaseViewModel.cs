using System.ComponentModel;
using System.Runtime.CompilerServices;
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));          
        }
    }
}
