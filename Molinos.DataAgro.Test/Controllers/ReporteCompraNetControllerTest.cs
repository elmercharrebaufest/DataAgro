using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
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
        private Mock<IMaterialManager> materialManagerMock;

        [SetUp]
        public void SetUp()
        {
            reportesManagerMock = new Mock<IReportesManager>();
            centroManagerMock = new Mock<ICentroManager>();
            materialManagerMock = new Mock<IMaterialManager>();

            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            HttpContext.Current.Session["perfil"] = 1;
            target = new ReporteCompraNetController(reportesManagerMock.Object, centroManagerMock.Object, materialManagerMock.Object);
        }

        [Test]
        public void IndexOk()
        {
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void PartialReporteCompraNetTest_RetornaPartialView()
        {
            var fecha = new DateTime(2018, 10, 26);
            var ids = new List<int> { 1, 2, 3, 4, 5 };
            reportesManagerMock.Setup(x => x.TraerAgenteDeCompra(fecha, fecha, ids)).Returns(new List<AgenteCompraDto>());
            reportesManagerMock.Setup(x => x.TraerToneladasGranoTipo(fecha, fecha, ids, 0)).Returns(new List<ToneladasGranoTipoDto>());
            reportesManagerMock.Setup(x => x.TraerToneladasSojaSust(fecha, fecha, 0)).Returns(new ReporteSojaSustDto());
            reportesManagerMock.Setup(x => x.TraerToneladasSojaEPAyEUDR(fecha, fecha, 0)).Returns(new ReporteSojaEPAyEUDRDto());
            reportesManagerMock.Setup(x => x.TraerPosicionCompras(fecha, fecha, ids, 0, false, It.IsAny<bool>())).Returns(new List<PosicionComprasDto>());
            reportesManagerMock.Setup(x => x.TraerMonedaCantidad(fecha, fecha, ids, 0, It.IsAny<bool>())).Returns(new List<PrecioCantidadDto>());
            reportesManagerMock.Setup(x => x.TraerTodosHedgeMaterial(fecha, fecha, ids)).Returns(new List<HedgeMaterialDto>());
            reportesManagerMock.Setup(x => x.TraerUltimoHedgeObjetivo()).Returns(new HedgeCargaObjetivoDto());
            reportesManagerMock.Setup(x => x.TraerTcPromedio(fecha, fecha, ids, It.IsAny<bool>())).Returns(new HedgeTCPromedioDto());
            reportesManagerMock.Setup(x => x.TraerHedgeObjetivo(fecha, fecha, ids, It.IsAny<bool>())).Returns(new HedgeCargaObjetivoDto());

            var result = target.PartialReporteCompraNet("26-10-2018", "26-10-2018", ids, "0") as PartialViewResult;

            Assert.NotNull(result);
        }

        [Test]
        public void DetalleExcelTest_RetornaExcelResult()
        {
            var fecha = new DateTime(2018, 10, 26);
            reportesManagerMock.Setup(x => x.DetallePosicion(1, 10, 2018, fecha, fecha, null, 0, It.IsAny<bool>()))
                .Returns(new ExcelDetallePosicionDto
                {
                    Headers = new[] { "1", "2" },
                    Data = new List<string[]> { new[] { "a", "b" } },
                    Name = "B",
                    SheetName = "C"
                });

            var result = target.DetalleExcel(10, 2018, 1, "26-10-2018", "26-10-2018", null);

            Assert.NotNull(result);
        }

        [Test]
        public void ReporteComprasDelDiaTest_RetornaFileResult()
        {
            var fecha = new DateTime(2018, 10, 26);
            reportesManagerMock.Setup(x => x.ObtenerDatosReporteCompraNet(fecha, fecha, "0", It.IsAny<List<int>>(), It.IsAny<bool>()))
                .Returns(new ReporteCompraNetModel
                {
                    AgenteCompras = new AgenteCompraModel { ListaOperadores = new List<AgenteCompraDto.OperadorCantidad>(), ListaAgenteCompras = new List<AgenteCompraDto>() },
                    HedgeMaterial = new List<HedgeMaterialModel>(),
                    HedgeObjetivo = new HedgeCargaObjetivoDto(),
                    PosicionCompras = new List<PosicionComprasDto>(),
                    PrecioCantidad = new List<PrecioCantidadDto>(),
                    PricingCampania = new List<PricingCampaniaDto>(),
                    SojaSustentable = new ReporteSojaSustDto(),
                    SojaEPAyEUDR = new ReporteSojaEPAyEUDRDto(),
                    TCPromedioDto = new HedgeTCPromedioDto(),
                    ToneladasGranoTipo = new List<ToneladasGranoTipoDto>()
                });
            reportesManagerMock.Setup(x => x.PosicionPorMaterial(fecha, fecha, It.IsAny<bool>()))
                .Returns(new List<ExcelPosicionMaterialDto>());

            var result = target.ReporteComprasDelDia("26-10-2018", "26-10-2018", "1, 2, 3, 4, 5", "0");

            Assert.NotNull(result);
        }

        [Test]
        public void DetalleExcelModalTest_RetornaJsonConResultado()
        {
            HttpContext.Current.Session["equipo"] = new List<int> { 1, 2 };
            reportesManagerMock.Setup(x => x.DetallePosicionModal(
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                    null, 0, It.IsAny<bool>()))
                .Returns("detalle");

            var result = target.DetalleExcelModal(2, 2018, 1, "26-10-2018", "26-10-2018", 2, "0") as JsonResult;

            Assert.NotNull(result);
            Assert.AreEqual("detalle", result.Data);
            reportesManagerMock.Verify(x => x.DetallePosicionModal(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                null, 0, It.IsAny<bool>()), Times.Once);
        }

        [Test]
        public void ExcelAgenteTest_RetornaExcelResult()
        {
            reportesManagerMock.Setup(x => x.DetalleAgente(It.IsAny<DateTime>(), It.IsAny<List<int>>()))
                .Returns(new ExcelDetallePosicionDto
                {
                    Headers = new[] { "1", "2" },
                    Data = new List<string[]> { new[] { "a", "b" } },
                    Name = "B",
                    SheetName = "C"
                });

            var result = target.ExcelAgente("26-10-2018", null);

            Assert.NotNull(result);
            reportesManagerMock.Verify(x => x.DetalleAgente(It.IsAny<DateTime>(), It.IsAny<List<int>>()), Times.Once);
        }

        [Test]
        public void DetalleAgenteModalTest_RetornaJsonConResultado()
        {
            reportesManagerMock.Setup(x => x.DetalleAgenteModal(It.IsAny<DateTime>(), It.IsAny<List<int>>()))
                .Returns("agente-detalle");

            var result = target.DetalleAgenteModal("26/10/2018", null) as JsonResult;

            Assert.NotNull(result);
            Assert.AreEqual("agente-detalle", result.Data);
            reportesManagerMock.Verify(x => x.DetalleAgenteModal(It.IsAny<DateTime>(), It.IsAny<List<int>>()), Times.Once);
        }

        [Test]
        public void DetalleIdsModal_RetornaJsonConResultado()
        {
            HttpContext.Current.Session["equipo"] = new List<int> { 1, 2 };
            reportesManagerMock.Setup(x => x.DetallePosicionModalIds(
                    It.IsAny<List<int>>(), It.IsAny<string>(), It.IsAny<int?>()))
                .Returns("ids-detalle");

            var result = target.DetalleIdsModal(new List<int> { 1 }, "", null) as JsonResult;

            Assert.NotNull(result);
            Assert.AreEqual("ids-detalle", result.Data);
            reportesManagerMock.Verify(x => x.DetallePosicionModalIds(
                It.IsAny<List<int>>(), It.IsAny<string>(), It.IsAny<int?>()), Times.Once);
        }

        [Test]
        public void ExcelModeloAltaMasivaTest_RetornaFileResult()
        {
            materialManagerMock.Setup(x => x.TraerTodoMaterial())
                .Returns(new ResultIniMaterial { Material = new List<MaterialIni> { new MaterialIni { MaterialId = 1, Descripcion = "a" } } });
            centroManagerMock.Setup(x => x.TraerTodoCentro())
                .Returns(new ResultIniCentro { Centro = new List<CentroIni> { new CentroIni { Id = 1, Descripcion = "a" } } });

            var result = target.ExcelModeloAltaMasiva();

            Assert.NotNull(result);
        }
    }
}
