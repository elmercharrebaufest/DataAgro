using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Moq;
using Newtonsoft.Json.Linq;
using NLog;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebDataAgro.Controllers;

namespace Molinos.DataAgro.Test.Controllers
{
    [TestFixture]
    public class DistribucionApiControllerTest
    {
        private DistribucionApiController target;
        private Mock<IDistribucionCuposManager> distribucionManagerMock;
        private Mock<ICupoManager> cupoManagerMock;
        private Mock<ILogger> loggerMock;

        [SetUp]
        public void SetUp()
        {
            distribucionManagerMock = new Mock<IDistribucionCuposManager>();
            cupoManagerMock = new Mock<ICupoManager>();
            loggerMock = new Mock<ILogger>();
            target = new DistribucionApiController(distribucionManagerMock.Object, cupoManagerMock.Object, loggerMock.Object);
        }

        [Test]
        public void ProcesarEnDataAgro_SapVacio_NoInvocaAltaMasiva()
        {
            ConfigurarContexto("{\"sap\":[]}");

            var result = target.ProcesarEnDataAgro() as JsonResult;
            var data = JObject.FromObject(result.Data);

            Assert.That((bool)data["resultado"], Is.False);
            Assert.That((string)data["error"], Is.EqualTo("El array 'sap' no puede estar vacío"));
            cupoManagerMock.Verify(x => x.AltaMasivaSugerenciaCuposV2(It.IsAny<DataSet>()), Times.Never);
        }

        [Test]
        public void ProcesarEnDataAgro_ConstruyeDataSetConOaDate()
        {
            DataSet capturado = null;
            cupoManagerMock.Setup(x => x.AltaMasivaSugerenciaCuposV2(It.IsAny<DataSet>()))
                .Callback<DataSet>(x => capturado = x)
                .Returns(new List<ExcelValidatorResumeItem> { new ExcelValidatorResumeItem { Row = 1 } });
            ConfigurarContexto("{\"sap\":[{\"fechaSugerida\":\"11/06/2026\",\"cantidadDeCupos\":6,\"contratoSAP\":\"4500012345\"}]}");

            var result = target.ProcesarEnDataAgro() as JsonResult;
            var data = JObject.FromObject(result.Data);

            Assert.That((bool)data["resultado"], Is.True);
            Assert.That(capturado, Is.Not.Null);
            Assert.That(capturado.Tables[0].Columns["ContratoSAP"].DataType, Is.EqualTo(typeof(string)));
            Assert.That(capturado.Tables[0].Columns["FechaSugerida"].DataType, Is.EqualTo(typeof(double)));
            Assert.That((string)capturado.Tables[0].Rows[0]["ContratoSAP"], Is.EqualTo("4500012345"));
            Assert.That((int)capturado.Tables[0].Rows[0]["CantidadDeCupos"], Is.EqualTo(6));
            Assert.That((double)capturado.Tables[0].Rows[0]["FechaSugerida"], Is.EqualTo(new DateTime(2026, 6, 11).ToOADate()));
        }

        [Test]
        public void ProcesarEnDataAgro_CuandoManagerFalla_RetornaErrorControlado()
        {
            cupoManagerMock.Setup(x => x.AltaMasivaSugerenciaCuposV2(It.IsAny<DataSet>())).Throws(new InvalidOperationException("falló"));
            ConfigurarContexto("{\"sap\":[{\"fechaSugerida\":\"11/06/2026\",\"cantidadDeCupos\":6,\"contratoSAP\":\"4500012345\"}]}");

            var result = target.ProcesarEnDataAgro() as JsonResult;
            var data = JObject.FromObject(result.Data);

            Assert.That((bool)data["resultado"], Is.False);
            Assert.That((string)data["error"], Does.Contain("falló"));
        }

        private void ConfigurarContexto(string body)
        {
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            response.SetupAllProperties();
            request.Setup(x => x.InputStream).Returns(new MemoryStream(Encoding.UTF8.GetBytes(body)));
            request.Setup(x => x.ContentType).Returns("application/json");

            var context = new Mock<HttpContextBase>();
            context.Setup(x => x.Request).Returns(request.Object);
            context.Setup(x => x.Response).Returns(response.Object);

            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);
        }
    }
}
