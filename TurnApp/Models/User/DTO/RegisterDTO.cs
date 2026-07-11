using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace TurnApp.Models.User.DTO
{
    public class RegisterDTO
    {
        [Required]
        [MinLength(7)]
        public string Dni { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(8)]
        [PasswordPropertyText]
        public string Password { get; set; } = null!;

        [Required]
        [MinLength(8)]
        [PasswordPropertyText]
        public string ConfirmPassword { get; set; } = null!;
    }
}
