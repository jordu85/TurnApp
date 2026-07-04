namespace TurnApp.Models.Turno.DTO
{
    public class TurnoDTO
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public string EstadoTurno { get; set; } = null!;
        public string PacienteNombreCompleto { get; set; } = null!;
        public string ProfesionalNombreCompleto { get; set; } = null!;


    }
}
