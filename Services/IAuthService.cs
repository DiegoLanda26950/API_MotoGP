using System.Security.Claims;
using MotoGP_API.Models.DTOs;

namespace MotoGP_API.Services
{
    // Interfaz que define los métodos de autenticación
    public interface IAuthService
    {
        // Genera un token JWT a partir de las credenciales de login
        string Login(LoginDtoIn loginDtoIn);
        // Registra un nuevo usuario y devuelve un token JWT
        string Register(UserDtoIn userDtoIn);
        // Genera el token JWT a partir de los datos del usuario
        string GenerateToken(UserDtoOut userDtoOut);
        // Comprueba si el usuario tiene acceso a un recurso concreto
        bool HasAccessToResource(int requestedUserID, ClaimsPrincipal user);
    }
}