using Busidex.Models.Domain;

namespace Busidex.Models.Dto
{
    public class BitlyResponse
    {
        public BitlyData data { get; set; }
        public string status_code { get; set; }
        public string status_txt { get; set; }
    }
}

