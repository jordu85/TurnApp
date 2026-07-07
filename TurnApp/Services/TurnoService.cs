using AutoMapper;
using System.Net;
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

        public TurnoService(IMapper mapper, IRepository<Turno> repo)
        {
            _mapper = mapper;
            _repo = repo;
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

    }
}
