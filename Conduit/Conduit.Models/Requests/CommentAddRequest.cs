using System.ComponentModel.DataAnnotations;

namespace Conduit.Models.Requests
{
    public class CommentAddRequest
    {
        [Required]
        [StringLength(200, ErrorMessage = "Body max character length is 200.")]
        public string Body { get; set; }
    }
}