﻿namespace Busidex.Models.Domain
{
    public class SMSShare
    {
        public long FromUserId { get; set; }
        public long CardId { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
    }
}
