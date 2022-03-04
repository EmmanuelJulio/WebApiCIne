namespace WebApiPeliculas.DTOS
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public string Comentario { get; set; }

        public int Puntuacion { get; set; }
        public int PeliculaID { get; set; }
       
        public string UsuarioID { get; set; }
        public string NombreUsuario { get; set; }
        
    }
}
