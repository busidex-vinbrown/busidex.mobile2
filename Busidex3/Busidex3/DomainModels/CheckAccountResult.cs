﻿namespace Busidex3.DomainModels
{
	public class CheckAccountResult
	{
		public bool Success{get;set;}
		public long UserId{get;set;}
		public string ReasonPhrase{get;set;}
	}
}