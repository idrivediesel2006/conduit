using System.ComponentModel.DataAnnotations;

namespace Conduit.Models.Requests
{
    public class Login
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email address must be valid.")]
        [StringLength(50, ErrorMessage = "Email max length is 50 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
