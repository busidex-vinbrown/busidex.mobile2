using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace Busidex.Mobile
{
	public class NotesController : BaseController
	{
		public NotesController ()
		{
		}

		public async Task<string> SaveNotes(long id, string notes, string userToken){

			string encodedNotes = System.Net.WebUtility.HtmlEncode (notes);
			string data = @"{'id':'" + id + "','notes':'" + encodedNotes + "'}";

			string url = Busidex.Mobile.Resources.BASE_API_URL + "Notes?id=" + id + "&notes=" + encodedNotes;

			return await MakeRequest (url, "PUT", userToken, data);
		}
	}
}