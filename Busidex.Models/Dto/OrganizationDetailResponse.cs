using Busidex.Models.Domain;

namespace Busidex.Models.Dto
{
    public class OrganizationDetailResponse
    {
        public OrganizationDetailResponse()
        {
        }
        public bool Success { get; set; }
        public Organization Model { get; set; }
    }
}

