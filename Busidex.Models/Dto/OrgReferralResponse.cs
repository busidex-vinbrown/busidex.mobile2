using Busidex.Models.Domain;
using System.Collections.Generic;

namespace Busidex.Models.Dto
{
    public class OrgReferralResponse
    {
        public bool Success { get; set; }
        public List<UserCard> Model { get; set; }

        public OrgReferralResponse()
        {
        }
    }
}

