namespace Busidex3.DomainModels
{
	public class Tag
	{
		public Tag ()
		{
		}

		public long TagId{ get; set; }
		public string Text{ get; set; }
		public bool Deleted{ get; set; }
		public int TagType{ get; set; }
		public int TagTypeId{ get; set; }
	}
}

