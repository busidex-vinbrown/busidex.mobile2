using System.Net;
using System.Threading.Tasks;
using Busidex.Http.Utils;
using Busidex.Models.Domain;
using Busidex.Models.Dto;

namespace Busidex.Http
{
    public class EmailTemplateHttpService : BaseHttpService
    {
        public static async Task<EmailTemplateResponse> GetTemplate(EmailTemplateCode code)
        {
            string url = string.Format(ServiceUrls.EmailTemplateUrl, code);

            return await MakeRequestAsync<EmailTemplateResponse>(url, WebRequestMethods.Http.Get, code);
        }
    }
}
