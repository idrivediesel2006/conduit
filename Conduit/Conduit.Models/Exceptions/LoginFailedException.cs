using System;

namespace Conduit.Models.Exceptions
{
    public class LoginFailedException : Exception
    {
        public LoginFailedException(string message) : base(message)
        {

        }
    }
}
