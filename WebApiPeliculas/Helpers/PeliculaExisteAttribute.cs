using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace WebApiPeliculas.Helpers
{
    public class PeliculaExisteAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly ApplicationDbContext Dbcontext;

        public PeliculaExisteAttribute(ApplicationDbContext context)
        {
            this.Dbcontext = context;
        }
        public  async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var peliculaIdObjet = context.HttpContext.Request.RouteValues["peliculaId"];
            if (peliculaIdObjet == null)
                return;

            var peliculaId = int.Parse(peliculaIdObjet.ToString());

            var existePelicula = await Dbcontext.Peliculas.AnyAsync(x => x.Id == peliculaId);
            if (!existePelicula)
                context.Result = new NotFoundResult();
            else
                await next();

        }
    }
}
