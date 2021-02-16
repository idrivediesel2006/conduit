using System;
using System.Collections.Generic;

namespace Conduit.Models.Exceptions
{
    public static class ExceptionHelper
    {
        public static Dictionary<string, object> ToDictionary(this Exception ex)
        {
            var returnVal = new Dictionary<string, object>();
            returnVal.Add("message", ex.Message);
            return returnVal;
        }
    }
}
