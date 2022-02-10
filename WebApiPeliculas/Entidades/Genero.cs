using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebApiPeliculas.Entidades
{
    public class Genero
    {
        [Required]
        public int Id { get; set; }
        
        [StringLength(40)]
        [Required]
        public string Nombre { get; set; } = String.Empty;
        public List<PeliculasGeneros> PeliculasGeneros { get; set; }
    }
}
