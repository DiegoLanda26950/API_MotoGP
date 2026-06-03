using MotoGP_API.Models;
using MotoGP_API.Models.DTOs;

namespace MotoGP_API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MotoGPDB") ?? "Not found";
        }

        // Simula el login buscando un usuario por sus credenciales
        // En un caso real buscaría en la base de datos
        public UserDtoOut GetUserFromCredentials(LoginDtoIn loginDtoIn)
        {
            // Admin hardcodeado para pruebas
            if (loginDtoIn.Email == "admin@motogp.com" && loginDtoIn.Password == "Admin1234!")
            {
                return new UserDtoOut
                {
                    UserId = 1,
                    UserName = "admin",
                    Email = "admin@motogp.com",
                    Role = Roles.Admin
                };
            }
            // User hardcodeado para pruebas
            if (loginDtoIn.Email == "user@motogp.com" && loginDtoIn.Password == "User1234!")
            {
                return new UserDtoOut
                {
                    UserId = 2,
                    UserName = "user",
                    Email = "user@motogp.com",
                    Role = Roles.User
                };
            }
            // Si no coincide ninguna credencial lanzamos excepción
            throw new KeyNotFoundException("Usuario o contraseña incorrectos.");
        }

        // Simula el registro creando un nuevo usuario con rol User por defecto
        // En un caso real insertaría en la base de datos
        public UserDtoOut AddUserFromCredentials(UserDtoIn userDtoIn)
        {
            var user = new UserDtoOut
            {
                UserId = 3, // ID simulado
                UserName = userDtoIn.UserName,
                Email = userDtoIn.Email,
                Role = Roles.User // Por defecto los nuevos usuarios son User
            };

            if (user == null)
                throw new Exception("Error al crear el usuario.");

            return user;
        }
    }
}