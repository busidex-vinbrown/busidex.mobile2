﻿using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;

namespace Busidex3.Services
{
    public class LoginHttpService : BaseHttpService
    {
        public async Task<User> DoLogin(string email, string password){
		
            var model = new LoginParams {
                UserName = email,
                Password = password, 
                Token = string.Empty,
                RememberMe = true,
                EventTag = string.Empty
            };
            string url = ServiceUrls.LoginUrl;
            return await MakeRequestAsync<User> (url, HttpVerb.Post, model);
        }
    }
}
