namespace WebApiPeliculas.Entidades
{
    public class PeliculasActores
    {
        public int ActorID { get; set; }
        public int PeliculaID { get; set; }
        public string Personaje { get; set; }
        public int Orden { get; set; }
        public Actor Actor { get; set; }
        public Pelicula Pelicula { get; set; }
    }
}
