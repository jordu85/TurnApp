using AutoMapper;
using HandlebarsDotNet;
using Microsoft.EntityFrameworkCore;
using System.Net;
using TurnApp.Config;
using TurnApp.Models.Especialidad.DTO;
using TurnApp.Models.Paciente;
using TurnApp.Models.Paciente.DTO;
using TurnApp.Models.Profesional;
using TurnApp.Models.Profesional.DTO;
using TurnApp.Models.Turno;
using TurnApp.Models.Turno.DTO;
using TurnApp.Repositories;
using TurnApp.Utils;

namespace TurnApp.Services
{
    public class ProfesionalService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Profesional> _repo;
        private readonly AppDbContext _db;
        private readonly EspecialidadService _espService;

        public ProfesionalService(IMapper mapper, EspecialidadService espService, IRepository<Profesional> repo, AppDbContext db)
        {
            _mapper = mapper;
            _repo = repo;
            _db = db;
            _espService = espService;
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

        public async Task<Profesional> CreateOne(CreateProfesionalDTO prof, HttpContext context)
        {
            var p = _mapper.Map<Profesional>(prof);

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

            var esps = await _espService.GetManyByIds(prof.EspecialidadesIds);
            p.Especialidades = esps;

            return await _repo.CreateOne(p);
        }

        public async Task<Profesional> UpdateOneById(int id, UpdateProfesionalDTO updateDto)
        {
            var prof = await _GetOneById(id);

            var updated = _mapper.Map(updateDto, prof);

            return await _repo.UpdateOne(updated);
        }

        public async Task<Profesional> AsignarEspecialidadesAProfesional(int id, AsignarEspecialidadesAProfesionalDTO asign)
        {
            var prof = await _GetOneById(id);

            List<int> ids = asign.EspecialidadesIds;
            var especialidades = await _espService.GetManyByIds(ids);
            prof.Especialidades = especialidades;

            return await _repo.UpdateOne(prof);
        }

        public async Task<List<EspecialidadDTO>> GetEspecialidadesByProfesionalId(int id)
        {
            var especialidades = await _db.Profesionales
                .Where(p => p.Id == id)
                .SelectMany(p => p.Especialidades)
                .ToListAsync();


            return _mapper.Map<List<EspecialidadDTO>>(especialidades);
        }

        //public async Task<List<TurnoDTO>> GetTurnosByProfesionalId(int id)
        //{
        //    var prof = await _GetOneById(id);

        //    List<int> Ids = new();

        //    foreach (var t in prof.Turnos)
        //    {
        //        Ids.Add(t.Id);
        //    }

        //    List<TurnoDTO> turnos = await _turnService.GetManyByIdsDto(Ids);
        //    return turnos;
        //}

        public async Task DeleteOneById(int id)
        {
            var prof = await _GetOneById(id);
            await _repo.DeleteOne(prof);
        }

    }
}
