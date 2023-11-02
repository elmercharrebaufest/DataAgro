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
    public class CentroControllerTest
    {
        private CentroController target;
        private Mock<ICentroManager> centroManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            centroManagerMock = new Mock<ICentroManager>();
            target = new CentroController(centroManagerMock.Object);
        }

        [Test]
        public void InicializarTest()
        {
            centroManagerMock.Setup(x => x.TraerDatosIniciales()).Returns( new DatosIniAbmCentro()
            {
                Centro = new List<CentroCombo>()
                    {
                        new CentroCombo
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
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":{\"Centro\":[{\"Id\":1,\"Descripcion\":\"A\",\"CodigoSap\":\"A\"}]},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void BuscarCentroTest()
        {
            centroManagerMock.Setup(x => x.TraerTodoCentro()).Returns(new ResultIniCentro
            {
                Centro = new List<CentroIni>()
                {
                    new CentroIni()
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
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":[{\"Id\":1,\"Descripcion\":\"A\",\"CodigoSap\":\"A\",\"ValidaRedespacho\":false,\"LocalidadId\":null,\"Localidad\":null,\"Acopio\":false,\"CodigoPostal\":null,\"Direccion\":null,\"Comision\":false,\"CargaNegocios\":false,\"CargaCupos\":false,\"NoPropio\":false,\"Orden\":null,\"CUIT\":null,\"RazonSocial\":null}],\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void CentroComboTest()
        {
            centroManagerMock.Setup(x => x.TraerCentro(1)).Returns(new CentroDto
            {
                Id = 1,
                Descripcion= "A",
                CodigoSap ="A"
            }                            
            );
            var result = target.CentroCombo(new AbmCentroParam { Id = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Centro\":{\"Id\":1,\"Descripcion\":\"A\",\"CodigoSap\":\"A\",\"Acopio\":false,\"ValidaRedespacho\":false,\"LocalidadId\":null,\"Localidad\":null,\"CodigoPostal\":null,\"Direccion\":null,\"Comision\":false,\"CargaNegocios\":false,\"CargaCupos\":false,\"NoPropio\":false,\"Orden\":null,\"CUIT\":null,\"RazonSocial\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void AplicarCentroTest()
        {
            centroManagerMock.Setup(x => x.TraerCentro(1)).Returns(new CentroDto
            {
                Id = 1,
                Descripcion = "A",
                CodigoSap = "A"
            }
            );
            var result = target.Aplicar(new AbmCentroParam { Id = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Centro\":{\"Id\":1,\"Descripcion\":\"A\",\"CodigoSap\":\"A\",\"Acopio\":false,\"ValidaRedespacho\":false,\"LocalidadId\":null,\"Localidad\":null,\"CodigoPostal\":null,\"Direccion\":null,\"Comision\":false,\"CargaNegocios\":false,\"CargaCupos\":false,\"NoPropio\":false,\"Orden\":null,\"CUIT\":null,\"RazonSocial\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GrabarCentroTest()
        {
            var centro = new Centro
            {
                Id = 1,
                Descripcion = "A",
                CodigoSap = "A"
            };
            centroManagerMock.Setup(x => x.GrabarCentro(centro)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Grabar(centro);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Centro\":{\"Id\":0,\"Descripcion\":null,\"CodigoSap\":null,\"Acopio\":false,\"ValidaRedespacho\":false,\"LocalidadId\":null,\"Localidad\":null,\"CodigoPostal\":null,\"Direccion\":null,\"Comision\":false,\"CargaNegocios\":false,\"CargaCupos\":false,\"NoPropio\":false,\"Orden\":null,\"CUIT\":null,\"RazonSocial\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void EliminarCentroTest()
        {
            centroManagerMock.Setup(x => x.EliminarCentro(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Eliminar(new AbmCentroParam {Id = 1});

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void CancelarCentroTest()
        {
            centroManagerMock.Setup(x => x.EliminarCentro(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Cancelar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Centro\":{\"Id\":0,\"Descripcion\":null,\"CodigoSap\":null,\"Acopio\":false,\"ValidaRedespacho\":false,\"LocalidadId\":null,\"Localidad\":null,\"CodigoPostal\":null,\"Direccion\":null,\"Comision\":false,\"CargaNegocios\":false,\"CargaCupos\":false,\"NoPropio\":false,\"Orden\":null,\"CUIT\":null,\"RazonSocial\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void ObtenerCentroPorCodigoSapTest()
        {
            centroManagerMock.Setup(x => x.ObtenerCentroPorCodigoSap("1")).Returns(new CentroDto { Id = 1 });
            var result = target.ObtenerCentroPorCodigoSap("1");

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Id\":1,\"Descripcion\":null,\"CodigoSap\":null,\"Acopio\":false,\"ValidaRedespacho\":false,\"LocalidadId\":null,\"Localidad\":null,\"CodigoPostal\":null,\"Direccion\":null,\"Comision\":false,\"CargaNegocios\":false,\"CargaCupos\":false,\"NoPropio\":false,\"Orden\":null,\"CUIT\":null,\"RazonSocial\":null},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
