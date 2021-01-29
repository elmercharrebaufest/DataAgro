using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report.Clases;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public class FormularioAltaNoGranosControllerTest
    {
        private FormularioAltaNoGranosController target;
        private Mock<IReportesManager> reportesManagerMock;

        [SetUp]
        public void SetUp()
        {
            reportesManagerMock = new Mock<IReportesManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            HttpContext.Current.Session["perfil"] = 1;
            target = new FormularioAltaNoGranosController(reportesManagerMock.Object);
        }




        [Test]
        public void GenerarTest()
        {
            ProveedorAltaDto oParam = new ProveedorAltaDto {
                RazonSocial = "rrr",
                Telefono = "21312312",
                Mail = "1",
                CUIT = "1",
                IngresoBruto = "1",
                SituacionIVA = "1",
                Observaciones = "1",
                CBU = "1",
                Rubro = "1",
                CondicionDePago = "1",
                ServicioPrestado = "1",
                OrganizacionDeCompra = "1",
                RazonDeEleccion = "1",
                FacturacionAnual = 1,
                SolicitanteInterno = "1",
            };
           
           
            var result = target.Generar(oParam);

            var model = result.Result as JsonResult;
            Assert.NotNull(result);
            Assert.IsTrue(((ReportesModel)model.Data).DownloadKey != "");
        }


    }
}
