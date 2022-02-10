using System.ComponentModel.DataAnnotations;

namespace WebApiPeliculas.DTOS
{
    public class GeneroCreacionDTO
    {
        [Required]
        [StringLength(40)]
        public string Nombre { get; set; } = String.Empty;
    }
}
