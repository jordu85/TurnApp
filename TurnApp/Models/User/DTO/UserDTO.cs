namespace TurnApp.Models.User.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Dni { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<string> Roles { get; set; } = new();
    }
}
