using System.ComponentModel.DataAnnotations;

namespace TurnApp.Models.Turno.DTO
{
    public class UpdateTurnoDTO
    {     
        public DateTime? FechaHora { get; set; }
        public string? EstadoTurno { get; set; } = null!;
    }
}
