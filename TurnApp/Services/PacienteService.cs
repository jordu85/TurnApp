using AutoMapper;
using System.Net;
using TurnApp.Models.Paciente;
using TurnApp.Models.Paciente.DTO;
using TurnApp.Models.Turno;
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

        public async Task<Paciente> CreateOne(CreatePacienteDTO prof)
        {
            var p = _mapper.Map<Paciente>(prof);

            return await _repo.CreateOne(p);
        }

        public async Task<Paciente> UpdateOneById(int id, UpdatePacienteDTO updateDto)
        {
            var pac = await _GetOneById(id);

            if (updateDto.TurnosIds != null)
            {
                var turns = await _turnService.GetManyByIds(updateDto.TurnosIds);
                pac.Turnos = turns;
            }

            var updated = _mapper.Map(updateDto, pac);

            return await _repo.UpdateOne(updated);
        }

        public async Task DeleteOneById(int id)
        {
            var pac = await _GetOneById(id);
            await _repo.DeleteOne(pac);
        }

    }
}
