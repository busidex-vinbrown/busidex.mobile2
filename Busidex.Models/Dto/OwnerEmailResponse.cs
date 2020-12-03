using Busidex.Models.Domain;
using System.Collections.Generic;

namespace Busidex.Models.Dto
{
    public class UnownedCardResponse
    {
        public bool Success { get; set; }
        public string StatusCode { get; set; }
        public List<UnownedCard> Cards { get; set; }
    }
}
