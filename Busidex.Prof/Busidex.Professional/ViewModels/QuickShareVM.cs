using Busidex.Http;
using Busidex.Models.Constants;
using Busidex.Models.Domain;
using Busidex.SharedUI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Forms;

namespace Busidex.Professional.ViewModels
{
    public class QuickShareVM : BaseViewModel
    {
        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            string name = HttpUtility.UrlDecode(query["name"]);
            string location = HttpUtility.UrlDecode(query["location"]);
        }

        private readonly CardHttpService _cardHttpService = new CardHttpService();


        public UserCard SelectedCard { get; set; }

        private string _greeting { get; set; }
        public string Greeting {
            get => _greeting;
            set {
                _greeting = value;
                OnPropertyChanged(nameof(Greeting));
            }
        }

        private string _personalMessage { get; set; }
        public string PersonalMessage {
            get => _personalMessage;
            set {
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
            var json = JsonConvert.SerializeObject(new {CardId = cardId, OwnerId = ownerId});
            return await _cardHttpService.UpdateCardOwner(json);
        }

        public IUserCardDisplay DisplaySettings { get; set; }

        public QuickShareVM(UserCard uc = null)
        {
            if(uc != null)
            {
                SelectedCard = uc;

                DisplaySettings = new UserCardDisplay(
                    DisplaySetting.Detail,
                    uc.Card.FrontOrientation == "H"
                        ? CardOrientation.Horizontal
                        : CardOrientation.Vertical,
                    uc.Card.FrontFileName,
                    uc.Card.FrontOrientation);
            }
            
        }
    }
}
