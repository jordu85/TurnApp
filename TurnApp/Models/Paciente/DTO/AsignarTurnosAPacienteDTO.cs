using System.ComponentModel.DataAnnotations;

namespace TurnApp.Models.Paciente.DTO
{
    public class AsignarTurnosAPacienteDTO
    {
        [Required]
        public List<int> TurnosIds { get; set; } = new();
    }
}
