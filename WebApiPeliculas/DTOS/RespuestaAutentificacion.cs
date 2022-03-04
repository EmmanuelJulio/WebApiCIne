namespace WebApiPeliculas.DTOS
{
    public class RespuestaAutentificacion
    {
        public string Token { get; set; }
        public DateTime Expiracion { get; set; }
    }
}
