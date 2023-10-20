using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using static WebDataAgro.MvcApplication;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class TareasProgramadasControllerTest
    {
        private TareasProgramadasController target;
        private Mock<ILogger> loggerMock;
        private Mock<IContratoManager> contratoManagerMock;
        private Mock<IFijacionDePrecioContratoManager> fijacionManagerMock;
        private Mock<ICupoManager> cupoManagerMock;
        private Mock<IContratoAcuerdoManager> contratoAcuerdoManagerMock;
        private Mock<IReportesManager> reportesManagerMock;
        private Mock<INegocioManager> negocioManagerMock;
        private Mock<IDiferencialManager> diferencialManagerMock;
        private Mock<IAdministracionCupoManager> administracionCupoManagerMock;
        private Mock<IHedgeManager> oHedgeManagerMock;
        private Mock<IProveedorManager> proveedorManagerMock;
        private Mock<IPrecioPizarraManager> precioPizarraManagerMock;
        private Mock<IFAQManager> faqManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            serializer = new JavaScriptSerializer();
            loggerMock = new Mock<ILogger>();
            contratoManagerMock = new Mock<IContratoManager>();
            fijacionManagerMock = new Mock<IFijacionDePrecioContratoManager>();
            cupoManagerMock = new Mock<ICupoManager>();
            contratoAcuerdoManagerMock = new Mock<IContratoAcuerdoManager>();
            reportesManagerMock = new Mock<IReportesManager>();
            negocioManagerMock = new Mock<INegocioManager>();
            administracionCupoManagerMock = new Mock<IAdministracionCupoManager>();
            oHedgeManagerMock = new Mock<IHedgeManager>();
            diferencialManagerMock = new Mock<IDiferencialManager>();
            proveedorManagerMock = new Mock<IProveedorManager>();
            precioPizarraManagerMock = new Mock<IPrecioPizarraManager>();
            faqManagerMock = new Mock<IFAQManager>();

            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new TareasProgramadasController(loggerMock.Object,
                                                     contratoManagerMock.Object,
                                                     fijacionManagerMock.Object,
                                                     cupoManagerMock.Object,
                                                     contratoAcuerdoManagerMock.Object,
                                                     reportesManagerMock.Object,
                                                     negocioManagerMock.Object,
                                                     administracionCupoManagerMock.Object,
                                                     oHedgeManagerMock.Object,
                                                     diferencialManagerMock.Object,
                                                     proveedorManagerMock.Object,
                                                     precioPizarraManagerMock.Object,
                                                     faqManagerMock.Object);
        }

        [Test]
        public void EnvioMailPendientesTest()
        {
            contratoManagerMock.Setup(x => x.EnviarMailPendiente());
            var result = target.EnvioMailPendientes() as ContentResult;

            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }
        [Test]
        public void FinalizacionContratoTest()
        {
            contratoManagerMock.Setup(x => x.FinalizacionAutomatica(GlobalVariables.IdActiveDirectory));
            fijacionManagerMock.Setup(x => x.FinalizacionAutomatica(GlobalVariables.IdActiveDirectory));
            var result = target.FinalizacionContratos() as ContentResult;

            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void BorradoContratosTest()
        {
            contratoManagerMock.Setup(x => x.BorradoAutomatico());
            var result = target.BorradoContratos() as ContentResult;

            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void CrearSugerenciaCupo()
        {
            cupoManagerMock.Setup(x => x.CrearSugerenciaCupo(It.IsAny<int>(), It.IsAny<FormulaDto>(), It.IsAny<ConfiguracionCupo>()));
            var result = target.CrearSugerenciaCupo() as ContentResult;

            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void GrabarDatosReporteCompraNetTest()
        {
            reportesManagerMock.Setup(x => x.GrabarDatosReporteCompraNet(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<List<int>>()));
            var result = target.GrabarDatosReporteCompraNet("") as ContentResult;

            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void EnviarMailSugerenciasPendientesPorComercialTest()
        {
            cupoManagerMock.Setup(x => x.EnviarMailSugerenciasPendientesPorComercial());
            var result = target.EnviarMailSugerenciasPendientesPorComercial() as ContentResult;

            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void ActualizarCumplimientoCuposTest()
        {
            var result = target.ActualizarCumplimientoCupos() as ContentResult;
            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void ActualizarRazonSocialTest()
        {
            var result = target.ActualizarRazonSocial() as ContentResult;
            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void ReportePagosDiferidosTest()
        {
            reportesManagerMock.Setup(x => x.ObtenerDatosReportePagosDiferidos(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(
                new ResultReportePagosDiferidos
                {
                    Desde = DateTime.Now.Date.AddDays(-7),
                    Hasta = DateTime.Now.Date,
                    Contratos = new List<ReportePagosDiferidos> {
                        new ReportePagosDiferidos{ContratoSAP = "1", Tn = 5000, PrecioUSD = 230, Precio = 230 * 94, Plazo = 33, Toma = DateTime.Now.Date, TNA = 30 ,
                            Estado = "Vigente",AcumuladoMesAnterior=1,AlVencimiento=1,Capital=1,CapitalMasIntereses=1,Corredor ="corr",CorredorCUIT="",DevengadoMes=1,
                            InteresesPorDia =1,InteresesTotales=1,M2MMes=1,TEA=1,TipoCambio=1,Vencimiento=DateTime.Now.Date.AddDays(1),Vendedor="",VendedorCUIT="",  }
                    }
                });

            var result = target.ReportePagosDiferidos("") as ContentResult;
            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void ActualizarCumplimientoCuposMasivoTest()
        {
            var result = target.ActualizarCumplimientoCuposMasivo("20210101", "20210201") as ContentResult;
            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
            cupoManagerMock.Verify(y => y.ActualizarCumplimientoCupos(It.IsAny<DateTime>()), Times.Exactly(32));

        }

        [Test]
        public void EnvioMailNegociosAnulaYReemplazaTest()
        {
            var result = target.EnvioMailNegociosAnulaYReemplaza() as ContentResult;
            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void ConsultarMisturnosActivosTest()
        {
            var result = target.ConsultarMisturnosActivos() as ContentResult;
            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void ActualizarEstadoDeContratosTest()
        {
            var result = target.ActualizarEstadoDeContratos() as ContentResult;
            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void ActualizarMailProveedorTest()
        {
            var result = target.ActualizarMailProveedor() as ContentResult;
            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void ActualizarPrecioPizarraTest()
        {
            var result = target.ActualizarPrecioPizarra() as ContentResult;
            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void ActualizarProveedoresHomeTest()
        {
            var result = target.ActualizarProveedoresHome() as ContentResult;
            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void ConfirmacionAutomaticaPizarra13HrsTest()
        {
            var result = target.ConfirmacionAutomaticaPizarra13Hrs() as ContentResult;
            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void ActualizarFechaUltimaActualizacionManualesFAQTest()
        {
            var result = target.ActualizarFechaUltimaActualizacionManualesFAQ() as ContentResult;
            Assert.NotNull(result);
            var expectedResult = new ContentResult { Content = "ok" };
            Assert.AreEqual(result.Content, expectedResult.Content);
        }
    }
}
