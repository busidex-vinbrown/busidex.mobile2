using System;
using System.Collections.Generic;
using Busidex.Mobile.Models;

namespace Busidex.Mobile
{
	public class SearchModel
	{
		public SearchModel ()
		{
		}

		public long? UserId{ get; set; }
		public string Criteria{ get; set; }
		public string SearchText{ get; set; }
		public string SearchAddress{ get; set; }
		public long SearchLocation{ get; set; }
		public List<Card> Results{ get; set; }
	}
}