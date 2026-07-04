using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace TurnApp.Models.User.DTO
{
    public class LoginDTO
    {
        [Required]
        [MinLength(7)]
        public string Dni { get; set; } = null!;

        [Required]
        [MinLength(8)]
        [PasswordPropertyText]
        public string Password { get; set; } = null!;
    }
}
