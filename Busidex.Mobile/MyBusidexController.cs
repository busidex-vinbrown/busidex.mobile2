using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace Busidex.Mobile
{
	public class MyBusidexController : BaseController
	{
		public MyBusidexController ()
		{
		}

		public Task<string> GetMyBusidex(string userToken){

			const string url = Busidex.Mobile.Resources.BASE_API_URL + "busidex?all=true";
			return MakeRequest (url, "GET", userToken);
		}

		public Task<string> AddToMyBusidex(long cardId, string userToken){
			string url = Busidex.Mobile.Resources.BASE_API_URL + "busidex?userId=0&cId=" + cardId;

			return MakeRequest (url, "POST", userToken);
		}


	}
}

