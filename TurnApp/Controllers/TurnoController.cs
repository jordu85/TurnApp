using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TurnApp.Enums;
using TurnApp.Models.Turno;
using TurnApp.Models.Turno.DTO;
using TurnApp.Services;
using TurnApp.Utils;

namespace TurnApp.Controllers
{
    [Route("api/turno")]
    [ApiController]
    [Authorize]
    [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status500InternalServerError)]
    public class TurnoController : ControllerBase
    {
        private readonly TurnoService _turnService;
        public TurnoController(TurnoService turnService)
        {
            _turnService = turnService;
        }

        [HttpGet]
        [Authorize(Roles = $"{ROLES.Administrador}, {ROLES.Profesional}, {ROLES.Paciente}")]
        [ProducesResponseType(typeof(List<TurnoDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TurnoDTO>>> GetAll()
        {
            var turnos = await _turnService.GetAll();
            return Ok(turnos);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{ROLES.Administrador}, {ROLES.Profesional}, {ROLES.Paciente}")]
        [ProducesResponseType(typeof(TurnoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TurnoDTO>> GetOneById(int id)
        {
            try
            {
                var turno = await _turnService.GetOneById(id);
                return Ok(turno);
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
        [ProducesResponseType(typeof(Turno), StatusCodes.Status201Created)]
        public async Task<ActionResult<Turno>> CreateOne([FromBody] CreateTurnoDTO createTurn)
        {
            try
            {
                var turno = await _turnService.CreateOne(createTurn);
                return Created("POST api/turno", turno);
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
        [Authorize(Roles = $"{ROLES.Administrador}, {ROLES.Profesional}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Turno), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Turno>> UpdateOneById(int id, [FromBody] UpdateTurnoDTO updateTurn)
        {
            try
            {
                var turno = await _turnService.UpdateOneById(id, updateTurn);
                return Ok(turno);
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

        [HttpPost("by-ids")]
        [Authorize(Roles = $"{ROLES.Administrador}, {ROLES.Profesional}, {ROLES.Paciente}")]
        [ProducesResponseType(typeof(List<TurnoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<TurnoDTO>>> GetManyByIds([FromBody] List<int> ids)
        {
            try
            {
                var turnos = await _turnService.GetManyByIdsDto(ids);
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

        [HttpGet("paciente/{pacienteId}")]
        [Authorize(Roles = $"{ROLES.Administrador}, {ROLES.Profesional}")]
        [ProducesResponseType(typeof(List<TurnoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<TurnoDTO>>> GetTurnosByPacienteId(int pacienteId)
        {
            try
            {
                var turnos = await _turnService.GetTurnosByPacienteId(pacienteId);
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

        [HttpGet("propios")]
        [Authorize(Roles = ROLES.Paciente)]
        [ProducesResponseType(typeof(List<TurnoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<TurnoDTO>>> GetTurnosPropios()
        {
            try
            {
                var turnos = await _turnService.GetTurnosPropios(HttpContext);
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

        [HttpPut("{turnoId}/paciente/{pacienteId}")]
        [Authorize(Roles = $"{ROLES.Administrador}, {ROLES.Profesional}, {ROLES.Paciente}")]
        [ProducesResponseType(typeof(TurnoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TurnoDTO>> AsignarPacienteATurno(int turnoId, int pacienteId)
        {
            try
            {
                var turno = await _turnService.AsignarPacienteATurno(turnoId, pacienteId);
                return Ok(turno);
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
                await _turnService.DeleteOneById(id);
                ResponseMessage msg = new ResponseMessage($"Turno con id = {id} se elimino.");
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
