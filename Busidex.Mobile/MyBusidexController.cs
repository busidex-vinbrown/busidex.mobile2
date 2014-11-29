using System.Threading.Tasks;

namespace Busidex.Mobile
{
	public class MyBusidexController : BaseController
	{

		public async Task<string> GetMyBusidex(string userToken){

			const string url = Resources.BASE_API_URL + "busidex?all=true";
			return await MakeRequest (url, "GET", userToken);
		}

		public Task<string> AddToMyBusidex(long cardId, string userToken){
			string url = Resources.BASE_API_URL + "busidex?userId=0&cId=" + cardId;

			return MakeRequest (url, "POST", userToken);
		}
	}
}

