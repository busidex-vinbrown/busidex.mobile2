﻿using System;

namespace Busidex.Models.Domain
{
    [Serializable]
    public class ExternalLink
    {
        public int ExternalLinkId { get; set; }
        public long CardId { get; set; }
        public string Link { get; set; }
        public int ExternalLinkTypeId { get; set; }
        public virtual ExternalLinkType ExternalLinkType { get; set; }
    }
}
