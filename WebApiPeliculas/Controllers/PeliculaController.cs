using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPeliculas.DTOS;
using WebApiPeliculas.Entidades;
using WebApiPeliculas.Servicios;

namespace WebApiPeliculas.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    public class PeliculaController : Controller
    {
        private readonly string contenedor = "peliculas";
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;

        public PeliculaController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<PeliculaDTO>>> Get()
        {
            var peliculas = await context.Peliculas.ToListAsync();
            return mapper.Map<List<PeliculaDTO>>(peliculas);
        }
        [HttpGet("{id:int}", Name = "obtenerPelicula")]

        public async Task<ActionResult<PeliculaDTO>> GetPelicula(int id)
        {
            var pelicula = await context.Peliculas.FirstAsync(x => x.Id == id);

            if (pelicula == null)
                return NotFound();

            return mapper.Map<PeliculaDTO>(pelicula);
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
            if (patchDocument == null)
                return BadRequest();

            var entidadDB = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);
            if (entidadDB == null)
                return NotFound();

            var entidadDTo = mapper.Map<PeliculaPatchDTO>(entidadDB);
            try
            {

                patchDocument.ApplyTo(entidadDTo, ModelState);

                var esValido = TryValidateModel(entidadDTo);
                if (esValido == false)
                    return BadRequest(ModelState);

                mapper.Map(entidadDTo, entidadDTo);
                await context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }


        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteById(int id)
        {
            var existe = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);
            if (existe == null)
                return NotFound();



            context.Peliculas.Remove(existe);
            context.SaveChanges();
            return Ok("Eliminado Actor");

        }
    }
}