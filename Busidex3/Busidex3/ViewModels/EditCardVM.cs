using Busidex.Http;
using Busidex.Models.Domain;

namespace Busidex3.ViewModels
{
    public class EditCardVM : BaseViewModel
    {
        private readonly CardHttpService _cardHttpService = new CardHttpService();

        public UserCard SelectedCard { get; set; }
        
    }
}
