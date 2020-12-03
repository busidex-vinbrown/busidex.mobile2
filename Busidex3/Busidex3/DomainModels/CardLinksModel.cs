using System.Collections.Generic;

namespace Busidex3.DomainModels
{
    public class CardLinksModel
    {
        public long CardId { get; set; }
        public List<ExternalLink> Links { get; set; }
    }
}
