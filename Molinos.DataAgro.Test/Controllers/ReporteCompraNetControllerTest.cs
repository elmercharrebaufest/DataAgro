using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Moq;
using NUnit.Framework;
using System;
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
    public class ReporteCompraNetControllerTest
    {
        private ReporteCompraNetController target;
        private Mock<IReportesManager> reportesManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            reportesManagerMock = new Mock<IReportesManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            HttpContext.Current.Session["perfil"] = 1;
            target = new ReporteCompraNetController(reportesManagerMock.Object);
        }

        [Test]
        public void IndexOk()
        {
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void PartialReporteCompraNetTest()
        {
            var fecha = new DateTime(2018,10,26);
            reportesManagerMock.Setup(x => x.TraerTodosHedgeMaterial(fecha, fecha)).Returns(new List<HedgeMaterialDto>());
            reportesManagerMock.Setup(x => x.TraerAgenteDeCompra(fecha, fecha)).Returns(new List<AgenteCompraDto>());
            var resultado = target.PartialReporteCompraNet("26-10-2018", "26-10-2018") as PartialViewResult;

            Assert.NotNull(resultado);
        }

        [Test]
        public void DetalleExcelTest()
        {
            var fecha = new DateTime(2018, 10, 26);
            var reportes = new ParamReportes() { ComercialActual = 1 };
            reportesManagerMock.Setup(x => x.DetallePosicion(1, 10, 2018, fecha, fecha, null)).Returns(new ExcelDetallePosicionDto()
            {
                Headers = new string[] { "1", "2" },
                Data = new List<string[]>() { new string[] { "a", "b" } },
                Name = "B",
                SheetName = "C"
            });

            var result = target.DetalleExcel(10, 2018, 1, "26-10-2018", "26-10-2018", null);
            Assert.NotNull(result);
        }
        [Test]
        public void ReporteComprasDelDiaTest()
        {
            var fecha = new DateTime(2018, 10, 26);
            reportesManagerMock.Setup(x => x.TraerAgenteDeCompra(fecha, fecha)).Returns(new List<AgenteCompraDto>());
            reportesManagerMock.Setup(x => x.TraerToneladasGranoTipo(fecha, fecha)).Returns(new List<ToneladasGranoTipoDto>());
            reportesManagerMock.Setup(x => x.TraerToneladasSojaSust(fecha, fecha)).Returns(new ReporteSojaSustDto());
            reportesManagerMock.Setup(x => x.TraerPosicionCompras(fecha, fecha)).Returns(new List<PosicionComprasDto>());
            reportesManagerMock.Setup(x => x.TraerMonedaCantidad(fecha, fecha)).Returns(new List<PrecioCantidadDto>());
            reportesManagerMock.Setup(x => x.TraerTodosHedgeMaterial(fecha, fecha)).Returns(new List<HedgeMaterialDto>());
            reportesManagerMock.Setup(x => x.TraerHedgeObjetivo(fecha, fecha)).Returns(new HedgeCargaObjetivoDto());
            reportesManagerMock.Setup(x => x.TraerTcPromedio(fecha, fecha)).Returns(new HedgeTCPromedioDto());
            var reportes = new ParamReportes() { ComercialActual = 1 };
            reportesManagerMock.Setup(x => x.PosicionPorMaterial(fecha, fecha)).Returns(new List<ExcelPosicionMaterialDto>());

            var result = target.ReporteComprasDelDia("26-10-2018", "26-10-2018");
            Assert.NotNull(result);
        }
    }
}
