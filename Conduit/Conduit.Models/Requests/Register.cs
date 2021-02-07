using System.ComponentModel.DataAnnotations;

namespace Conduit.Models.Requests
{
    public class Register
    {
        [Required(ErrorMessage = "User name is required.")]
        [MaxLength(50, ErrorMessage = "User name max length is 50 characters.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email address must be valid.")]
        [MaxLength(50, ErrorMessage = "Email max length is 50 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "Password must contain 1 Lower-case, 1 Upper-case and 1 Numeric character.")]
        public string Password { get; set; }
    }
}
