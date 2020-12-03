using System;
using System.Collections.Generic;

namespace Busidex.Models.Domain
{
    public class User
    {
        public string UserName { get; set; }
        public long UserId { get; set; }
        public bool IsAdmin { get; set; }
        public bool HasCard { get; set; }
        public long UserAccountId { get; set; }
        public int AccountTypeId { get; set; }
        public long CardId { get; set; }
        public string Token { get; set; }
        public string StartPage { get; set; }
        public List<Tuple<string, long>> Organizations { get; set; }
        public string DisplayName { get; set; }
        public bool? OnboardingComplete { get; set; }
        public Guid? ActivationToken { get; set; }
    }
}
