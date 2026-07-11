using System.ComponentModel.DataAnnotations;

namespace TurnApp.Models.Profesional.DTO
{
    public class AsignarEspecialidadesAProfesionalDTO
    {
        [Required]
        public List<int> EspecialidadesIds { get; set; } = new();
    }
}