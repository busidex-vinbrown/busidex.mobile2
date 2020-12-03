using System;

namespace Busidex.Models.Domain
{
    public class Settings
    {
        public long SettingsId { get; set; }
        public long UserId { get; set; }
        public int StartPage { get; set; }
        public DateTime Updated { get; set; }
        public bool AllowGoogleSync { get; set; }
        public bool Deleted { get; set; }
    }
}

