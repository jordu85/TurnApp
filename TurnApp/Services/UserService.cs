using AutoMapper;
using System.Net;
using TurnApp.Models.User.DTO;
using TurnApp.Models.User;
using TurnApp.Repositories;
using TurnApp.Utils;

namespace TurnApp.Services
{
    public class UserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _repo;

        public UserService(IMapper mapper, IUserRepository repo)
        {
            _mapper = mapper;
            _repo = repo;
        }

        public async Task<List<User>> GetAll() => await _repo.GetAll();

        public async Task<User> GetOneById(int id)
        {
            var user = await _repo.GetOne(x => x.Id == id);

            if (user == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.NotFound,
                    $"User con ID = {id} no encontrada"
                );
            }
            return user;
        }

        public async Task<User> CreateOne(User u)
        {
            return await _repo.CreateOne(u);
        }

        public async Task<User> UpdateOneById(int id, UpdateUserDTO updateDto)
        {
            var user = await GetOneById(id);

            var updated = _mapper.Map(updateDto, user);

            return await _repo.UpdateOne(updated);
        }

        public async Task<User> UpdateOne(User u)
        {
            User updated = await _repo.UpdateOne(u);
            return updated;
        }

        public async Task DeleteOneById(int id)
        {
            var u = await GetOneById(id);
            await _repo.DeleteOne(u);
        }

        public async Task<User> GetOneByDni(string dni)
        {
            if (string.IsNullOrEmpty(dni))
            {
                throw new ErrorResponse(HttpStatusCode.BadRequest, "Debes ingresar un dni");
            }
            
            return await _repo.GetOne(x => x.Dni == dni);
        }

        public async Task<string[]> GeneratePwdToken(int id, HttpRequest request)
        {
            User user = await GetOneById(id);
            Guid token = Guid.NewGuid();
            user.PasswordToken = token.ToString();
            await UpdateOne(user);
            string callbackUrl = $"{request.Scheme}://{request.Host}/api/auth/verify-pwdtoken?=userId={id}&token={token.ToString()}";
            return [callbackUrl, user.Dni];
        }
        public async Task<bool> VerifyPwdToken(int id, string token)
        {
            User user = await GetOneById(id);
            if (user.PasswordToken != token)
            {
                return false;
            }
            user.PasswordToken = null;
            await UpdateOne(user);
            return true;
        }

    }
}
