using System;

namespace Busidex.Mobile.Models
{
	public class EmailTemplate
	{
		public EmailTemplate ()
		{
		}

		public int EmailTemplateId { get; set; }
		public string Code { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
	}
}

