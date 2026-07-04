using AutoMapper;
using TurnApp.Models.Paciente.DTO;
using TurnApp.Models.Paciente;
using TurnApp.Models.Role;
using TurnApp.Models.Role.DTO;
using TurnApp.Models.User;
using TurnApp.Models.User.DTO;
using TurnApp.Models.Profesional.DTO;
using TurnApp.Models.Profesional;
using TurnApp.Models.Especialidad.DTO;
using TurnApp.Models.Especialidad;
using TurnApp.Models.Turno.DTO;
using TurnApp.Models.Turno;

namespace TurnApp.Config
{
    public class Mapping : Profile
    {

        public Mapping()
        {
            //Tipos
            CreateMap<int?, int>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<bool?, bool>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<List<string>?, List<string>>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<List<int>?, List<int>>().ConvertUsing((src, dest) => src ?? dest);

            //Paciente
            CreateMap<Paciente, PacienteDTO>()
            .ForMember(d => d.NombreCompleto,
                o => o.MapFrom(s => s.Nombre + " " + s.Apellido))
            .ReverseMap();

            CreateMap<CrearPacienteDTO, Paciente>();

            CreateMap<UpdatePacienteDTO, Paciente>()
                .ForAllMembers(cfg => cfg.Condition((_, _, value) => value != null));

            //Profesional
            CreateMap<Profesional, ProfesionalDTO>()
            .ForMember(d => d.NombreCompleto,
                o => o.MapFrom(s => s.Nombre + " " + s.Apellido))
            .ForMember(d => d.Especialidades,
                o => o.MapFrom(s => s.Especialidades.Select(e => e.Nombre)))
            .ReverseMap();

            CreateMap<CreateProfesionalDTO, Profesional>();

            CreateMap<UpdateProfesionalDTO, Profesional>()
                .ForAllMembers(cfg => cfg.Condition((_, _, value) => value != null));


            //Especialidad
            CreateMap<Especialidad, EspecialidadDTO>()
           .ForMember(d => d.Profesionales,
               o => o.MapFrom(s => s.Profesionales.Select(p => p.Nombre + " " + p.Apellido)))
           .ReverseMap();

            CreateMap<CreateEspecialidadDTO, Especialidad>();

            CreateMap<UpdateEspecialidadDTO, Especialidad>()
                .ForAllMembers(cfg => cfg.Condition((_, _, value) => value != null));

            //Turnos
            CreateMap<Turno, TurnoDTO>()
           .ForMember(d => d.PacienteNombreCompleto,
               o => o.MapFrom(s => s.Paciente.Nombre + " " + s.Paciente.Apellido))
           .ForMember(d => d.ProfesionalNombreCompleto,
               o => o.MapFrom(s => s.Profesional.Nombre + " " + s.Profesional.Apellido));

            CreateMap<CreateTurnoDTO, Turno>();

            CreateMap<UpdateTurnoDTO, Turno>()
                .ForAllMembers(cfg => cfg.Condition((_, _, value) => value != null));


            // User
            CreateMap<User, UserDTO>().ForMember(
                dest => dest.Roles,
                opt => opt.MapFrom(src => src.Roles.Select(r => r.Name).ToList())
                );
            CreateMap<RegisterDTO, User>().ReverseMap();
            CreateMap<UpdateUserDTO, User>()
                    .ForAllMembers(cfg => cfg.Condition((_, _, value) => value != null));

            // Role
            CreateMap<Role, RoleDTO>().ReverseMap();
        }
    }
}
