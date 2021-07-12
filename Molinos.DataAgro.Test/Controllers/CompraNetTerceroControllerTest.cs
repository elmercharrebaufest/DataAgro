using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.Containers;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Test.Mock;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class CompraNetControllerTerceroTest
    {
        private CompraNetTerceroController target;
        private Mock<IContratoManager> contratoManagerMock;
        private Mock<IComercialManager> comercialManagerMock;
        private Mock<IProveedorManager> proveedorManagerMock;
        private Mock<ILogger> logger;
        private Mock<IFijacionDePrecioContratoManager> fijacionDePrecioContratoManagerMock;
        private Mock<IConfiguracionInternaManager> configuracionInternaManagerMock;
        private Mock<ITipoDeCambioAgent> tipoDeCambioAgentMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            contratoManagerMock = new Mock<IContratoManager>();
            comercialManagerMock = new Mock<IComercialManager>();
            proveedorManagerMock = new Mock<IProveedorManager>();
            fijacionDePrecioContratoManagerMock = new Mock<IFijacionDePrecioContratoManager>();
            configuracionInternaManagerMock = new Mock<IConfiguracionInternaManager>();
            tipoDeCambioAgentMock = new Mock<ITipoDeCambioAgent>();
            //
            logger = new Mock<ILogger>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new CompraNetTerceroController(proveedorManagerMock.Object,
               contratoManagerMock.Object,
                comercialManagerMock.Object, logger.Object, fijacionDePrecioContratoManagerMock.Object, configuracionInternaManagerMock.Object, 
                tipoDeCambioAgentMock.Object);
            HttpContext.Current.Session["comercialId"] = 1;
        }

        [Test]
        public void GrabarContratoAPrecioTestOk()
        {
            Thread.CurrentPrincipal = new TestPrincipal(new Claim[] {
            new Claim(ClaimTypes.Role, PermisosDataAgro.IngresoExterno.ToString()) });
            var proveedor = new StoredPorProveedorResult
            {
                BasicoProveedorTraerPorProveedores = new List<BasicoProveedor> {
                    new BasicoProveedor { CUIT = "201", RazonSocial = "A", ProveedorId = 1 }
                }
            };

            comercialManagerMock.Setup(x => x.ComercialAsociado(It.IsAny<int>())).Returns(1);
            comercialManagerMock.Setup(x => x.TraerComercial(It.IsAny<int>())).Returns(new ComercialDto { IdActiveDirectory = "a" });
            proveedorManagerMock.Setup(x => x.TraerProveedor(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<int>>())).Returns(proveedor);
            var result = target.GrabarContratoAPrecio(
                new Contrato()
                {
                    Base = false,
                    NoInformaSio = false,
                    TrigoEspecial = false,
                    EsFason = false,
                    ComercialId = 1,
                    ProveedorId = 1,
                    ProveedorCreadorId = 1,
                    CorredorId = 1,
                    GrupoCompra = 1

                });
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":null,\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void GrabarContratoAFijarTestOk()
        {
            Thread.CurrentPrincipal = new TestPrincipal(new Claim[] {
            new Claim(ClaimTypes.Role, PermisosDataAgro.IngresoExterno.ToString()) });
            var proveedor = new StoredPorProveedorResult
            {
                BasicoProveedorTraerPorProveedores = new List<BasicoProveedor> {
                    new BasicoProveedor { CUIT = "201", RazonSocial = "A", ProveedorId = 1 }
                }
            };

            comercialManagerMock.Setup(x => x.ComercialAsociado(It.IsAny<int>())).Returns(1);
            comercialManagerMock.Setup(x => x.TraerComercial(It.IsAny<int>())).Returns(new ComercialDto { IdActiveDirectory = "a" });
            proveedorManagerMock.Setup(x => x.TraerProveedor(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<int>>())).Returns(proveedor);
            var result = target.GrabarContratoAFijar(
                new Contrato()
                {
                    Base = false,
                    NoInformaSio = false,
                    TrigoEspecial = false,
                    EsFason = false,
                    ComercialId = 1,
                    ProveedorId = 1,
                    ProveedorCreadorId = 1,
                    CorredorId = 1,
                    GrupoCompra = 1

                });
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":null,\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void ValidarDirectoTestOk()
        {
            Thread.CurrentPrincipal = new TestPrincipal(new Claim[] {
            new Claim(ClaimTypes.Role, PermisosDataAgro.IngresoExterno.ToString()) });

            proveedorManagerMock.Setup(x => x.ValidarDirecto(It.IsAny<string>())).Returns(true);

            var result = target.ValidarDirecto("");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":true,\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GrabarFijacionTestOk()
        {
            Thread.CurrentPrincipal = new TestPrincipal(new Claim[] {
            new Claim(ClaimTypes.Role, PermisosDataAgro.IngresoExterno.ToString()) });
            var proveedor = new StoredPorProveedorResult
            {
                BasicoProveedorTraerPorProveedores = new List<BasicoProveedor> {
                    new BasicoProveedor { CUIT = "201", RazonSocial = "A", ProveedorId = 1 }
                }
            };

            comercialManagerMock.Setup(x => x.ComercialAsociado(It.IsAny<int>())).Returns(1);
            comercialManagerMock.Setup(x => x.TraerComercial(It.IsAny<int>())).Returns(new ComercialDto { IdActiveDirectory = "a" });
            proveedorManagerMock.Setup(x => x.TraerProveedor(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<int>>())).Returns(proveedor);
            var result = target.GrabarFijacion(
                new FijacionDePrecioContrato()
                {
                    TrigoEspecial = false,
                    ComercialId = 1,
                    ProveedorId = 1,
                    ProveedorCreadorId = 1,
                    CorredorId = 1,
                    GrupoCompra = 1,
                    ContratoSAP = "000111111"

                });
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":null,\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void HabilitarPizarraTestOk()
        {
            Thread.CurrentPrincipal = new TestPrincipal(new Claim[] {
            new Claim(ClaimTypes.Role, PermisosDataAgro.IngresoExterno.ToString()) });

            configuracionInternaManagerMock.Setup(x => x.HabilitarPizarraExterno(It.IsAny<int>(), It.IsAny<int>())).Returns(new HabilitacionPizarraDto { Id = 1 });

            var result = target.HabilitarPizarra(1, 1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
        }
        [Test]
        public void HabilitarCampañaTestOk()
        {
            Thread.CurrentPrincipal = new TestPrincipal(new Claim[] {
            new Claim(ClaimTypes.Role, PermisosDataAgro.IngresoExterno.ToString()) });

            configuracionInternaManagerMock.Setup(x => x.HabilitarCampañaExterno(It.IsAny<int>())).Returns(new List<HabilitacionCampañaDto> { new HabilitacionCampañaDto { Id = 1 } });

            var result = target.HabilitarCampaña(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
        }

        [Test]
        public void TraerPrecioMoaTestOk()
        {
            Thread.CurrentPrincipal = new TestPrincipal(new Claim[] {
            new Claim(ClaimTypes.Role, PermisosDataAgro.IngresoExterno.ToString()) });

            configuracionInternaManagerMock.Setup(x => x.TraerPrecioCompraNet(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<PrecioMoaCompraNetDto> { new PrecioMoaCompraNetDto { Id = 1 } });

            var result = target.TraerPrecioMoa(1, 1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
        }

        [Test]
        public void ObtenerContratosAcuerdoPorCorredorTest()
        {
            contratoManagerMock.Setup(x => x.TraerContratosAcuerdoPorCorredor(It.IsAny<int>()))
                .Returns(new List<ContratoCopiar>() { new ContratoCopiar { CantidadD = 1, Comercial = "", ContratoSap = "a", Fecha = "a", Filtro = "a", Id = 1, Material = "a", RazonSocial = "a", tipoNegocio = "a" } });
            var result = target.TraerContratosAcuerdoPorCorredor(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.TraerContratosAcuerdoPorCorredor(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Id\":1,\"RazonSocial\":\"a\",\"Comercial\":\"\",\"Material\":\"a\",\"Fecha\":\"a\",\"ContratoSap\":\"a\",\"Filtro\":\"a\",\"tipoNegocio\":\"a\",\"CantidadD\":1,\"Cantidad\":\"1\",\"Precio\":\"0\",\"NegocioDescripcion\":null,\"PrecioD\":0,\"FechaDesde\":null,\"FechaHasta\":null,\"Moneda\":null,\"MonedaId\":null}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GrabarContratoMasivoTest()
        {
            List<BasicoContrato> contratos = new List<BasicoContrato> {
                new BasicoContrato{ ContratoAcuerdoId=1 }
            };
            
            var result = target.GrabarContratoMasivo(contratos);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            contratoManagerMock.Setup(x => x.GrabarContratoMasivo(It.IsAny<List<BasicoContrato>>()))
                .Returns(new List<GrabarContratoResult>() { new GrabarContratoResult { } });

            contratoManagerMock.Verify(x => x.GrabarContratoMasivo(It.IsAny<List<BasicoContrato>>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":null,\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
    }
}
