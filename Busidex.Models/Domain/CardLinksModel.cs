using System.Collections.Generic;

namespace Busidex.Models.Domain
{
    public class CardLinksModel
    {
        public long CardId { get; set; }
        public List<ExternalLink> Links { get; set; }
    }
}
