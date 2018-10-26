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
            
            var resultado = target.PartialReporteCompraNet("26-10-2018") as PartialViewResult;

            Assert.NotNull(resultado);
        }

        [Test]
        public void DetalleExcelTest()
        {
            DateTime.TryParse("26-10-2018", out DateTime fecha);
            var reportes = new ParamReportes() { ComercialActual = 1 };
            reportesManagerMock.Setup(x => x.DetallePosicion(1,10, fecha, null)).Returns(new ExcelDetallePosicionDto()
            {
                Headers = new string[] { "1","2" },
                Data = new List<string[]>() { new string[] {"a","b" } },
                Name = "B",
                SheetName = "C"
            });
            
            var result = target.DetalleExcel(10, 1, "26-10-2018", null);
            Assert.NotNull(result);
        }

        //[Test]
        //public void ReporteComprasDelDiaTest()
        //{
        //    DateTime.TryParse("26-10-2018", out DateTime fecha);
        //    var reportes = new ParamReportes() { ComercialActual = 1 };
        //    reportesManagerMock.Setup(x => x.PosicionPorMaterial(fecha)).Returns(new List<ExcelPosicionMaterialDto>()
        //    {
        //        new ExcelPosicionMaterialDto()
        //        {
        //            Contrato = "123",
        //            Comercial = "as",
        //            RazonSocial = "BF"
        //        }
        //    });

        //    var result = target.ReporteComprasDelDia("26-10-2018");
        //    Assert.NotNull(result);
        //}
    }
}
