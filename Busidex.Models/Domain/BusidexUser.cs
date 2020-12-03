using System;

namespace Busidex.Models.Domain
{
    public class BusidexUser
    {
        public AccountAddress Address { get; set; }

        public Settings Settings { get; set; }

        public string Email { get; set; }

        public string ApplicationId { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }

        public string LoweredUserName { get; set; }

        public string MobileAlias { get; set; }

        public bool IsAnonymous { get; set; }

        public DateTime LastActivityDate { get; set; }

        public string CardFileId { get; set; }

        public string CardFileType { get; set; }

        public UserAccount UserAccount { get; set; }
        public bool IsAdmin { get; set; }
    }
}

