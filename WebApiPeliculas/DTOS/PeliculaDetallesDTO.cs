using WebApiPeliculas.Entidades;

namespace WebApiPeliculas.DTOS
{
    public class PeliculaDetallesDTO : PeliculaDTO
    {
        public List<ActorPDetalleDTO> Actores { get; set; }
        public List<GeneroDTO> Generos { get; set; }   
    }
}
