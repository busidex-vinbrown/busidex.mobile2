﻿using System;

namespace Busidex.Mobile.Models
{
	public class LoginResponse
	{
		public LoginResponse ()
		{
		}
		public bool Success{get;set;}
		public string Message { get; set; }
		public long UserId{get;set;}
	}
}

