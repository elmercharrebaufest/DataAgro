using Autofac.Extras.NLog;
using KendoGridBinder;
using KendoGridBinder.Containers;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
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
    public class ContratoControllerTest
    {
        private ContratoController target;
        private Mock<IContratoManager> contratoManagerMock;
        private Mock<IProveedorManager> proveedorManagerMock;
        private Mock<IComercialManager> comercialManagerMock;
        private Mock<ILocalidadManager> localidadManagerMock;
        private Mock<IProvinciaManager> provinciaManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            contratoManagerMock = new Mock<IContratoManager>();
            comercialManagerMock = new Mock<IComercialManager>();
            localidadManagerMock = new Mock<ILocalidadManager>();
            provinciaManagerMock = new Mock<IProvinciaManager>();
            proveedorManagerMock = new Mock<IProveedorManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new ContratoController(proveedorManagerMock.Object, contratoManagerMock.Object, comercialManagerMock.Object,provinciaManagerMock.Object, localidadManagerMock.Object);
        }

        [Test]
        public void IndexOk()
        {
            HttpContext.Current.Session["perfil"] = 1;
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
        [Test]
        public void BuscaDatosTablaSinSortTest()
        {
            HttpContext.Current.Session["equipo"] = new List<int> { 1, 2 };
            var request = new KendoGridMvcRequest();
            contratoManagerMock.Setup(x => x.TraerTodosContratos(request, GlobalVariables.Equipo)).Returns(new KendoGrid<BasicoContrato>(new List<BasicoContrato>() { new BasicoContrato { ContratoId = 1, ComercialId = 1 } }, 15));
            var result = target.BuscaDatosTabla(request);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            Assert.That(request.SortObjects.Count() == 1);
            contratoManagerMock.Verify(x => x.TraerTodosContratos(It.IsAny<KendoGridMvcRequest>(), It.IsAny<List<int>>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Groups\":null,\"Data\":[{\"Cuit\":null,\"ContratoId\":1,\"MaterialId\":0,\"TipoNegocioId\":0,\"Cantidad\":0,\"Precio\":0,\"PrecioPlazo\":null,\"FechaEntrega\":null,\"CampanaId\":0,\"FechaDesde\":null,\"FechaDesdeFormateado\":null,\"FechaHasta\":null,\"FechaHastaFormateado\":null,\"ProveedorId\":0,\"MonedaId\":null,\"Moneda\":null,\"Fecha\":null,\"FechaFormateado\":null,\"GrupoCompra\":0,\"ComercialId\":1,\"ComercialCreadorId\":null,\"ProvinciaId\":null,\"LocalidadId\":null,\"Base\":null,\"Importe_Sustentable\":null,\"MonedaId_Sustentable\":null,\"Moneda_Sustentable\":null,\"Fecha_Dolarizado\":null,\"Fecha_DolarizadoFormateado\":null,\"Dias_Pesificado\":null,\"NoInformaSIO\":null,\"TrigoEspecial\":null,\"Estado\":null,\"UsuarioId\":null,\"ContratoSAP\":null,\"Ampliaciones\":null,\"TipoNegocio\":null,\"Proveedor\":null,\"Comercial\":null,\"ComercialCreador\":null,\"Material\":null,\"Campania\":null,\"Provincia\":null,\"Localidad\":null,\"Estado_Contrato\":null,\"Cantidad_F\":null,\"Precio_F\":null,\"Proveedor_F\":null,\"Fecha_F\":null,\"Material_F\":null,\"MonedaId_F\":null,\"Moneda_F\":null,\"Ampliaciones_F\":null,\"Fecha_Order\":\"\\/Date(-62135586000000)\\/\",\"Estado_Order\":0,\"Observacion\":null,\"Observacion_F\":null,\"FijacionDePrecioContratoId\":null,\"Sustentable\":null,\"Dolarizado\":null,\"Pesificado\":null,\"Negocio\":null,\"ClasificacionId\":null,\"ClasificacionDescripcion\":null,\"DestinoId\":null,\"DestinoDescripcion\":null,\"CantidadCamiones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"CondicionFijacion\":null,\"CD\":null,\"Warrant\":null,\"PagoDirectoVendedor\":null,\"CalidadDescripcion\":null,\"EstablecimientoPropio\":null,\"BoletoId\":null,\"BolsaId\":null,\"BoletoDescripcion\":null,\"BolsaDescripcion\":null,\"DesdeFijacion\":null,\"DesdeFijacionFormateado\":null,\"HastaFijacion\":null,\"HastaFijacionFormateado\":null,\"CondicionFijacionDescripcion\":null,\"Descuentos\":null,\"Calidades\":null,\"MercsDeposito\":null,\"Corredor\":null,\"CorredorId\":0,\"DatosFijacion\":null,\"PorcentajeComision\":null}],\"Aggregates\":null,\"Total\":15},\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void BuscaDatosTablaConSortTest()
        {
            HttpContext.Current.Session["equipo"] = new List<int> { 1, 2 };
            var request = new KendoGridMvcRequest() { SortObjects = new List<SortObject> { new SortObject("Fecha", "asc"), new SortObject("ContratoId", "asc") } };
            contratoManagerMock.Setup(x => x.TraerTodosContratos(request, GlobalVariables.Equipo)).Returns(new KendoGrid<BasicoContrato>(new List<BasicoContrato>() { new BasicoContrato { ContratoId = 1, ComercialId = 1 } }, 15));
            var result = target.BuscaDatosTabla(request);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            Assert.That(request.SortObjects.Count() > 1);
            contratoManagerMock.Verify(x => x.TraerTodosContratos(It.IsAny<KendoGridMvcRequest>(), It.IsAny<List<int>>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Groups\":null,\"Data\":[{\"Cuit\":null,\"ContratoId\":1,\"MaterialId\":0,\"TipoNegocioId\":0,\"Cantidad\":0,\"Precio\":0,\"PrecioPlazo\":null,\"FechaEntrega\":null,\"CampanaId\":0,\"FechaDesde\":null,\"FechaDesdeFormateado\":null,\"FechaHasta\":null,\"FechaHastaFormateado\":null,\"ProveedorId\":0,\"MonedaId\":null,\"Moneda\":null,\"Fecha\":null,\"FechaFormateado\":null,\"GrupoCompra\":0,\"ComercialId\":1,\"ComercialCreadorId\":null,\"ProvinciaId\":null,\"LocalidadId\":null,\"Base\":null,\"Importe_Sustentable\":null,\"MonedaId_Sustentable\":null,\"Moneda_Sustentable\":null,\"Fecha_Dolarizado\":null,\"Fecha_DolarizadoFormateado\":null,\"Dias_Pesificado\":null,\"NoInformaSIO\":null,\"TrigoEspecial\":null,\"Estado\":null,\"UsuarioId\":null,\"ContratoSAP\":null,\"Ampliaciones\":null,\"TipoNegocio\":null,\"Proveedor\":null,\"Comercial\":null,\"ComercialCreador\":null,\"Material\":null,\"Campania\":null,\"Provincia\":null,\"Localidad\":null,\"Estado_Contrato\":null,\"Cantidad_F\":null,\"Precio_F\":null,\"Proveedor_F\":null,\"Fecha_F\":null,\"Material_F\":null,\"MonedaId_F\":null,\"Moneda_F\":null,\"Ampliaciones_F\":null,\"Fecha_Order\":\"\\/Date(-62135586000000)\\/\",\"Estado_Order\":0,\"Observacion\":null,\"Observacion_F\":null,\"FijacionDePrecioContratoId\":null,\"Sustentable\":null,\"Dolarizado\":null,\"Pesificado\":null,\"Negocio\":null,\"ClasificacionId\":null,\"ClasificacionDescripcion\":null,\"DestinoId\":null,\"DestinoDescripcion\":null,\"CantidadCamiones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"CondicionFijacion\":null,\"CD\":null,\"Warrant\":null,\"PagoDirectoVendedor\":null,\"CalidadDescripcion\":null,\"EstablecimientoPropio\":null,\"BoletoId\":null,\"BolsaId\":null,\"BoletoDescripcion\":null,\"BolsaDescripcion\":null,\"DesdeFijacion\":null,\"DesdeFijacionFormateado\":null,\"HastaFijacion\":null,\"HastaFijacionFormateado\":null,\"CondicionFijacionDescripcion\":null,\"Descuentos\":null,\"Calidades\":null,\"MercsDeposito\":null,\"Corredor\":null,\"CorredorId\":0,\"DatosFijacion\":null,\"PorcentajeComision\":null}],\"Aggregates\":null,\"Total\":15},\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ListarComercialTest()
        {
            HttpContext.Current.Session["equipo"] = new List<int> { 1, 2 };
            comercialManagerMock.Setup(x => x.ListarComercial("A", GlobalVariables.Equipo)).Returns(new List<ComercialDto>() { new ComercialDto { ComercialId = 1, IdActiveDirectory = "ds", Nombres = "A", Apellido = "B" } });
            var result = target.ListarComercial("A");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            comercialManagerMock.Verify(x => x.ListarComercial(It.IsAny<string>(), It.IsAny<List<int>>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"ComercialId\":1,\"Comercial\":\"A B\"}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ListarProvinciaTest()
        {
            provinciaManagerMock.Setup(x => x.ListarProvincia("A")).Returns(new List<ProvinciaDto>() { new ProvinciaDto { Nombre = "A", ProvinciaId = 1 } });
            var result = target.ListarProvincia("A");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            provinciaManagerMock.Verify(x => x.ListarProvincia(It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"ProvinciaId\":1,\"Provincia\":\"A\"}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ListarLocalidadTest()
        {
            localidadManagerMock.Setup(x => x.ListarLocalidad("A")).Returns(new List<LocalidadDto>() { new LocalidadDto { LocalidadId = 1, CodLocalidad = "A", Provincia_Nombre = "B", Nombre = "A", ProvinciaId = 1 } });
            var result = target.ListarLocalidad("A");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            localidadManagerMock.Verify(x => x.ListarLocalidad(It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"LocalidadId\":1,\"Localidad\":\"A\"}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ListarProveedorTest()
        {
            proveedorManagerMock.Setup(x => x.ListarProveedor("A")).Returns(new List<ProveedorDto>() { new ProveedorDto {ProveedorId=1, LocalidadId=1,ProvinciaId=1,CUIT="201",RazonSocial="A"} });
            var result = target.ListarProveedor("A");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            proveedorManagerMock.Verify(x => x.ListarProveedor(It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"ProveedorId\":1,\"Proveedor\":\"A\"}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
    }
}
