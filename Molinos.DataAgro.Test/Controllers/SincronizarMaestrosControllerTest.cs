using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
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
    public class SincronizarMaestrosControllerTest
    {
        private SincronizarMaestrosController target;
        private Mock<IRG2300Manager> rg2300Mock;
        private Mock<IFacacopManager> facacopManagerMock;
        private Mock<IEstadoProveedorManager> estadoProveedorMock;
        private Mock<ISISAManager> sisaManagerMock;
        private Mock<ILogger> loggerMock;
        private Mock<IComprasManager> comprasManagerMock;
        private JavaScriptSerializer serializer;


        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            rg2300Mock = new Mock<IRG2300Manager>();
            facacopManagerMock = new Mock<IFacacopManager>();
            estadoProveedorMock = new Mock<IEstadoProveedorManager>();
            sisaManagerMock = new Mock<ISISAManager>();
            loggerMock = new Mock<ILogger>();
            comprasManagerMock = new Mock<IComprasManager>();
            target = new SincronizarMaestrosController(loggerMock.Object, comprasManagerMock.Object, rg2300Mock.Object, facacopManagerMock.Object, estadoProveedorMock.Object, sisaManagerMock.Object);
        }

        [Test]
        public void ProcessComprasAyerTest()
        {
            comprasManagerMock.Setup(x => x.ActualizarComprasAyer());
            var result = target.ProcessComprasAyer() as ContentResult;

            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void ProcessEstadoTest()
        {
            estadoProveedorMock.Setup(x => x.ActualizarProveedores());
            var result = target.ProcessEstado() as ContentResult;

            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void ProcessComprasTest()
        {
            comprasManagerMock.Setup(x => x.ActualizarCompras());
            var result = target.ProcessCompras() as ContentResult;

            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }
    }
}
