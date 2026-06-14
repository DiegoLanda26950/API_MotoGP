using System.ComponentModel.DataAnnotations;

namespace MotoGP_API.Models.DTOs
{
    // DTO para crear un circuito con imagen obligatoria
    public class CircuitoCreateDTO
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        public string Pais { get; set; }

        [Required]
        public string Ciudad { get; set; }

        [Required]
        public double Longitud { get; set; }

        [Required]
        public int Curvas { get; set; }

        [Required]
        public bool Homologado { get; set; }

        [Required]
        public DateTime FechaInauguracion { get; set; }

        // La imagen es obligatoria al crear un circuito
        [Required(ErrorMessage = "Debe proporcionar una imagen.")]
        public IFormFile Imagen { get; set; }
    }
}