using System;

namespace Conduit.Models.Exceptions
{
    public class UserExistException : Exception
    {
        public UserExistException(string message) : base(message)
        {

        }
    }
}
