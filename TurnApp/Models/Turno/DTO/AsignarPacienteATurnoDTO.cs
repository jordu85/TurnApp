using System.ComponentModel.DataAnnotations;

namespace TurnApp.Models.Turno.DTO
{
    public class AsignarPacienteATurnoDTO
    {
        [Required]
        public int PacienteId { get; set; }
    }
}
