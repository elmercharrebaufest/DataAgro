using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using WebDataAgro.Models;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ZonaCupoControllerTest
    {
        private ZonaCupoController target;
        private Mock<IZonaCupoManager> zonaCupoManagerMock;
        private Mock<IComercialManager> comercialManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            zonaCupoManagerMock = new Mock<IZonaCupoManager>();
            comercialManagerMock = new Mock<IComercialManager>();
            target = new ZonaCupoController(zonaCupoManagerMock.Object, comercialManagerMock.Object);
        }

        [Test]
        public void InicializarTest()
        {
            zonaCupoManagerMock.Setup(x => x.TraerDatosIniciales()).Returns(new DatosIniAbmZonaCupo()
            {
                ZonaCupo = new List<ZonaCupoCombo>()
                    {
                        new ZonaCupoCombo
                        {
                            Id = 1,
                            CodigoSap = "A",
                            Descripcion = "A"
                        },
                    }
            });
            var result = target.Inicializar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":{\"ZonaCupo\":[{\"Id\":1,\"Descripcion\":\"A\",\"CodigoSap\":\"A\"}]},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void BuscarZonaTest()
        {
            zonaCupoManagerMock.Setup(x => x.TraerTodoZonaCupo()).Returns(new ResultIniZonaCupo
            {
                ZonaCupo = new List<ZonaCupoIni>()
                {
                    new ZonaCupoIni()
                    {
                        Id = 1,
                        CodigoSap = "A",
                        Descripcion = "A"
                    }
                }
            });
            var result = target.Buscar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":[{\"Id\":1,\"Descripcion\":\"A\",\"CodigoSap\":\"A\"}],\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void ZonaComboTest()
        {
            zonaCupoManagerMock.Setup(x => x.TraerZonaCupo(1)).Returns(new ZonaCupoDto
            {
                Id = 1,
                Descripcion = "A",
                CodigoSap = "A"
            }
            );
            var result = target.ZonaCupoCombo(new AbmZonaCupoParam { Id = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ZonaCupo\":{\"Id\":1,\"Descripcion\":\"A\",\"CodigoSap\":\"A\"},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void AplicarZonaCupoTest()
        {
            zonaCupoManagerMock.Setup(x => x.TraerZonaCupo(1)).Returns(new ZonaCupoDto
            {
                Id = 1,
                Descripcion = "A",
                CodigoSap = "A"
            }
            );
            var result = target.Aplicar(new AbmZonaCupoParam { Id = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ZonaCupo\":{\"Id\":1,\"Descripcion\":\"A\",\"CodigoSap\":\"A\"},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GrabarZonaCupoTest()
        {
            var centro = new ZonaCupo
            {
                Id = 1,
                Descripcion = "A",
                CodigoSap = "A"
            };
            zonaCupoManagerMock.Setup(x => x.GrabarZonaCupo(centro)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Grabar(centro);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ZonaCupo\":{\"Id\":0,\"Descripcion\":null,\"CodigoSap\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void EliminarZonaCupoTest()
        {
            zonaCupoManagerMock.Setup(x => x.EliminarZonaCupo(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Eliminar(new AbmZonaCupoParam { Id = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void CancelarZonaTest()
        {
            zonaCupoManagerMock.Setup(x => x.EliminarZonaCupo(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Cancelar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ZonaCupo\":{\"Id\":0,\"Descripcion\":null,\"CodigoSap\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
