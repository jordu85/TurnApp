using AutoMapper;
using System.Net;
using TurnApp.Enums;
using TurnApp.Models.Paciente;
using TurnApp.Models.Turno;
using TurnApp.Models.Turno.DTO;
using TurnApp.Repositories;
using TurnApp.Utils;

namespace TurnApp.Services
{
    public class TurnoService
    {

        private readonly IMapper _mapper;
        private readonly IRepository<Turno> _repo;
        private readonly PacienteService _pacService;
        private readonly ProfesionalService _profService;

        public TurnoService(IMapper mapper, PacienteService pacService, ProfesionalService profService, IRepository<Turno> repo)
        {
            _mapper = mapper;
            _repo = repo;
            _pacService = pacService;
            _profService = profService;
        }

        public async Task<List<TurnoDTO>> GetAll()
        {
            var lista = await _repo.GetAll();
            var turns = _mapper.Map<List<TurnoDTO>>(lista);

            return turns;
        }

        private async Task<Turno> _GetOneById(int id)
        {
            var turno = await _repo.GetOne(e => e.Id == id);

            if (turno == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.NotFound,
                    $"No se encontro turno con ID = {id}."
                );
            }

            return turno;
        }

        public async Task<Turno> GetOneById(int id) => await _GetOneById(id);

        public async Task<Turno> CreateOne(CreateTurnoDTO turn)
        {
            var turno = _mapper.Map<Turno>(turn);

            turno.EstadoTurno = ESTADOTURNO.Disponible;

            return await _repo.CreateOne(turno);
        }

        public async Task<Turno> UpdateOneById(int id, UpdateTurnoDTO updateDto)
        {
            var turn = await _GetOneById(id);

            var updated = _mapper.Map(updateDto, turn);

            return await _repo.UpdateOne(updated);
        }

        public async Task DeleteOneById(int id)
        {
            var turn = await _GetOneById(id);
            await _repo.DeleteOne(turn);
        }

        public async Task<List<Turno>> GetManyByIds(List<int> ids)
        {
            if (ids.Count == 0 || ids == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.BadRequest,
                    "La lista de TurnosIds no puede estar vacia"
                );
            }

            var lista = await _repo.GetAll(x => ids.Contains(x.Id));
            if (lista.Count == 0)
            {
                throw new ErrorResponse(
                    HttpStatusCode.BadRequest,
                    "No coincide ningun Id"
                );
            }
            return lista;
        }

        public async Task<List<TurnoDTO>> GetManyByIdsDto(List<int> ids)
        {
            if (ids.Count == 0 || ids == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.BadRequest,
                    "La lista de TurnosIds no puede estar vacia"
                );
            }

            var lista = await _repo.GetAll(x => ids.Contains(x.Id));
            if (lista.Count == 0)
            {
                throw new ErrorResponse(
                    HttpStatusCode.BadRequest,
                    "No coincide ningun Id"
                );
            }
            return _mapper.Map<List<TurnoDTO>>(lista);
        }

        public async Task<List<TurnoDTO>> GetTurnosByPacienteId(int id)
        {
            var pac = await _pacService.GetOneById(id);

            var turnos = await _repo.GetAll(t => t.PacienteId == pac.Id);
            return _mapper.Map<List<TurnoDTO>>(turnos);
            
        }

        public async Task<List<TurnoDTO>> GetTurnosByProfesionalId(int id)
        {
            var prof = await _profService.GetOneById(id);

            var turnos = await _repo.GetAll(t => t.PacienteId == prof.Id);
            return _mapper.Map<List<TurnoDTO>>(turnos);

        }

        public async Task<List<TurnoDTO>> GetTurnosPropios(HttpContext context)
        {
            var userId = context.User.FindFirst("UserId")?.Value;
            if(userId == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.Unauthorized,
                    "No se pudo obtener el UserId del token."
                    );
            }

            int pacienteId = int.Parse(userId);
            var turnos = await _repo.GetAll(t => t.PacienteId == pacienteId);
            return _mapper.Map<List<TurnoDTO>>(turnos);
        }

        public async Task<Turno> AsignarPacienteATurno(int turnoId, int pacienteId)
        {
            var turno = await _GetOneById(turnoId);
            if (turno.EstadoTurno != ESTADOTURNO.Disponible)
            {
                throw new ErrorResponse(
                    HttpStatusCode.BadRequest,
                    "Solo se pueden asignar turnos con estado Disponible.");
            }
            turno.PacienteId = pacienteId;
            return await _repo.UpdateOne(turno);
        }

    }
}
