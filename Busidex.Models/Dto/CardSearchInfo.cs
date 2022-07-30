using System;

namespace Busidex.Models.Dto
{
    [Serializable]
    public class CardSearchInfo
    {
        public long CardId { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string Title { get; set; }
    }
}
