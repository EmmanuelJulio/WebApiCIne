using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using WebApiPeliculas.DTOS;
using WebApiPeliculas.Entidades;

namespace WebApiPeliculas.Helpers
{
    public class AutomaperProfiles : Profile
    {
        public AutomaperProfiles(GeometryFactory geometryFactory)
        {
          
            //maper de generos
            CreateMap<Genero, GeneroDTO>().ReverseMap();

            CreateMap<GeneroCreacionDTO, Genero>();

            CreateMap<IdentityUser, UsuarioDTO>();

            CreateMap<Review, ReviewDTO>()
                .ForMember(x => x.NombreUsuario, x => x.MapFrom(y => y.Usuario.UserName));

            CreateMap<ReviewDTO, Review>();

            CreateMap<ReviewCreacionDTO,Review>();

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

            CreateMap<Pelicula, PeliculaDetallesDTO>()
                .ForMember(x => x.Generos, options => options.MapFrom(MapPeliculasGeneros))
                .ForMember(x => x.Actores, options => options.MapFrom(MapPeliculasActores));

            CreateMap<PeliculaPatchDTO, Pelicula>().ReverseMap();


            //salas de cine
            CreateMap<SalaDeCine, SalaDeCineDTO>()
                .ForMember(x => x.Latitud, x => x.MapFrom(y => y.Ubicacion.Y))
                .ForMember(x => x.Longitud,x=>x.MapFrom(y => y.Ubicacion.X));
            

            CreateMap<SalaDeCineDTO, SalaDeCine>()
                .ForMember(x=>x.Ubicacion,
                x=>x.MapFrom(y=>geometryFactory.CreatePoint(new Coordinate(y.Longitud,y.Latitud))));
              
            CreateMap<SalaDeCineCreacionDTO, SalaDeCine>()
                .ForMember(x => x.Ubicacion,
                x => x.MapFrom(y => geometryFactory.CreatePoint(new Coordinate(y.Longitud, y.Latitud))));
        }
        private List<ActorPDetalleDTO> MapPeliculasActores (Pelicula pelicula, PeliculaDetallesDTO peliculaDetallesDTO)
        {
            var resultado = new List<ActorPDetalleDTO> ();
            if(pelicula.PeliculasActores== null)
                return resultado;

            foreach (var actorPelicula in pelicula.PeliculasActores)
            {   
                resultado.Add(new ActorPDetalleDTO() { ActorId = actorPelicula.ActorID, Personaje = actorPelicula.Personaje, NombrePersona = actorPelicula.Actor.Nombre });

            }
            return resultado;
        }
        private List<GeneroDTO> MapPeliculasGeneros(Pelicula pelicula, PeliculaDetallesDTO peliculaDetallesDTO)
        {
            var resultado = new List<GeneroDTO>();
            if (pelicula.peliculasGeneros == null) 
                return resultado;

            foreach(var generoPelicula in pelicula.peliculasGeneros)
            {
                resultado.Add(new GeneroDTO() { Id = generoPelicula.GeneroID, Nombre = generoPelicula.Genero.Nombre });
            }
            return resultado;
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
