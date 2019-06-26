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
    public class TareasProgramadasControllerTest
    {
        private TareasProgramadasController target;
        private Mock<ILogger> loggerMock;
        private Mock<IContratoManager> contratoManagerMock;
        private Mock<IFijacionDePrecioContratoManager> fijacionManagerMock;
        private JavaScriptSerializer serializer;


        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            loggerMock = new Mock<ILogger>();
            contratoManagerMock = new Mock<IContratoManager>();
            fijacionManagerMock = new Mock<IFijacionDePrecioContratoManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new TareasProgramadasController(loggerMock.Object, contratoManagerMock.Object, fijacionManagerMock.Object);
        }

        [Test]
        public void EnvioMailPendientesTest()
        {
            contratoManagerMock.Setup(x => x.EnviarMailPendiente());
            var result = target.EnvioMailPendientes() as ContentResult;

            Assert.NotNull(result);            
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }
        [Test]
        public void FinalizacionContratoTest()
        {
            contratoManagerMock.Setup(x => x.FinalizacionAutomatica(GlobalVariables.IdActiveDirectory));
            fijacionManagerMock.Setup(x => x.FinalizacionAutomatica(GlobalVariables.IdActiveDirectory));
            var result = target.FinalizacionContratos() as ContentResult;

            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void BorradoContratosTest()
        {
            contratoManagerMock.Setup(x => x.BorradoAutomatico());
            var result = target.BorradoContratos() as ContentResult;

            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }
    }
}
