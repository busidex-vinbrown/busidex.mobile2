
namespace Busidex.Models.Dto
{
    public class SystemSettingsResponse
    {
        public bool Success { get; set; }
        public string StatusCode { get; set; }
        public SystemSettingsModel Model { get; set; }
    }
}
