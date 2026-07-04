using System.ComponentModel.DataAnnotations;

namespace TurnApp.Models.Especialidad.DTO
{
    public class EspecialidadDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public List<string> Profesionales { get; set; } = new();
    }
}
