using System.ComponentModel.DataAnnotations;

namespace MotoGP_API.Models.DTOs
{
    // DTO para crear una moto con imagen obligatoria
    public class MotoCreateDTO
    {
        [Required]
        [MaxLength(100)]
        public string Marca { get; set; }

        [Required]
        [MaxLength(100)]
        public string Modelo { get; set; }

        [Required]
        public int Cilindrada { get; set; }

        [Required]
        public decimal Potencia { get; set; }

        [Required]
        public double Peso { get; set; }

        [Required]
        public int Anio { get; set; }

        [Required]
        public string Color { get; set; }

        // La imagen es obligatoria al crear una moto
        [Required(ErrorMessage = "Debe proporcionar una imagen.")]
        public IFormFile Imagen { get; set; }
    }
}