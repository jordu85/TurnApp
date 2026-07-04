using Microsoft.EntityFrameworkCore;
using TurnApp.Models;
using TurnApp.Models.Especialidad;
using TurnApp.Models.Paciente;
using TurnApp.Models.Profesional;
using TurnApp.Models.Role;
using TurnApp.Models.Turno;
using TurnApp.Models.User;

namespace TurnApp.Config
{
    public class AppDbContext :DbContext
    {         
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

        internal DbSet<Turno> Turnos { get; set; }
        internal DbSet<Profesional> Profesionales { get; set; }
        internal DbSet<Paciente> Pacientes { get; set; }
        internal DbSet<Especialidad> Especialidades { get; set; }
        internal DbSet<User> Users { get; set; }
        internal DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasIndex(x => x.Name).IsUnique();
            
            modelBuilder.Entity<Role>().HasData(
                new Role() { Id = 1, Name = "Admin" },
                new Role() { Id = 2, Name = "User" },
                new Role() { Id = 3, Name = "Mod" }
            );

            modelBuilder.Entity<Especialidad>().HasIndex(x => x.Nombre).IsUnique();

            modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();

            modelBuilder.Entity<User>().HasIndex(x => x.Dni).IsUnique();

            modelBuilder.Entity<Profesional>().HasIndex(x => x.Matricula).IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(x => x.Roles)
                .WithMany()
                .UsingEntity<RoleUser>(
                    l => l.HasOne<Role>().WithMany().HasForeignKey(x => x.RoleId),
                    r => r.HasOne<User>().WithMany().HasForeignKey(x => x.UserId)
                );

            modelBuilder.Entity<Profesional>()
                .HasMany(p => p.Especialidades)
                .WithMany()
                .UsingEntity<ProfesionalEspecialidad>(
                     l => l.HasOne<Especialidad>().WithMany().HasForeignKey(x => x.EspecialidadId),
                     r => r.HasOne<Profesional>().WithMany().HasForeignKey(x => x.ProfesionalId)
                );

            modelBuilder.Entity<Paciente>()
                .HasMany(p => p.Turnos)
                .WithOne(t => t.Paciente)
                .HasForeignKey(t => t.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Profesional>()
                .HasMany(p => p.Turnos)
                .WithOne(t => t.Profesional)
                .HasForeignKey(t => t.ProfesionalId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
