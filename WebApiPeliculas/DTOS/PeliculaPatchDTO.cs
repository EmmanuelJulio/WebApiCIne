using System.ComponentModel.DataAnnotations;

namespace WebApiPeliculas.DTOS
{
    public class PeliculaPatchDTO
    {
   
        [Required]
        [StringLength(100)]
        public string Titulo { get; set; }

        public bool EnCines { get; set; }
        public DateTime FechaEstreno { get; set; }
    }
}
