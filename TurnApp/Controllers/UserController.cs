using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TurnApp.Enums;
using TurnApp.Models.Paciente.DTO;
using TurnApp.Models.User.DTO;
using TurnApp.Services;
using TurnApp.Utils;

namespace TurnApp.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize(Roles = ROLES.Administrador)]
    [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status500InternalServerError)]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = $"{ROLES.Administrador}")]
        [ProducesResponseType(typeof(List<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<UserDTO>>> GetAll()
        {
            var users = await _userService.GetAll();
            return Ok(users);
        }
    }
}
