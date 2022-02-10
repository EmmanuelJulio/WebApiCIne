using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WebApiPeliculas.Validaciones
{
    public class TipoArchivoValidacion :ValidationAttribute
    {
        private readonly string[] tipoValidos;

        public TipoArchivoValidacion(string[] tipoValidos)
        {
            this.tipoValidos = tipoValidos;
        }

        public TipoArchivoValidacion(GrupoTipoArchivo grupoTipoArchivo)
        {
            if (grupoTipoArchivo == GrupoTipoArchivo.Imagen)
                tipoValidos = new string[] { "image/jpg", "image/jpeg", "image/png", "image/gif" };
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            if (value == null)
                return ValidationResult.Success;

#pragma warning disable CS8600 // Se va a convertir un literal nulo o un posible valor nulo en un tipo que no acepta valores NULL
            IFormFile formFile = value as IFormFile;
#pragma warning restore CS8600 // Se va a convertir un literal nulo o un posible valor nulo en un tipo que no acepta valores NULL

            if (formFile == null)
                return ValidationResult.Success;

            if (!tipoValidos.Contains(formFile.ContentType))
                return new ValidationResult($"El tipo de archivo debe ser uno de los siguietnes: {string.Join(" ,", tipoValidos)}");

            return ValidationResult.Success;
        }
    }
}
