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
        private Mock<IMaterialManager> materialManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
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
        public void PartialReporteCompraNetTest()
        {
            var fecha = new DateTime(2018, 10, 26);
            reportesManagerMock.Setup(x => x.TraerAgenteDeCompra(fecha, fecha, new List<int>() { 1, 2, 3, 4, 5 })).Returns(new List<AgenteCompraDto>());
            reportesManagerMock.Setup(x => x.TraerToneladasGranoTipo(fecha, fecha, new List<int>() { 1, 2, 3, 4, 5 }, 0)).Returns(new List<ToneladasGranoTipoDto>());
            reportesManagerMock.Setup(x => x.TraerToneladasSojaSust(fecha, fecha, 0)).Returns(new ReporteSojaSustDto());
            reportesManagerMock.Setup(x => x.TraerPosicionCompras(fecha, fecha, new List<int>() { 1, 2, 3, 4, 5 }, 0)).Returns(new List<PosicionComprasDto>());
            reportesManagerMock.Setup(x => x.TraerMonedaCantidad(fecha, fecha, new List<int>() { 1, 2, 3, 4, 5 }, 0)).Returns(new List<PrecioCantidadDto>());
            reportesManagerMock.Setup(x => x.TraerTodosHedgeMaterial(fecha, fecha, new List<int>() { 1, 2, 3, 4, 5 })).Returns(new List<HedgeMaterialDto>());
            reportesManagerMock.Setup(x => x.TraerUltimoHedgeObjetivo()).Returns(new HedgeCargaObjetivoDto());
            reportesManagerMock.Setup(x => x.TraerTcPromedio(fecha, fecha, new List<int>() { 1, 2, 3, 4, 5 })).Returns(new HedgeTCPromedioDto());
            reportesManagerMock.Setup(x => x.TraerHedgeObjetivo(fecha, fecha, new List<int>() { 1, 2, 3, 4, 5 })).Returns(new HedgeCargaObjetivoDto());
            var resultado = target.PartialReporteCompraNet("26-10-2018", "26-10-2018", new List<int>() { 1, 2, 3, 4, 5 }, "0") as PartialViewResult;

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
            reportesManagerMock.Setup(x => x.TraerAgenteDeCompra(fecha, fecha, new List<int>() { 1, 2, 3, 4, 5 })).Returns(new List<AgenteCompraDto>());
            reportesManagerMock.Setup(x => x.TraerToneladasGranoTipo(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<List<int>>(), It.IsAny<int>())).Returns(new List<ToneladasGranoTipoDto>());
            reportesManagerMock.Setup(x => x.TraerToneladasSojaSust(fecha, fecha, 0)).Returns(new ReporteSojaSustDto());
            reportesManagerMock.Setup(x => x.TraerPosicionCompras(fecha, fecha, new List<int>() { 1, 2, 3, 4, 5 }, 0)).Returns(new List<PosicionComprasDto>());
            reportesManagerMock.Setup(x => x.TraerMonedaCantidad(fecha, fecha, new List<int>() { 1, 2, 3, 4, 5 }, 0)).Returns(new List<PrecioCantidadDto>());
            reportesManagerMock.Setup(x => x.TraerTodosHedgeMaterial(fecha, fecha, new List<int>() { 1, 2, 3, 4, 5 })).Returns(new List<HedgeMaterialDto>());
            reportesManagerMock.Setup(x => x.TraerUltimoHedgeObjetivo()).Returns(new HedgeCargaObjetivoDto());
            reportesManagerMock.Setup(x => x.TraerTcPromedio(fecha, fecha, new List<int>() { 1, 2, 3, 4, 5 })).Returns(new HedgeTCPromedioDto());
            reportesManagerMock.Setup(x => x.TraerHedgeObjetivo(fecha, fecha, new List<int>() { 1, 2, 3, 4, 5 })).Returns(new HedgeCargaObjetivoDto());
            reportesManagerMock.Setup(x => x.ObtenerDatosReporteCompraNet(fecha, fecha, "0", new List<int>() { 1, 2, 3, 4, 5 })).Returns(new ReporteCompraNetModel { AgenteCompras = new AgenteCompraModel {ListaOperadores = new List<AgenteCompraDto.OperadorCantidad>(),ListaAgenteCompras= new List<AgenteCompraDto>() }, HedgeMaterial = new List<HedgeMaterialModel>(), HedgeObjetivo = new HedgeCargaObjetivoDto(), PosicionCompras = new List<PosicionComprasDto>(), PrecioCantidad = new List<PrecioCantidadDto>(), PricingCampania = new List<PricingCampaniaDto>(), SojaSustentable = new ReporteSojaSustDto(), TCPromedioDto = new HedgeTCPromedioDto(), ToneladasGranoTipo = new List<ToneladasGranoTipoDto>() });

            var reportes = new ParamReportes() { ComercialActual = 1 };
            reportesManagerMock.Setup(x => x.PosicionPorMaterial(fecha, fecha)).Returns(new List<ExcelPosicionMaterialDto>());

            var result = target.ReporteComprasDelDia("26-10-2018", "26-10-2018", "1, 2, 3, 4, 5", "0");
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
            reportesManagerMock.Setup(x => x.DetalleAgente(fecha, It.IsAny<List<int>>())).Returns(new ExcelDetallePosicionDto()
            {
                Headers = new string[] { "1", "2" },
                Data = new List<string[]>() { new string[] { "a", "b" } },
                Name = "B",
                SheetName = "C"
            });
            var result = target.ExcelAgente("26-10-2018", null);
            reportesManagerMock.Verify(x => x.DetalleAgente(It.IsAny<DateTime>(), It.IsAny<List<int>>()), Times.Once);
            Assert.NotNull(result);
        }
        [Test]
        public void DetalleAgenteModalTest()
        {
            var fecha = new DateTime(2018, 10, 26);
            reportesManagerMock.Setup(x => x.DetalleAgenteModal(fecha, It.IsAny<List<int>>())).Returns("");
            var result = target.DetalleAgenteModal("26/10/2018", null);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            reportesManagerMock.Verify(x => x.DetalleAgenteModal(It.IsAny<DateTime>(), It.IsAny<List<int>>()), Times.Once);
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

            reportesManagerMock.Verify(x => x.DetallePosicionModalIds(It.IsAny<List<int>>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":\"\",\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }

    }
}
