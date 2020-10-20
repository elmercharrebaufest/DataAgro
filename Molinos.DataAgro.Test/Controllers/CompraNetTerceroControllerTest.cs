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
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            contratoManagerMock = new Mock<IContratoManager>();
            comercialManagerMock = new Mock<IComercialManager>();
            proveedorManagerMock = new Mock<IProveedorManager>();
            logger = new Mock<ILogger>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new CompraNetTerceroController(proveedorManagerMock.Object,
               contratoManagerMock.Object,
                comercialManagerMock.Object, logger.Object);
            HttpContext.Current.Session["comercialId"] = 1;
        }

        [Test]
        public void GrabarContratoTest()
        {
            Thread.CurrentPrincipal = new TestPrincipal(new Claim[] {
            new Claim(ClaimTypes.Role, PermisosDataAgro.IngresoExterno.ToString()) });
            var proveedor = new StoredPorProveedorResult {
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

    }
}
