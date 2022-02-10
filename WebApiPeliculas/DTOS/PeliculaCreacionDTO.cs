using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebApiPeliculas.Helpers;
using WebApiPeliculas.Validaciones;

namespace WebApiPeliculas.DTOS
{
    public class PeliculaCreacionDTO : PeliculaPatchDTO
    {
       
        [PesoArchivoValidacion(4)]
        [TipoArchivoValidacion(GrupoTipoArchivo.Imagen)]
        public IFormFile Poster { get; set; }
        [ModelBinder(BinderType =typeof(TypeBinder<List<int>>))]
        public List<int> GenerosIDs { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<ActorPeliculasCreacionDTO>>))]
        public List<ActorPeliculasCreacionDTO> Actores { get; set; }
    }
}
