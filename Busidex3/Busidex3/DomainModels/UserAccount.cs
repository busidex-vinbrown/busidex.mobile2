using System;

namespace Busidex3.DomainModels
{
	public class UserAccount
	{
		public long UserAccountId{ get; set; }
		public long UserId{ get; set; }
		public int AccountTypeId{ get; set; }
		public DateTime Created{ get; set; }
		public bool Active{ get; set; }
		public string Notes{ get; set; }
		public string ActivationToken{ get; set; }
		public string DisplayName{ get; set; }
		public AccountType AccountType{ get; set; }
		public BusidexUser BusidexUser{ get; set; }
		public string ReferredBy{ get; set; }
	}
}

