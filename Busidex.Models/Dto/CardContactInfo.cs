using Busidex.Models.Domain;
using System;
using System.Collections.Generic;

namespace Busidex.Models.Dto
{
    [Serializable]
    public class CardContactInfo
    {
        public long CardId { get; set; }
        public List<PhoneNumber> PhoneNumbers { get; set; }
        public string Url { get; set; }
        public string Email { get; set; }
    }
}
