using Molinos.DataAgro.Entities.Dto.Distribucion;
using Molinos.DataAgro.Interfaces;
using Moq;
using Newtonsoft.Json.Linq;
using NLog;
using NUnit.Framework;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebDataAgro.Controllers;

namespace Molinos.DataAgro.Test.Controllers
{
    [TestFixture]
    public class ConfiguracionDistribucionControllerTest
    {
        private ConfiguracionDistribucionController target;
        private Mock<IConfiguracionDistribucionManager> managerMock;
        private Mock<ILogger> loggerMock;
        private Mock<HttpResponseBase> responseMock;

        [SetUp]
        public void SetUp()
        {
            managerMock = new Mock<IConfiguracionDistribucionManager>();
            loggerMock = new Mock<ILogger>();
            target = new ConfiguracionDistribucionController(managerMock.Object, loggerMock.Object);
            responseMock = new Mock<HttpResponseBase>();
            responseMock.SetupAllProperties();
        }

        [Test]
        public void ObtenerConfiguracion_DevuelveConfiguracionDelManager()
        {
            var configuracion = new ConfiguracionDistribucionDto { PlantaCodigo = "SL" };
            managerMock.Setup(x => x.ObtenerConfiguracion()).Returns(configuracion);

            var result = target.ObtenerConfiguracion() as JsonResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Data, Is.SameAs(configuracion));
        }

        [Test]
        public void GuardarConfiguracion_ManagerLanzaArgumentException_Retorna400()
        {
            ConfigurarContexto("{\"cuitMaxPct\": 1.5}");
            managerMock.Setup(x => x.GuardarConfiguracion(It.IsAny<ConfiguracionDistribucionDto>()))
                .Throws(new System.ArgumentException("cuitMaxPct debe ser un valor entre 0.0 y 1.0"));

            var result = target.GuardarConfiguracion() as JsonResult;
            var data = JObject.FromObject(result.Data);

            Assert.That(responseMock.Object.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That((string)data["error"], Does.Contain("cuitMaxPct"));
        }

        [Test]
        public void GuardarConfiguracion_Valida_PersisteYDevuelveConfiguracionActualizada()
        {
            ConfigurarContexto("{\"cuitMaxPct\": 0.30}");
            var configuracionActualizada = new ConfiguracionDistribucionDto { PlantaCodigo = "SL", CuitMaxPct = 0.30m };
            managerMock.Setup(x => x.ObtenerConfiguracion()).Returns(configuracionActualizada);

            var result = target.GuardarConfiguracion() as JsonResult;

            managerMock.Verify(x => x.GuardarConfiguracion(It.IsAny<ConfiguracionDistribucionDto>()), Times.Once);
            Assert.That(result.Data, Is.SameAs(configuracionActualizada));
        }

        private void ConfigurarContexto(string body)
        {
            var request = new Mock<HttpRequestBase>();
            request.Setup(x => x.InputStream).Returns(new MemoryStream(Encoding.UTF8.GetBytes(body)));
            request.Setup(x => x.ContentType).Returns("application/json");

            var context = new Mock<HttpContextBase>();
            context.Setup(x => x.Request).Returns(request.Object);
            context.Setup(x => x.Response).Returns(responseMock.Object);

            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);
        }
    }
}
