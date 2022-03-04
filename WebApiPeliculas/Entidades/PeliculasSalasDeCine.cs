namespace WebApiPeliculas.Entidades
{
    public class PeliculasSalasDeCine
    {
        public int PeliculaID { get; set; }
        public int SalaDeCineID { get; set; }
        public Pelicula Pelicula { get; set; }
        public SalaDeCine SalaDeCine { get; set; }
    }
}
