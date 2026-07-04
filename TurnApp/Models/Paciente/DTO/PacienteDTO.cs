namespace TurnApp.Models.Paciente.DTO
{
    public class PacienteDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public DateTime FechaNacimiento { get; set; }
        public string NombreCompleto { get; set; } = null!;
    }
}
