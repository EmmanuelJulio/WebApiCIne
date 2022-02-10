using AutoMapper;
using WebApiPeliculas.DTOS;
using WebApiPeliculas.Entidades;

namespace WebApiPeliculas.Helpers
{
    public class AutomaperProfiles : Profile
    {
        public AutomaperProfiles()
        {

            //maper de generos
            CreateMap<Genero, GeneroDTO>().ReverseMap();

            CreateMap<GeneroCreacionDTO, Genero>();

            //maper de actores
            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<ActorCreacionDTO, Actor>()
                .ForMember(x => x.Foto, options => options.Ignore());

            CreateMap<ActorPathDTO, Actor>().ReverseMap();
            //maper de peliculas
            CreateMap<PeliculaDTO,Pelicula>().ReverseMap();
            CreateMap<PeliculaCreacionDTO, Pelicula>()
                .ForMember(x=>x.peliculasGeneros,option => option.MapFrom(MapPeliculasGeneros))
                .ForMember(x=>x.PeliculasActores,option => option.MapFrom(MapPeliculasActores))
                .ForMember(x => x.Poster, options => options.Ignore());

            CreateMap<PeliculaPatchDTO, Pelicula>().ReverseMap();
        }
        private List<PeliculasGeneros> MapPeliculasGeneros(PeliculaCreacionDTO peliculaCreacionDTO,Pelicula pelicula)
        {
            var resultado = new List<PeliculasGeneros>();
            if (peliculaCreacionDTO.GenerosIDs==null)
                return resultado;

            foreach (var id in peliculaCreacionDTO.GenerosIDs)
            {
                resultado.Add(new PeliculasGeneros { GeneroID = id });
            }
            return resultado;
        }

        private List<PeliculasActores> MapPeliculasActores(PeliculaCreacionDTO peliculaCreacionDTO ,Pelicula pelicula)
        {
            var resultado = new List<PeliculasActores>();
            if(peliculaCreacionDTO.Actores == null)
                return resultado;

            foreach(var actor in peliculaCreacionDTO.Actores)
            {
                resultado.Add(new PeliculasActores { ActorID = actor.ActorID, Personaje = actor.NombrePersonaje });
            }

            return resultado;
        }
    }
}
