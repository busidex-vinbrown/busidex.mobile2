using Busidex3.DomainModels;
using Busidex3.Services;
using System.Threading.Tasks;

namespace Busidex3.ViewModels
{
    public class QuickShareVM : BaseViewModel
    {
        public UserCard SelectedCard { get; set; }

        private string _greeting { get; set; }
        public string Greeting
        {
            get => _greeting;
            set
            {
                _greeting = value;
                OnPropertyChanged(nameof(Greeting));
            }
        }

        private string _personalMessage { get; set; }
        public string PersonalMessage
        {
            get => _personalMessage;
            set
            {
                _personalMessage = value;
                OnPropertyChanged(nameof(PersonalMessage));
            }
        }

        public async Task<bool> AddCardToMyBusidex(long cardId)
        {
            var service = new MyBusidexHttpService();
            return await service.AddToMyBusidex(cardId);
        }

        public async Task<bool> UpdateOwner(long cardId, long ownerId)
        {
            var service = new CardHttpService();
            return await service.UpdateCardOwner(cardId, ownerId);
        }
    }
}
