using System.ComponentModel.DataAnnotations;

namespace TurnApp.Models.Paciente.DTO
{
    public class CrearPacienteDTO
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Apellido { get; set; } = null!;

        [Required]
        public DateTime FechaNacimiento { get; set; }

    }
}
