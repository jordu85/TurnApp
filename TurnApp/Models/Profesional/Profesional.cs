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

        public List<Turno.Turno> Turnos = new();

        public List<Especialidad.Especialidad> Especialidades = new();

        [ForeignKey(nameof(User))]
        public int UsuarioId { get; set; }

        public User.User user { get; set; } = null!;
    }
}
