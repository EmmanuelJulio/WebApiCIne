using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using WebApiPeliculas.DTOS;
using WebApiPeliculas.Entidades;

namespace WebApiPeliculas.Controllers
{
    [Route("api/SalasDeCine")]
    [ApiController]
    public class SalasDeCineController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly GeometryFactory geometryFactory;

        public SalasDeCineController(ApplicationDbContext context, IMapper mapper,GeometryFactory geometryFactory) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.geometryFactory = geometryFactory;
        }
        [HttpGet]
        public async Task<ActionResult<List<SalaDeCineDTO>>> Get()
        {
            return await Get<SalaDeCine, SalaDeCineDTO>();
        }
        [HttpGet("Cercanos")]
        public async Task<ActionResult<List<SalaDeCineCercanoDTO>>> 
            GetCercano([FromQuery]SalaDeCineCercanoFiltroDTO Filtro)
        {
            var ubicacionUsuario = geometryFactory.CreatePoint(new Coordinate (Filtro.Longitud, Filtro.Latitud ));
            var SalasDeCine = await context.salaDeCines
                                    .OrderBy(x => x.Ubicacion.Distance(ubicacionUsuario))
                                    .Where(x => x.Ubicacion.IsWithinDistance(ubicacionUsuario, Filtro.DistanciaEnKms * 1000))
                                    .Select(x => new SalaDeCineCercanoDTO
                                    {
                                        Id = x.Id,
                                        Nombre = x.Nombre,
                                        Latitud = x.Ubicacion.Y,
                                        Longitud = x.Ubicacion.X,
                                        DistanciaEnMetros = Math.Round(x.Ubicacion.Distance(ubicacionUsuario))
                                    }).ToListAsync();
            return SalasDeCine;
        }

        [HttpGet("{id:int}", Name = "obtenerSalaDeCine")]
        public async Task<ActionResult<SalaDeCineDTO>> Get(int id)
        {
            return await Get<SalaDeCine, SalaDeCineDTO>(id);
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SalaDeCineCreacionDTO salaDeCineCreacionDTO)
        {
            return await Post<SalaDeCineCreacionDTO, SalaDeCine, SalaDeCineDTO>(salaDeCineCreacionDTO, "obtenerSalaDeCine");
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id,[FromBody]SalaDeCineCreacionDTO salaDeCineCreacionDTO)
        {
            return await Put<SalaDeCineCreacionDTO, SalaDeCine>(id,salaDeCineCreacionDTO);
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<SalaDeCine>(id);
        }

        
    }
}
