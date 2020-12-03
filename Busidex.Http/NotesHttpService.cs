using System.Threading.Tasks;
using Busidex.Http.Utils;
using Busidex.Models.Dto;

namespace Busidex.Http
{
    public class NotesHttpService : BaseHttpService
    {
        public async Task<SaveNotesResponse> SaveNotes(long id, string notes)
        {

            var encodedNotes = System.Net.WebUtility.HtmlEncode(notes);
            var data = @"{'id':'" + id + "','notes':'" + encodedNotes + "'}";

            var url = string.Format(ServiceUrls.UpdateNotesUrl, +id, encodedNotes);

            return await MakeRequestAsync<SaveNotesResponse>(url, HttpVerb.Put, data);
        }
    }
}
