using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPeliculas.DTOS;
using WebApiPeliculas.Entidades;
using WebApiPeliculas.Helpers;
using WebApiPeliculas.Servicios;
using System.Linq.Dynamic.Core;

namespace WebApiPeliculas.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    public class PeliculaController : CustomBaseController
    {
        private readonly string contenedor = "peliculas";
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly ILogger<PeliculaController> logger;

        public PeliculaController(ApplicationDbContext context, IMapper mapper, 
            IAlmacenadorArchivos almacenadorArchivos,
            ILogger<PeliculaController> logger) : base(context,mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PeliculasIndexDTO>> Get()
        {
            var top = 5;
            var hoy = DateTime.Today;

            var proximosEstrenos = await context.Peliculas
                .Where(x=>x.FechaEstreno>hoy)
                .OrderBy(x=>x.FechaEstreno)
                .Take(top)
                .ToListAsync();

            var enCines = await context.Peliculas
                .Where(x => x.EnCines)
                .Take(top)
                .ToListAsync();


             var resultado = new PeliculasIndexDTO
            {
                EnCines = mapper.Map<List<PeliculaDTO>>(enCines),
                FuturosEstrenos = mapper.Map<List<PeliculaDTO>>(proximosEstrenos)
            };

            return resultado;
        }

        [HttpGet("filtro")]
        public async Task<ActionResult<List<PeliculaDTO>>> Filtrar([FromQuery] FiltroPeliculaDTO filtroPeliculaDTO)
        {
            var peliculasQueryable = context.Peliculas.AsQueryable();
            if (!string.IsNullOrEmpty(filtroPeliculaDTO.Titulo))
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.Titulo.Contains(filtroPeliculaDTO.Titulo));
            }
            if (filtroPeliculaDTO.EnCines)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.EnCines);
            }
            if (filtroPeliculaDTO.ProximosExtrenoss)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.FechaEstreno > DateTime.Today);
            }
            if (filtroPeliculaDTO.GeneroID != 0)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.peliculasGeneros
                  .Select(y => y.GeneroID)
                  .Contains(filtroPeliculaDTO.GeneroID));
            }
            if (!string.IsNullOrEmpty(filtroPeliculaDTO.CampoOrdenar))

            {
                var TipoOrden = filtroPeliculaDTO.OrdenAsendente ? "ascending" : "descending";

                try
                {
                    peliculasQueryable = peliculasQueryable.OrderBy($"{filtroPeliculaDTO.CampoOrdenar} {TipoOrden}");

                }
                catch (Exception ex)
                {

                    logger.LogError(ex.Message, ex);

                }
            
            }
            await HttpContext.InsertarParametrosPaginacion(peliculasQueryable,
                filtroPeliculaDTO.CantidadDeRegistrosPorPagina);
            var peliculas = await peliculasQueryable.Paginar(filtroPeliculaDTO.Paginacion).ToListAsync();
            return mapper.Map<List<PeliculaDTO>>(peliculas);
        
        }


        [HttpGet("{id:int}", Name = "obtenerPelicula")]

        public async Task<ActionResult<PeliculaDetallesDTO>> GetPelicula(int id)
        {
            var pelicula = await context.Peliculas
                .Include(x=>x.PeliculasActores).ThenInclude(x=>x.Actor)
                .Include(x=>x.peliculasGeneros).ThenInclude(x=>x.Genero)
                .FirstAsync(x => x.Id == id);

            if (pelicula == null)
                return NotFound();
            pelicula.PeliculasActores = pelicula.PeliculasActores.OrderBy(x=>x.Orden).ToList();
            return mapper.Map<PeliculaDetallesDTO>(pelicula);
        }
        [HttpPost]

        public async Task<ActionResult> Post([FromForm] PeliculaCreacionDTO peliculaDreacionDTO)
        {
            var pelicula = mapper.Map<Pelicula>(peliculaDreacionDTO);


            if (peliculaDreacionDTO.Poster != null)
            {
                using (var memoriStream = new MemoryStream())
                {
                    await peliculaDreacionDTO.Poster.CopyToAsync(memoriStream);
                    var contenido = memoriStream.ToArray();
                    var extension = Path.GetExtension(peliculaDreacionDTO.Poster.FileName);
                    pelicula.Poster = await almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor, peliculaDreacionDTO.Poster.ContentType);
                }
            }
            AsignarOrdenActores(pelicula);
            context.Add(pelicula);
            await context.SaveChangesAsync();
            var PeliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
            return new CreatedAtRouteResult("obtenerPelicula", new { id = PeliculaDTO.Id }, PeliculaDTO);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var peliculaDB = await context.Peliculas
                .Include(x=>x.PeliculasActores)
                .Include(x=>x.peliculasGeneros)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (peliculaDB == null)
                return NotFound();
            else
                peliculaDB = mapper.Map(peliculaCreacionDTO, peliculaDB);

            if (peliculaCreacionDTO.Poster != null)
            {
                using (var memoriStream = new MemoryStream())
                {
                    await peliculaCreacionDTO.Poster.CopyToAsync(memoriStream);
                    var contenido = memoriStream.ToArray();
                    var extension = Path.GetExtension(peliculaCreacionDTO.Poster.FileName);
                    peliculaDB.Poster = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor, peliculaDB.Poster, peliculaCreacionDTO.Poster.ContentType);
                }
            }
            AsignarOrdenActores(peliculaDB);
            await context.SaveChangesAsync();
            return NoContent();

        }

        private void AsignarOrdenActores(Pelicula pelicula)
        {
            if (pelicula.PeliculasActores != null)
            {
                for(int i = 0;i< pelicula.PeliculasActores.Count; i++)
                {
                    pelicula.PeliculasActores[i].Orden = i;
                }
            }

        }


        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PeliculaPatchDTO> patchDocument)
        {
            return await Patch<Pelicula, PeliculaPatchDTO>(id,patchDocument);

        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteById(int id)
        {
            return await Delete<Pelicula>(id);

        }
    }
}