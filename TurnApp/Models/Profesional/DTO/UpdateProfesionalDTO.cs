using System.ComponentModel.DataAnnotations;

namespace TurnApp.Models.Profesional.DTO
{
    public class UpdateProfesionalDTO
    {
        public string? Nombre { get; set; } 
        public string? Apellido { get; set; }   
        public string? Matricula { get; set; }

        public List<int>? TurnosIds { get; set; } = new();
    }
}
