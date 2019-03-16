using Busidex3.DomainModels;
using Busidex3.Services;

namespace Busidex3.ViewModels
{
    public class EditCardVM : BaseViewModel
    {
        private readonly CardHttpService _cardHttpService = new CardHttpService();

        public UserCard SelectedCard { get; set; }
        
    }
}
