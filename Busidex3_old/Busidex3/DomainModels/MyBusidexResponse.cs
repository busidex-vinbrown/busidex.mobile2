namespace Busidex3.DomainModels
{
	public class MyBusidexResponse
	{
		public MyBusidexResponse ()
		{
		}

		public bool Success{ get; set; }
		public MyBusidexCollection MyBusidex{ get; set; }
	}
}