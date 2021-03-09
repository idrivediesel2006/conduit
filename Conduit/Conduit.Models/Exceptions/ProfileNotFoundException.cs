using System;

namespace Conduit.Models.Exceptions
{
    public class ProfileNotFoundException : Exception
    {
        public ProfileNotFoundException(string message) : base(message)
        {

        }
    }
}
