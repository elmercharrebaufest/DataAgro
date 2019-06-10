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
    public class ZonaControllerTest
    {
        private ZonaController target;
        private Mock<IZonaManager> centroManagerMock;
        private Mock<IComercialManager> comercialManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            centroManagerMock = new Mock<IZonaManager>();
            comercialManagerMock = new Mock<IComercialManager>();
            target = new ZonaController(centroManagerMock.Object, comercialManagerMock.Object);
        }

        [Test]
        public void InicializarTest()
        {
            centroManagerMock.Setup(x => x.TraerDatosIniciales()).Returns( new DatosIniAbmZona()
            {
                Zona = new List<ZonaCombo>()
                    {
                        new ZonaCombo
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
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":{\"Zona\":[{\"Id\":1,\"Descripcion\":\"A\",\"CodigoSap\":\"A\"}]},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void BuscarZonaTest()
        {
            centroManagerMock.Setup(x => x.TraerTodoZona()).Returns(new ResultIniZona
            {
                Zona = new List<ZonaIni>()
                {
                    new ZonaIni()
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
            centroManagerMock.Setup(x => x.TraerZona(1)).Returns(new ZonaDto
            {
                Id = 1,
                Descripcion= "A",
                CodigoSap ="A"
            }                            
            );
            var result = target.ZonaCombo(new AbmZonaParam { Id = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Zona\":{\"Id\":1,\"Descripcion\":\"A\",\"CodigoSap\":\"A\"},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void AplicarZonaTest()
        {
            centroManagerMock.Setup(x => x.TraerZona(1)).Returns(new ZonaDto
            {
                Id = 1,
                Descripcion = "A",
                CodigoSap = "A"
            }
            );
            var result = target.Aplicar(new AbmZonaParam { Id = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Zona\":{\"Id\":1,\"Descripcion\":\"A\",\"CodigoSap\":\"A\"},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GrabarZonaTest()
        {
            var centro = new Zona
            {
                Id = 1,
                Descripcion = "A",
                CodigoSap = "A"
            };
            centroManagerMock.Setup(x => x.GrabarZona(centro)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Grabar(centro);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Zona\":{\"Id\":0,\"Descripcion\":null,\"CodigoSap\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void EliminarZonaTest()
        {
            centroManagerMock.Setup(x => x.EliminarZona(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Eliminar(new AbmZonaParam {Id = 1});

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void CancelarZonaTest()
        {
            centroManagerMock.Setup(x => x.EliminarZona(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Cancelar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Zona\":{\"Id\":0,\"Descripcion\":null,\"CodigoSap\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
