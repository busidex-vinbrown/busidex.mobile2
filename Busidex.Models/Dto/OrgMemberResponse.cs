using Busidex.Models.Domain;
using System.Collections.Generic;

namespace Busidex.Models.Dto
{
    public class OrgMemberResponse
    {
        public bool Success { get; set; }
        public List<Card> Model { get; set; }

        public OrgMemberResponse()
        {
        }
    }
}

