using System.ComponentModel.DataAnnotations;

namespace Conduit.Models.Requests
{
    public class UpdateUser
    {
        [MaxLength(50, ErrorMessage = "User name max length is 50 characters.")]
        public string UserName { get; set; }

        [EmailAddress(ErrorMessage = "Email address must be valid.")]
        [MaxLength(50, ErrorMessage = "Email max length is 50 characters.")]
        public string Email { get; set; }

        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "Password must contain 1 Lower-case, 1 Upper-case and 1 Numeric character.")]
        public string Password { get; set; }

        [MaxLength(200, ErrorMessage = "Bio max length is 200 characters.")]
        public string Bio { get; set; }

        [MaxLength(200, ErrorMessage = "Image max length is 200 characters.")]
        public string Image { get; set; }
    }
}
