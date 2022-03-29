using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiPeliculas.DTOS;
using WebApiPeliculas.Entidades;

namespace WebApiPeliculas.Test.PruebasDeIntegracion
{
    [TestClass]
    public class ReviewControllerTest : BasePruebas
    {
        private static readonly string Url = "/api/peliculas/1/reviews";
        [TestMethod]
        public async Task ObtenerReviewsDevuelve404PeliculaInexistente()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);
            var cliente = factory.CreateClient();
            var respuesta = await cliente.GetAsync(Url);
            Assert.AreEqual(404,(int)respuesta.StatusCode);
        } 
        [TestMethod]
        public async Task ObtenerReviewsDevuelveListadoVacio()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);
            var context = ConstruirContext(nombreDB);
            context.Peliculas.Add(new Pelicula() { Titulo = "Pelicula 1" });
            var cliente = factory.CreateClient();
            var respuesta = await cliente.GetAsync(Url);
            respuesta.EnsureSuccessStatusCode();
            

            var reviews = JsonConvert.DeserializeObject<List<ReviewDTO>>(await respuesta.Content.ReadAsStringAsync());
            Assert.AreEqual(0,reviews.Count);
        }

    }
}
