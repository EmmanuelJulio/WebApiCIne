

using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApiPeliculas.DTOS;
using WebApiPeliculas.Entidades;
using WebApiPeliculas.Helpers;

namespace WebApiPeliculas.Controllers
{
    [ApiController]
    [Route("api/peliculas/{peliculaId:int}/reviews")]
    [ServiceFilter(typeof(PeliculaExisteAttribute))]
    public class ReviewController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ReviewController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReviewDTO>>> Get(int peliculaId,[FromQuery] PaginacionDTO paginacionDTO)
        {
          

            var queryable = context.Reviews.Include(x => x.Usuario).AsQueryable();
            queryable= queryable.Where(x=>x.PeliculaID==peliculaId);
            return await Get<Review, ReviewDTO>(paginacionDTO, queryable);
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int peliculaId,[FromBody]ReviewCreacionDTO reviewCreacionDTO)
        {
           

            var usuarioId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            var ReviewExiste = await context.Reviews.
                AnyAsync(x => x.PeliculaID == peliculaId && x.UsuarioID == usuarioId);

            if (ReviewExiste)
                return BadRequest("El usuario ya escribio una review de esta pelicula");

            var review = mapper.Map<Review>(reviewCreacionDTO);
            review.PeliculaID=peliculaId;
            review.UsuarioID=usuarioId;
            context.Add(review);
            await context.SaveChangesAsync();
            return NoContent();

        }
        [HttpPut("{reviewId:int}")]
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Put(int peliculaId,int reviewId,[FromBody]ReviewCreacionDTO reviewCreacionDTO)
        {
         

            var ReviewDB = await context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);

            if (ReviewDB == null)
                return NotFound();

            var usuarioId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (ReviewDB.UsuarioID != usuarioId) { return Forbid(); }

            ReviewDB = mapper.Map(reviewCreacionDTO, ReviewDB);
            await context.SaveChangesAsync();
            return NoContent();

        }
        [HttpDelete("{reviewId:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Delete(int reviewId)
        {
            var ReviewDB = await context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);

            if (ReviewDB == null)
                return NotFound();

            var usuarioId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (ReviewDB.UsuarioID != usuarioId) { return Forbid(); }

            context.Remove(ReviewDB);
            await context.SaveChangesAsync();
            return NoContent();
        }


    }
}
