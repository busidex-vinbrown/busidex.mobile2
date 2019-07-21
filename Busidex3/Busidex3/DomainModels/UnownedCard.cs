using System;

namespace Busidex3.DomainModels
{
    public class UnownedCard : Card
    {
        public DateTime? LastContactDate { get; set; }
        public string EmailSentTo { get; set; }
    }
}
