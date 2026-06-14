using System.ComponentModel.DataAnnotations;

namespace MotoGP_API.Models.DTOs
{
    // DTO para crear un equipo con imagen obligatoria
    public class EquipoCreateDTO
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        public string Pais { get; set; }

        [Required]
        public double Presupuesto { get; set; }

        [Required]
        public int Victorias { get; set; }

        [Required]
        public DateTime FechaFundacion { get; set; }

        [Required]
        public bool EsFabricante { get; set; }

        // La imagen es obligatoria al crear un equipo
        [Required(ErrorMessage = "Debe proporcionar una imagen.")]
        public IFormFile Imagen { get; set; }
    }
}