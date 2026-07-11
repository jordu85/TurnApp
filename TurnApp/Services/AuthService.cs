using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using TurnApp.Models.Role;
using TurnApp.Models.User.DTO;
using TurnApp.Models.User;
using TurnApp.Utils;
using Microsoft.Identity.Client;

namespace TurnApp.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginDTO login, HttpContext context);
        Task Logout(HttpContext context);
        Task<UserDTO> Register(RegisterDTO register, HttpContext context);
        Task<UserDTO> UpdateRolesToUser(int userId, List<int> roleIds);
        Task<UserDTO> Me(HttpContext context);
        Task<List<UserDTO>> GetAllUsers();
        Task<UserDTO> GetUserByDni(string dni);
        Task GeneratePwdTokenToUser(HttpContext context);
        Task VerifyUserPwdToken(int userId, string token);
    }

    public class AuthService : IAuthService
    {
        private readonly UserService _userService;
        private readonly RoleService _roleService;
        private readonly IEncoderService _encoder;
        private readonly EmailService _emailService;
        private readonly IMapper _mapper;
        private readonly string _secret;

        public AuthService(UserService userService, IMapper mapper, IConfiguration config, RoleService roleService, IEncoderService encoder, EmailService emailService)
        {
            _userService = userService;
            _mapper = mapper;
            _secret = config
                .GetSection("Secrets:jwt")?.Value?.ToString()
                ?? throw new Exception("invalid jwt secret");
            _roleService = roleService;
            _encoder = encoder;
            _emailService = emailService;
        }

        public async Task GeneratePwdTokenToUser(HttpContext context)
        {
            Console.WriteLine(context.Request.Scheme);
            Console.WriteLine(context.Request.Host);
            int userId = GetUserIdFromContext(context);
            string[] data = await _userService.GeneratePwdToken(userId, context.Request);
            await _emailService.SendResetPwdAsync(data[1], data[0]);
        }
        public async Task VerifyUserPwdToken(int userId, string token)
        {
            bool IsCorrect = await _userService.VerifyPwdToken(userId, token);
            if (!IsCorrect)
            {
                throw new ErrorResponse(HttpStatusCode.BadRequest, "Invalid Token");
            }
        }

        public async Task<LoginResponse> Login(LoginDTO login, HttpContext context)
        {
            User? user;

            user = await _userService.GetOneByDni(login.Dni);

            if (user == null) throw new ErrorResponse(HttpStatusCode.BadRequest, "Invalid Credentials");

            bool verified = _encoder.Verify(user.Password, login.Password);
            if (!verified)
            {
                throw new ErrorResponse(HttpStatusCode.BadRequest, "Invalid Credentials");
            }

            await SetCookie(user, context);

            var loginReponse = new LoginResponse()
            {
                Token = GenerateJwtToken(user),
                User = _mapper.Map<UserDTO>(user)
            };
            return loginReponse;
        }

        public async Task<List<UserDTO>> GetAllUsers()
        {
            var users = await _userService.GetAll();
            return _mapper.Map<List<UserDTO>>(users);
        }
       
        public async Task<UserDTO> GetUserByDni(string dni)
        {
            var user = await _userService.GetOneByDni(dni);
            if (user == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.NotFound,
                    $"Usuerio con DNI = {dni} no encontrado."
                    );
            }
            return _mapper.Map<UserDTO>(user);
        }

        private int GetUserIdFromContext(HttpContext context)
        {
            var user = context.User.Claims.FirstOrDefault(claim => claim.Type == "id");
            if (user?.Value == null)
            {
                throw new ErrorResponse(HttpStatusCode.Unauthorized, "Token invalido.");
            }

            if (!int.TryParse(user.Value, out int id))
            {
                throw new ErrorResponse(HttpStatusCode.Unauthorized, "Token invalido.");
            }

            return id;
        }

        public async Task Logout(HttpContext context)
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task SetCookie(User user, HttpContext context)
        {
            var claims = new List<Claim>()
            {
                new Claim("id", user.Id.ToString())
            };

            if (user.Roles?.Count > 0)
            {
                foreach (var role in user.Roles)
                {
                    var roleClaim = new Claim(ClaimTypes.Role, role.Name);
                    claims.Add(roleClaim);
                }
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties()
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7),
                }
            );
        }

        public async Task<UserDTO> Register(RegisterDTO register, HttpContext context)
        {
            User? u = await _userService
                .GetOneByDni(
                    register.Dni
                );
            if (u != null)
            {
                throw new ErrorResponse(HttpStatusCode.BadRequest, "User already exists");
            }
            if (register.Password != register.ConfirmPassword)
            {
                throw new ErrorResponse(HttpStatusCode.BadRequest, "Passwords don't match");
            }

            var user = _mapper.Map<User>(register);
            user.Password = _encoder.Encrypt(user.Password);

            // Asignación del rol por defecto
            var role = await _roleService.GetOneByName("Pac");
            user.Roles.Add(role);

            var created = await _userService.CreateOne(user);
            await SetCookie(created, context);
            return _mapper.Map<UserDTO>(created);
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new ClaimsIdentity();
            var idClaim = new Claim("id", user.Id.ToString());
            claims.AddClaim(idClaim);

            if (user.Roles != null)
            {
                foreach (var role in user.Roles)
                {
                    var roleClaim = new Claim(ClaimTypes.Role, role.Name);
                    claims.AddClaim(roleClaim);
                }
            }

            var key = Encoding.UTF8.GetBytes(_secret);
            var symmetricKey = new SymmetricSecurityKey(key);

            var credentials = new SigningCredentials(
                symmetricKey,
                SecurityAlgorithms.HmacSha256Signature
            );
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(120),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);
            string token = tokenHandler.WriteToken(tokenConfig);

            return token;
        }

        public async Task<UserDTO> UpdateRolesToUser(int userId, List<int> roleIds)
        {
            User user = await _userService.GetOneById(userId);

            List<Role> roles = await _roleService.GetManyByIds(roleIds);

            user.Roles = roles;

            var updatedUser = await _userService.UpdateOne(user);

            UserDTO mapped = _mapper.Map<UserDTO>(updatedUser);

            return mapped;
        }

        public async Task<UserDTO> Me(HttpContext context)
        {
            var idClaim = context.User.FindFirst("id")?.Value;
            if (idClaim == null || !int.TryParse(idClaim, out int userId))
            {
                throw new ErrorResponse(HttpStatusCode.Unauthorized, "No se pudo identificar al usuario");
            }

            var user = await _userService.GetOneById(userId);
            if (user == null)
            {
                throw new ErrorResponse(HttpStatusCode.Unauthorized, "Usuario no encontrado");
            }

            return _mapper.Map<UserDTO>(user);
        }
    }
}
