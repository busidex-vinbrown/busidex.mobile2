using System;
using System.Collections.Generic;

namespace Busidex.Mobile.Models
{
	public class MyBusidexResponse
	{
		public MyBusidexResponse ()
		{
		}

		public bool Success{ get; set; }
		public MyBusidexCollection MyBusidex{ get; set; }
	}
}