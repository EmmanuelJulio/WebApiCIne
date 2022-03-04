using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite;
using NetTopologySuite.Geometries;
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
    public class SalasDeCineControllerTest : BasePruebas
    {
        [TestMethod]
        public async Task ObtenerSalasDeCineA5KilometrosOMenos()
        {
            var geometriFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            using (var context = LocalDbDatabaseInitializer.GetDbContextLocalDb(false))
            {
                var salasDeCine = new List<SalaDeCine>()
                {
                    new SalaDeCine{ Nombre = "Agora", Ubicacion = geometriFactory.CreatePoint(new Coordinate(-69.9388777, 18.4839233)) }
                };

                context.AddRange(salasDeCine);
                await context.SaveChangesAsync();
            }
            var Filtro = new SalaDeCineCercanoFiltroDTO()
            {
                DistanciaEnKms = 5,
                Latitud = 18.481139,
                Longitud = -69.938950
            };
            using (var  context  = LocalDbDatabaseInitializer.GetDbContextLocalDb(false))
            {
                var mapper = ConfigurarAutoMapper();
                var controller = new SalasDeCineController(context,mapper,geometriFactory);
                var respuesta = await  controller.GetCercano(Filtro);
                var valor = respuesta.Value;
                if(valor != null)
                {

                Assert.AreEqual(2, valor.Count);
                }
            }
        }
    }
}
