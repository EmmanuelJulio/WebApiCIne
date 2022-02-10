namespace WebApiPeliculas.Entidades
{
    public class PeliculasGeneros
    {
        public int GeneroID { get; set; }
        public int PeliculaID { get; set; }
        public Genero Genero { get; set; }
        public Pelicula Pelicula { get; set; }
    }
}
