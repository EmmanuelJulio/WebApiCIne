using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace WebApiPeliculas.Entidades
{
    public class SalaDeCine : IId
    {
        public int Id { get; set; }
        [Required]
        [StringLength(120)]
        public string Nombre { get; set; }
        public List<PeliculasSalasDeCine> SalasDeCine { get; set; }
        public Point Ubicacion { get; set; }
    }
}
