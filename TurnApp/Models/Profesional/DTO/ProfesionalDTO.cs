namespace TurnApp.Models.Profesional.DTO
{
    public class ProfesionalDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Matricula { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public List<int> EspecialidadesIds { get; set; } = new();
    }
}
