using System.ComponentModel.DataAnnotations;

namespace MotoGP_API.Models.DTOs
{
    // DTO para crear un piloto con imagen obligatoria
    public class PilotoCreateDTO
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        public string Nacionalidad { get; set; }

        [Required]
        public int Dorsal { get; set; }

        [Required]
        public int CampeonatosGanados { get; set; }

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        public bool Activo { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int MotoId { get; set; }

        [Required]
        public int EquipoId { get; set; }

        [Required]
        public int CircuitoId { get; set; }

        // La imagen es obligatoria al crear un piloto
        [Required(ErrorMessage = "Debe proporcionar una imagen.")]
        public IFormFile Imagen { get; set; }
    }
}