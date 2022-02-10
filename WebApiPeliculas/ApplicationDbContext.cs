﻿using Microsoft.EntityFrameworkCore;
using WebApiPeliculas.Entidades;

namespace WebApiPeliculas
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PeliculasActores>()
                .HasKey(x => new { x.ActorID, x.PeliculaID });
            modelBuilder.Entity<PeliculasGeneros>()
                .HasKey(x=> new {x.GeneroID, x.PeliculaID });

            base.OnModelCreating(modelBuilder);
        }
        private void SeedData(ModelBuilder modelBuilder)
        {

            //var rolAdminId = "9aae0b6d-d50c-4d0a-9b90-2a6873e3845d";
            //var usuarioAdminId = "5673b8cf-12de-44f6-92ad-fae4a77932ad";

            //var rolAdmin = new IdentityRole()
            //{
            //    Id = rolAdminId,
            //    Name = "Admin",
            //    NormalizedName = "Admin"
            //};

            //var passwordHasher = new PasswordHasher<IdentityUser>();

            //var username = "felipe@hotmail.com";

            //var usuarioAdmin = new IdentityUser()
            //{
            //    Id = usuarioAdminId,
            //    UserName = username,
            //    NormalizedUserName = username,
            //    Email = username,
            //    NormalizedEmail = username,
            //    PasswordHash = passwordHasher.HashPassword(null, "Aa123456!")
            //};

            //modelBuilder.Entity<IdentityUser>()
            //    .HasData(usuarioAdmin);

            //modelBuilder.Entity<IdentityRole>()
            //    .HasData(rolAdmin);

            //modelBuilder.Entity<IdentityUserClaim<string>>()
            //    .HasData(new IdentityUserClaim<string>()
            //    {
            //        Id = 1,
            //        ClaimType = ClaimTypes.Role,
            //        UserId = usuarioAdminId,
            //        ClaimValue = "Admin"
            //    });

            //var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            //modelBuilder.Entity<SalaDeCine>()
            //   .HasData(new List<SalaDeCine>
            //   {
            //        //new SalaDeCine{Id = 1, Nombre = "Agora", Ubicacion = geometryFactory.CreatePoint(new Coordinate(-69.9388777, 18.4839233))},
            //        new SalaDeCine{Id = 4, Nombre = "Sambil", Ubicacion = geometryFactory.CreatePoint(new Coordinate(-69.9118804, 18.4826214))},
            //        new SalaDeCine{Id = 5, Nombre = "Megacentro", Ubicacion = geometryFactory.CreatePoint(new Coordinate(-69.856427, 18.506934))},
            //        new SalaDeCine{Id = 6, Nombre = "Village East Cinema", Ubicacion = geometryFactory.CreatePoint(new Coordinate(-73.986227, 40.730898))}
            //   });

            var aventura = new Genero() { Id = 4, Nombre = "Aventura" };
            var animation = new Genero() { Id = 5, Nombre = "Animación" };
            var suspenso = new Genero() { Id = 6, Nombre = "Suspenso" };
            var romance = new Genero() { Id = 7, Nombre = "Romance" };

            modelBuilder.Entity<Genero>()
                .HasData(new List<Genero>
                {
                    aventura, animation, suspenso, romance
                });

            var jimCarrey = new Actor() { id = 5, Nombre = "Jim Carrey", FechaNacimiento = new DateTime(1962, 01, 17) };
            var robertDowney = new Actor() { id = 6, Nombre = "Robert Downey Jr.", FechaNacimiento = new DateTime(1965, 4, 4) };
            var chrisEvans = new Actor() { id = 7, Nombre = "Chris Evans", FechaNacimiento = new DateTime(1981, 06, 13) };

            modelBuilder.Entity<Actor>()
                .HasData(new List<Actor>
                {
                    jimCarrey, robertDowney, chrisEvans
                });

            var endgame = new Pelicula()
            {
                Id = 2,
                Titulo = "Avengers: Endgame",
                EnCines = true,
                FechaEstreno = new DateTime(2019, 04, 26)
            };

            var iw = new Pelicula()
            {
                Id = 3,
                Titulo = "Avengers: Infinity Wars",
                EnCines = false,
                FechaEstreno = new DateTime(2019, 04, 26)
            };

            var sonic = new Pelicula()
            {
                Id = 4,
                Titulo = "Sonic the Hedgehog",
                EnCines = false,
                FechaEstreno = new DateTime(2020, 02, 28)
            };
            var emma = new Pelicula()
            {
                Id = 5,
                Titulo = "Emma",
                EnCines = false,
                FechaEstreno = new DateTime(2020, 02, 21)
            };
            var wonderwoman = new Pelicula()
            {
                Id = 6,
                Titulo = "Wonder Woman 1984",
                EnCines = false,
                FechaEstreno = new DateTime(2020, 08, 14)
            };

            modelBuilder.Entity<Pelicula>()
                .HasData(new List<Pelicula>
                {
                    endgame, iw, sonic, emma, wonderwoman
                });

            modelBuilder.Entity<PeliculasGeneros>().HasData(
                new List<PeliculasGeneros>()
                {
                    new PeliculasGeneros(){PeliculaID = endgame.Id, GeneroID = suspenso.Id},
                    new PeliculasGeneros(){PeliculaID = endgame.Id, GeneroID = aventura.Id},
                    new PeliculasGeneros(){PeliculaID = iw.Id, GeneroID = suspenso.Id},
                    new PeliculasGeneros(){PeliculaID = iw.Id, GeneroID = aventura.Id},
                    new PeliculasGeneros(){PeliculaID = sonic.Id, GeneroID = aventura.Id},
                    new PeliculasGeneros(){PeliculaID = emma.Id, GeneroID = suspenso.Id},
                    new PeliculasGeneros(){PeliculaID = emma.Id, GeneroID = romance.Id},
                    new PeliculasGeneros(){PeliculaID = wonderwoman.Id, GeneroID = suspenso.Id},
                    new PeliculasGeneros(){PeliculaID = wonderwoman.Id, GeneroID = aventura.Id},
                }); 

            modelBuilder.Entity<PeliculasActores>().HasData(
                new List<PeliculasActores>()
                {
                    new PeliculasActores(){PeliculaID = endgame.Id, ActorID = robertDowney.id, Personaje = "Tony Stark", Orden = 1},
                    new PeliculasActores(){PeliculaID = endgame.Id, ActorID= chrisEvans.id, Personaje = "Steve Rogers", Orden = 2},
                    new PeliculasActores(){PeliculaID = iw.Id, ActorID = robertDowney.id, Personaje = "Tony Stark", Orden = 1},
                    new PeliculasActores(){PeliculaID = iw.Id, ActorID = chrisEvans.id, Personaje = "Steve Rogers", Orden = 2},
                    new PeliculasActores(){PeliculaID = sonic.Id, ActorID = jimCarrey.id, Personaje = "Dr. Ivo Robotnik", Orden = 1}
                });
        }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Actor> Actores { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<PeliculasActores> PeliculasActores { get; set; }
        public DbSet<PeliculasGeneros> PeliculasGeneros { get; set; }
    }
}
