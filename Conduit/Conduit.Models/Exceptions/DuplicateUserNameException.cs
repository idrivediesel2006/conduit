using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conduit.Models.Exceptions
{
    public class DuplicateUserNameException : Exception
    {
        public DuplicateUserNameException(string message) : base(message)
        {

        }
    }
}
