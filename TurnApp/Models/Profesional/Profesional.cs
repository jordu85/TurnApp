using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TurnApp.Models.Profesional
{
    public class Profesional
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Matricula { get; set; } = null!;

        public List<Turno.Turno> Turnos { get; set; } = new();
        public List<Especialidad.Especialidad> Especialidades { get; set; } = new();
        
        //[ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User.User User { get; set; } = null!;
    }
}
