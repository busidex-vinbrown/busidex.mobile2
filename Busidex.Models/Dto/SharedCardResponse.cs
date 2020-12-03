using Busidex.Models.Domain;
using System.Collections.Generic;

namespace Busidex.Models.Dto
{
    public class SharedCardResponse
    {
        public bool Success { get; set; }
        public string StatusCode { get; set; }
        public List<SharedCard> SharedCards { get; set; }
    }
}