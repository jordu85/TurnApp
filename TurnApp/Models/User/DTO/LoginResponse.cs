namespace TurnApp.Models.User.DTO
{
    public class LoginResponse
    {
        public string Token { get; set; } = null!;
        public UserDTO User { get; set; } = null!;
    }
}
