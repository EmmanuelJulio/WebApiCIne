using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPeliculas.DTOS;
using WebApiPeliculas.Entidades;

namespace WebApiPeliculas.Controllers
{
    [ApiController]
    [Route("api/generos")]
    public class GenerosController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GenerosController(ApplicationDbContext context ,IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task <ActionResult<List<GeneroDTO>>> Get()
        {
            var Generos = await context.Generos.ToListAsync();
            return  mapper.Map<List<GeneroDTO>>(Generos);
            
        }
        [HttpGet("{id:int}",Name ="obtenerGenero")]
        public async Task<ActionResult<GeneroDTO>> GetbyId(int id)
        {
            var Genero = await context.Generos.FirstAsync(x=>x.Id==id);
            return mapper.Map<GeneroDTO>(Genero);
        }

        [HttpPost]
        public async Task<ActionResult<GeneroDTO>> Post([FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            var genero = mapper.Map<Genero>(generoCreacionDTO);
            context.Add(genero);
             await context.SaveChangesAsync();
            var generoDTO = mapper.Map<GeneroDTO>(genero);
            return new CreatedAtRouteResult("obtenerGenero", new { id = generoDTO.Id }, generoDTO);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id,GeneroCreacionDTO generoCreacionDTO)
        {
            var entidad = mapper.Map<Genero>(generoCreacionDTO);
            entidad.Id = id;

            context.Entry(entidad).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent(); 

        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteById(int id)
        {
            var existe = await context.Generos.FirstOrDefaultAsync(x=>x.Id==id);
            if (existe==null)
                return NotFound();

            context.Generos.Remove(existe);
            context.SaveChanges();
            return Ok("Eliminado genero");

        }

       
    }
}
