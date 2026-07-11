using System.ComponentModel.DataAnnotations;

namespace TurnApp.Models.Profesional.DTO
{
    public class CreateProfesionalDTO
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Apellido { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Matricula { get; set; } = null!;

        [Required]
        public int UsuarioId { get; set; }

        public List<int> EspecialidadesIds { get; set; } = new();
    }
}
