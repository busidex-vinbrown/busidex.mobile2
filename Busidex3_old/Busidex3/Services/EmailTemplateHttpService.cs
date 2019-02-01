using System.Net;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;

namespace Busidex3.Services
{
    public class EmailTemplateHttpService : BaseHttpService
    {
        public static async Task<EmailTemplate> GetTemplate(EmailTemplateCode code)
        {
            string url = string.Format(ServiceUrls.EmailTemplateUrl, code);

            return await MakeRequestAsync<EmailTemplate> (url, WebRequestMethods.Http.Get, code);
        }
    }
}
