using System.ComponentModel.DataAnnotations;

namespace WebApiPeliculas.DTOS
{
    public class CredencialesUsuario
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
