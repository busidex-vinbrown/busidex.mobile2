using System;

namespace Busidex.Models.Domain
{
    [Serializable]
    public class ExternalLinkType
    {
        public int ExternalLinkTypeId { get; set; }
        public string LinkType { get; set; }
    }
}
