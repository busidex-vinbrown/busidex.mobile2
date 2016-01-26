using System;

namespace Busidex.Mobile.Models
{
	public class EmailTemplateResponse
	{
		public EmailTemplateResponse ()
		{
		}

		public bool Success { get; set; }
		public EmailTemplate Template { get; set; }

	}
}

