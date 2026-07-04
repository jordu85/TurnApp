using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;



namespace TurnApp.Models.Paciente
{
    public class Paciente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public DateTime FechaNacimiento { get; set; }     
        public int UserId { get; set; }
        public User.User User { get; set; } = null!;
        public List<Turno.Turno> Turnos { get; set; } = new ();
    }
}
