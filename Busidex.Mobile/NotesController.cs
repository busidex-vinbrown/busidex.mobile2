using System.Threading.Tasks;

namespace Busidex.Mobile
{
	public class NotesController : BaseController
	{
		public async Task<string> SaveNotes(long id, string notes, string userToken){

			string encodedNotes = System.Net.WebUtility.HtmlEncode (notes);
			string data = @"{'id':'" + id + "','notes':'" + encodedNotes + "'}";

			string url = Resources.BASE_API_URL + "Notes?id=" + id + "&notes=" + encodedNotes;

			return await MakeRequestAsync (url, "PUT", userToken, data,  new ModernHttpClient.NativeMessageHandler());
		}
	}
}