using System.ComponentModel.DataAnnotations;

namespace WebApiPeliculas.Entidades
{
    public class Pelicula
    {
        public int  Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Titulo { get; set; }

        public bool EnCines { get; set; }
        public DateTime FechaEstreno { get; set; }
        public string Poster { get; set; }
        public List<PeliculasActores> PeliculasActores { get; set; }
        public List<PeliculasGeneros> peliculasGeneros { get; set; }
    }
}
