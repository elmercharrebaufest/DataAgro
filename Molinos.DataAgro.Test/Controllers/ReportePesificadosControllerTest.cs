using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using WebDataAgro.Models;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    class ReportePesificadosControllerTest
    {
        private ReportePesificadosController target;
        private Mock<IProveedorManager> proveedorManagerMock;
        private Mock<IReportesManager> reporteManagerMock;
        private Mock<ICentroManager> centroManagerMock;
        private Mock<IMaterialManager> materialManagerMock;
        private Mock<IComercialManager> comercialManagerMock;
        private Mock<IContratoManager> contratoManagerMock;
        private Mock<ICampañaManager> campaniaManagerMock;
        private Mock<ILogger> loggerManagerMock;

        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            centroManagerMock = new Mock<ICentroManager>();
            materialManagerMock = new Mock<IMaterialManager>();
            comercialManagerMock = new Mock<IComercialManager>();
            proveedorManagerMock = new Mock<IProveedorManager>();
            reporteManagerMock = new Mock<IReportesManager>();
            contratoManagerMock = new Mock<IContratoManager>();
            loggerManagerMock = new Mock<ILogger>();
            campaniaManagerMock = new Mock<ICampañaManager>();

            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new ReportePesificadosController(proveedorManagerMock.Object, reporteManagerMock.Object, centroManagerMock.Object, 
                materialManagerMock.Object, comercialManagerMock.Object, contratoManagerMock.Object, campaniaManagerMock.Object, loggerManagerMock.Object);

            HttpContext.Current.Session["perfil"] = 1;
            HttpContext.Current.Session["EsCupera"] = true;
            HttpContext.Current.Session["comercialId"] = 1;
            materialManagerMock.Setup(x => x.TraerTodoMaterial()).Returns(new ResultIniMaterial { Material = new List<MaterialIni>() });
            centroManagerMock.Setup(x => x.TraerTodoCentro()).Returns(new ResultIniCentro { Centro = new List<CentroIni>() });
            comercialManagerMock.Setup(x => x.TraerTodoComercial()).Returns(new ResultIniComercial { Comercial = new List<ComercialIni>() });

        }

        [Test]
        public void IndexOk()
        {
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.IsEmpty(result.ViewName);
        }
     
      
        [Test]
        public void ListarProveedorTest()
        {
            proveedorManagerMock.Setup(x => x.ListarProveedor(It.IsAny<string>())).Returns(new List<ProveedorDto>()
            { new ProveedorDto { Alias = "PA", RazonSocial = "PARISI", CUIT ="0003434343"}});
            var result = target.ListarProveedor(It.IsAny<string>());

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            proveedorManagerMock.Verify(x => x.ListarProveedor(It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
               "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"CUIT\":\"0003434343\",\"Proveedor\":\"PA - PARISI\"}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
               a);
        }

        [Test]
        public void ListarCorredorOk()
        {
            proveedorManagerMock.Setup(x => x.ListarCorredor(It.IsAny<string>())).Returns(new List<ProveedorDto>()
            { new ProveedorDto { Alias = "PA", RazonSocial = "PARISI", CUIT ="0003434343"}});
            var result = target.ListarCorredor(It.IsAny<string>());

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            proveedorManagerMock.Verify(x => x.ListarCorredor(It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
               "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"CUIT\":\"0003434343\",\"Proveedor\":\"PA - PARISI\"}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
               a);
        }


        [Test]
        public void BuscaDatosTablaOk()
        {
            reporteManagerMock.Setup(x => x.TraerTodoDatoPesificado(It.IsAny<DataSourceRequest>(), It.IsAny<List<int>>()))
                .Returns(new DataSourceResult { Total = 15, Data = new List<ReportePesificadoDto>() { new ReportePesificadoDto { ComercialId = 1 } } });
            var result = target.BuscaDatosTabla(new DataSourceRequest());

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            reporteManagerMock.Verify(x => x.TraerTodoDatoPesificado(It.IsAny<DataSourceRequest>(), It.IsAny<List<int>>()), Times.Once);
            Assert.AreEqual(
               "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Data\":[{\"MaterialDesc\":null,\"Contrato\":null,\"CantidadPendiente\":null,\"ComercialDesc\":null,\"Fijacion\":null,\"FechaFijacion\":null,\"FechaHastaDolarizado\":null,\"FechaUltimaAplicacion\":null,\"DolarizadoNoProductor\":null,\"CuitCorredor\":null,\"CuitVendedor\":null,\"DolarizadoExpress\":null,\"MonedaId\":null,\"KgNoPesificable\":null,\"KgVencimientoPesificable\":null,\"Precio\":null,\"NombreCorredor\":null,\"NombreVendedor\":null,\"Unidad\":null,\"Dolarizado\":null,\"Clasificacion\":null,\"KgTotales\":null,\"Id\":null,\"MaterialId\":null,\"ComercialId\":1,\"NingunDolarizado\":false,\"USDPesificable\":null,\"USDNoPesificable\":null,\"USDTotal\":null,\"USDTotalizador\":null,\"Pase\":false,\"Plus\":null,\"Posicion\":null,\"KgTotalesPase\":null,\"Excepcion\":null,\"Cantidad\":0,\"CantidadRecibida\":0,\"RazonSocialProveedor\":null,\"RazonSocialCorredor\":null,\"Corredor\":null,\"AgrupracionPesificados\":null,\"FechaInstruccion\":null,\"EsCorredor\":false,\"EsOperacionDirecta\":false,\"Cesion\":false,\"CesionDescripcion\":null,\"Status\":null,\"StatusDescripcion\":null,\"NegocioId\":null,\"NegocioPesificacionId\":null,\"ConFechaInstruccion\":false}],\"Total\":15,\"Aggregates\":null},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
               a);
        }

        

    }
}
