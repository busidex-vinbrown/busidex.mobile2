namespace Busidex.Models.Domain
{
    public class MyBusidexResponse
    {
        public MyBusidexResponse()
        {
        }

        public bool Success { get; set; }
        public MyBusidexCollection MyBusidex { get; set; }
    }
}