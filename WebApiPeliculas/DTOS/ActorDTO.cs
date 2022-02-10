using System.ComponentModel.DataAnnotations;

namespace WebApiPeliculas.DTOS
{
    public class ActorDTO
    {
        public int id { get; set; }
        [Required]
        [StringLength(120)]
        public string Nombre { get; set; }
        [Required]

        public DateTime FechaNacimiento { get; set; }


        public string Foto { get; set; } = null!;
    }
}