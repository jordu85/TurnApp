using System.ComponentModel.DataAnnotations;

namespace TurnApp.Models.Paciente.DTO
{
    public class UpdatePacienteDTO
    {
        public string? Nombre { get; set; } 
        public string? Apellido { get; set; }     
        public DateTime? FechaNacimiento { get; set; }
    }
}
