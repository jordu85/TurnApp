using System.ComponentModel.DataAnnotations;

namespace TurnApp.Models.Turno.DTO
{
    public class CreateTurnoDTO
    {
        [Required]
        public DateTime FechaHora { get; set; }

        [Required]
        public int ProfesionalId { get; set; } 
    }
}
