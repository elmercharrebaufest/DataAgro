using Molinos.DataAgro.Entities.Dto.Distribucion;
using Molinos.DataAgro.Interfaces;
using Moq;
using NLog;
using NUnit.Framework;
using System;
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
    public class ContratosApiControllerTest
    {
        private ContratosApiController target;
        private Mock<ISapArchivoParserManager> parserManagerMock;
        private Mock<ILogger> loggerMock;
        private Mock<HttpResponseBase> responseMock;

        [SetUp]
        public void SetUp()
        {
            parserManagerMock = new Mock<ISapArchivoParserManager>();
            loggerMock = new Mock<ILogger>();
            target = new ContratosApiController(parserManagerMock.Object, loggerMock.Object);

            var request = new Mock<HttpRequestBase>();
            responseMock = new Mock<HttpResponseBase>();
            responseMock.SetupAllProperties();

            var context = new Mock<HttpContextBase>();
            context.Setup(x => x.Request).Returns(request.Object);
            context.Setup(x => x.Response).Returns(responseMock.Object);

            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);
        }

        [Test]
        public void Importar_SinArchivo_Retorna400()
        {
            var result = target.Importar(null, "2026-06-11") as JsonResult;

            Assert.That(responseMock.Object.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            parserManagerMock.Verify(x => x.ParsearArchivo(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Test]
        public void Importar_ExtensionInvalida_Retorna400()
        {
            var file = CrearArchivo("contratos.pdf", "contenido");

            target.Importar(file, "2026-06-11");

            Assert.That(responseMock.Object.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            parserManagerMock.Verify(x => x.ParsearArchivo(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Test]
        public void Importar_FechaConFormatoInvalido_Retorna400()
        {
            var file = CrearArchivo("contratos.tsv", "contenido");

            target.Importar(file, "11-06-2026-invalida");

            Assert.That(responseMock.Object.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            parserManagerMock.Verify(x => x.ParsearArchivo(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Test]
        public void Importar_ManagerLanzaInvalidOperationException_Retorna422()
        {
            var file = CrearArchivo("contratos.tsv", "contenido");
            parserManagerMock.Setup(x => x.ParsearArchivo(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Throws(new InvalidOperationException("No se encontró la columna 'Tipo'"));

            target.Importar(file, "2026-06-11");

            Assert.That(responseMock.Object.StatusCode, Is.EqualTo(422));
        }

        [Test]
        public void Importar_Valido_InvocaManagerYRetornaResultado()
        {
            var file = CrearArchivo("contratos.tsv", "contenido");
            var resultadoEsperado = new ImportacionSapResultadoDto();
            parserManagerMock.Setup(x => x.ParsearArchivo(It.IsAny<Stream>(), ".tsv", new DateTime(2026, 6, 11)))
                .Returns(resultadoEsperado);

            var result = target.Importar(file, "2026-06-11") as JsonResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Data, Is.SameAs(resultadoEsperado));
            parserManagerMock.Verify(x => x.ParsearArchivo(It.IsAny<Stream>(), ".tsv", new DateTime(2026, 6, 11)), Times.Once);
        }

        private static HttpPostedFileBase CrearArchivo(string nombre, string contenido)
        {
            var bytes = Encoding.UTF8.GetBytes(contenido);
            var file = new Mock<HttpPostedFileBase>();
            file.Setup(x => x.FileName).Returns(nombre);
            file.Setup(x => x.ContentLength).Returns(bytes.Length);
            file.Setup(x => x.InputStream).Returns(new MemoryStream(bytes));
            return file.Object;
        }
    }
}
