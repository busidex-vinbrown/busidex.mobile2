using System;
using Busidex.Mobile.Models;
using System.Collections.Generic;

namespace Busidex.Mobile
{
	public class SharedCardResponse
	{
		public bool Success{ get; set; }
		public string StatusCode { get; set; }
		public List<SharedCard> SharedCards{ get; set; }
	}
}