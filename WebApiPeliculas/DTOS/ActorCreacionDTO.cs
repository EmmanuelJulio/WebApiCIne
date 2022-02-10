using System.ComponentModel.DataAnnotations;
using WebApiPeliculas.Validaciones;

namespace WebApiPeliculas.DTOS
{
    public class ActorCreacionDTO : ActorPathDTO
    {
      

        [PesoArchivoValidacion(PesoMaximoEnMegaBytes: 4)]
        [TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
        public IFormFile Foto { get; set; }




    }
}