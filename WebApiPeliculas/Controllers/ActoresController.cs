#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApiPeliculas;
using WebApiPeliculas.DTOS;
using WebApiPeliculas.Entidades;
using WebApiPeliculas.Helpers;
using WebApiPeliculas.Servicios;

namespace WebApiPeliculas.Controllers
{
    [ApiController]
    [Route("api/actores")]
    public class ActoresController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "actores";

        public ActoresController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = context.Actores.AsQueryable();
            await HttpContext.InsertarParametrosPaginacion(queryable,paginacionDTO.CantidadRegistrosPorPagina);
            var actors = await queryable.Paginar(paginacionDTO).ToListAsync();
            if (actors.Any())
                return mapper.Map<List<ActorDTO>>(actors);

            return NoContent();
        }

        [HttpGet("{id:int}", Name = "obtenerActor")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            var resultado = await context.Actores.FirstOrDefaultAsync(x => x.id == id);

            if (resultado == null)
                return NotFound();

            return mapper.Map<ActorDTO>(resultado);
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            var actor = mapper.Map<Actor>(actorCreacionDTO);

            if (actorCreacionDTO.Foto != null)
            {
                using (var memoriStream = new MemoryStream())
                {
                    await actorCreacionDTO.Foto.CopyToAsync(memoriStream);
                    var contenido = memoriStream.ToArray();
                    var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);
                    actor.Foto = await almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor, actorCreacionDTO.Foto.ContentType);
                }
            }
            context.Add(actor);
            await context.SaveChangesAsync();
            var ActorDTO = mapper.Map<ActorDTO>(actor);
            return new CreatedAtRouteResult("obtenerActor", new { id = ActorDTO.id }, ActorDTO);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> put(int id, [FromBody] ActorCreacionDTO actorCreacionDTO)
        {
            var actorDB = await context.Actores.FirstOrDefaultAsync(x => x.id == id);
            if (actorDB == null)
                return NotFound();
            else
                actorDB = mapper.Map(actorCreacionDTO, actorDB);

            if (actorCreacionDTO.Foto != null)
            {
                using (var memoriStream = new MemoryStream())
                {
                    await actorCreacionDTO.Foto.CopyToAsync(memoriStream);
                    var contenido = memoriStream.ToArray();
                    var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);
                    actorDB.Foto = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor, actorDB.Foto, actorCreacionDTO.Foto.ContentType);
                }
            }

            await context.SaveChangesAsync();
            return NoContent();

        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteById(int id)
        {
            var existe = await context.Actores.FirstOrDefaultAsync(x => x.id == id);
            if (existe == null)
                return NotFound();



            context.Actores.Remove(existe);
            context.SaveChanges();
            return Ok("Eliminado Actor");

        }
        [HttpPatch("{id:int}")]
        public  async Task<ActionResult> Patch(int id,[FromBody] JsonPatchDocument<ActorPathDTO> patchDocument)
        {
            if (patchDocument == null)
                return BadRequest();

            var entidadDB = await context.Actores.FirstOrDefaultAsync(x => x.id == id);
            if (entidadDB == null)
                return NotFound();

            var entidadDTo = mapper.Map<ActorPathDTO>(entidadDB);
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
    }
}
