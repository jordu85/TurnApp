using AutoMapper;
using System.Net;
using TurnApp.Models.Profesional;
using TurnApp.Models.Profesional.DTO;
using TurnApp.Models.Turno;
using TurnApp.Repositories;
using TurnApp.Utils;

namespace TurnApp.Services
{
    public class ProfesionalService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Profesional> _repo;
        private readonly EspecialidadService _espService;
        private readonly TurnoService _turnService;

        public ProfesionalService(IMapper mapper, EspecialidadService espService, TurnoService turnService, IRepository<Profesional> repo)
        {
            _mapper = mapper;
            _repo = repo;
            _espService = espService;
            _turnService = turnService;
        }

        public async Task<List<ProfesionalDTO>> GetAll()
        {
            var lista = await _repo.GetAll();
            var profs = _mapper.Map<List<ProfesionalDTO>>(lista);

            return profs;
        }

        private async Task<Profesional> _GetOneById(int id)
        {
            var profesional = await _repo.GetOne(e => e.Id == id);

            if (profesional == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.NotFound,
                    $"No se encontro profesional con ID = {id}"
                );
            }
            return profesional;
        }

        public async Task<ProfesionalDTO> GetOneById(int id)
        {
            var prof = await _GetOneById(id);
            var dto = _mapper.Map<ProfesionalDTO>(prof);
            return dto;
        }

        public async Task<Profesional> CreateOne(CreateProfesionalDTO prof)
        {
            var p = _mapper.Map<Profesional>(prof);

            var esps = await _espService.GetManyByIds(prof.EspecialidadesIds);
            p.Especialidades = esps;

            return await _repo.CreateOne(p);
        }

        public async Task<Profesional> UpdateOneById(int id, UpdateProfesionalDTO updateDto)
        {
            var prof = await _GetOneById(id);

            if (updateDto.EspecialidadesIds != null)
            {                
                List<int> espIds = updateDto.EspecialidadesIds;
                var especialidades = await _espService.GetManyByIds(espIds);
                prof.Especialidades = especialidades;
            }

            if (updateDto.TurnosIds != null)
            {
                List<int> turnIds = updateDto.TurnosIds;
                var turns = await _turnService.GetManyByIds(turnIds);
                prof.Turnos = turns;
            }

            var updated = _mapper.Map(updateDto, prof);

            return await _repo.UpdateOne(updated);
        }

        public async Task DeleteOneById(int id)
        {
            var emp = await _GetOneById(id);
            await _repo.DeleteOne(emp);
        }

    }
}
