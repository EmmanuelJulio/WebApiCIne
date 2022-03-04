using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPeliculas.Controllers;
using WebApiPeliculas.DTOS;
using WebApiPeliculas.Entidades;
using WebApiPeliculas.Servicios;

namespace WebApiPeliculas.Test.PruebasUnitarias
{
    [TestClassAttribute]
    public class ActoresControllerTest : BasePruebas
    {
        [TestMethod]
        public async Task ObtenerPersonasPaginadas()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto=  ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            contexto.Actores.Add(new Actor() { Nombre = "Actor 1" });
            contexto.Actores.Add(new Actor() { Nombre = "Actor 2" });
            contexto.Actores.Add(new Actor() { Nombre = "Actor 3" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBD);
            var controller = new ActoresController(contexto2, mapper,null);

            //aca definimos un contexto http para que se pueda realizar la ejecucion puesto que estamos
            //llamando al controller desde un metodo y no desde un contexto http y este metodo
            //puntualmente requiere eso
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var pagina1 = await  controller.Get(new PaginacionDTO() { CantidadRegistrosPorPagina=2,Pagina=1});
            var actoresPagina1 = pagina1.Value;

            Assert.AreEqual(2, actoresPagina1.Count);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var pagina2 = await controller.Get(new PaginacionDTO() { CantidadRegistrosPorPagina = 2, Pagina = 2 });
            
            var actoresPagina2=  pagina2.Value;

            Assert.AreEqual(1,actoresPagina2.Count);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var pagina3 = await controller.Get(new PaginacionDTO() { CantidadRegistrosPorPagina = 2, Pagina = 3 });
            var actoresPagina3= pagina3.Value;
            Assert.AreEqual(0, actoresPagina3.Count);

        }
        [TestMethod]
        public async Task CrearActorSinFoto()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            var actor = new ActorCreacionDTO() { Nombre="Emmanuel",FechaNacimiento=DateTime.Now};
             var Moq = new Mock<IAlmacenadorArchivos>();
            Moq.Setup(x => x.GuardarArchivo(null, null, null,null))
                .Returns(Task.FromResult("url"));
            var controller = new ActoresController(contexto, mapper, Moq.Object);

            var respuesta = await  controller.Post(actor);
            var resultado = respuesta as CreatedAtRouteResult;
            Assert.AreEqual(201, resultado.StatusCode);

            var contexto2 = ConstruirContext(nombreBD);
            var listado = await contexto2.Actores.ToListAsync();
            Assert.AreEqual(1,listado.Count);
            Assert.IsNull(listado[0].Foto);
            Assert.AreEqual(0,Moq.Invocations.Count);
        }
        [TestMethod]

        public async Task CrearActorConFoto()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            var content =Encoding.UTF8.GetBytes("Imagen de prueba");
            var archivo = new FormFile(new MemoryStream(content), 0, content.Length, "Data", "Imagen.jpg");
            archivo.Headers=new HeaderDictionary();
            archivo.ContentType = "image/jpg";
            var actor = new ActorCreacionDTO() { Nombre="Nuevo actor",FechaNacimiento=DateTime.Now,Foto=archivo};
            var mok = new Mock<IAlmacenadorArchivos>();
            mok.Setup(x=>x.GuardarArchivo(content,".jpg","actores",archivo.ContentType))
                .Returns(Task.FromResult("Url"));

            var controller = new ActoresController(contexto, mapper, mok.Object);
            var respuesta = await controller.Post(actor);
            var resultado = respuesta as CreatedAtRouteResult;
            Assert.AreEqual(201, resultado.StatusCode);

            var contexto2 = ConstruirContext(nombreBD);
            var listado =await contexto2.Actores.ToListAsync();
            Assert.AreEqual(1,listado.Count);
            Assert.AreEqual("Url", listado[0].Foto);
            Assert.AreEqual(1,mok.Invocations.Count);
        }
        [TestMethod]
        public async Task PathRetorna404siActorNoExiste()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            var controller = new ActoresController(contexto, mapper, null);
            var patchDoc= new JsonPatchDocument<ActorPathDTO>();
            var respuesta=await controller.Patch(1,patchDoc);
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(404,resultado.StatusCode);
        }
        [TestMethod]
        public async Task PatchActualizaUnSoloCampo()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();
            
            var fechaNacimiento = DateTime.Now;
            var actor = new Actor()
            {
                Nombre = "Emmanuel",
                FechaNacimiento = fechaNacimiento
            };
            contexto.Actores.Add(actor);
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBD);
            var controller = new ActoresController(contexto2, mapper, null);

            var objetValidator = new Mock<IObjectModelValidator>();
            objetValidator.Setup(x => x.Validate(
                It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<object>()));
            controller.ObjectValidator=objetValidator.Object;

            var jsonPatchDocument = new JsonPatchDocument<ActorPathDTO>();
            jsonPatchDocument.Operations.Add(new Operation<ActorPathDTO>("replace","/nombre",null,"Carlos"));
            var respuesta = await controller.Patch(1, jsonPatchDocument);
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado.StatusCode);
            var contexto3 = ConstruirContext(nombreBD);
            var actorDB = await contexto3.Actores.FirstAsync();
            Assert.AreEqual("Carlos", actorDB.Nombre);
            Assert.AreEqual(fechaNacimiento, actorDB.FechaNacimiento);

        }
    }
}
