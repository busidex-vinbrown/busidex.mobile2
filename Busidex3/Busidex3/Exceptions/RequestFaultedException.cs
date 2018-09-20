using System;

namespace Busidex3.Exceptions
{
    public class RequestFaultedException : Exception
    {
        public RequestFaultedException(string message)
            : base(message)
        {

        }
    }
}
