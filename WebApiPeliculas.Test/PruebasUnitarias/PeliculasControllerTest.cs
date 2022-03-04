using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPeliculas.Controllers;
using WebApiPeliculas.DTOS;
using WebApiPeliculas.Entidades;

namespace WebApiPeliculas.Test.PruebasUnitarias
{
    [TestClass]
    public class PeliculasControllerTest : BasePruebas
    {
        private string CrearDataPrueba()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = ConstruirContext(databaseName);
            var genero = new Genero() { Nombre = "genre 1" };

            var peliculas = new List<Pelicula>()
            {
                new Pelicula(){Titulo = "Película 1", FechaEstreno = new DateTime(2010, 1,1), EnCines = false},
                new Pelicula(){Titulo = "No estrenada", FechaEstreno = DateTime.Today.AddDays(1), EnCines = false},
                new Pelicula(){Titulo = "Película en Cines", FechaEstreno = DateTime.Today.AddDays(-1), EnCines = true}
            };

            var peliculaConGenero = new Pelicula()
            {
                Titulo = "Película con Género",
                FechaEstreno = new DateTime(2010, 1, 1),
                EnCines = false
            };
            peliculas.Add(peliculaConGenero);

            context.Add(genero);
            context.AddRange(peliculas);
            context.SaveChanges();

            var peliculaGenero = new PeliculasGeneros() { GeneroID = genero.Id, PeliculaID = peliculaConGenero.Id };
            context.Add(peliculaGenero);
            context.SaveChanges();

            return databaseName;
        }

        [TestMethod]
        public async Task FiltroPorTitulo()
        {
            var nombreBD = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreBD);

            var controller = new PeliculaController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var tituloPelicula = "Película 1";
            var filtroDto = new FiltroPeliculaDTO()
            {
                Titulo = tituloPelicula,
                CantidadDeRegistrosPorPagina = 10
            };
            var respuesta = await controller.Filtrar(filtroDto);
            var peliculas = respuesta.Value;
            if (peliculas != null)
            {

                Assert.AreEqual(1, peliculas.Count);
                Assert.AreEqual("Película 1", peliculas[0].Titulo);

            }

        }
        [TestMethod]
        public async Task FiltrarEnCines()
        {
            var nombreBD = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreBD);

            var controller = new PeliculaController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDTO = new FiltroPeliculaDTO()
            {
                EnCines = true

            };
            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;
            if (peliculas != null)
            {

                Assert.AreEqual(1, peliculas.Count);
                Assert.AreEqual("Película en Cines", peliculas[0].Titulo);

            }
        }
        [TestMethod]
        public async Task FiltrarProximosEstrenos()
        {
            var nombreBD = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreBD);

            var controller = new PeliculaController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDTO = new FiltroPeliculaDTO()
            {
                ProximosExtrenoss = true

            };
            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;
            if (peliculas != null)
            {

                Assert.AreEqual(1, peliculas.Count);
                Assert.AreEqual("No estrenada", peliculas[0].Titulo);

            }
        }
        [TestMethod]
        public async Task FiltrarPorGenero()
        {
            var nombreBD = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreBD);

            var controller = new PeliculaController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var generoID = contexto.Generos.Select(x => x.Id).First();
            var filtroDTO = new FiltroPeliculaDTO()
            {
                GeneroID = generoID
            };
            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;
            Assert.AreEqual(1, peliculas.Count);
            Assert.AreEqual("Película con Género", peliculas[0].Titulo);
        }
        [TestMethod]
        public async Task FiltrarOrdenaTituloAcendente()
        {
            var nombreBD = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreBD);

            var controller = new PeliculaController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDTO = new FiltroPeliculaDTO()
            {
                CampoOrdenar = "titulo",
                OrdenAsendente = true
            };
            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;

            var contexto2 = ConstruirContext(nombreBD);
            var peliculasDb = contexto2.Peliculas.OrderBy(x => x.Titulo).ToList();
            Assert.AreEqual(peliculasDb.Count, peliculas.Count);
            for (int i=0;i<peliculas.Count ;i++ )
            {
                var peliculaDelControlador  = peliculas[i];
                var PeliculaDB = peliculasDb[i];
                Assert.AreEqual(PeliculaDB.Id, peliculaDelControlador.Id);
            }
        }
        [TestMethod]
        public async Task FiltrarTituloDecendente()
        {
            var nombreBD = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreBD);

            var controller = new PeliculaController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDTO = new FiltroPeliculaDTO()
            {
                CampoOrdenar = "titulo",
                OrdenAsendente = false
            };
            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;

            var contexto2 = ConstruirContext(nombreBD);
            var peliculasDb = contexto2.Peliculas.OrderByDescending(x => x.Titulo).ToList();
            Assert.AreEqual(peliculasDb.Count, peliculas.Count);
            for (int i = 0; i < peliculas.Count; i++)
            {
                var peliculaDelControlador = peliculas[i];
                var PeliculaDB = peliculasDb[i];
                Assert.AreEqual(PeliculaDB.Id, peliculaDelControlador.Id);
            }
        }
        [TestMethod]
        public async Task FiltrarPorCampoIncorrectoDevuelvePeliculas()
        {
            var nombreBD = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreBD);

            var moq = new Mock<ILogger<PeliculaController>>();

            var controller = new PeliculaController(contexto, mapper, null, moq.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDTO = new FiltroPeliculaDTO()
            {
                CampoOrdenar = "abc",
                OrdenAsendente = true
            };
            var respuesta= await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;
            var contexto2 = ConstruirContext(nombreBD);

            var peliculasDb = contexto2.Peliculas.ToList();
            Assert.AreEqual(peliculasDb.Count,peliculas.Count);
            Assert.AreEqual(1,moq.Invocations.Count);
        }
    }

}
