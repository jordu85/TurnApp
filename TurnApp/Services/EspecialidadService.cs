using AutoMapper;
using System.Net;
using TurnApp.Models.Especialidad;
using TurnApp.Models.Especialidad.DTO;
using TurnApp.Repositories;
using TurnApp.Utils;
namespace TurnApp.Services
{
    public class EspecialidadService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Especialidad> _repo;

        public EspecialidadService(IMapper mapper, IRepository<Especialidad> repo)
        {
            _mapper = mapper;
            _repo = repo;
        }

        public async Task<List<Especialidad>> GetAll() => await _repo.GetAll();

        public async Task<Especialidad> GetOneById(int id)
        {
            var especialidad = await _repo.GetOne(x => x.Id == id);

            if (especialidad == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.NotFound,
                    $"No se encontro especialidad con ID = {id}"
                );
            }
            return especialidad;
        }

        public async Task<Especialidad> CreateOne(EspecialidadDTO esp)
        {
            var e = _mapper.Map<Especialidad>(esp);
            return await _repo.CreateOne(e);
        }

        public async Task<Especialidad> UpdateOneById(int id, EspecialidadDTO updateDto)
        {
            var esp = await GetOneById(id);

            var updated = _mapper.Map(updateDto, esp);

            return await _repo.UpdateOne(updated);
        }

        public async Task DeleteOneById(int id)
        {
            var esp = await GetOneById(id);
            await _repo.DeleteOne(esp);
        }

        public async Task<List<Especialidad>> GetManyByIds(List<int> ids)
        {
            if (ids.Count == 0 || ids == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.BadRequest,
                    "La lista de EspecialidadesIds no puede estar vacia"
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

    }
}
