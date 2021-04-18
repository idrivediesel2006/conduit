using System;

namespace Conduit.Models.Responses
{
    public class Comment
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Body { get; set; }
        public UserProfile Author { get; set; }

        public Comment()
        {
            Author = new UserProfile();
        }
    }
}