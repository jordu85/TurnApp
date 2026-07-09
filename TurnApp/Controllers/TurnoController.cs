using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TurnApp.Enums;
using TurnApp.Models.Especialidad;
using TurnApp.Models.Especialidad.DTO;
using TurnApp.Services;
using TurnApp.Utils;

namespace TurnApp.Controllers
{
    [Route("api/turno")]
    [ApiController]
    [Authorize(Roles = ROLES.Administrador)]
    [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status500InternalServerError)]
    public class TurnoController : ControllerBase
    {
        private readonly TurnoService _turnService;
        public TurnoController(TurnoService turnService)
        {
            _turnService = turnService;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<TurnoDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TurnoDTO>>> GetAll()
        {
            var turnos = await _turnService.GetAll();
            return Ok(turnos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Turno), StatusCodes.Status201Created)]
        public async Task<ActionResult<Turno>> CreateOne([FromBody] CreateTurnoDTO createTurn)
        {
            try
            {
                var turno = await _turnService.CreateOne(createTurn);
                return Created("POST api/turnos", turno);
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

        [HttpDelete("{id}")]
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
