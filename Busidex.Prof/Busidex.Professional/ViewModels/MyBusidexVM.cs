using Busidex.Http.Utils;
using Busidex.Models.Domain;
using Busidex.Models.Dto;
using Busidex.Resources.String;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Busidex.Professional.ViewModels
{
    public class MyBusidexVM : CardListVM
    {
        public override async Task<List<UserCard>> GetCards()
        {
            var result = await _myBusidexHttpService.GetMyBusidex();
            return result.MyBusidex.Busidex;
        }

        public override void SaveCardsToFile(string json)
        {
            Serialization.SaveResponse(json, StringResources.MY_BUSIDEX_FILE);
            Serialization.SetDataRefreshDate(RefreshItem.MyBusidex);
        }
    }
}
