using System.ComponentModel.DataAnnotations;

namespace TurnApp.Models.User.DTO
{
    public class UpdateUserDTO
    {
        [EmailAddress]
        public string? Email { get; set; }
    }
}
