using Molinos.DataAgro.Entities.Dto.Distribucion;
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
using System.Net;
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
        private Mock<HttpResponseBase> responseMock;

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

        [Test]
        public void Calcular_LimitesNegativos_Retorna400SinInvocarManager()
        {
            ConfigurarContexto("{\"limitesPorMaterial\":{\"Soja Poroto\":-1}}");

            target.Calcular();

            Assert.That(responseMock.Object.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            distribucionManagerMock.Verify(x => x.Calcular(It.IsAny<DistribucionCuposRequestDto>()), Times.Never);
        }

        [Test]
        public void Calcular_ManagerLanzaArgumentException_Retorna400()
        {
            ConfigurarContexto("{\"limitesPorMaterial\":{\"Soja Poroto\":10}}");
            distribucionManagerMock.Setup(x => x.Calcular(It.IsAny<DistribucionCuposRequestDto>())).Throws(new ArgumentException("error de negocio"));

            var result = target.Calcular() as JsonResult;
            var data = JObject.FromObject(result.Data);

            Assert.That(responseMock.Object.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That((string)data["error"], Is.EqualTo("error de negocio"));
        }

        [Test]
        public void Calcular_Valido_RetornaResultadoDelManager()
        {
            ConfigurarContexto("{\"limitesPorMaterial\":{\"Soja Poroto\":10}}");
            var respuestaEsperada = new DistribucionResponseDto();
            distribucionManagerMock.Setup(x => x.Calcular(It.IsAny<DistribucionCuposRequestDto>())).Returns(respuestaEsperada);

            var result = target.Calcular() as JsonResult;

            Assert.That(result.Data, Is.SameAs(respuestaEsperada));
        }

        [Test]
        public void CalcularMultiDia_LimitesNegativos_Retorna400SinInvocarManager()
        {
            ConfigurarContexto("{\"limitesPorDia\":[{\"fecha\":\"2026-06-11\",\"limites\":{\"Soja Poroto\":-5}}]}");

            target.CalcularMultiDia();

            Assert.That(responseMock.Object.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            distribucionManagerMock.Verify(x => x.CalcularMultiDia(It.IsAny<DistribucionMultiDiaRequestDto>()), Times.Never);
        }

        [Test]
        public void CalcularMultiDia_ManagerLanzaArgumentException_Retorna400()
        {
            ConfigurarContexto("{\"fechas\":[\"2026-06-11\"]}");
            distribucionManagerMock.Setup(x => x.CalcularMultiDia(It.IsAny<DistribucionMultiDiaRequestDto>()))
                .Throws(new ArgumentException("El máximo de días permitido es 7"));

            var result = target.CalcularMultiDia() as JsonResult;
            var data = JObject.FromObject(result.Data);

            Assert.That(responseMock.Object.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That((string)data["error"], Is.EqualTo("El máximo de días permitido es 7"));
        }

        [Test]
        public void CalcularMultiDia_ManagerLanzaInvalidOperationException_Retorna422()
        {
            ConfigurarContexto("{\"fechas\":[\"2026-06-11\",\"2026-06-12\"]}");
            distribucionManagerMock.Setup(x => x.CalcularMultiDia(It.IsAny<DistribucionMultiDiaRequestDto>()))
                .Throws(new InvalidOperationException("No hay límites definidos para la fecha 2026-06-12"));

            var result = target.CalcularMultiDia() as JsonResult;
            var data = JObject.FromObject(result.Data);

            Assert.That(responseMock.Object.StatusCode, Is.EqualTo(422));
            Assert.That((string)data["error"], Does.Contain("2026-06-12"));
        }

        [Test]
        public void CalcularMultiDia_Valido_RetornaResultadoDelManager()
        {
            ConfigurarContexto("{\"fechas\":[\"2026-06-11\"]}");
            var respuestaEsperada = new DistribucionMultiDiaResponseDto();
            distribucionManagerMock.Setup(x => x.CalcularMultiDia(It.IsAny<DistribucionMultiDiaRequestDto>())).Returns(respuestaEsperada);

            var result = target.CalcularMultiDia() as JsonResult;

            Assert.That(result.Data, Is.SameAs(respuestaEsperada));
        }

        private void ConfigurarContexto(string body)
        {
            var request = new Mock<HttpRequestBase>();
            responseMock = new Mock<HttpResponseBase>();
            responseMock.SetupAllProperties();
            request.Setup(x => x.InputStream).Returns(new MemoryStream(Encoding.UTF8.GetBytes(body)));
            request.Setup(x => x.ContentType).Returns("application/json");

            var context = new Mock<HttpContextBase>();
            context.Setup(x => x.Request).Returns(request.Object);
            context.Setup(x => x.Response).Returns(responseMock.Object);

            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);
        }
    }
}
