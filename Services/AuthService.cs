using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MotoGP_API.Models;
using MotoGP_API.Models.DTOs;
using MotoGP_API.Repositories;

namespace MotoGP_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _repository;

        public AuthService(IConfiguration configuration, IUserRepository repository)
        {
            _configuration = configuration;
            _repository = repository;
        }

        // Hace login y devuelve un token JWT
        public string Login(LoginDtoIn loginDtoIn)
        {
            var user = _repository.GetUserFromCredentials(loginDtoIn);
            return GenerateToken(user);
        }

        // Registra un nuevo usuario y devuelve un token JWT
        public string Register(UserDtoIn userDtoIn)
        {
            var user = _repository.AddUserFromCredentials(userDtoIn);
            return GenerateToken(user);
        }

        // Genera el token JWT con los datos del usuario y su rol
        public string GenerateToken(UserDtoOut userDtoOut)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWT:ValidIssuer"],
                Audience = _configuration["JWT:ValidAudience"],
                Subject = new ClaimsIdentity(new Claim[]
                {
                    // ID del usuario
                    new Claim(ClaimTypes.NameIdentifier, Convert.ToString(userDtoOut.UserId)),
                    // Nombre del usuario
                    new Claim(ClaimTypes.Name, userDtoOut.UserName),
                    // Rol del usuario (Admin, User o Guest)
                    new Claim(ClaimTypes.Role, userDtoOut.Role),
                    // Email del usuario
                    new Claim(ClaimTypes.Email, userDtoOut.Email),
                }),
                // El token expira en 7 días
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Comprueba si el usuario tiene acceso a un recurso concreto
        // Un usuario solo puede acceder a sus propios recursos o si es Admin
        public bool HasAccessToResource(int requestedUserID, ClaimsPrincipal user)
        {
            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out int userId))
                return false;

            // Comprueba si el recurso pertenece al usuario
            var isOwnResource = userId == requestedUserID;

            // Comprueba si el usuario es Admin
            var roleClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            if (roleClaim is null) return false;
            var isAdmin = roleClaim.Value == Roles.Admin;

            return isOwnResource || isAdmin;
        }
    }
}