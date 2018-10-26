using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
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
    public class ReportesControllerTest
    {
        private ReportesController target;
        private Mock<IComercialManager> comercialManagerMock;
        private Mock<IHomeManager> homeManagerMock;
        private Mock<IReportesManager> reportesManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            comercialManagerMock = new Mock<IComercialManager>();
            homeManagerMock = new Mock<IHomeManager>();
            reportesManagerMock = new Mock<IReportesManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            HttpContext.Current.Session["perfil"] = 1;
            target = new ReportesController(reportesManagerMock.Object, homeManagerMock.Object, comercialManagerMock.Object);
        }

        [Test]
        public void IndexOk()
        {
            comercialManagerMock.Setup(x => x.ComercialExiste(GlobalVariables.IdActiveDirectory)).Returns(true);
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void TraerDatosComboReportesTest()
        {
            reportesManagerMock.Setup(x => x.TraerDatosIniciales(GlobalVariables.IdActiveDirectory)).Returns(new DatosInicialesReportes
            {
                mat = new List<MaterialesQry>(){new MaterialesQry()},
                camp = new List<CampañaQry>(){new CampañaQry()},
                come = new List<ComercialQry>(){new ComercialQry()},
                estic = new List<EstadoICQry>(){new EstadoICQry()},
                provs = new List<ProvinciaQry>() { new ProvinciaQry()},
                segm = new List<SegmentacionQry>() { new SegmentacionQry()}
            });
            var result = target.TraerDatosCombo();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"mat\":[{\"MaterialId\":0,\"Descripcion\":null}],\"camp\":[{\"CampañaId\":0,\"Descripcion\":null}],\"segm\":[{\"SegmentacionId\":0,\"Descripcion\":null,\"Grupo\":null}],\"come\":[{\"ComercialId\":0,\"IdActiveDirectory\":null,\"Nombre\":null,\"Apellido\":null,\"Comercial\":null,\"EmpleadorACargo\":null}],\"provs\":[{\"Provinciaid\":0,\"Nombre\":null,\"Orden\":0}],\"estic\":[{\"EstadoInformeId\":0,\"Descripcion\":null}]},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void TraerDatosReporteComprasBarraTest()
        {
            var reportes = new ParamReportes() { ComercialActual = 1 };
            homeManagerMock.Setup(x => x.TraerIdComercial(GlobalVariables.IdActiveDirectory)).Returns(1);
            reportesManagerMock.Setup(x => x.TraerComprasBarra(reportes)).Returns(new List<ResultComprasBarrasReportes>(){ new ResultComprasBarrasReportes() { MasCl100 = 1} });
            var result = target.TraerDatosReporteComprasBarra(reportes);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"MasTn5000\":0,\"MasTn1000\":0,\"MasTn10000\":0,\"MasTn20000\":0,\"MasTn40000\":0,\"MasTn100\":0,\"MenosTn10000\":0,\"MasCl5000\":0,\"MasCl1000\":0,\"MasCl10000\":0,\"MasCl100\":1,\"MenosCl10000\":0,\"MasCl20000\":0,\"MasCl40000\":0}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerDatosReporteObjetivoGaugeTest()
        {
            var reportes = new ParamReportes() { ComercialActual = 1 };
            homeManagerMock.Setup(x => x.TraerIdComercial(GlobalVariables.IdActiveDirectory)).Returns(1);
            reportesManagerMock.Setup(x => x.TraerObjetivosGauge(reportes)).Returns(new List<ResultObjetivoGaugeReportes>() { new ResultObjetivoGaugeReportes() { Cuit = "1" } });
            var result = target.TraerDatosReporteObjetivoGauge(reportes);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Cuit\":\"1\",\"razonSocial\":null,\"Objetivos\":0,\"Toneladas\":0,\"Porcentajes\":0,\"Material\":null,\"Campaña\":null,\"Mes\":null,\"Año\":null,\"Comercial\":null,\"Provincia\":null,\"Segmentación\":null}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerDatosReporteComprasComprasMapaTest()
        {
            var reportes = new ParamReportes() { ComercialActual = 1, Indicadores = "compras", Grafico = "mapa" };
            homeManagerMock.Setup(x => x.TraerIdComercial(GlobalVariables.IdActiveDirectory)).Returns(1);
            reportesManagerMock.Setup(x => x.TraerComprasMapa(reportes)).Returns(new List<ResulIndicadores>() { new ResulIndicadores() { Cuit = 1, Grano = 1 } });
            var result = target.TraerDatosReporteCompras(reportes);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Cuit\":1,\"Tonelada\":0,\"Segmentacion\":null,\"Estado\":null,\"Grano\":1,\"Provincia\":null,\"CantidadDeCliente\":0}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerDatosReporteComprasComprasTortaTest()
        {
            var reportes = new ParamReportes() { ComercialActual = 1,Indicadores = "compras", Grafico = "mapa" };
            homeManagerMock.Setup(x => x.TraerIdComercial(GlobalVariables.IdActiveDirectory)).Returns(1);
            reportesManagerMock.Setup(x => x.TraerComprasTorta(reportes)).Returns(new List<ResulIndicadores>() { new ResulIndicadores() { Cuit = 1,Grano=1 } });
            var result = target.TraerDatosReporteCompras(reportes);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":null,\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerDatosReporteComprasCapacidadProductivaMapaTest()
        {
            var reportes = new ParamReportes() { ComercialActual = 1, Indicadores = "capacidadproductiva", Grafico = "mapa" };
            homeManagerMock.Setup(x => x.TraerIdComercial(GlobalVariables.IdActiveDirectory)).Returns(1);
            reportesManagerMock.Setup(x => x.TraerCapacidadProductivaMapa(reportes)).Returns(new List<ResulIndicadores>() { new ResulIndicadores() { Cuit = 1, Grano = 1 } });
            var result = target.TraerDatosReporteCompras(reportes);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Cuit\":1,\"Tonelada\":0,\"Segmentacion\":null,\"Estado\":null,\"Grano\":1,\"Provincia\":null,\"CantidadDeCliente\":0}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerDatosReporteComprasCapacidadProductivaBarraTest()
        {
            var reportes = new ParamReportes() { ComercialActual = 1, Indicadores = "capacidadproductiva", Grafico = "barras" };
            homeManagerMock.Setup(x => x.TraerIdComercial(GlobalVariables.IdActiveDirectory)).Returns(1);
            reportesManagerMock.Setup(x => x.TraerCapacidadProductivaBarra(reportes)).Returns(new List<ResultComprasBarrasReportes>() { new ResultComprasBarrasReportes() { MasCl1000 = 1 } });
            var result = target.TraerDatosReporteCompras(reportes);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"MasTn5000\":0,\"MasTn1000\":0,\"MasTn10000\":0,\"MasTn20000\":0,\"MasTn40000\":0,\"MasTn100\":0,\"MenosTn10000\":0,\"MasCl5000\":0,\"MasCl1000\":1,\"MasCl10000\":0,\"MasCl100\":0,\"MenosCl10000\":0,\"MasCl20000\":0,\"MasCl40000\":0}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerDatosReporteComprasCapacidadAcopioMapaTest()
        {
            var reportes = new ParamReportes() { ComercialActual = 1, Indicadores = "capacidadacopio", Grafico = "mapa" };
            homeManagerMock.Setup(x => x.TraerIdComercial(GlobalVariables.IdActiveDirectory)).Returns(1);
            reportesManagerMock.Setup(x => x.TraerCapacidadDeAcopioMapa(reportes)).Returns(new List<ResulIndicadores>() { new ResulIndicadores() { Cuit = 1, Grano = 1 } });
            var result = target.TraerDatosReporteCompras(reportes);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Cuit\":1,\"Tonelada\":0,\"Segmentacion\":null,\"Estado\":null,\"Grano\":1,\"Provincia\":null,\"CantidadDeCliente\":0}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerDatosReporteComprasCapacidadAcopioBarraTest()
        {
            var reportes = new ParamReportes() { ComercialActual = 1, Indicadores = "capacidadacopio", Grafico = "barras" };
            homeManagerMock.Setup(x => x.TraerIdComercial(GlobalVariables.IdActiveDirectory)).Returns(1);
            reportesManagerMock.Setup(x => x.TraerCapacidadDeAcopioBarra(reportes)).Returns(new List<ResultComprasBarrasReportes>() { new ResultComprasBarrasReportes() { MasCl1000 = 1 } });
            var result = target.TraerDatosReporteCompras(reportes);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"MasTn5000\":0,\"MasTn1000\":0,\"MasTn10000\":0,\"MasTn20000\":0,\"MasTn40000\":0,\"MasTn100\":0,\"MenosTn10000\":0,\"MasCl5000\":0,\"MasCl1000\":1,\"MasCl10000\":0,\"MasCl100\":0,\"MenosCl10000\":0,\"MasCl20000\":0,\"MasCl40000\":0}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerDatosReporteComprasBaseDeDatosTortaTest()
        {
            var reportes = new ParamReportes() { ComercialActual = 1, Indicadores = "basededatos", Grafico = "torta" };
            homeManagerMock.Setup(x => x.TraerIdComercial(GlobalVariables.IdActiveDirectory)).Returns(1);
            reportesManagerMock.Setup(x => x.TraerCapacidadDeAcopioBarra(reportes)).Returns(new List<ResultComprasBarrasReportes>() { new ResultComprasBarrasReportes() { MasCl1000 = 1 } });
            var result = target.TraerDatosReporteCompras(reportes);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void TraerDatosGrillaBDTest()
        {
            var reportes = new ParamReportes() { ComercialActual = 1, Indicadores = "basededatos", Grafico = "torta" };
            homeManagerMock.Setup(x => x.TraerIdComercial(GlobalVariables.IdActiveDirectory)).Returns(1);
            reportesManagerMock.Setup(x => x.TraerDatosGrillaBD(reportes)).Returns(new BaseDeDatosReturn() { graficoBaseDatos = new List<graficoBaseDatos>() { new graficoBaseDatos() { segmentacion = "A", Toneladas = 100 } }, valoresGrilla = new List<valoresGrilla>() { new valoresGrilla() { Comercial = "A" } } });
            var result = target.TraerDatosGrillaBD(reportes);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"valoresGrilla\":[{\"Cuit\":null,\"Segmentación\":null,\"Toneladas\":null,\"Estado\":null,\"Grano\":null,\"RazonSocial\":null,\"Comercial\":\"A\",\"FechaAlta\":\"\\/Date(-62135586000000)\\/\"}],\"graficoBaseDatos\":[{\"segmentacion\":\"A\",\"Toneladas\":100}]},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        //[Test]
        //public void ExportarIndicadoresTestComprasMapaTest()
        //{
        //    var reportes = new ParamReportes() { ComercialActual = 1, Indicadores = "compras", Grafico = "mapa" };            
        //    homeManagerMock.Setup(x => x.TraerIdComercial(GlobalVariables.IdActiveDirectory)).Returns(1);
        //    reportesManagerMock.Setup(x => x.TransformarFiltros(reportes)).Returns(reportes);
        //    reportesManagerMock.Setup(x => x.TraerComprasMapaExportacion(reportes)).Returns(new List<ResultIndicadoresReportesmini>() { new ResultIndicadoresReportesmini() { Cuit = "1", Comercial = "A" } });
        //    var result = target.ExportarIndicadores(reportes);

        //    Assert.NotNull(result);

        //    var a = serializer.Serialize(result);
        //    Assert.AreEqual(
        //        "{\"Result\":{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"DownloadKey\":\"WszmTKLE1LCMZDYuGkZMecLKST6pYKlLDhzvShJIufWpJpewxSsGSuGMDa%2bo44GI\",\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null},\"Id\":1,\"Exception\":null,\"Status\":5,\"IsCanceled\":false,\"IsCompleted\":true,\"CreationOptions\":0,\"AsyncState\":null,\"IsFaulted\":false}",
        //        a);
        //}        
    }
}
