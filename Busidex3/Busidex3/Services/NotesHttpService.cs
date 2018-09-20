using System.Net.Http;
using System.Threading.Tasks;
using Busidex3.Services.Utils;

namespace Busidex3.Services
{
    public class NotesHttpService: BaseHttpService
    {
        public async Task<bool> SaveNotes(long id, string notes, string userToken){

            var encodedNotes = System.Net.WebUtility.HtmlEncode (notes);
            var data = @"{'id':'" + id + "','notes':'" + encodedNotes + "'}";

            var url = string.Format(ServiceUrls.UpdateNotesUrl, + id, encodedNotes);

            var resp = await MakeRequestAsync<HttpResponseMessage> (url, HttpVerb.Put, userToken, data);
            return resp.IsSuccessStatusCode;
        }
    }
}
