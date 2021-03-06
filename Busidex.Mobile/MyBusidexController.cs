﻿using System.Threading.Tasks;
using System.Net.Http;

namespace Busidex.Mobile
{
	public class MyBusidexController : BaseController
	{

		public async Task<string> GetMyBusidex(string userToken){

			const string url = Resources.BASE_API_URL + "busidex?all=true";
			return await MakeRequestAsync (url, "GET", userToken, null, new ModernHttpClient.NativeMessageHandler());
		}

		public Task<string> AddToMyBusidex(long cardId, string userToken){
			string url = Resources.BASE_API_URL + "busidex?userId=0&cId=" + cardId;

			return MakeRequestAsync (url, "POST", userToken, null, new ModernHttpClient.NativeMessageHandler());
		}

		public Task<string> RemoveFromMyBusidex(long cardId, string userToken){
			string url = Resources.BASE_API_URL + "busidex?id=" + cardId + "&userId=0";

			return MakeRequestAsync (url, "DELETE", userToken, null, new ModernHttpClient.NativeMessageHandler());
		}
	}
}

