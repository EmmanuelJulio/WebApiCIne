using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPeliculas.DTOS;
using WebApiPeliculas.Entidades;

namespace WebApiPeliculas.Controllers
{
    [ApiController]
    [Route("api/generos")]
    public class GenerosController : CustomBaseController
    {
        public GenerosController(ApplicationDbContext context ,IMapper mapper)
            : base( context, mapper  )
        {
         
        }
        [HttpGet]
        public async Task <ActionResult<List<GeneroDTO>>> Get()
        {
            return await Get<Genero, GeneroDTO>();
            
        }
        [HttpGet("{id:int}",Name ="obtenerGenero")]
        public async Task<ActionResult<GeneroDTO>> GetbyId(int id)
        {
            return await Get<Genero, GeneroDTO>(id);
        }

        [HttpPost]
        public async Task<ActionResult<GeneroDTO>> Post([FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
           return await Post<GeneroCreacionDTO, Genero,GeneroDTO>(generoCreacionDTO, "obtenerGenero");
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id,GeneroCreacionDTO generoCreacionDTO)
        {

            return await Put<GeneroCreacionDTO,Genero>(id, generoCreacionDTO);

        }
        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles="Admin")]
        public async Task<ActionResult> DeleteById(int id)
        {
          return await Delete<Genero>(id);

        }

       
    }
}
