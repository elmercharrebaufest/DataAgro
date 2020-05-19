using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using WebDataAgro.Models;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class LocalidadControllerTest
    {
        private LocalidadController target;
        private Mock<ILocalidadManager>localidadManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();            
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            HttpContext.Current.Session["perfil"] = 1;
            localidadManagerMock = new Mock<ILocalidadManager>();
            target = new LocalidadController(localidadManagerMock.Object);
        }

        [Test]
        public void FiltrarLocalidadTest()
        {
            var localidadFiltro = new ParamAbmLocalidad() { Nombre = "A", ProvinciaId = 1 };
            localidadManagerMock.Setup(x => x.TraerFiltroLocalidad(localidadFiltro)).Returns(new ResultIniLocalidad
            {
                Localidad = new List<LocalidadIni>()
                {
                    new LocalidadIni()
                    {
                        LocalidadId = 1,
                        Nombre= "A",
                        ProNombre="A",
                        CodLocalidad = "A"
                    }
                }
            });
            var result = target.Filtrar(localidadFiltro);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":[{\"LocalidadId\":1,\"CodLocalidad\":\"A\",\"Nombre\":\"A\",\"ProNombre\":\"A\"}],\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void AplicarCondicionTest()
        {
            localidadManagerMock.Setup(x => x.TraerLocalidad(1)).Returns(new LocalidadDto
            {
                LocalidadId = 1,
                Nombre = "A",
                ProvinciaId = 1,
                CodLocalidad = "A"
            }
            );
            var result = target.Aplicar(new AbmLocalidadParam { LocalidadId = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Localidad\":{\"LocalidadId\":1,\"Nombre\":\"A\",\"CodLocalidad\":\"A\",\"ProvinciaId\":1,\"Provincia_Nombre\":null,\"Partido_Nombre\":null,\"PartidoId\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GrabarCondicionTest()
        {
            var localidad = new Localidad
            {
                LocalidadId = 1,
                Nombre = "A",
                ProvinciaId = 1,
                CodLocalidad = "A"
            };
            localidadManagerMock.Setup(x => x.GrabarLocalidad(localidad)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Grabar(localidad);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Localidad\":{\"LocalidadId\":0,\"CodLocalidad\":\"\",\"Nombre\":\"\",\"ProvinciaId\":0,\"PartidoId\":null,\"Provincia\":null,\"Partido\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void EliminarCondicionTest()
        {
            localidadManagerMock.Setup(x => x.EliminarLocalidad(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Eliminar(new AbmLocalidadParam { LocalidadId = 1});

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"Result\":{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null},\"Id\":1,\"Exception\":null,\"Status\":5,\"IsCanceled\":false,\"IsCompleted\":true,\"CreationOptions\":0,\"AsyncState\":null,\"IsFaulted\":false}",
                a);
        }

        [Test]
        public void CancelarCondicionTest()
        {
            var result = target.Cancelar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Localidad\":{\"LocalidadId\":0,\"CodLocalidad\":\"\",\"Nombre\":\"\",\"ProvinciaId\":0,\"PartidoId\":null,\"Provincia\":null,\"Partido\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
