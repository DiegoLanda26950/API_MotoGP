using System.ComponentModel.DataAnnotations;

namespace MotoGP_API.Models.DTOs
{
    // DTO para devolver los datos del usuario sin exponer la contraseña
    public class UserDtoOut
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        // Rol del usuario: Admin, User o Guest
        [Required]
        public string Role { get; set; }
    }
}