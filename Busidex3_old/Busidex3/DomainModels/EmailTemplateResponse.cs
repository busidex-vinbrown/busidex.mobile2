namespace Busidex3.DomainModels
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

