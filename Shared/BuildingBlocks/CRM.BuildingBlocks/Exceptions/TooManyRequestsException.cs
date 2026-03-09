using System;

namespace CRM.BuildingBlocks.Exceptions
{
    public class TooManyRequestsException : Exception
    {
        public TooManyRequestsException() : base("TOO_MANY_REQUESTS") { }
        public TooManyRequestsException(string message) : base(message) { }
    }
}
