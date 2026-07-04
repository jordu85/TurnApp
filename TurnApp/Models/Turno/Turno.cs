using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TurnApp.Enums;

namespace TurnApp.Models.Turno
{
    public class Turno
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime FechaHora { get; set; } 

        public string EstadoTurno { get; set; } = null!;
        
        public int PacienteId { get; set; }
        public Paciente.Paciente Paciente { get; set; } = null!;
       
        public int ProfesionalId { get; set; }
        public Profesional.Profesional Profesional { get; set; } = null!;
    }
}
