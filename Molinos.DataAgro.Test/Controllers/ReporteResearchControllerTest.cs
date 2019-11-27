using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
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
    public class ReporteResearchControllerTestTest
    {
        private ReporteResearchController target;
        private Mock<IResearchManager> researchManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            researchManagerMock = new Mock<IResearchManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            HttpContext.Current.Session["perfil"] = 1;
            target = new ReporteResearchController(researchManagerMock.Object);
        }

        [Test]
        public void IndexOk()
        {
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void ReporteAvanceCosechaPartialTest()
        {
            
            var result = target.ReporteAvanceCosechaPartial() as PartialViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Not.Null.Or.Empty);
        }
        [Test]
        public void BuscarDatosAvanceCosechaTest()
        {
            researchManagerMock.Setup(y => y.TraerAvanceCosecha(It.IsAny<KendoGridMvcRequest>()))
                .Returns(new KendoGrid<ResearchAvanceCosechaDto>(new List<ResearchAvanceCosechaDto>(),1));

            var result = target.BuscarDatosAvanceCosecha(new KendoGridMvcRequest());
            
            var a = serializer.Serialize(result);
            Assert.NotNull(result);
            researchManagerMock.Verify(x => x.TraerAvanceCosecha(It.IsAny<KendoGridMvcRequest>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Groups\":null,\"Data\":[],\"Aggregates\":null,\"Total\":1},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void AvanceSiembraPartialTest()
        {

            var result = target.AvanceSiembraPartial() as PartialViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Not.Null.Or.Empty);
        }
        [Test]
        public void BuscaDatosAvanceSiembraTest()
        {
            researchManagerMock.Setup(y => y.TraerAvanceSiembra(It.IsAny<KendoGridMvcRequest>()))
                .Returns(new KendoGrid<ResearchAvanceSiembraDto>(new List<ResearchAvanceSiembraDto>(), 1));

            var result = target.BuscaDatosAvanceSiembra(new KendoGridMvcRequest());

            var a = serializer.Serialize(result);
            Assert.NotNull(result);
            researchManagerMock.Verify(x => x.TraerAvanceSiembra(It.IsAny<KendoGridMvcRequest>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Groups\":null,\"Data\":[],\"Aggregates\":null,\"Total\":1},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ReporteSituacionCultivoPartialTest()
        {

            var result = target.ReporteSituacionCultivoPartial() as PartialViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Not.Null.Or.Empty);
        }
        [Test]
        public void BuscarDatosSituacionCultivoParcialTest()
        {
            researchManagerMock.Setup(y => y.TraerSituacionCultivoParcial(It.IsAny<KendoGridMvcRequest>()))
                .Returns(new KendoGrid<ResearchSituacionCultivoDto>(new List<ResearchSituacionCultivoDto>(), 1));

            var result = target.BuscarDatosSituacionCultivoParcial(new KendoGridMvcRequest());

            var a = serializer.Serialize(result);
            Assert.NotNull(result);
            researchManagerMock.Verify(x => x.TraerSituacionCultivoParcial(It.IsAny<KendoGridMvcRequest>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Groups\":null,\"Data\":[],\"Aggregates\":null,\"Total\":1},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ReporteVentaStockPartialTest()
        {

            var result = target.ReporteVentaStockPartial() as PartialViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Not.Null.Or.Empty);
        }
        [Test]
        public void BuscarDatosVentaStockTest()
        {
            researchManagerMock.Setup(y => y.TraerVentaStock(It.IsAny<KendoGridMvcRequest>()))
                .Returns(new KendoGrid<ResearchVentaStockDto>(new List<ResearchVentaStockDto>(), 1));

            var result = target.BuscarDatosVentaStock(new KendoGridMvcRequest());

            var a = serializer.Serialize(result);
            Assert.NotNull(result);
            researchManagerMock.Verify(x => x.TraerVentaStock(It.IsAny<KendoGridMvcRequest>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Groups\":null,\"Data\":[],\"Aggregates\":null,\"Total\":1},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
