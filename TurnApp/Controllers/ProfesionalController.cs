using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TurnApp.Enums;
using TurnApp.Models.Paciente;
using TurnApp.Models.Paciente.DTO;
using TurnApp.Models.Profesional;
using TurnApp.Models.Profesional.DTO;
using TurnApp.Models.Turno.DTO;
using TurnApp.Services;
using TurnApp.Utils;

namespace TurnApp.Controllers
{
    [Route("api/profesional")]
    [ApiController]
    [Authorize(Roles = $"{ROLES.Administrador}, ${ROLES.Profesional}")]
    [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status500InternalServerError)]
    public class ProfesionalController : ControllerBase
    {
        private ProfesionalService _profService;
        public ProfesionalController(ProfesionalService profService)
        {
            _profService = profService;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<ProfesionalDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ProfesionalDTO>>> GetAll()
        {
            var profs = await _profService.GetAll();
            return Ok(profs);
        }

        [HttpGet("{id}")]
        [Authorize(ROLES = $"{ROLES.Administrador}, {ROLES.Profesional}, {ROLES.Paciente}")]
        [ProducesResponseType(typeof(ProfesionalDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProfesionalDTO>> GetOneById(int id)
        {
            try
            {
                var prof = await _profService.GetOneById(id);
                return Ok(prof);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                ResponseMessage msg = new ResponseMessage(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }
        }

        [HttpPost]
        [Authorize(Roles = ROLES.Administrador)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Profesional), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseValidation), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProfesionalDTO>> CreateOne([FromBody] CreateProfesionalDTO createProf)
        {
            try
            {
                var prof = await _profService.CreateOne(createProf, HttpContext);
                return Created("POST api/profesionales", prof);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                ResponseMessage msg = new ResponseMessage(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }
        }

        [HttpPut("{id}")]
        [Authorize($"{ROLES.Administrador}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Profesional), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseValidation), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProfesionalDTO>> UpdateOneById(int id, [FromBody] UpdateProfesionalDTO updateProfesional)
        {
            try
            {
                var prof = await _profService.UpdateOneById(id, updateProfesional);
                return Ok(prof);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                ResponseMessage msg = new ResponseMessage(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }
        }

        [HttpPut("{id}/especialidades")]
        [Authorize(Roles = ROLES.Administrador)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Profesional), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseValidation), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProfesionalDTO>> AsignarEspecialidadesAProfesional(int id, [FromBody] AsignarEspecialidadesAProfesionalDTO asign)
        {
            try
            {
                var prof = await _profService.AsignarEspecialidadesAProfesional(id, asign);
                return Ok(prof);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                ResponseMessage msg = new ResponseMessage(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }
        }

        [HttpGet("{id}/turnos")]
        [Authorize(Roles = $"{ROLES.Administrador}, ${ROLES.Profesional}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Profesional), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseValidation), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<TurnoDTO>>> GetTurnosByProfesionalId(int id)
        {
            try
            {
                var turnos = await _profService.GetTurnosByProfesionalId(id);
                return Ok(turnos);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                ResponseMessage msg = new ResponseMessage(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = ROLES.Administrador)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteOneById(int id)
        {
            try
            {
                await _profService.DeleteOneById(id);
                ResponseMessage msg = new ResponseMessage($"Se elimino profesional con id = {id}.");
                //return NoContent();

                return Ok(msg);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                ResponseMessage msg = new ResponseMessage(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }
        }
    }
}
