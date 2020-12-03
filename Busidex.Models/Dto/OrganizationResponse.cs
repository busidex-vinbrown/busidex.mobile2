using Busidex.Models.Domain;
using System.Collections.Generic;

namespace Busidex.Models.Dto
{
    public class OrganizationResponse
    {
        public OrganizationResponse()
        {
        }
        public bool Success { get; set; }
        public List<Organization> Model { get; set; }
    }
}

