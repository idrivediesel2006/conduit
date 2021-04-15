using System;

namespace Conduit.Models.Exceptions
{
    public class ArticleNotFoundException : Exception
    {
        public ArticleNotFoundException(string message) : base(message)
        {

        }
    }
}
