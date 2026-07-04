using System.ComponentModel.DataAnnotations;

namespace TurnApp.Models.Especialidad.DTO
{
    public class CreateEspecialidadDTO
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = null!;
    }
}
