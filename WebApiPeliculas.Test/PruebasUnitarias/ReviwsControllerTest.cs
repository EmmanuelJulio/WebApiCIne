using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class ReviwsControllerTest :  BasePruebas
    {
        [TestMethod]
        public async Task UsuarioNoPuedeCrear2ReviwsParaLaMismaPelicula()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            CrearPeliculas(nombreDB);

            var peliculaID= contexto.Peliculas.Select(x=>x.Id).First();

            var review1 = new Review()
            {
                PeliculaID = peliculaID,
                UsuarioID = usuarioPorDefectoId,
                Puntuacion = 5
            };
            contexto.Add(review1);
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();


            var controller = new ReviewController(contexto, mapper);
            controller.ControllerContext = ConstruirControllerContext();

            var revewCreacionDTO =new ReviewCreacionDTO() { Puntuacion = 5 };
            var respuesta = await controller.Post(peliculaID, revewCreacionDTO);

            var resultado = respuesta as IStatusCodeActionResult;

            Assert.AreEqual(400, resultado.StatusCode.Value);

        }
        [TestMethod]
        public async Task ReviewCreacion()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            CrearPeliculas(nombreDB);

            var peliculaID = contexto.Peliculas.Select(x => x.Id).First();

            var contexto2 = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();
            var controller =new  ReviewController(contexto2, mapper);
            controller.ControllerContext = ConstruirControllerContext();
            var reviewCreacionDto = new ReviewCreacionDTO() { Puntuacion = 5 };
            var respuesta = await controller.Post(peliculaID, reviewCreacionDto);

            var resultado = respuesta as NoContentResult;
            Assert.IsNotNull(resultado);

            var contexto3 = ConstruirContext(nombreDB);
            var reviewdb = contexto3.Reviews.First();
            Assert.AreEqual(usuarioPorDefectoId,reviewdb.UsuarioID);



        }
        private void CrearPeliculas(string nombreDb)
        {
            var contexto = ConstruirContext(nombreDb);
            contexto.Peliculas.Add(new Pelicula() { Titulo = "Pelicula 1" });
            contexto.SaveChanges();
        }
    }
}
