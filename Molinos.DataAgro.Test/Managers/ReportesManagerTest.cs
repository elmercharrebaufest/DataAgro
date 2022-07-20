using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq.Expressions;
using System.Web.Script.Serialization;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ReportesManagerTest
    {
        private ReportesManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<IComercialManager> comercialManagerMock;
        private Mock<ITipoDeCambioAgent> tipoDeCambioMock;
        private JavaScriptSerializer serializer;
        private Mock<IContratosAPesificarAgent> pesificarAgent;
        private Mock<IMailManager> mailManager;
        private Mock<IHttpContextManager> httpContextManager;

        [SetUp]
        public void SetUp()
        {
            ConfigurationManager.AppSettings["TNAReportePagosDiferidos"] = "30";
            ConfigurationManager.AppSettings["CredentialUserName"] = "dataagro.baufest@gmail.com";
            ConfigurationManager.AppSettings["UrlBaseMOAOperaciones"] = "www.sitio.com";
            ConfigurationManager.AppSettings["SmtpServerPort"] = "587";
            ConfigurationManager.AppSettings["SmtpServer"] = "smtp.gmail.com";
            ConfigurationManager.AppSettings["UseDefaultCredentials"] = "S";
            ConfigurationManager.AppSettings["EnableSSL"] = "S";
            ConfigurationManager.AppSettings["CredentialPassword"] = "Hola1234";
            this.serializer = new JavaScriptSerializer();
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            comercialManagerMock = new Mock<IComercialManager>();
            tipoDeCambioMock = new Mock<ITipoDeCambioAgent>();
            pesificarAgent = new Mock<IContratosAPesificarAgent>();
            mailManager = new Mock<IMailManager>();
            httpContextManager = new Mock<IHttpContextManager>();
            httpContextManager.Setup(x => x.ObtenerPathLogoMail()).Returns(TestContext.CurrentContext.TestDirectory + "\\Util\\MolinosAgro.png");
            target = new ReportesManager(logger.Object, repositorioMock.Object, comercialManagerMock.Object, tipoDeCambioMock.Object, 
                pesificarAgent.Object, mailManager.Object, httpContextManager.Object);
            tipoDeCambioMock.Setup(x => x.TraerTipoDeCambio(null)).Returns(45);
            tipoDeCambioMock.Setup(x => x.TraerTipoDeCambio(It.IsAny<DateTime>())).Returns(94);

        }

        [Test]
        public void GrabarReporteOkTest()
        {
            var fecha = new DateTime(2018, 10, 26);
            var reporte = new Reportes { Contenido = new byte[10], FileName = "A", Identificador = "1" };

            var result = target.GrabarReporte(reporte);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<Reportes>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }
        [Test]
        public void ObtenerReporteOkTest()
        {
            var reporte = new ReportesDto { Contenido = new byte[10], FileName = "A", Identificador = "1" };
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Reportes, bool>>>(), It.IsAny<Expression<Func<Reportes, ReportesDto>>>())).Returns(reporte);
            var result = target.ObtenerReporte("a");

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Reportes, bool>>>(), It.IsAny<Expression<Func<Reportes, ReportesDto>>>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.NotNull(result);
        }
        [Test]
        public void TraerDatosInicialesOkTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Campaña, CampañaQry>>>(), It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Desc))
                .Returns(new List<CampañaQry>() { new CampañaQry { CampañaId = 1, Descripcion = "18-19" } });
            comercialManagerMock.Setup(y => y.ListarEquipo("a"))
                .Returns(new EquipoDto { Equipo = new List<int>() { 1, 2, 3 }, EquipoReal = new List<int>() { 1, 2, 3 } });

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, ComercialQry>>>())).Returns(new ComercialQry { ComercialId = 1, Nombre = "a a" });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, MaterialesQry>>>(), It.IsAny<Expression<Func<Material, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<MaterialesQry>() { new MaterialesQry { MaterialId = 1, Descripcion = "a" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Provincia, ProvinciaQry>>>(), It.IsAny<Expression<Func<Provincia, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProvinciaQry>() { new ProvinciaQry { Provinciaid = 1, Nombre = "a" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Segmentacion, SegmentacionQry>>>(), It.IsAny<Expression<Func<Segmentacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<SegmentacionQry>() { new SegmentacionQry { SegmentacionId = 1, Descripcion = "a" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<InformeComercialEstado, EstadoICQry>>>(), It.IsAny<Expression<Func<InformeComercialEstado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<EstadoICQry>() { new EstadoICQry { EstadoInformeId = 1, Descripcion = "a" } });

            var result = target.TraerDatosIniciales("a");

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Campaña, CampañaQry>>>(), It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Desc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Material, MaterialesQry>>>(), It.IsAny<Expression<Func<Material, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Provincia, ProvinciaQry>>>(), It.IsAny<Expression<Func<Provincia, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Segmentacion, SegmentacionQry>>>(), It.IsAny<Expression<Func<Segmentacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<InformeComercialEstado, EstadoICQry>>>(), It.IsAny<Expression<Func<InformeComercialEstado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, ComercialQry>>>()), Times.Exactly(3));

            Assert.NotNull(result);
        }
        #region Compras
        [Test]
        public void TraerComprasMapaTest()
        {
            repositorioMock.Setup(y => y.SelStore<ResulIndicadores>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, null, ""))
                .Returns(new List<ResulIndicadores>() { new ResulIndicadores { Cuit = 1, CantidadDeCliente = 1 } });

            var result = target.TraerComprasMapa(new ParamReportes(), new List<int>());

            repositorioMock.Verify(x => x.SelStore<ResulIndicadores>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, null, ""), Times.Once);
            Assert.NotNull(result);
        }
        [Test]
        public void TraerComprasMapaExportacionTest()
        {
            repositorioMock.Setup(y => y.SelStore<ResultIndicadoresReportesmini>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, null, ""))
                .Returns(new List<ResultIndicadoresReportesmini>() { new ResultIndicadoresReportesmini { Cuit = "1", Toneladas = 1 } });

            var result = target.TraerComprasMapaExportacion(new ParamReportes(), new List<int>());

            repositorioMock.Verify(x => x.SelStore<ResultIndicadoresReportesmini>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, null, ""), Times.Once);
            Assert.NotNull(result);
        }
        [Test]
        public void TraerComprasTortaTest()
        {
            repositorioMock.Setup(y => y.SelStore<ResulIndicadores>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<string>()))
                .Returns(new List<ResulIndicadores>() { new ResulIndicadores { Cuit = 1, CantidadDeCliente = 1 } });

            var result = target.TraerComprasTorta(new ParamReportes(), new List<int>());

            repositorioMock.Verify(x => x.SelStore<ResulIndicadores>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<string>()), Times.Once);
            Assert.NotNull(result);
        }
        [Test]
        public void TraerComprasTortaExportacionTest()
        {
            repositorioMock.Setup(y => y.SelStore<ResultIndicadoresReportesTorta>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, ""))
                .Returns(new List<ResultIndicadoresReportesTorta>() { new ResultIndicadoresReportesTorta { Cuit = "1", Toneladas = 1 } });

            var result = target.TraerComprasTortaExportacion(new ParamReportes(), new List<int>());

            repositorioMock.Verify(x => x.SelStore<ResultIndicadoresReportesTorta>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, ""), Times.Once);
            Assert.NotNull(result);
        }
        [Test]
        public void TraerComprasBarraTest()
        {
            repositorioMock.Setup(y => y.SelStore<ResultComprasBarrasReportes>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, null, ""))
                .Returns(new List<ResultComprasBarrasReportes>() { new ResultComprasBarrasReportes { MasTn100 = 1 } });

            var result = target.TraerComprasBarra(new ParamReportes(), new List<int>());

            repositorioMock.Verify(x => x.SelStore<ResultComprasBarrasReportes>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, null, ""), Times.Once);
            Assert.NotNull(result);
        }
        [Test]
        public void TraerComprasBarraExportacionTest()
        {
            repositorioMock.Setup(y => y.SelStore<ResultComprasBarrasReportesmini>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, null, ""))
                .Returns(new List<ResultComprasBarrasReportesmini>() { new ResultComprasBarrasReportesmini { Cuit = "1", Toneladas = 1 } });

            var result = target.TraerComprasBarraExportacion(new ParamReportes(), new List<int>());

            repositorioMock.Verify(x => x.SelStore<ResultComprasBarrasReportesmini>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, null, ""), Times.Once);
            Assert.NotNull(result);
        }
        #endregion
        #region Productiva
        [Test]
        public void TraerCapacidadProductivaMapaTest()
        {
            repositorioMock.Setup(y => y.SelStore<ResulIndicadores>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, null, ""))
                .Returns(new List<ResulIndicadores>() { new ResulIndicadores { Cuit = 1, CantidadDeCliente = 1 } });

            var result = target.TraerCapacidadProductivaMapa(new ParamReportes(), new List<int>());

            repositorioMock.Verify(x => x.SelStore<ResulIndicadores>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, null, ""), Times.Once);
            Assert.NotNull(result);
        }
        [Test]
        public void TraerCapacidadProductivaMapaExportacionTest()
        {
            repositorioMock.Setup(y => y.SelStore<ResultProduccionMapaReportes>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, null, ""))
                .Returns(new List<ResultProduccionMapaReportes>() { new ResultProduccionMapaReportes { Cuit = "1", Toneladas = 1 } });

            var result = target.TraerCapacidadProductivaMapaExportacion(new ParamReportes(), new List<int>());

            repositorioMock.Verify(x => x.SelStore<ResultProduccionMapaReportes>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, null, ""), Times.Once);
            Assert.NotNull(result);
        }
        [Test]
        public void TraerCapacidadProductivaBarraTest()
        {
            repositorioMock.Setup(y => y.SelStore<ResultComprasBarrasReportes>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, ""))
                .Returns(new List<ResultComprasBarrasReportes>() { new ResultComprasBarrasReportes { MasTn100 = 1 } });

            var result = target.TraerCapacidadProductivaBarra(new ParamReportes(), new List<int>());

            repositorioMock.Verify(x => x.SelStore<ResultComprasBarrasReportes>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, ""), Times.Once);
            Assert.NotNull(result);
        }
        [Test]
        public void TraerCapacidadProductivaBarraExportacionTest()
        {
            repositorioMock.Setup(y => y.SelStore<ResultProduccionBarraReportes>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, ""))
                .Returns(new List<ResultProduccionBarraReportes>() { new ResultProduccionBarraReportes { Cuit = "1", Toneladas = 1 } });

            var result = target.TraerCapacidadProductivaBarraExportacion(new ParamReportes(), new List<int>());

            repositorioMock.Verify(x => x.SelStore<ResultProduccionBarraReportes>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, ""), Times.Once);
            Assert.NotNull(result);
        }
        #endregion
        #region Acopio
        [Test]
        public void TraerCapacidadDeAcopioMapaTest()
        {
            repositorioMock.Setup(y => y.SelStore<ResulIndicadores>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, null, ""))
                .Returns(new List<ResulIndicadores>() { new ResulIndicadores { Cuit = 1, CantidadDeCliente = 1 } });

            var result = target.TraerCapacidadDeAcopioMapa(new ParamReportes(), new List<int>());

            repositorioMock.Verify(x => x.SelStore<ResulIndicadores>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, null, ""), Times.Once);
            Assert.NotNull(result);
        }
        [Test]
        public void TraerCapacidadDeAcopioMapaExportacionTest()
        {
            repositorioMock.Setup(y => y.SelStore<ResultAcopioMapaReportes>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, null, ""))
                .Returns(new List<ResultAcopioMapaReportes>() { new ResultAcopioMapaReportes { Cuit = "1", Toneladas = 1 } });

            var result = target.TraerCapacidadDeAcopioMapaExportacion(new ParamReportes(), new List<int>());

            repositorioMock.Verify(x => x.SelStore<ResultAcopioMapaReportes>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, null, ""), Times.Once);
            Assert.NotNull(result);
        }
        [Test]
        public void TraerCapacidadDeAcopioBarraTest()
        {
            repositorioMock.Setup(y => y.SelStore<ResultComprasBarrasReportes>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, ""))
                .Returns(new List<ResultComprasBarrasReportes>() { new ResultComprasBarrasReportes { MasTn100 = 1 } });

            var result = target.TraerCapacidadDeAcopioBarra(new ParamReportes(), new List<int>());

            repositorioMock.Verify(x => x.SelStore<ResultComprasBarrasReportes>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, ""), Times.Once);
            Assert.NotNull(result);
        }
        [Test]
        public void TraerCapacidadDeAcopioBarraExportacionTest()
        {
            repositorioMock.Setup(y => y.SelStore<ResultAcopioBarraReportes>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, ""))
                .Returns(new List<ResultAcopioBarraReportes>() { new ResultAcopioBarraReportes { Cuit = "1", Toneladas = 1 } });

            var result = target.TraerCapacidadDeAcopioBarraExportacion(new ParamReportes(), new List<int>());

            repositorioMock.Verify(x => x.SelStore<ResultAcopioBarraReportes>(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, ""), Times.Once);
            Assert.NotNull(result);
        }
        #endregion
        #region Objetivos
        [Test]
        public void TraerObjetivosGaugeTest()
        {
            repositorioMock.Setup(y => y.SelStore<ResultObjetivoGaugeReportes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .Returns(new List<ResultObjetivoGaugeReportes>() { new ResultObjetivoGaugeReportes { Cuit = "1", Toneladas = 1 } });

            var result = target.TraerObjetivosGauge(new ParamReportes(), new List<int>());

            repositorioMock.Verify(x => x.SelStore<ResultObjetivoGaugeReportes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()), Times.Once);
            Assert.NotNull(result);
        }
        [Test]
        public void TraerObjetivosGaugeExportacionTest()
        {
            repositorioMock.Setup(y => y.SelStore<ResultObjetivoGaugeReportes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .Returns(new List<ResultObjetivoGaugeReportes>() { new ResultObjetivoGaugeReportes { Cuit = "1", Toneladas = 1 } });

            var result = target.TraerObjetivosGaugeExportacion(new ParamReportes(), new List<int>());

            repositorioMock.Verify(x => x.SelStore<ResultObjetivoGaugeReportes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()), Times.Once);
            Assert.NotNull(result);
        }
        #endregion
        [Test]
        public void TraerDatosGrillaBDTest()
        {
            var fecha = new DateTime(2018, 10, 26);
            var reporte = new ParamReportes { FechaDesde = fecha, FechaHasta = fecha, Mes = 1, Segmentacion = "a", Cosecha = 1, Toneladas = 1, Comercial = 1, ComercialActual = 1, Grano = 1 };
            repositorioMock.Setup(y => y.SelStore<valoresGrilla>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<valoresGrilla>() { new valoresGrilla { Cuit = "1", Toneladas = 1, Segmentación = "a" } });

            var result = target.TraerDatosGrillaBD(reporte, new List<int>());

            repositorioMock.Verify(x => x.SelStore<valoresGrilla>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            Assert.NotNull(result);
        }
        [Test]
        public void TraerBasedeDatosTest()
        {
            var result = target.TraerBasedeDatos(new ParamReportes());

            Assert.NotNull(result);
        }
        #region TransformarFiltros
        [Test]
        public void TransformarFiltrosTestSinFiltros()
        {
            var result = target.TransformarFiltros(new ParamReportes());
            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, string>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<Expression<Func<Provincia, string>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<Expression<Func<Campaña, string>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Objetivo, bool>>>(), It.IsAny<Expression<Func<Objetivo, string>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, string>>>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Segmentacion, string>>>(), It.IsAny<Expression<Func<Segmentacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Desc), Times.Never);

            Assert.Null(result.FECHADESDE_);
            Assert.Null(result.FECHAHASTA_);
            Assert.Null(result.MES_);
            Assert.Null(result.Indicadores);
            Assert.Null(result.GRANO_);
            Assert.Null(result.PROVINCIA_);
            Assert.Null(result.COSECHA_);
            Assert.Null(result.SEGMENTACION_);
            Assert.Null(result.OBJETIVOS_);
            Assert.Null(result.TONELADAS_);
            Assert.Null(result.COMERCIAL_);
            Assert.Null(result.COMERCIALACTUAL_);
        }
        [Test]
        public void TransformarFiltrosTestConFiltros()
        {
            var fecha = new DateTime(2018, 10, 26);
            var param = new ParamReportes
            {
                Indicadores = "compras",
                Grano = 2,
                Mes = 1,
                Provincia = 1,
                Cosecha = 1,
                Segmentacion = "1,2",
                Comercial = 1,
                Objetivos = 1,
                Toneladas = 1,
                FechaDesde = fecha,
                FechaHasta = fecha,
                ComercialActual = 1
            };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<Expression<Func<Provincia, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<Expression<Func<Campaña, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Objetivo, bool>>>(), It.IsAny<Expression<Func<Objetivo, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Segmentacion, string>>>(), It.IsAny<Expression<Func<Segmentacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<string>() { "a", "b" });

            var result = target.TransformarFiltros(param);
            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, string>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<Expression<Func<Provincia, string>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<Expression<Func<Campaña, string>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Objetivo, bool>>>(), It.IsAny<Expression<Func<Objetivo, string>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, string>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Segmentacion, string>>>(), It.IsAny<Expression<Func<Segmentacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);

            Assert.AreEqual("26/10/2018", result.FECHADESDE_);
            Assert.AreEqual("26/10/2018", result.FECHAHASTA_);
            Assert.AreEqual("Enero", result.MES_);
            Assert.AreEqual("Compras", result.Indicadores);
            Assert.AreEqual("a", result.GRANO_);
            Assert.AreEqual("a", result.PROVINCIA_);
            Assert.AreEqual("a", result.COSECHA_);
            Assert.AreEqual("a,b", result.SEGMENTACION_);
            Assert.AreEqual("a", result.OBJETIVOS_);
            Assert.AreEqual("Menos de 2500 Tn.", result.TONELADAS_);
            Assert.AreEqual("a", result.COMERCIAL_);
            Assert.AreEqual("a", result.COMERCIALACTUAL_);
        }
        [Test]
        public void TransformarFiltrosTestConFiltrosMesIndicadoresToneladas()
        {
            var param = new ParamReportes
            {
                Indicadores = "objetivoscomercial",
                Mes = 2,
                Toneladas = 2,
                ComercialActual = 1
            };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, string>>>())).Returns("a");

            var result = target.TransformarFiltros(param);
            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, string>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<Expression<Func<Provincia, string>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<Expression<Func<Campaña, string>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Objetivo, bool>>>(), It.IsAny<Expression<Func<Objetivo, string>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, string>>>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Segmentacion, string>>>(), It.IsAny<Expression<Func<Segmentacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Never);

            Assert.Null(result.FECHADESDE_);
            Assert.Null(result.FECHAHASTA_);
            Assert.AreEqual("Febrero", result.MES_);
            Assert.AreEqual("Objetivos Comerciales", result.Indicadores);
            Assert.AreEqual("Entre 2500 y 5000 Tn.", result.TONELADAS_);
            Assert.AreEqual("a", result.COMERCIALACTUAL_);
            Assert.Null(result.GRANO_);
            Assert.Null(result.PROVINCIA_);
            Assert.Null(result.COSECHA_);
            Assert.Null(result.OBJETIVOS_);
            Assert.Null(result.COMERCIAL_);
            Assert.Null(result.SEGMENTACION_);
        }
        [Test]
        public void TransformarFiltrosTestConFiltrosMes3IndicadoresToneladas()
        {
            var param = new ParamReportes
            {
                Indicadores = "basededatos",
                Mes = 3,
                Toneladas = 3,
                ComercialActual = 1
            };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, string>>>())).Returns("a");

            var result = target.TransformarFiltros(param);
            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, string>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<Expression<Func<Provincia, string>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<Expression<Func<Campaña, string>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Objetivo, bool>>>(), It.IsAny<Expression<Func<Objetivo, string>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, string>>>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Segmentacion, string>>>(), It.IsAny<Expression<Func<Segmentacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Never);

            Assert.Null(result.FECHADESDE_);
            Assert.Null(result.FECHAHASTA_);
            Assert.AreEqual("Marzo", result.MES_);
            Assert.AreEqual("Base De Datos", result.Indicadores);
            Assert.AreEqual("Mas de 5000 Tn.", result.TONELADAS_);
            Assert.AreEqual("a", result.COMERCIALACTUAL_);
            Assert.Null(result.GRANO_);
            Assert.Null(result.PROVINCIA_);
            Assert.Null(result.COSECHA_);
            Assert.Null(result.OBJETIVOS_);
            Assert.Null(result.COMERCIAL_);
            Assert.Null(result.SEGMENTACION_);
        }
        [Test]
        public void TransformarFiltrosTestConFiltrosMes4Indicadores()
        {
            var param = new ParamReportes
            {
                Indicadores = "capacidadproductiva",
                Mes = 4,
                ComercialActual = 1
            };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, string>>>())).Returns("a");

            var result = target.TransformarFiltros(param);
            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, string>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<Expression<Func<Provincia, string>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<Expression<Func<Campaña, string>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Objetivo, bool>>>(), It.IsAny<Expression<Func<Objetivo, string>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, string>>>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Segmentacion, string>>>(), It.IsAny<Expression<Func<Segmentacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Never);

            Assert.Null(result.FECHADESDE_);
            Assert.Null(result.FECHAHASTA_);
            Assert.AreEqual("Abril", result.MES_);
            Assert.AreEqual("Capacidad Productiva", result.Indicadores);
            Assert.AreEqual("a", result.COMERCIALACTUAL_);
            Assert.Null(result.TONELADAS_);
            Assert.Null(result.GRANO_);
            Assert.Null(result.PROVINCIA_);
            Assert.Null(result.COSECHA_);
            Assert.Null(result.OBJETIVOS_);
            Assert.Null(result.COMERCIAL_);
            Assert.Null(result.SEGMENTACION_);
        }
        [Test]
        public void TransformarFiltrosTestConFiltrosMes5Indicadores()
        {
            var param = new ParamReportes
            {
                Indicadores = "capacidadacopio",
                Mes = 5,
                ComercialActual = 1
            };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, string>>>())).Returns("a");

            var result = target.TransformarFiltros(param);
            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, string>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<Expression<Func<Provincia, string>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<Expression<Func<Campaña, string>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Objetivo, bool>>>(), It.IsAny<Expression<Func<Objetivo, string>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, string>>>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Segmentacion, string>>>(), It.IsAny<Expression<Func<Segmentacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Never);

            Assert.Null(result.FECHADESDE_);
            Assert.Null(result.FECHAHASTA_);
            Assert.AreEqual("Mayo", result.MES_);
            Assert.AreEqual("Capacidad De Acopio", result.Indicadores);
            Assert.AreEqual("a", result.COMERCIALACTUAL_);
            Assert.Null(result.TONELADAS_);
            Assert.Null(result.GRANO_);
            Assert.Null(result.PROVINCIA_);
            Assert.Null(result.COSECHA_);
            Assert.Null(result.OBJETIVOS_);
            Assert.Null(result.COMERCIAL_);
            Assert.Null(result.SEGMENTACION_);
        }
        [Test]
        public void TransformarFiltrosTestConFiltrosMes6()
        {
            var param = new ParamReportes
            {
                Mes = 6,
                ComercialActual = 1
            };

            var result = target.TransformarFiltros(param);
            Assert.NotNull(result);

            Assert.AreEqual("Junio", result.MES_);
        }
        [Test]
        public void TransformarFiltrosTestConFiltrosMes7()
        {
            var param = new ParamReportes
            {
                Mes = 7,
                ComercialActual = 1
            };

            var result = target.TransformarFiltros(param);
            Assert.NotNull(result);

            Assert.AreEqual("Julio", result.MES_);
        }
        [Test]
        public void TransformarFiltrosTestConFiltrosMes8()
        {
            var param = new ParamReportes
            {
                Mes = 8,
                ComercialActual = 1
            };

            var result = target.TransformarFiltros(param);
            Assert.NotNull(result);

            Assert.AreEqual("Agosto", result.MES_);
        }
        [Test]
        public void TransformarFiltrosTestConFiltrosMes9()
        {
            var param = new ParamReportes
            {
                Mes = 9,
                ComercialActual = 1
            };

            var result = target.TransformarFiltros(param);
            Assert.NotNull(result);

            Assert.AreEqual("Septiembre", result.MES_);
        }
        [Test]
        public void TransformarFiltrosTestConFiltrosMes10()
        {
            var param = new ParamReportes
            {
                Mes = 10,
                ComercialActual = 1
            };

            var result = target.TransformarFiltros(param);
            Assert.NotNull(result);

            Assert.AreEqual("Octubre", result.MES_);
        }
        [Test]
        public void TransformarFiltrosTestConFiltrosMes11()
        {
            var param = new ParamReportes
            {
                Mes = 11,
                ComercialActual = 1
            };

            var result = target.TransformarFiltros(param);
            Assert.NotNull(result);

            Assert.AreEqual("Noviembre", result.MES_);
        }
        [Test]
        public void TransformarFiltrosTestConFiltrosMes12()
        {
            var param = new ParamReportes
            {
                Mes = 12,
                ComercialActual = 1
            };

            var result = target.TransformarFiltros(param);
            Assert.NotNull(result);

            Assert.AreEqual("Diciembre", result.MES_);
        }
        #endregion
        [Test]
        public void TraerToneladasGranoTipoTest()
        {
            var fecha = new DateTime(2018, 10, 26);
            repositorioMock.Setup(y => y.ObtenerConsultaEscalar(It.IsAny<TraerToneladasPorGrano>()))
                .Returns(new ToneladasGranoTipoDto { Material = "a", Total = 0 });
            var result = target.TraerToneladasGranoTipo(fecha, fecha, new List<int>() { 1, 2, 3, 4, 5 });

            repositorioMock.Verify(x => x.ObtenerConsultaEscalar(It.IsAny<TraerToneladasPorGrano>()), Times.Exactly(5));
            Assert.NotNull(result);
            Assert.AreEqual(5, result.Count);
        }
        [Test]
        public void TraerToneladasSojaSustTest()
        {
            var fecha = new DateTime(2018, 10, 26);
            repositorioMock.Setup(y => y.ObtenerConsultaEscalar(It.IsAny<TraerToneladasSojaSustentable>()))
                .Returns(new ReporteSojaSustDto { Fijar = 1, Precio = 1, Total = 1 });
            var result = target.TraerToneladasSojaSust(fecha, fecha);

            repositorioMock.Verify(x => x.ObtenerConsultaEscalar(It.IsAny<TraerToneladasSojaSustentable>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Fijar);
            Assert.AreEqual(1, result.Precio);
            Assert.AreEqual(1, result.Total);
        }
        [Test]
        public void TraerPosicionComprasTest()
        {
            var fecha = new DateTime(2018, 10, 26);
            var fecha1 = new DateTime(2018, 11, 16);
            var fecha2 = new DateTime(2018, 12, 26);
            var fecha3 = DateTime.Now.AddDays(312);

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, PosicionPorMaterial>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<PosicionPorMaterial>() { new PosicionPorMaterial { Id = 1, Precio = 1, Cantidad = 1, FechaDesde = fecha, FechaHasta = fecha } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, PosicionPorMaterial>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<PosicionPorMaterial>() { new PosicionPorMaterial { Id = 1, Precio = 1, Cantidad = 1, FechaDesde = fecha, FechaHasta = fecha2 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Fason, PosicionPorMaterial>>>(), It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<PosicionPorMaterial>() { new PosicionPorMaterial { Id = 1, Precio = 1, Cantidad = 1, FechaDesde = fecha, FechaHasta = fecha, Posicion = "02.2019" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, PosicionPorMaterial>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<PosicionPorMaterial>() { new PosicionPorMaterial { Id = 1, Precio = 1, Cantidad = 1, FechaDesde = fecha2, FechaHasta = fecha2 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PrecioPizarra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc))
                .Returns(new List<PrecioPizarra>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Negocio, BasicoContrato>>>(), It.IsAny<Expression<Func<Negocio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
             .Returns(new List<BasicoContrato>() {
                 new BasicoContrato { Fecha = fecha,EsFason = false, Id = 1, Precio = 1, Cantidad = 1, FechaDesde = fecha, FechaHasta = fecha2,TipoNegocioId=3,ContratoSAP="1", Estado = 2,TrigoEspecial=true,MaterialId=1,ContratoAcuerdoId = null,OcultarEnTablero=false },
                 new BasicoContrato { Fecha = fecha,EsFason=false, Id = 2, Precio = 1, Cantidad = 1, FechaDesde = fecha, FechaHasta = fecha1,TipoNegocioId=3,StandardCalidadId =1, Estado = 2 ,MaterialId=1,ContratoAcuerdoId = null,OcultarEnTablero=false},
                 new BasicoContrato { Fecha = fecha,EsFason=false, Id = 3, Precio = 1, Cantidad = 1, FechaDesde = fecha1, FechaHasta = fecha,TipoNegocioId=1, Estado = 2,MaterialId=1,StandardCalidadId=1,DestinoId=1,ContratoAcuerdoId= null,OcultarEnTablero=false },
                 new BasicoContrato { Fecha = fecha,EsFason=false, Id = 3, Precio = 1, Cantidad = 1, FechaDesde = fecha, FechaHasta = fecha1,TipoNegocioId=2, Estado = 2,MaterialId=1,StandardCalidadId=1,DestinoId=1,ContratoAcuerdoId= null,OcultarEnTablero=false },
                 new BasicoContrato { Fecha = fecha,EsFason=false, Id = 3, Precio = 1, Cantidad = 1, FechaDesde = fecha, FechaHasta = fecha2,TipoNegocioId=3, Estado = 2,MaterialId=1,StandardCalidadId=null,DestinoId=1,ContratoAcuerdoId= null,OcultarEnTablero=false },
                 new BasicoContrato { Fecha = fecha,EsFason=false, Id = 3, Precio = 1, Cantidad = 1, FechaDesde = fecha3, FechaHasta = fecha,TipoNegocioId=4, Estado = 2,MaterialId=1,StandardCalidadId=1,DestinoId=1,ContratoAcuerdoId= null,OcultarEnTablero=false },
                 new BasicoContrato { Fecha = fecha,EsFason=false, Id = 3, Precio = 1, Cantidad = 1, FechaDesde = fecha, FechaHasta = fecha,TipoNegocioId=5, Estado = 2,MaterialId=1,StandardCalidadId=1,DestinoId=1,ContratoAcuerdoId= null,OcultarEnTablero=false },
                 new BasicoContrato { Fecha = fecha,EsFason=false, Id = 3, Precio = 1, Cantidad = 1, FechaDesde = fecha, FechaHasta = fecha,TipoNegocioId=6, Estado = 2,MaterialId=1,StandardCalidadId=1,DestinoId=1,ContratoAcuerdoId= null,OcultarEnTablero=false },
                 new BasicoContrato { Fecha = fecha,EsFason=false, Id = 3, Precio = 1, Cantidad = 1, FechaDesde = fecha, FechaHasta = fecha,TipoNegocioId=1, Estado = 2,MaterialId=1,StandardCalidadId=1,DestinoId=1,ContratoAcuerdoId= null,OcultarEnTablero=false }
             });
            //var contratosDeFijaciones = repositorio.Listar<Negocio>(x => x.TipoNegocioId == 1 && contratoSapFijaciones.Contains(x.ContratoSAP)).ToList();
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Negocio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
             .Returns(new List<Negocio> { new Negocio { ContratoSAP = "1", StandardDeCalidadId = 4 } });
            var result = target.TraerPosicionCompras(fecha, fecha, new List<int>() { 1, 2, 3, 4, 5 });

            //repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Contrato, PosicionPorMaterial>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Exactly(7));
            //repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, PosicionPorMaterial>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Exactly(7));
            //repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Fason, PosicionPorMaterial>>>(), It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Exactly(7));
            //repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, PosicionPorMaterial>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Exactly(7));
            Assert.NotNull(result);
            Assert.AreEqual(5, result.Count);
        }
        [Test]
        public void TraerMonedaCantidadTest()
        {
            var fecha = new DateTime(2018, 10, 26);
            repositorioMock.Setup(y => y.ListarConsulta(It.IsAny<TraerMonedaKilo>()))
                .Returns(new List<PrecioCantidadDto>() { new PrecioCantidadDto { Moneda = "ARP  ", Cantidad = 10 } });
            var result = target.TraerMonedaCantidad(fecha, fecha, new List<int>() { 1, 2, 3, 4, 5 });

            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<TraerMonedaKilo>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Pesos", result[0].Moneda);
        }
        [Test]
        public void TraerTodosHedgeMaterialTestOk()
        {
            var fecha = new DateTime(2018, 10, 26);
            var fecha1 = new DateTime(2018, 10, 28);

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<HedgeMaterial, HedgeMaterialDto>>>(), It.IsAny<Expression<Func<HedgeMaterial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<HedgeMaterialDto>() { new HedgeMaterialDto { Id = 1, Cantidad = 1, Fecha = fecha, MaterialId = 1 } });

            var result = target.TraerTodosHedgeMaterial(fecha, fecha, new List<int>() { 1, 2, 3, 4, 5 });

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<HedgeMaterial, HedgeMaterialDto>>>(), It.IsAny<Expression<Func<HedgeMaterial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
        }
        [Test]
        public void TraerTodosHedgeMaterialTestError()
        {
            var fecha = new DateTime(2018, 10, 26);
            var fecha1 = new DateTime(2018, 10, 28);

            var result = target.TraerTodosHedgeMaterial(fecha, fecha1, new List<int>() { 1, 2, 3, 4, 5 });

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<HedgeMaterial, HedgeMaterialDto>>>(), It.IsAny<Expression<Func<HedgeMaterial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Never);
            Assert.NotNull(result);
            Assert.AreEqual(0, result.Count);
        }
        [Test]
        public void TraerHedgeObjetivoTestOk()
        {
            var fecha = new DateTime(2018, 10, 26);

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<HedgeObjetivo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<HedgeObjetivo>() {
                    new HedgeObjetivo { Id = 1, Cantidad = 5, Fecha = fecha, MaterialId = 1, TipoObjetivoId = 1 },
                    new HedgeObjetivo { Id = 1, Cantidad = 5, Fecha = fecha, MaterialId = 1, TipoObjetivoId = 2 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Contrato>() { new Contrato { Id = 1, Cantidad = 5, Fecha = fecha, MaterialId = 1, CampanaId = 7, EstadoId = 5, TipoNegocioId = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<FijacionDePrecioContrato>() { new FijacionDePrecioContrato { Id = 1, Cantidad = 5, Fecha = fecha, MaterialId = 1, CampanaId = 7, EstadoId = 5 } });

            var result = target.TraerHedgeObjetivo(fecha, fecha, new List<int>() { 1, 2, 3, 4, 5 });

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<HedgeObjetivo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(5, result.PricingCumplido);
            Assert.AreEqual(5, result.PricingCumplido);
            Assert.AreEqual(5, result.RemitirObjetivo);
            Assert.AreEqual(5, result.RemitirCumplido);
        }
        [Test]
        public void TraerHedgeObjetivoTestError()
        {
            var fecha = new DateTime(2018, 10, 26);
            var fecha1 = new DateTime(2018, 10, 28);

            var result = target.TraerHedgeObjetivo(fecha, fecha1, new List<int>() { 1, 2, 3, 4, 5 });

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<HedgeObjetivo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Never);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Never);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Never);
            Assert.NotNull(result);
        }
        [Test]
        public void TraerTcPromedioTestOk()
        {
            var fecha = new DateTime(2018, 10, 26);
            var mat = new Material { MaterialId = 1, CampañaId = 1, Descripcion = "a" };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<HedgeTC, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<HedgeTC>() {
                    new HedgeTC { Id = 1, Fecha = fecha, TipoCambio = 10, HedgePesos = 10 },
                    new HedgeTC { Id = 2, Fecha = fecha, TipoCambio = 20, HedgePesos = 10 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Contrato>() { new Contrato { Id = 1, Cantidad = 5, Fecha = fecha, MaterialId = 1, MonedaId = "ARP  ", CampanaId = 1, EstadoId = 5, TipoNegocioId = 1, Material = mat, Precio = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<FijacionDePrecioContrato>() { new FijacionDePrecioContrato { Id = 1, Cantidad = 5, Fecha = fecha, MaterialId = 1, MonedaId = "ARP  ", CampanaId = 1, EstadoId = 5, Material = mat, Precio = 2 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                            .Returns(new List<Fason>() { new Fason { Id = 1, Cantidad = 5, Fecha = fecha, MaterialId = 1, MonedaId = "ARP  ", CampanaId = 1, EstadoId = 5, Material = mat, Precio = 2 } });

            var result = target.TraerTcPromedio(fecha, fecha, new List<int>() { 1, 2, 3, 4, 5 });

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<HedgeTC, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(15, result.PromedioTC);
            Assert.AreEqual(15, result.TotalTC);
        }
        [Test]
        public void TraerTcPromedioTestError()
        {
            var fecha = new DateTime(2018, 10, 26);
            var fecha1 = new DateTime(2018, 10, 28);

            var result = target.TraerTcPromedio(fecha, fecha1, new List<int>() { 1, 2, 3, 4, 5 });

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<HedgeTC, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Never);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Never);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Never);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Never);

            Assert.NotNull(result);
        }
        [Test]
        public void TraerAgenteDeCompraTestOk()
        {
            var fecha = new DateTime(2018, 10, 26);
            var mat = new Material { MaterialId = 1, CampañaId = 1, Descripcion = "a" };
            var ope = new Operador { Descripcion = "a", Id = 1 };
            var tac = new TipoAgenteCompra { Id = 1, Descripcion = "a" };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AgenteCompra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AgenteCompra>() {
                    new AgenteCompra { Id = 1, Fecha = fecha, Posicion ="2.2019",MaterialId =1, Precio=10,TipoAgenteCompraId=1, OperadorId=1,EstadoId= 5,Cantidad=10,Material= mat, Operador=ope, TipoAgenteCompra = tac , MonedaId = ""},
                    new AgenteCompra { Id = 1, Fecha = fecha, Posicion ="1.2019",MaterialId =1, Precio=10,TipoAgenteCompraId=1, OperadorId=1,EstadoId= 5,Cantidad=10,Material= mat, Operador=ope, TipoAgenteCompra = tac , MonedaId = ""} });

            var result = target.TraerAgenteDeCompra(fecha, fecha, new List<int>() { 1, 2, 3, 4, 5 });

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<AgenteCompra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count);
        }
        //[Test]
        //public void TraerAgenteDeCompraTestError()
        //{
        //    var fecha = new DateTime(2018, 10, 26);
        //    var fecha1 = new DateTime(2018, 10, 28);

        //    var result = target.TraerAgenteDeCompra(fecha, fecha1, new List<int>() { 1, 2, 3, 4, 5 });

        //    repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<AgenteCompra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Never);
        //    Assert.NotNull(result);
        //}
        [Test]
        public void DetallePosicionTestOkMesAnio()
        {
            var fecha = new DateTime(2018, 1, 16);
            var fechadesde = new DateTime(2018, 1, 16);
            var fechahasta = new DateTime(2018, 3, 16);
            var mat = new Material { MaterialId = 1, CampañaId = 1, Descripcion = "a" };
            var ope = new Operador { Descripcion = "a", Id = 1 };
            var tac = new TipoAgenteCompra { Id = 1, Descripcion = "a" };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                            .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Fason, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                            .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, DetalleContratoDto>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                            .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });

            var result = target.DetallePosicion(1, 1, 2018, fecha, fecha, 3, 1);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Contrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Fason, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, DetalleContratoDto>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Data.Count);
        }
        [Test]
        public void DetallePosicionTestOkDesdeSuperiorMesAnio()
        {
            var fecha = new DateTime(2018, 1, 16);
            var fechadesde = new DateTime(2018, 1, 26);
            var fechahasta = new DateTime(2018, 3, 16);
            var mat = new Material { MaterialId = 1, CampañaId = 1, Descripcion = "a" };
            var ope = new Operador { Descripcion = "a", Id = 1 };
            var tac = new TipoAgenteCompra { Id = 1, Descripcion = "a" };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fecha.ToShortDateString() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                            .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fecha.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Fason, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                            .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, DetalleContratoDto>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                            .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });

            var result = target.DetallePosicion(1, 1, 2018, fecha, fecha, 3, 1);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Contrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Fason, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, DetalleContratoDto>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Data.Count);
        }
        [Test]
        public void DetallePosicionTestOkHastaInferiorMesDesdeAnio()
        {
            var fecha = new DateTime(2018, 1, 16);
            var fechadesde = new DateTime(2018, 1, 26);
            var fechahasta = new DateTime(2018, 1, 16);
            var fechahasta2 = new DateTime(2018, 3, 16);
            var mat = new Material { MaterialId = 1, CampañaId = 1, Descripcion = "a" };
            var ope = new Operador { Descripcion = "a", Id = 1 };
            var tac = new TipoAgenteCompra { Id = 1, Descripcion = "a" };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fecha.ToShortDateString() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                            .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fecha.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Fason, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                            .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fecha.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, DetalleContratoDto>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                            .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta2.ToShortDateString() } });

            var result = target.DetallePosicion(1, 1, 2018, fecha, fecha, 3, 1);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Contrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Fason, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, DetalleContratoDto>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Data.Count);
        }
        [Test]
        public void DetalleAgenteTestOk()
        {
            var fecha = new DateTime(2018, 1, 16);
            var mat = new Material { MaterialId = 1, CampañaId = 1, Descripcion = "a" };
            var ope = new Operador { Descripcion = "a", Id = 1 };
            var tac = new TipoAgenteCompra { Id = 1, Descripcion = "a" };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AgenteCompra, DetalleAgenteDto>>>(), It.IsAny<Expression<Func<AgenteCompra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<DetalleAgenteDto>() {
                    new DetalleAgenteDto { Agente="1",Cantidad=1,Comercial="a",Fecha=fecha.ToShortDateString(),Material="a",Moneda="a",Operador="a",Posicion="01.2019",Precio="1"},
                    new DetalleAgenteDto { Agente="1",Cantidad=1,Comercial="a",Fecha=fecha.ToShortDateString(),Material="a",Moneda="a",Operador="a",Posicion="01.2019",Precio="1"}});

            var result = target.DetalleAgente(fecha, new List<int>() { 1, 2, 3, 4, 5 });

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<AgenteCompra, DetalleAgenteDto>>>(), It.IsAny<Expression<Func<AgenteCompra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Data.Count);
        }
        [Test]
        public void PosicionPorMaterialTestOk()
        {
            var fecha = new DateTime(2018, 1, 16);
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<TraerPosicionMaterialMes>()))
                .Returns(new List<ExcelPosicionMaterialDto>() { });

            var result = target.PosicionPorMaterial(fecha, fecha);

            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<TraerPosicionMaterialMes>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(0, result.Count);
        }
        [Test]
        public void DetallePosicionModalTestOk()
        {
            var fecha = new DateTime(2018, 1, 16);
            var fechadesde = new DateTime(2018, 1, 16);
            var fechahasta = new DateTime(2018, 3, 16);
            var mat = new Material { MaterialId = 1, CampañaId = 1, Descripcion = "a" };
            var ope = new Operador { Descripcion = "a", Id = 1 };
            var tac = new TipoAgenteCompra { Id = 1, Descripcion = "a" };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Cantidad = "1", Precio = "1", PrecioNeto = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                            .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Precio = "1", PrecioNeto = "1", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Fason, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                            .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Precio = "1", PrecioNeto = "1", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, DetalleContratoDto>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                            .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Precio = "1", PrecioNeto = "1", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });

            var result = target.DetallePosicionModal(1, 1, 2018, fecha, fecha, 3, 1);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Contrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Fason, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, DetalleContratoDto>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.NotNull(result);
            //Assert.AreEqual(
            //    "{\"items\":[{\"Contrato\":\"a\",\"RazonSocial\":\"a\",\"Cuit\":\"1\",\"Material\":null,\"TipoNegocio\":\"1\",\"Comercial\":\"a\",\"Cantidad\":\"1\",\"CantidadCamiones\":null,\"Campana\":null,\"FechaDesde\":\"16/1/2018\",\"FechaHasta\":\"16/3/2018\",\"Precio\":\"1,00\",\"PrecioNeto\":\"1,00\",\"Moneda\":null,\"Fecha\":\"16/1/2018\",\"Provincia\":null,\"Localidad\":null,\"Boleto\":null,\"Bolsa\":null,\"Destino\":null,\"CondicionFijacion\":null,\"DesdeFijacion\":null,\"HastaFijacion\":null,\"Base\":null,\"ImporteSustentable\":null,\"FechaDolarizado\":null,\"DiasPesificado\":null,\"NoInformaSio\":null,\"Ampliaciones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"Pago\":null,\"CalidadEspecial\":null,\"EstablecimientoPropio\":null,\"Observacion\":null},{\"Contrato\":\"a\",\"RazonSocial\":\"a\",\"Cuit\":\"1\",\"Material\":null,\"TipoNegocio\":\"1\",\"Comercial\":\"a\",\"Cantidad\":\"1\",\"CantidadCamiones\":null,\"Campana\":null,\"FechaDesde\":\"16/1/2018\",\"FechaHasta\":\"16/3/2018\",\"Precio\":\"1,00\",\"PrecioNeto\":\"1,00\",\"Moneda\":null,\"Fecha\":\"16/1/2018\",\"Provincia\":null,\"Localidad\":null,\"Boleto\":null,\"Bolsa\":null,\"Destino\":null,\"CondicionFijacion\":null,\"DesdeFijacion\":null,\"HastaFijacion\":null,\"Base\":null,\"ImporteSustentable\":null,\"FechaDolarizado\":null,\"DiasPesificado\":null,\"NoInformaSio\":null,\"Ampliaciones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"Pago\":null,\"CalidadEspecial\":null,\"EstablecimientoPropio\":null,\"Observacion\":null},{\"Contrato\":\"a\",\"RazonSocial\":\"a\",\"Cuit\":\"1\",\"Material\":null,\"TipoNegocio\":\"1\",\"Comercial\":\"a\",\"Cantidad\":\"1\",\"CantidadCamiones\":null,\"Campana\":null,\"FechaDesde\":\"16/1/2018\",\"FechaHasta\":\"16/3/2018\",\"Precio\":\"1,00\",\"PrecioNeto\":\"1,00\",\"Moneda\":null,\"Fecha\":\"16/1/2018\",\"Provincia\":null,\"Localidad\":null,\"Boleto\":null,\"Bolsa\":null,\"Destino\":null,\"CondicionFijacion\":null,\"DesdeFijacion\":null,\"HastaFijacion\":null,\"Base\":null,\"ImporteSustentable\":null,\"FechaDolarizado\":null,\"DiasPesificado\":null,\"NoInformaSio\":null,\"Ampliaciones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"Pago\":null,\"CalidadEspecial\":null,\"EstablecimientoPropio\":null,\"Observacion\":null},{\"Contrato\":\"a\",\"RazonSocial\":\"a\",\"Cuit\":\"1\",\"Material\":null,\"TipoNegocio\":\"1\",\"Comercial\":\"a\",\"Cantidad\":\"1\",\"CantidadCamiones\":null,\"Campana\":null,\"FechaDesde\":\"16/1/2018\",\"FechaHasta\":\"16/3/2018\",\"Precio\":\"1,00\",\"PrecioNeto\":\"1,00\",\"Moneda\":null,\"Fecha\":\"16/1/2018\",\"Provincia\":null,\"Localidad\":null,\"Boleto\":null,\"Bolsa\":null,\"Destino\":null,\"CondicionFijacion\":null,\"DesdeFijacion\":null,\"HastaFijacion\":null,\"Base\":null,\"ImporteSustentable\":null,\"FechaDolarizado\":null,\"DiasPesificado\":null,\"NoInformaSio\":null,\"Ampliaciones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"Pago\":null,\"CalidadEspecial\":null,\"EstablecimientoPropio\":null,\"Observacion\":null}],\"total\":4}",
            //    result);
        }
        [Test]
        public void DetalleAgenteModalTestOk()
        {
            var fecha = new DateTime(2018, 10, 16);
            var fechadesde = new DateTime(2018, 10, 16);
            var fechahasta = new DateTime(2018, 12, 16);
            var mat = new Material { MaterialId = 1, CampañaId = 1, Descripcion = "a" };
            var ope = new Operador { Descripcion = "a", Id = 1 };
            var tac = new TipoAgenteCompra { Id = 1, Descripcion = "a" };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AgenteCompra, DetalleAgenteDto>>>(), It.IsAny<Expression<Func<AgenteCompra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<DetalleAgenteDto>() { new DetalleAgenteDto { Agente = "a", Operador = "a", Moneda = "A", Comercial = "a", Cantidad = 1, Precio = "1", Material = "A", Posicion = "01.2019", Fecha = fecha.ToShortDateString() } });

            var result = target.DetalleAgenteModal(fecha, new List<int>() { 1, 2, 3, 4, 5 });

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<AgenteCompra, DetalleAgenteDto>>>(), It.IsAny<Expression<Func<AgenteCompra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(
                "{\"items\":[{\"Agente\":\"a\",\"Operador\":\"a\",\"Material\":\"A\",\"Posicion\":\"01.2019\",\"Cantidad\":1.0,\"Precio\":\"1,00\",\"Moneda\":\"A\",\"Fecha\":\"16/10/2018\",\"Comercial\":\"a\"}],\"total\":1}",
                result);
        }
        [Test]
        public void TraerPricingCampaniaTest()
        {
            var fechaDesde = new DateTime(2018, 10, 16);
            var fechaHasta = new DateTime(2018, 12, 16);
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Contrato, PricingCampaniaDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<PricingCampaniaDto>() { new PricingCampaniaDto { Id = 1, CampaniaId = 1, MaterialId = 1, Campania = "a", Pricing = 1000, Material = "a" } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, PricingCampaniaDto>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<PricingCampaniaDto>() { new PricingCampaniaDto { Id = 1, CampaniaId = 1, MaterialId = 1, Campania = "a", Pricing = 1000, Material = "a" } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Fason, PricingCampaniaDto>>>(), It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<PricingCampaniaDto>() { new PricingCampaniaDto { Id = 1, CampaniaId = 1, MaterialId = 1, Campania = "a", Pricing = 1000, Material = "a" } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<AgenteCompra, PricingCampaniaDto>>>(), It.IsAny<Expression<Func<AgenteCompra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<PricingCampaniaDto>() { new PricingCampaniaDto { Id = 1, CampaniaId = 1, MaterialId = 1, Campania = "01.2019", Pricing = 1000, Material = "a" } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, PricingCampaniaDto>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<PricingCampaniaDto>() { new PricingCampaniaDto { Id = 1, CampaniaId = 1, MaterialId = 1, Campania = "01.2019", Pricing = 1000, Material = "a" } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Material, MaterialDto>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<MaterialDto>() { new MaterialDto { MaterialId = 1, CampañaId = 1, Campana = "a", Descripcion = "a" } });

            var result = target.TraerPricingCampania(fechaDesde, fechaHasta, new List<int> { 1, 2, 3, 4, 5 }, 1);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Contrato, PricingCampaniaDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, PricingCampaniaDto>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Fason, PricingCampaniaDto>>>(), It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<AgenteCompra, PricingCampaniaDto>>>(), It.IsAny<Expression<Func<AgenteCompra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, PricingCampaniaDto>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Material, MaterialDto>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void DetallePosicionModalIdsTestOk()
        {
            var fecha = new DateTime(2018, 1, 16);
            var fechadesde = new DateTime(2018, 1, 16);
            var fechahasta = new DateTime(2018, 3, 16);
            var mat = new Material { MaterialId = 1, CampañaId = 1, Descripcion = "a" };
            var ope = new Operador { Descripcion = "a", Id = 1 };
            var tac = new TipoAgenteCompra { Id = 1, Descripcion = "a" };
            var ids = new List<int>() { 1, 1 };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Cantidad = "1", Precio = "1", PrecioNeto = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                            .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Precio = "1", PrecioNeto = "1", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Fason, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                            .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Precio = "1", PrecioNeto = "1", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, DetalleContratoDto>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                            .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Precio = "1", PrecioNeto = "1", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });

            var result = target.DetallePosicionModalIds(ids, "",null);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Contrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Fason, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, DetalleContratoDto>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.NotNull(result);
            //Assert.AreEqual(
            //    "{\"items\":[{\"Contrato\":\"a\",\"RazonSocial\":\"a\",\"Cuit\":\"1\",\"Material\":null,\"TipoNegocio\":\"1\",\"Comercial\":\"a\",\"Cantidad\":\"1\",\"CantidadCamiones\":null,\"Campana\":null,\"FechaDesde\":\"16/1/2018\",\"FechaHasta\":\"16/3/2018\",\"Precio\":\"1,00\",\"PrecioNeto\":\"1,00\",\"Moneda\":null,\"Fecha\":\"16/1/2018\",\"Provincia\":null,\"Localidad\":null,\"Boleto\":null,\"Bolsa\":null,\"Destino\":null,\"CondicionFijacion\":null,\"DesdeFijacion\":null,\"HastaFijacion\":null,\"Base\":null,\"ImporteSustentable\":null,\"FechaDolarizado\":null,\"DiasPesificado\":null,\"NoInformaSio\":null,\"Ampliaciones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"Pago\":null,\"CalidadEspecial\":null,\"EstablecimientoPropio\":null,\"Observacion\":null},{\"Contrato\":\"a\",\"RazonSocial\":\"a\",\"Cuit\":\"1\",\"Material\":null,\"TipoNegocio\":\"1\",\"Comercial\":\"a\",\"Cantidad\":\"1\",\"CantidadCamiones\":null,\"Campana\":null,\"FechaDesde\":\"16/1/2018\",\"FechaHasta\":\"16/3/2018\",\"Precio\":\"1,00\",\"PrecioNeto\":\"1,00\",\"Moneda\":null,\"Fecha\":\"16/1/2018\",\"Provincia\":null,\"Localidad\":null,\"Boleto\":null,\"Bolsa\":null,\"Destino\":null,\"CondicionFijacion\":null,\"DesdeFijacion\":null,\"HastaFijacion\":null,\"Base\":null,\"ImporteSustentable\":null,\"FechaDolarizado\":null,\"DiasPesificado\":null,\"NoInformaSio\":null,\"Ampliaciones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"Pago\":null,\"CalidadEspecial\":null,\"EstablecimientoPropio\":null,\"Observacion\":null},{\"Contrato\":\"a\",\"RazonSocial\":\"a\",\"Cuit\":\"1\",\"Material\":null,\"TipoNegocio\":\"1\",\"Comercial\":\"a\",\"Cantidad\":\"1\",\"CantidadCamiones\":null,\"Campana\":null,\"FechaDesde\":\"16/1/2018\",\"FechaHasta\":\"16/3/2018\",\"Precio\":\"1,00\",\"PrecioNeto\":\"1,00\",\"Moneda\":null,\"Fecha\":\"16/1/2018\",\"Provincia\":null,\"Localidad\":null,\"Boleto\":null,\"Bolsa\":null,\"Destino\":null,\"CondicionFijacion\":null,\"DesdeFijacion\":null,\"HastaFijacion\":null,\"Base\":null,\"ImporteSustentable\":null,\"FechaDolarizado\":null,\"DiasPesificado\":null,\"NoInformaSio\":null,\"Ampliaciones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"Pago\":null,\"CalidadEspecial\":null,\"EstablecimientoPropio\":null,\"Observacion\":null},{\"Contrato\":\"a\",\"RazonSocial\":\"a\",\"Cuit\":\"1\",\"Material\":null,\"TipoNegocio\":\"1\",\"Comercial\":\"a\",\"Cantidad\":\"1\",\"CantidadCamiones\":null,\"Campana\":null,\"FechaDesde\":\"16/1/2018\",\"FechaHasta\":\"16/3/2018\",\"Precio\":\"1,00\",\"PrecioNeto\":\"1,00\",\"Moneda\":null,\"Fecha\":\"16/1/2018\",\"Provincia\":null,\"Localidad\":null,\"Boleto\":null,\"Bolsa\":null,\"Destino\":null,\"CondicionFijacion\":null,\"DesdeFijacion\":null,\"HastaFijacion\":null,\"Base\":null,\"ImporteSustentable\":null,\"FechaDolarizado\":null,\"DiasPesificado\":null,\"NoInformaSio\":null,\"Ampliaciones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"Pago\":null,\"CalidadEspecial\":null,\"EstablecimientoPropio\":null,\"Observacion\":null}],\"total\":4}",
            //    result);
        }

        [Test]
        public void DetallePosicionModalIdsTestVerAPrecioOk()
        {
            var fecha = new DateTime(2018, 1, 16);
            var fechadesde = new DateTime(2018, 1, 16);
            var fechahasta = new DateTime(2018, 3, 16);
            var mat = new Material { MaterialId = 1, CampañaId = 1, Descripcion = "a" };
            var ope = new Operador { Descripcion = "a", Id = 1 };
            var tac = new TipoAgenteCompra { Id = 1, Descripcion = "a" };
            var ids = new List<int>() { 1, 1 };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Cantidad = "1", Precio = "1", PrecioNeto = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                            .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Precio = "1", PrecioNeto = "1", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Fason, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                            .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Precio = "1", PrecioNeto = "1", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, DetalleContratoDto>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                            .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Precio = "1", PrecioNeto = "1", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });

            var result = target.DetallePosicionModalIds(ids, "", 2);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Contrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Fason, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, DetalleContratoDto>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.NotNull(result);
            //Assert.AreEqual(
            //    "{\"items\":[{\"Contrato\":\"a\",\"RazonSocial\":\"a\",\"Cuit\":\"1\",\"Material\":null,\"TipoNegocio\":\"1\",\"Comercial\":\"a\",\"Cantidad\":\"1\",\"CantidadCamiones\":null,\"Campana\":null,\"FechaDesde\":\"16/1/2018\",\"FechaHasta\":\"16/3/2018\",\"Precio\":\"1,00\",\"PrecioNeto\":\"1,00\",\"Moneda\":null,\"Fecha\":\"16/1/2018\",\"Provincia\":null,\"Localidad\":null,\"Boleto\":null,\"Bolsa\":null,\"Destino\":null,\"CondicionFijacion\":null,\"DesdeFijacion\":null,\"HastaFijacion\":null,\"Base\":null,\"ImporteSustentable\":null,\"FechaDolarizado\":null,\"DiasPesificado\":null,\"NoInformaSio\":null,\"Ampliaciones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"Pago\":null,\"CalidadEspecial\":null,\"EstablecimientoPropio\":null,\"Observacion\":null},{\"Contrato\":\"a\",\"RazonSocial\":\"a\",\"Cuit\":\"1\",\"Material\":null,\"TipoNegocio\":\"1\",\"Comercial\":\"a\",\"Cantidad\":\"1\",\"CantidadCamiones\":null,\"Campana\":null,\"FechaDesde\":\"16/1/2018\",\"FechaHasta\":\"16/3/2018\",\"Precio\":\"1,00\",\"PrecioNeto\":\"1,00\",\"Moneda\":null,\"Fecha\":\"16/1/2018\",\"Provincia\":null,\"Localidad\":null,\"Boleto\":null,\"Bolsa\":null,\"Destino\":null,\"CondicionFijacion\":null,\"DesdeFijacion\":null,\"HastaFijacion\":null,\"Base\":null,\"ImporteSustentable\":null,\"FechaDolarizado\":null,\"DiasPesificado\":null,\"NoInformaSio\":null,\"Ampliaciones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"Pago\":null,\"CalidadEspecial\":null,\"EstablecimientoPropio\":null,\"Observacion\":null},{\"Contrato\":\"a\",\"RazonSocial\":\"a\",\"Cuit\":\"1\",\"Material\":null,\"TipoNegocio\":\"1\",\"Comercial\":\"a\",\"Cantidad\":\"1\",\"CantidadCamiones\":null,\"Campana\":null,\"FechaDesde\":\"16/1/2018\",\"FechaHasta\":\"16/3/2018\",\"Precio\":\"1,00\",\"PrecioNeto\":\"1,00\",\"Moneda\":null,\"Fecha\":\"16/1/2018\",\"Provincia\":null,\"Localidad\":null,\"Boleto\":null,\"Bolsa\":null,\"Destino\":null,\"CondicionFijacion\":null,\"DesdeFijacion\":null,\"HastaFijacion\":null,\"Base\":null,\"ImporteSustentable\":null,\"FechaDolarizado\":null,\"DiasPesificado\":null,\"NoInformaSio\":null,\"Ampliaciones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"Pago\":null,\"CalidadEspecial\":null,\"EstablecimientoPropio\":null,\"Observacion\":null},{\"Contrato\":\"a\",\"RazonSocial\":\"a\",\"Cuit\":\"1\",\"Material\":null,\"TipoNegocio\":\"1\",\"Comercial\":\"a\",\"Cantidad\":\"1\",\"CantidadCamiones\":null,\"Campana\":null,\"FechaDesde\":\"16/1/2018\",\"FechaHasta\":\"16/3/2018\",\"Precio\":\"1,00\",\"PrecioNeto\":\"1,00\",\"Moneda\":null,\"Fecha\":\"16/1/2018\",\"Provincia\":null,\"Localidad\":null,\"Boleto\":null,\"Bolsa\":null,\"Destino\":null,\"CondicionFijacion\":null,\"DesdeFijacion\":null,\"HastaFijacion\":null,\"Base\":null,\"ImporteSustentable\":null,\"FechaDolarizado\":null,\"DiasPesificado\":null,\"NoInformaSio\":null,\"Ampliaciones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"Pago\":null,\"CalidadEspecial\":null,\"EstablecimientoPropio\":null,\"Observacion\":null}],\"total\":4}",
            //    result);
        }

        [Test]
        public void DetallePosicionModalIdsVerFijacionesTestOk()
        {
            var fecha = new DateTime(2018, 1, 16);
            var fechadesde = new DateTime(2018, 1, 16);
            var fechahasta = new DateTime(2018, 3, 16);
            var mat = new Material { MaterialId = 1, CampañaId = 1, Descripcion = "a" };
            var ope = new Operador { Descripcion = "a", Id = 1 };
            var tac = new TipoAgenteCompra { Id = 1, Descripcion = "a" };
            var ids = new List<int>() { 1, 1 };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Cantidad = "1", Precio = "1", PrecioNeto = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                            .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Precio = "1", PrecioNeto = "1", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Fason, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                            .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Precio = "1", PrecioNeto = "1", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, DetalleContratoDto>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                            .Returns(new List<DetalleContratoDto>() { new DetalleContratoDto { Contrato = "a", RazonSocial = "a", Cuit = "1", Comercial = "a", Precio = "1", PrecioNeto = "1", Cantidad = "1", TipoNegocio = "1", Fecha = fecha.ToShortDateString(), FechaDesde = fechadesde.ToShortDateString(), FechaHasta = fechahasta.ToShortDateString() } });

            var result = target.DetallePosicionModalIds(ids, "", 3);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Contrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, DetalleContratoDto>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Fason, DetalleContratoDto>>>(), It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, DetalleContratoDto>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.NotNull(result);
            //Assert.AreEqual(
            //    "{\"items\":[{\"Contrato\":\"a\",\"RazonSocial\":\"a\",\"Cuit\":\"1\",\"Material\":null,\"TipoNegocio\":\"1\",\"Comercial\":\"a\",\"Cantidad\":\"1\",\"CantidadCamiones\":null,\"Campana\":null,\"FechaDesde\":\"16/1/2018\",\"FechaHasta\":\"16/3/2018\",\"Precio\":\"1,00\",\"PrecioNeto\":\"1,00\",\"Moneda\":null,\"Fecha\":\"16/1/2018\",\"Provincia\":null,\"Localidad\":null,\"Boleto\":null,\"Bolsa\":null,\"Destino\":null,\"CondicionFijacion\":null,\"DesdeFijacion\":null,\"HastaFijacion\":null,\"Base\":null,\"ImporteSustentable\":null,\"FechaDolarizado\":null,\"DiasPesificado\":null,\"NoInformaSio\":null,\"Ampliaciones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"Pago\":null,\"CalidadEspecial\":null,\"EstablecimientoPropio\":null,\"Observacion\":null},{\"Contrato\":\"a\",\"RazonSocial\":\"a\",\"Cuit\":\"1\",\"Material\":null,\"TipoNegocio\":\"1\",\"Comercial\":\"a\",\"Cantidad\":\"1\",\"CantidadCamiones\":null,\"Campana\":null,\"FechaDesde\":\"16/1/2018\",\"FechaHasta\":\"16/3/2018\",\"Precio\":\"1,00\",\"PrecioNeto\":\"1,00\",\"Moneda\":null,\"Fecha\":\"16/1/2018\",\"Provincia\":null,\"Localidad\":null,\"Boleto\":null,\"Bolsa\":null,\"Destino\":null,\"CondicionFijacion\":null,\"DesdeFijacion\":null,\"HastaFijacion\":null,\"Base\":null,\"ImporteSustentable\":null,\"FechaDolarizado\":null,\"DiasPesificado\":null,\"NoInformaSio\":null,\"Ampliaciones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"Pago\":null,\"CalidadEspecial\":null,\"EstablecimientoPropio\":null,\"Observacion\":null},{\"Contrato\":\"a\",\"RazonSocial\":\"a\",\"Cuit\":\"1\",\"Material\":null,\"TipoNegocio\":\"1\",\"Comercial\":\"a\",\"Cantidad\":\"1\",\"CantidadCamiones\":null,\"Campana\":null,\"FechaDesde\":\"16/1/2018\",\"FechaHasta\":\"16/3/2018\",\"Precio\":\"1,00\",\"PrecioNeto\":\"1,00\",\"Moneda\":null,\"Fecha\":\"16/1/2018\",\"Provincia\":null,\"Localidad\":null,\"Boleto\":null,\"Bolsa\":null,\"Destino\":null,\"CondicionFijacion\":null,\"DesdeFijacion\":null,\"HastaFijacion\":null,\"Base\":null,\"ImporteSustentable\":null,\"FechaDolarizado\":null,\"DiasPesificado\":null,\"NoInformaSio\":null,\"Ampliaciones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"Pago\":null,\"CalidadEspecial\":null,\"EstablecimientoPropio\":null,\"Observacion\":null},{\"Contrato\":\"a\",\"RazonSocial\":\"a\",\"Cuit\":\"1\",\"Material\":null,\"TipoNegocio\":\"1\",\"Comercial\":\"a\",\"Cantidad\":\"1\",\"CantidadCamiones\":null,\"Campana\":null,\"FechaDesde\":\"16/1/2018\",\"FechaHasta\":\"16/3/2018\",\"Precio\":\"1,00\",\"PrecioNeto\":\"1,00\",\"Moneda\":null,\"Fecha\":\"16/1/2018\",\"Provincia\":null,\"Localidad\":null,\"Boleto\":null,\"Bolsa\":null,\"Destino\":null,\"CondicionFijacion\":null,\"DesdeFijacion\":null,\"HastaFijacion\":null,\"Base\":null,\"ImporteSustentable\":null,\"FechaDolarizado\":null,\"DiasPesificado\":null,\"NoInformaSio\":null,\"Ampliaciones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"Pago\":null,\"CalidadEspecial\":null,\"EstablecimientoPropio\":null,\"Observacion\":null}],\"total\":4}",
            //    result);
        }

        [Test]
        public void TraerUltimoHedgeObjetivoTestOk()
        {
            var fecha = new DateTime(2018, 10, 26);

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<HedgeObjetivo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<HedgeObjetivo>() {
                    new HedgeObjetivo { Id = 1, Cantidad = 5, Fecha = fecha, MaterialId = 1, TipoObjetivoId = 1 },
                    new HedgeObjetivo { Id = 1, Cantidad = 5, Fecha = fecha, MaterialId = 1, TipoObjetivoId = 2 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Contrato>() { new Contrato { Id = 1, Cantidad = 5, Fecha = fecha, MaterialId = 1, CampanaId = 7, EstadoId = 5, TipoNegocioId = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc))
                .Returns(new List<FijacionDePrecioContrato>() { new FijacionDePrecioContrato { Id = 1, Cantidad = 5, Fecha = fecha, MaterialId = 1, CampanaId = 7, EstadoId = 5 } });

            var result = target.TraerUltimoHedgeObjetivo();

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<HedgeObjetivo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(5, result.PricingCumplido);
            Assert.AreEqual(5, result.PricingCumplido);
            Assert.AreEqual(5, result.RemitirObjetivo);
            Assert.AreEqual(5, result.RemitirCumplido);
        }

        [Test]
        public void TraerTodoPrecioMoaPizarraTest()
        {
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<TraerTodoPrecioMoaPizarra>())).Returns(new DataSourceResult());
            var resultado = target.TraerTodoPrecioMoaPizarra(It.IsAny<DataSourceRequest>());
            repositorioMock.Verify(x => x.ObtenerConsultaEscalar(It.IsAny<TraerTodoPrecioMoaPizarra>()), Times.Once);
        }

        [Test]
        public void ObtenerDatosReportePagosDiferidosTest()
        {

            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Negocio, ReportePagosDiferidos>>>(), It.IsAny<Expression<Func<Negocio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ReportePagosDiferidos>() { new ReportePagosDiferidos {
                    ContratoSAP = "1", Tn = 5000, PrecioUSD = 230, Precio = 230 * 94, Plazo = 33, Toma = new DateTime(2021, 5, 29), TNA = 30 ,
                            Estado = "Vigente",AcumuladoMesAnterior=1,AlVencimiento=1,Capital=1,CapitalMasIntereses=1,Corredor ="corr",CorredorCUIT="",DevengadoMes=1,
                            InteresesPorDia =1,InteresesTotales=1,M2MMes=1,TEA=1,TipoCambio=1,Vendedor="",VendedorCUIT="",
                } });

            var resultado = target.ObtenerDatosReportePagosDiferidos(new DateTime(2021, 5, 29), new DateTime(2021, 6, 4));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Negocio, ReportePagosDiferidos>>>(), It.IsAny<Expression<Func<Negocio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
        }

        [Test]
        public void TraerTodoDatoPesificadoTestOk()
        {
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<TraerTodosReportePesificado>())).Returns(new DataSourceResult());

            target.TraerTodoDatoPesificado(It.IsAny<DataSourceRequest>(), It.IsAny<List<int>>());

            repositorioMock.Verify(x => x.ObtenerConsultaEscalar(It.IsAny<TraerTodosReportePesificado>()), Times.Once);

        }

        [Test]
        public void BuscarDatosNegocioPesificacionOk()
        {
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<TraerTodosPesificacion>())).Returns(new DataSourceResult());

            var resultado = target.BuscarDatosNegocioPesificacion(It.IsAny<DataSourceRequest>(), It.IsAny<List<int>>());

            repositorioMock.Verify(x => x.ObtenerConsultaEscalar(It.IsAny<TraerTodosPesificacion>()), Times.Once);

        }

        [Test]
        public void ConfigurarExcedenteOk()
        {
            var reporte = new ReportePesificado()
            {
                //Excep = false
            };

            repositorioMock.Setup(x => x.Obtener<ReportePesificado>(It.IsAny<int>())).Returns(reporte);
            target.ConfigurarExcedente(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<int>());
            repositorioMock.Verify(x => x.Obtener<ReportePesificado>(It.IsAny<int>()), Times.Once);

        }

        [Test]
        public void EnviarMailCorredorOk()
        {
            var reporte = new ReportePesificadoDto()
            {
                CuitCorredor = "20043159381",
                CuitVendedor = "20061870610",
                Contrato = "0002599447",
                Cantidad = 1000,
                Excepcion = false,
                RazonSocialCorredor = "ACA",
                RazonSocialProveedor = "ACA",
                Fijacion = "0259944701"
            };
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<ReportePesificado, ReportePesificadoDto>>>(), It.IsAny<Expression<Func<ReportePesificado, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
              .Returns(new List<ReportePesificadoDto>() { reporte });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
             .Returns(new List<Comercial>() { new Comercial { ComercialId = 1, IdActiveDirectory = "bau@baufest.com" } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<MailProveedor, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
            .Returns(new List<MailProveedor>() { new MailProveedor { Pesificado = "aaa@baufest.com", Proveedor = new Proveedor { RazonSocial  = "ACA", }  } });
            mailManager.Setup(x => x.GetEmailUserActiveDirectory("bmelgarej")).Returns("aaa@baufest.com");

            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Negocio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
             .Returns(new List<Negocio>() { new Negocio { ContratoSAP = "0002599447", Comercial = new Comercial { ComercialId = 1, IdActiveDirectory = "bau@baufest.com" } } });
            
            target.EnviarMail(It.IsAny<List<int>>(), It.IsAny<DateTime>(), It.IsAny<int>(), true, false);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ReportePesificado, ReportePesificadoDto>>>(), It.IsAny<Expression<Func<ReportePesificado, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

        }
        [Test]
        public void EnviarMailProveedorOk()
        {
            var reporte = new ReportePesificadoDto()
            {
                CuitCorredor = "",
                CuitVendedor = "20061870610",
                Contrato = "0002599447",
                Cantidad = 1000,
                Excepcion = false,
                RazonSocialCorredor = "ACA",
                RazonSocialProveedor = "ACA",
                Fijacion = "0259944701"
            };
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<ReportePesificado, ReportePesificadoDto>>>(), It.IsAny<Expression<Func<ReportePesificado, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
              .Returns(new List<ReportePesificadoDto>() { reporte });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
             .Returns(new List<Comercial>() { new Comercial { ComercialId = 1, IdActiveDirectory = "bau@baufest.com" } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<MailProveedor, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
            .Returns(new List<MailProveedor>() { new MailProveedor { Pesificado = "aaa@baufest.com", Proveedor = new Proveedor { RazonSocial = "ACA", } } });
            mailManager.Setup(x => x.GetEmailUserActiveDirectory("bmelgarej")).Returns("aaa@baufest.com");

            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Negocio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
             .Returns(new List<Negocio>() { new Negocio { ContratoSAP = "0002599447", Comercial = new Comercial { ComercialId = 1, IdActiveDirectory = "bau@baufest.com" } } });

            target.EnviarMail(It.IsAny<List<int>>(), It.IsAny<DateTime>(), It.IsAny<int>(), true, false);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ReportePesificado, ReportePesificadoDto>>>(), It.IsAny<Expression<Func<ReportePesificado, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

        }

    }
}
