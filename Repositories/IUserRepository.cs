using MotoGP_API.Models.DTOs;

namespace MotoGP_API.Repositories
{
    // Interfaz que define los métodos para gestionar usuarios
    public interface IUserRepository
    {
        // Obtiene un usuario a partir de sus credenciales de login
        UserDtoOut GetUserFromCredentials(LoginDtoIn loginDtoIn);
        // Añade un usuario a partir de sus credenciales de registro
        UserDtoOut AddUserFromCredentials(UserDtoIn userDtoIn);
    }
}