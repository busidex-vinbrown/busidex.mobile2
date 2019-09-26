using System.Collections.Generic;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Models;
using Busidex3.Services.Utils;

namespace Busidex3.ViewModels
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
