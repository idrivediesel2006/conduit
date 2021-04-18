using System.ComponentModel.DataAnnotations;

namespace Conduit.Models.Requests
{
    public class ArticleUpdateRequest
    {
        [Required]
        [StringLength(50, ErrorMessage = "Title max character length is 50.")]
        public string Title { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Description max character length is 50.")]
        public string Description { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Body max character length is 200.")]
        public string Body { get; set; }
    }
}