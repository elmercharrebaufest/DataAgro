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
    public class ProvinciaControllerTest
    {
        private ProvinciaController target;
        private Mock<IProvinciaManager> provinciaManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            HttpContext.Current.Session["perfil"] = 1;
            provinciaManagerMock = new Mock<IProvinciaManager>();
            target = new ProvinciaController(provinciaManagerMock.Object);
        }

        [Test]
        public void BuscarProvinciaTest()
        {
            provinciaManagerMock.Setup(x => x.TraerTodoProvincia()).Returns(new ResultIniProvincia
            {
                Provincia = new List<ProvinciaIni>()
            { new ProvinciaIni{
                ProvinciaId = 1,
                Nombre = "A"
            }}
            });
            var result = target.Aplicar(new AbmProvinciaParam { ProvinciaId = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Provincia\":null,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void AplicarProvinciaTest()
        {
            provinciaManagerMock.Setup(x => x.TraerProvincia(1)).Returns(new ProvinciaDto
            {
                ProvinciaId = 1,
                Nombre = "A"
            }
            );
            var result = target.Aplicar(new AbmProvinciaParam { ProvinciaId = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Provincia\":{\"ProvinciaId\":1,\"Nombre\":\"A\",\"HabilitadoVenta\":false,\"Inscripto\":false},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GrabarCondicionTest()
        {
            var provincia = new Provincia
            {
                ProvinciaId = 1,
                Nombre = "A"
            };
            provinciaManagerMock.Setup(x => x.GrabarProvincia(provincia)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Grabar(provincia);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Provincia\":{\"ProvinciaId\":0,\"Nombre\":\"\",\"Orden\":0,\"HabilitadoVenta\":false,\"Inscripto\":false},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void EliminarCondicionTest()
        {
            provinciaManagerMock.Setup(x => x.EliminarProvincia(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Eliminar(new AbmProvinciaParam { ProvinciaId = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void CancelarCondicionTest()
        {
            var result = target.Cancelar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Provincia\":{\"ProvinciaId\":0,\"Nombre\":\"\",\"Orden\":0,\"HabilitadoVenta\":false,\"Inscripto\":false},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
