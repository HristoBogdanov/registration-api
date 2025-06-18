using dotnet_registration_api.Data.Entities;
using dotnet_registration_api.Data.Models;
using dotnet_registration_api.Data.Repositories;
using dotnet_registration_api.Helpers;

namespace dotnet_registration_api.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<List<User>> GetAll()
        {
            return await _userRepository.GetAllUsers();
        }
        public async Task<User> GetById(int id)
        {
            var user = await _userRepository.GetUserById(id);

            if(user == null)
            {
                throw new NotFoundException("This user does not exist");
            }

            return user;
        }
        public async Task<User> Login(LoginRequest login)
        {
            if(login.Username == String.Empty || login.Password == String.Empty)
            {
                throw new AppException("Enter username and password");
            }

            var user = await _userRepository.GetUserByUsernameAndPassword(login.Username, login.Password);

            if(user == null)
            {
                throw new NotFoundException("User with this username and password does not exist");
            }

            return user;
        }
        public async Task<User> Register(RegisterRequest register)
        {
            var users = await _userRepository.GetAllUsers();
            if (register.Password == String.Empty)
            {
                throw new AppException("Enter a password");
            }

            if(users.Any(u => u.Username == register.Username))
            {
                throw new AppException("Username already taken");
            }

            User user = new()
            {
                Username = register.Username,
                FirstName = register.FirstName,
                LastName = register.LastName,
                PasswordHash = HashHelper.HashPassword(register.Password)
            };

            await _userRepository.CreateUser(user);
            return user;
        }
        public async Task<User> Update(int id, UpdateRequest updateRequest)
        {
            var user = await _userRepository.GetUserById(id);

            if(user == null)
            {
                throw new NotFoundException();
            }

            if(user.PasswordHash != HashHelper.HashPassword(updateRequest.OldPassword))
            {
                throw new AppException("Wrong password");
            }

            var users = await _userRepository.GetAllUsers();
            
            if(user.Username != updateRequest.Username && 
                users.Any(u => u.Username == updateRequest.Username))
            {
                throw new AppException("Username already taken");
            }

            user.FirstName = updateRequest.FirstName;
            user.LastName = updateRequest.LastName;
            user.Username = updateRequest.Username;
            user.PasswordHash = HashHelper.HashPassword(updateRequest.NewPassword);

            return await _userRepository.UpdateUser(user);
        }
        public async Task Delete(int id)
        {
            var user = await _userRepository.GetUserById(id);
            if(user == null)
            {
                throw new NotFoundException("That user does not exist");
            }
            await _userRepository.DeleteUser(id);
        }

    }
}
