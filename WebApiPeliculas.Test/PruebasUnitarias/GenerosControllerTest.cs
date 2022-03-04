using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class GenerosControllerTest : BasePruebas
    {
        

        [TestMethod]
        public async Task ObtenerTodosLosGeneros()
        {
            //preparacion
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            contexto.Generos.Add(new Genero { Nombre = "Genero 1" });
            contexto.Generos.Add(new Genero { Nombre = "Genero 2" });
            await contexto.SaveChangesAsync();
            var contexto2 = ConstruirContext(nombreBD);
            //Prueba
            var contoller = new GenerosController(contexto2,mapper);
            var respuesta = await contoller.Get();
            ///Verificacion
            var generos = respuesta.Value;
            Assert.AreEqual(2,generos.Count);
        }
       
        [TestMethod]
        public async Task ObtenerGeneroPorIdInexistente()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            var controler = new GenerosController(contexto, mapper);
            var respuesta = await controler.GetbyId(1);
            var resultado = respuesta.Result as StatusCodeResult;
            Assert.AreEqual(404, resultado.StatusCode);
        }
        [TestMethod]
        public async Task ObtenerGeneroPorIdExistente()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

       
            contexto.Add(new Genero() { Nombre = "Genero1" });
            await contexto.SaveChangesAsync();


          

            var contexto2 = ConstruirContext(nombreBD);
            var controller2 = new GenerosController(contexto2, mapper);

            var id = 1;
            var respuesta = await controller2.GetbyId(id);
            var resultado = respuesta.Value;
            Assert.AreEqual(id,resultado.Id);

        }
        [TestMethod]
        public async Task CrearGenero()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            var nuevoGenero = new GeneroCreacionDTO()
            {
                Nombre = "nuevo Genero"
            };
            var controller = new GenerosController(contexto, mapper);
            var respuesta = await controller.Post(nuevoGenero);
            var resultado = respuesta.Result as CreatedAtRouteResult;
            Assert.IsNotNull(resultado);

            var contexto2 = ConstruirContext(nombreBD);
            var cantidad = await contexto2.Generos.CountAsync();
            Assert.AreEqual(1, cantidad);
         }
        [TestMethod]
        public async Task ActualizarGenero()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            contexto.Generos.Add(new Genero() { Nombre = "genero 1" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBD);

            var controller = new GenerosController(contexto2,mapper);

            var generoCreacionDto = new GeneroCreacionDTO() { Nombre = "Nuevo nombre" };
            var id = 1;
            var respuesta = await controller.Put(id, generoCreacionDto);
            
            var  resultado = respuesta as StatusCodeResult;

            Assert.AreEqual(204, resultado.StatusCode);
            var context3 = ConstruirContext(nombreBD);
            var existe = await context3.Generos.AnyAsync(x => x.Nombre == "Nuevo nombre");
            Assert.IsTrue(existe);
        }
        [TestMethod]
        public async Task IntentaBorrarGeneroNoExistente()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            var controller =new GenerosController(contexto,mapper);
            var respuesta = controller.DeleteById(1);

            var resultado = respuesta.Result as StatusCodeResult;

            Assert.AreEqual(404, resultado.StatusCode);


        }
        [TestMethod]
        public async Task IntentaBorrarGeneroExistente()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();
            contexto.Add(new Genero() { Nombre = "Genero1" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBD);
            var controller = new GenerosController(contexto2, mapper);
            var respuesta =  await controller.DeleteById(1);

            var resultado = respuesta as StatusCodeResult;

            Assert.AreEqual(200, resultado.StatusCode);

            var contexto3 = ConstruirContext(nombreBD);
            var Existe =  await contexto3.Generos.AnyAsync();
            Assert.IsFalse(Existe);


        }
    }
}
