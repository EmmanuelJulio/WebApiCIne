using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPeliculas.DTOS;
using WebApiPeliculas.Entidades;
using WebApiPeliculas.Helpers;

namespace WebApiPeliculas.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public CustomBaseController(ApplicationDbContext context , IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        protected async Task<List<TDTO>> Get<TEntidad,TDTO>() where TEntidad : class

        {
            var entidades = await context.Set<TEntidad>().ToListAsync();
             var dtos = mapper.Map<List<TDTO>>(entidades);
            return dtos;
        }
        protected async Task<ActionResult<TDTO>> Get<TEntidad, TDTO>(int id) where TEntidad : class,IId
        {
            var entidad = await context.Set<TEntidad>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (entidad == null)
                return NotFound();

            return mapper.Map<TDTO>(entidad);


        }

        protected async Task<ActionResult> Post<TCreacion,TEntidad, TLectura>
            (TCreacion creacionDTO,string nombreRuta) where TEntidad : class, IId
        {
            var entidad = mapper.Map<TEntidad>(creacionDTO);
            context.Add(entidad);
            await context.SaveChangesAsync();
            var dtoLectura = mapper.Map<TLectura>(entidad);
            return new CreatedAtRouteResult(nombreRuta, new { id = entidad.Id }, dtoLectura);
        }
        protected async Task<ActionResult> Put<TCreacion, TEntidad>
           (int id, TCreacion creacionDTO) where TEntidad : class, IId
        {

            var entidad = mapper.Map<TEntidad>(creacionDTO);
            entidad.Id = id;

            context.Entry(entidad).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        protected async Task<ActionResult> Patch<TEntidad,TDTO>(int id,JsonPatchDocument<TDTO> patchDocument) 
            where TDTO :class
            where TEntidad : class, IId
        {
            if (patchDocument == null)
                return BadRequest();

            var entidadDB = await context.Set<TEntidad>().FirstOrDefaultAsync(x => x.Id == id);
            if (entidadDB == null)
                return NotFound();

            var entidadDTo = mapper.Map<TDTO>(entidadDB);
            try
            {

                patchDocument.ApplyTo(entidadDTo, ModelState);

                var esValido = TryValidateModel(entidadDTo);
                if (esValido == false)
                    return BadRequest(ModelState);

                mapper.Map(entidadDTo, entidadDB);
                await context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }

        }
        protected async Task<ActionResult> Delete<TEntidad>(int id) where TEntidad : class, IId,new()
        {
            var existe = await context.Set<TEntidad>().FirstOrDefaultAsync(x => x.Id == id);
            if (existe == null)
                return NotFound();

            context.Set<TEntidad>().Remove(existe);
            context.SaveChanges();
            return Ok();
        }
        protected async Task <List<TDTO>> Get<TEntidad,TDTO>(PaginacionDTO paginacionDTO)  where TEntidad : class
        {
            var queryable = context.Set<TEntidad>().AsQueryable();
            return await Get<TEntidad, TDTO>(paginacionDTO, queryable);

           
        }
        protected async Task<List<TDTO>> Get<TEntidad, TDTO>(PaginacionDTO paginacionDTO,IQueryable<TEntidad> queryable ) where TEntidad : class
        {
            
            await HttpContext.InsertarParametrosPaginacion(queryable, paginacionDTO.CantidadRegistrosPorPagina);
            var entidad = await queryable.Paginar(paginacionDTO).ToListAsync();
            return mapper.Map<List<TDTO>>(entidad);


        }
    }
}
