using System.Threading.Tasks;
using Busidex3.Analytics;
using Busidex3.DomainModels;
using Busidex3.Services;

namespace Busidex3.ViewModels
{
    public class EditCardVM : BaseViewModel
    {
        private readonly CardHttpService _cardHttpService = new CardHttpService();

        public UserCard SelectedCard { get; set; }

        private CardVisibility _visibility { get; set; }
        public CardVisibility Visibility { get => _visibility;
            set
            {
                _visibility = value;
                OnPropertyChanged(nameof(Visibility));
            }
        }
    }
}
