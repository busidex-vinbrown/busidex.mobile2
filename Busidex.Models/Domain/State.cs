using System;

namespace Busidex.Models.Domain
{
    [Serializable]
    public class State
    {
        public State()
        {
        }

        public int StateCodeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

    }
}

