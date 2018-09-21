namespace Busidex3.DomainModels
{
	public class LoginParams
	{
		public string UserName { get; set; }
		public string Password { get; set; }
		public string Token { get; set; }
		public bool RememberMe { get; set; }
		public bool AcceptSharedCards { get; set; }
		public string EventTag { get; set; }
	}
}

