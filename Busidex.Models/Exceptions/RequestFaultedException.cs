using System;

namespace Busidex.Models.Exceptions
{
    public class RequestFaultedException : Exception
    {
        public RequestFaultedException(string message)
            : base(message)
        {

        }
    }
}
