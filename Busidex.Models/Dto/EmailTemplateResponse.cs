using Busidex.Models.Domain;

namespace Busidex.Models.Dto
{
    public class EmailTemplateResponse
    {
        public EmailTemplateResponse()
        {
        }

        public bool Success { get; set; }
        public EmailTemplate Template { get; set; }

    }
}

