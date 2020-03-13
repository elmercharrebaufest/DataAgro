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
        private Mock<ICentroManager> centroManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            reportesManagerMock = new Mock<IReportesManager>();
            centroManagerMock = new Mock<ICentroManager>();

            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            HttpContext.Current.Session["perfil"] = 1;
            target = new ReporteCompraNetController(reportesManagerMock.Object, centroManagerMock.Object);
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
            var fecha = new DateTime(2018, 10, 26);
            reportesManagerMock.Setup(x => x.TraerAgenteDeCompra(fecha, fecha)).Returns(new List<AgenteCompraDto>());
            reportesManagerMock.Setup(x => x.TraerToneladasGranoTipo(fecha, fecha, 0)).Returns(new List<ToneladasGranoTipoDto>());
            reportesManagerMock.Setup(x => x.TraerToneladasSojaSust(fecha, fecha, 0)).Returns(new ReporteSojaSustDto());
            reportesManagerMock.Setup(x => x.TraerPosicionCompras(fecha, fecha, 0)).Returns(new List<PosicionComprasDto>());
            reportesManagerMock.Setup(x => x.TraerMonedaCantidad(fecha, fecha, 0)).Returns(new List<PrecioCantidadDto>());
            reportesManagerMock.Setup(x => x.TraerTodosHedgeMaterial(fecha, fecha)).Returns(new List<HedgeMaterialDto>());
            reportesManagerMock.Setup(x => x.TraerHedgeObjetivo(fecha, fecha)).Returns(new HedgeCargaObjetivoDto());
            reportesManagerMock.Setup(x => x.TraerTcPromedio(fecha, fecha)).Returns(new HedgeTCPromedioDto());
            var resultado = target.PartialReporteCompraNet("26-10-2018", "26-10-2018", "0") as PartialViewResult;

            Assert.NotNull(resultado);
        }

        [Test]
        public void DetalleExcelTest()
        {
            var fecha = new DateTime(2018, 10, 26);
            var reportes = new ParamReportes() { ComercialActual = 1 };
            reportesManagerMock.Setup(x => x.DetallePosicion(1, 10, 2018, fecha, fecha, null, 0)).Returns(new ExcelDetallePosicionDto()
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
            reportesManagerMock.Setup(x => x.TraerToneladasGranoTipo(fecha, fecha, 0)).Returns(new List<ToneladasGranoTipoDto>());
            reportesManagerMock.Setup(x => x.TraerToneladasSojaSust(fecha, fecha, 0)).Returns(new ReporteSojaSustDto());
            reportesManagerMock.Setup(x => x.TraerPosicionCompras(fecha, fecha, 0)).Returns(new List<PosicionComprasDto>());
            reportesManagerMock.Setup(x => x.TraerMonedaCantidad(fecha, fecha, 0)).Returns(new List<PrecioCantidadDto>());
            reportesManagerMock.Setup(x => x.TraerTodosHedgeMaterial(fecha, fecha)).Returns(new List<HedgeMaterialDto>());
            reportesManagerMock.Setup(x => x.TraerHedgeObjetivo(fecha, fecha)).Returns(new HedgeCargaObjetivoDto());
            reportesManagerMock.Setup(x => x.TraerTcPromedio(fecha, fecha)).Returns(new HedgeTCPromedioDto());
            var reportes = new ParamReportes() { ComercialActual = 1 };
            reportesManagerMock.Setup(x => x.PosicionPorMaterial(fecha, fecha)).Returns(new List<ExcelPosicionMaterialDto>());

            var result = target.ReporteComprasDelDia("26-10-2018", "26-10-2018", "0");
            Assert.NotNull(result);
        }
        [Test]
        public void DetalleExcelModalTest()
        {
            var fecha = new DateTime(2018, 10, 26);
            HttpContext.Current.Session["equipo"] = new List<int> { 1, 2 };
            reportesManagerMock.Setup(x => x.DetallePosicionModal(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), null, 0)).Returns("");
            var result = target.DetalleExcelModal(2, 2018, 1, "26-10-2018", "26-10-2018", 2, "0");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            reportesManagerMock.Verify(x => x.DetallePosicionModal(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), null, 0), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":\"\",\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ExcelAgenteTest()
        {
            var fecha = new DateTime(2018, 10, 26);
            var reportes = new ParamReportes() { ComercialActual = 1 };
            reportesManagerMock.Setup(x => x.DetalleAgente(fecha)).Returns(new ExcelDetallePosicionDto()
            {
                Headers = new string[] { "1", "2" },
                Data = new List<string[]>() { new string[] { "a", "b" } },
                Name = "B",
                SheetName = "C"
            });
            var result = target.ExcelAgente("26-10-2018");
            reportesManagerMock.Verify(x => x.DetalleAgente(It.IsAny<DateTime>()), Times.Once);
            Assert.NotNull(result);
        }
        [Test]
        public void DetalleAgenteModalTest()
        {
            var fecha = new DateTime(2018, 10, 26);
            reportesManagerMock.Setup(x => x.DetalleAgenteModal(fecha)).Returns("");
            var result = target.DetalleAgenteModal("26/10/2018");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            reportesManagerMock.Verify(x => x.DetalleAgenteModal(It.IsAny<DateTime>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":\"\",\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void DetalleIdsModal()
        {
            var fecha = new DateTime(2018, 10, 26);
            HttpContext.Current.Session["equipo"] = new List<int> { 1, 2 };
            reportesManagerMock.Setup(x => x.DetallePosicionModalIds(It.IsAny<List<int>>(), It.IsAny<string>())).Returns("");
            var result = target.DetalleIdsModal("1", "");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            reportesManagerMock.Verify(x => x.DetallePosicionModalIds(It.IsAny<List <int>>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":\"\",\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }

    }
}
