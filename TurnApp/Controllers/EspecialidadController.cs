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
    [Route("api/especialidades")]
    [ApiController]
    [Authorize]
    [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status500InternalServerError)]
    public class EspecialidadController : ControllerBase
    {
        private readonly EspecialidadService _espService;
        public EspecialidadController(EspecialidadService espService)
        {
            _espService = espService;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<EspecialidadDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<EspecialidadDTO>>> GetAll()
        {
            var e = await _espService.GetAll();
            return Ok(e);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{ROLES.Administrador}, {ROLES.Profesional}")]
        [ProducesResponseType(typeof(EspecialidadDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EspecialidadDTO>> GetOneById(int id)
        {
            try
            {
                var esp = await _espService.GetOneById(id);
                return Ok(esp);
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
        [Authorize(Roles = $"{ROLES.Administrador}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Especialidad), StatusCodes.Status201Created)]
        public async Task<ActionResult<Especialidad>> CreateOne([FromBody] EspecialidadDTO createEsp)
        {
            try
            {
                var esp = await _espService.CreateOne(createEsp);
                return Created("POST api/especialidades", esp);
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
        [Authorize(Roles = $"{ROLES.Administrador}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Especialidad), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Especialidad>> UpdateOneById(int id, [FromBody] EspecialidadDTO updateEsp)
        {
            try
            {
                var esp = await _espService.UpdateOneById(id, updateEsp);
                return Ok(esp);
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
        [Authorize(Roles = $"{ROLES.Administrador}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteOneById(int id)
        {
            try
            {
                await _espService.DeleteOneById(id);
                ResponseMessage msg = new ResponseMessage($"Se elimino la especialidad con id = {id}.");
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
