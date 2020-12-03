using System;

namespace Busidex.Models.Domain
{
    [Serializable]
    public class BusidexAuthFile
    {
        public BusidexAuthFile() { }
        public string Value { get; set; }
        public DateTime Expires { get; set; }

        public bool Expired => Expires < DateTime.Now;
    }
}
