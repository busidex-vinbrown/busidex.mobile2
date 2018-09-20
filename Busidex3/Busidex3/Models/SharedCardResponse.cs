using System.Collections.Generic;

namespace Busidex3.Models
{
	public class SharedCardResponse
	{
		public bool Success{ get; set; }
		public string StatusCode { get; set; }
		public List<SharedCard> SharedCards{ get; set; }
	}
}