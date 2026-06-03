using System.ComponentModel.DataAnnotations;

namespace MotoGP_API.Models.DTOs
{
    // DTO para registrar un nuevo usuario
    public class UserDtoIn
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "La contraseña no puede tener más de 15 caracteres")]
        public string Password { get; set; }
    }
}