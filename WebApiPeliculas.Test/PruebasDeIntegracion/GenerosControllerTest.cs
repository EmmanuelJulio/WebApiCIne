using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPeliculas.DTOS;
using WebApiPeliculas.Entidades;

namespace WebApiPeliculas.Test.PruebasDeIntegracion
{
    [TestClass]
    public class GenerosControllerTest : BasePruebas
    {
        private static readonly string url = "/api/generos";
        [TestMethod]
        public async Task ObtnerTodosLosGenerosListadoVacio()
        {
            var nombreDb = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDb);
            var cliente = factory.CreateClient();
            var respuesta = await cliente.GetAsync(url);

           respuesta.EnsureSuccessStatusCode();
            var generos = JsonConvert.DeserializeObject<List<GeneroDTO>>(await respuesta.Content.ReadAsStringAsync() );
            Assert.AreEqual(0, generos.Count);
        }
        [TestMethod]
        public async Task ObtnerTodosLosGeneros()
        {
            var nombreDb = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDb);
            var contexto = ConstruirContext(nombreDb);

            contexto.Generos.Add(new Genero() { Nombre = "Genero 1" });
            contexto.Generos.Add(new Genero() { Nombre = "Genero 2" });
            await contexto.SaveChangesAsync(); 


            var cliente = factory.CreateClient();
            var respuesta = await cliente.GetAsync(url);

           respuesta.EnsureSuccessStatusCode();
            var generos = JsonConvert.DeserializeObject<List<GeneroDTO>>(await respuesta.Content.ReadAsStringAsync() );
            Assert.AreEqual(2, generos.Count);
        }
        [TestMethod]
        public async Task BorrarGenero()
        {
            var nombreDb = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDb);
            var contexto = ConstruirContext(nombreDb);

            contexto.Generos.Add(new Genero() { Nombre = "Genero 1" });
            await contexto.SaveChangesAsync();

            var cliente = factory.CreateClient();

            var respuesta = await cliente.DeleteAsync($"{url}/1");

            respuesta.EnsureSuccessStatusCode();

            var contexto2 = ConstruirContext(nombreDb);
            var existe = await contexto2.Generos.AnyAsync();
            Assert.IsFalse(existe);

        }
        [TestMethod]
        public async Task BorrarGeneroRetorna401()
        {
            var nombreDb = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDb,ingnorarSeguridad:false);
           

           

            var cliente = factory.CreateClient();

            var respuesta = await cliente.DeleteAsync($"{url}/1");

           

         
       
            Assert.AreEqual("Unauthorized",respuesta.ReasonPhrase);

        }
    }
}
