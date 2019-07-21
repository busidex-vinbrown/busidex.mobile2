using System.Collections.Generic;

namespace Busidex3.DomainModels
{
    public class UnownedCardResponse
    {
        public bool Success { get; set; }
        public string StatusCode { get; set; }
        public List<UnownedCard> Cards { get; set; }
    }
}
