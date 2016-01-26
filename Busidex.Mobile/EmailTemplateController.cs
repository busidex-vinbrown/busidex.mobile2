using System;
using System.Threading.Tasks;
using Busidex.Mobile.Models;

namespace Busidex.Mobile
{
	public class EmailTemplateController : BaseController
	{
		public EmailTemplateController ()
		{
		}

		public static async Task<string> GetTemplate(EmailTemplateCode code, string userToken){

			string url = Resources.BASE_API_URL + "EmailTemplate/Get?code=" + code;

			return await MakeRequestAsync (url, "GET", userToken, code,  new ModernHttpClient.NativeMessageHandler());
		}
	}
}

