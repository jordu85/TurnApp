using AutoMapper;
using System.Net;
using System.Security.Claims;
using TurnApp.Enums;
using TurnApp.Models.Paciente;
using TurnApp.Models.Paciente.DTO;
using TurnApp.Models.Turno;
using TurnApp.Models.Turno.DTO;
using TurnApp.Repositories;
using TurnApp.Utils;

namespace TurnApp.Services
{
    public class PacienteService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Paciente> _repo;
        private readonly TurnoService _turnService;

        public PacienteService(IMapper mapper, TurnoService turnService, IRepository<Paciente> repo)
        {
            _mapper = mapper;
            _repo = repo;
            _turnService = turnService;
        }

        public async Task<List<PacienteDTO>> GetAll()
        {
            var lista = await _repo.GetAll();
            var pacs = _mapper.Map<List<PacienteDTO>>(lista);

            return pacs;
        }

        private async Task<Paciente> _GetOneById(int id)
        {
            var paciente = await _repo.GetOne(e => e.Id == id);

            if (paciente == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.NotFound,
                    $"No se encontro paciente con ID = {id}"
                );
            }
            return paciente;
        }

        public async Task<PacienteDTO> GetOneById(int id)
        {
            var pac = await _GetOneById(id);
            var dto = _mapper.Map<PacienteDTO>(pac);
            return dto;
        }

        public async Task<Paciente> CreateOne(CreatePacienteDTO pac, HttpContext context)
        {
            var p = _mapper.Map<Paciente>(pac);

            var user = context.User.Claims.FirstOrDefault(claim => claim.Type == "id");
            bool ok = int.TryParse(user?.Value, out int id);

            if (!ok)
            {
                throw new ErrorResponse(
                    HttpStatusCode.BadRequest,
                    "Token invalido."
                    );
            }
            p.UserId = id;

            return await _repo.CreateOne(p);
        }

        public async Task<Paciente> UpdateOneById(int id, UpdatePacienteDTO updateDto)
        {
            var pac = await _GetOneById(id);

            var updated = _mapper.Map(updateDto, pac);

            return await _repo.UpdateOne(updated);
        }

        //public async Task<Paciente> AsignarTurnosAPaciente(int id, AsignarTurnosAPacienteDTO asign)
        //{
        //    var pac = await _GetOneById(id);

        //    List<int> Ids = asign.TurnosIds;
        //    var turnos = await _turnService.GetManyByIds(Ids);

        //    var turnosNoDisponibles = turnos.Where(t => t.EstadoTurno != ESTADOTURNO.Disponible).ToList();
        //    if (turnosNoDisponibles.Count > 0)
        //    {
        //        throw new ErrorResponse(
        //            HttpStatusCode.BadRequest,
        //            "Solo se pueden asignar turnos con estado Disponible."
        //        );
        //    }

        //    pac.Turnos = turnos;

        //    return await _repo.UpdateOne(pac);
        //}

        //public async Task<List<TurnoDTO>> GetTurnosByPacienteId(int id)
        //{
        //    var pac = await _GetOneById(id);

        //    List<int> Ids = new();

        //    foreach(var t in pac.Turnos)
        //    {
        //        Ids.Add(t.Id);
        //    }

        //    List<TurnoDTO> turnos = await _turnService.GetManyByIdsDto(Ids);
        //    return turnos;
        //}

        //public async Task<List<TurnoDTO>> GetTurnosPropios(HttpContext context)
        //{
        //    var user = context.User.Claims.FirstOrDefault(claim => claim.Type == "id");
        //    bool ok = int.TryParse(user?.Value, out int id);

        //    if (!ok)
        //    {
        //        throw new ErrorResponse(
        //            HttpStatusCode.BadRequest,
        //            "Token invalido."
        //            );
        //    }

        //    var pac = await _repo.GetOne(x => x.UserId == id);

        //    if (pac == null)
        //    {
        //        throw new ErrorResponse(
        //            HttpStatusCode.NotFound,
        //            $"No se encontro paciente con UserId = {id}"
        //            );
        //    }

        //    List<int> Ids = new();

        //    foreach (var t in pac.Turnos)
        //    {
        //        Ids.Add(t.Id);
        //    }

        //    List<TurnoDTO> turnos = await _turnService.GetManyByIdsDto(Ids);
        //    return turnos;
        //}
        public async Task DeleteOneById(int id)
        {
            var pac = await _GetOneById(id);
            await _repo.DeleteOne(pac);
        }

    }
}
