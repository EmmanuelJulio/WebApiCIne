using System.ComponentModel.DataAnnotations;

namespace WebApiPeliculas.DTOS
{
    public class ActorPathDTO
    {
        [Required]
        [StringLength(120)]
        public string Nombre { get; set; }
        [Required]

        public DateTime FechaNacimiento { get; set; }
    }
}
