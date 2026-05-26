using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class FAQControllerTest
    {
        private FAQController target;
        private JavaScriptSerializer serializer;
        private Mock<IFAQManager> faqManagerMock;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            faqManagerMock = new Mock<IFAQManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            target = new FAQController(faqManagerMock.Object);
        }

        [Test]
        public void IndexTest()
        {
            var result = target.Index() as ViewResult;
            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void TraerManualesTest()
        {
            faqManagerMock.Setup(x => x.TraerManuales())
                .Returns(new List<ManualesDto> { new ManualesDto { Id = 1 } });
            var result = target.TraerManuales();

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Id\":1,\"Titulo\":null,\"Descripcion\":null,\"Path\":null,\"FechaUltimaActualizacion\":\"\\/Date(-62135578800000)\\/\",\"Version\":0,\"CantidadVisitas\":0}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void EnviarSugerenciaTest()
        {
            faqManagerMock.Setup(x => x.EnviarSugerencia(It.IsAny<ManualesDto>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ManualResult { Errores = new List<ErrorMessage>() });
            var result = target.EnviarSugerencia(It.IsAny<ManualesDto>(), It.IsAny<string>());

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void ContarVisitaTest()
        {
            faqManagerMock.Setup(x => x.RegistrarVisita(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(new ManualResult { Errores = new List<ErrorMessage>() });
            var result = target.ContarVisita(It.IsAny<int>());

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
    }
}
