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
        private Mock<IMaterialManager> materialManagerMock;
        private Mock<ITipoNegocioManager> tipoNegocioManagerMock;
        private Mock<ICampañaManager> campaniaManagerMock;
        private Mock<ICentroManager> centroManagerMock;

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
            materialManagerMock = new Mock<IMaterialManager>();
            tipoNegocioManagerMock = new Mock<ITipoNegocioManager>();
            campaniaManagerMock = new Mock<ICampañaManager>();
            centroManagerMock = new Mock<ICentroManager>();

            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new ContratoController(proveedorManagerMock.Object, contratoManagerMock.Object, comercialManagerMock.Object,provinciaManagerMock.Object, 
                localidadManagerMock.Object, materialManagerMock.Object, tipoNegocioManagerMock.Object, campaniaManagerMock.Object,
                centroManagerMock.Object);
        }

        [Test]
        public void IndexOk()
        {
            materialManagerMock.Setup(x => x.TraerTodoMaterial())
                .Returns(new ResultIniMaterial { Material = new List<MaterialIni>() { new MaterialIni { CampaniaActual = "a", CampaniaIdActual = 1, Codigo = "a", Descripcion = "A", MaterialId = 1 } } });
            tipoNegocioManagerMock.Setup(x => x.TraerTodoTipoNegocio())
                .Returns(new List<TipoNegocioDto> { new TipoNegocioDto { Descripcion="a",TipoNegocioId=1} });
            campaniaManagerMock.Setup(x => x.TraerTodoCampania())
                .Returns(new List<CampañaDto> { new CampañaDto { Descripcion = "a", CampañaId = 1 } });
            contratoManagerMock.Setup(x => x.TraerTodoGrupoDeCompras())
                .Returns(new List<GrupoDeComprasDto> { new GrupoDeComprasDto { Descripcion = "a", Id = 1 } });
            contratoManagerMock.Setup(x => x.TraerTodoLosEstados())
                .Returns(new List<EstadoContratoDto> { new EstadoContratoDto { Descripcion = "a", EstadoContratoId = 1 } });
            centroManagerMock.Setup(x => x.TraerTodoCentro())
                .Returns(new ResultIniCentro { Centro = new List<CentroIni>() { new CentroIni { CodigoSap="a",Descripcion="a",Id=1} } });
            contratoManagerMock.Setup(x => x.TraerTodosLosBoletos())
                .Returns(new List<BoletoCompraNetDto> { new BoletoCompraNetDto { Descripcion = "a", Id = 1 } });
            comercialManagerMock.Setup(x => x.TraerTodoComercial())
                .Returns(new ResultIniComercial { Comercial = new List<ComercialIni>() { new ComercialIni { Apellido = "a", ComercialId = 1, Nombres = "a", PerDescripcion = "a", Rol = "a" } } });

            var result = target.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
        [Test]
        public void BuscaDatosTablaSinSortTest()
        {
            HttpContext.Current.Session["equipo"] = new List<int> { 1, 2 };
            HttpContext.Current.Session["perfil"] = 7;
            var request = new FiltroReporteNegocioDto();
            contratoManagerMock.Setup(x => x.TraerContratosFiltrados(request, It.IsAny<bool>(), GlobalVariables.EquipoReal, GlobalVariables.CorredoresComercial))
                .Returns(new KendoGridContratoDto { Data = new List<BasicoContrato>() { new BasicoContrato { ContratoId = 1, ComercialId = 1 } } });
            var result = target.BuscaDatosTabla(request);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.TraerContratosFiltrados(It.IsAny<FiltroReporteNegocioDto>(), It.IsAny<bool>(), It.IsAny<List<int>>(), It.IsAny<List<int>>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Data\":[{\"Id\":0,\"Cuit\":null,\"ContratoId\":1,\"MaterialId\":0,\"NivelTarifaId\":null,\"TipoNegocioId\":0,\"Cantidad\":0,\"Precio\":0,\"PrecioPlazo\":null,\"FechaEntrega\":null,\"CampanaId\":0,\"FechaDesde\":null,\"FechaDesdeFormateado\":null,\"FechaHasta\":null,\"FechaHastaFormateado\":null,\"ProveedorId\":0,\"MonedaId\":null,\"Moneda\":null,\"Fecha\":null,\"FechaFormateado\":null,\"Hora\":null,\"NivelTarifa\":null,\"TarifaFlete\":null,\"GrupoCompra\":0,\"GrupoCompraDescripcion\":null,\"ComercialId\":1,\"ComercialCreadorId\":null,\"ProvinciaId\":null,\"LocalidadId\":null,\"Base\":null,\"Importe_Sustentable\":null,\"MonedaId_Sustentable\":null,\"Moneda_Sustentable\":null,\"Fecha_Dolarizado\":null,\"Fecha_DolarizadoFormateado\":null,\"Dias_Pesificado\":null,\"NoInformaSIO\":null,\"TrigoEspecial\":null,\"Estado\":null,\"UsuarioId\":null,\"ContratoSAP\":null,\"Ampliaciones\":null,\"TipoNegocio\":null,\"Proveedor\":null,\"Corredor\":null,\"Comercial\":null,\"ComercialCreador\":null,\"Material\":null,\"Campania\":null,\"Provincia\":null,\"Localidad\":null,\"Estado_Contrato\":null,\"Cantidad_F\":null,\"Precio_F\":null,\"Proveedor_F\":null,\"Fecha_F\":null,\"Material_F\":null,\"MonedaId_F\":null,\"Moneda_F\":null,\"Ampliaciones_F\":null,\"Fecha_Order\":\"\\/Date(-62135586000000)\\/\",\"Estado_Order\":0,\"Observacion\":null,\"Observacion_F\":null,\"FijacionDePrecioContratoId\":null,\"Sustentable\":null,\"Dolarizado\":null,\"Pesificado\":null,\"Negocio\":null,\"ClasificacionId\":null,\"ClasificacionDescripcion\":null,\"DestinoId\":null,\"DestinoDescripcion\":null,\"CantidadCamiones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"CondicionFijacion\":null,\"CD\":null,\"Warrant\":null,\"PagoDirectoVendedor\":null,\"CalidadDescripcion\":null,\"EstablecimientoPropio\":null,\"BoletoId\":null,\"BolsaId\":null,\"BoletoDescripcion\":null,\"BolsaDescripcion\":null,\"DesdeFijacion\":null,\"DesdeFijacionFormateado\":null,\"HastaFijacion\":null,\"HastaFijacionFormateado\":null,\"CondicionFijacionDescripcion\":null,\"Descuentos\":null,\"Calidades\":null,\"MercsDeposito\":null,\"CorredorId\":0,\"DatosFijacion\":null,\"PorcentajeComision\":null,\"ContratoCorredor\":null,\"ContratoVendedor\":null,\"SelCargoMOA\":null,\"SelCargoVendedor\":null,\"Madre\":null,\"ContratoMadre\":null,\"Posicion\":null,\"TipoFason\":null,\"TipoFasonId\":0,\"FasonId\":0,\"Operador\":null,\"OperadorId\":0,\"AgenteId\":0,\"AperturaPrecios\":null,\"PrecioNeto\":null,\"Pizarra\":null,\"StandardCalidadId\":null,\"StandardDeCalidadDescripcion\":null,\"PagoDiferido\":null,\"ZonaId\":null,\"ZonaDescripcion\":null,\"AcuerdoId\":null,\"ImporteFinanciero\":null,\"ImporteRedespacho\":null,\"ImporteComision\":null,\"ImporteBonificacion\":null,\"PorcentajeBonificacion\":null,\"Compensacion\":null,\"Acuerdo\":null,\"Rechazo\":null,\"ComercialZonaId\":null,\"ComercialZonaDescripcion\":null}],\"Total\":0},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
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
            proveedorManagerMock.Setup(x => x.ListarProveedor("A")).Returns(new List<ProveedorDto>() { new ProveedorDto { ProveedorId = 1, LocalidadId = 1, ProvinciaId = 1, CUIT = "201", RazonSocial = "A" } });
            var result = target.ListarProveedor("A");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            proveedorManagerMock.Verify(x => x.ListarProveedor(It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"ProveedorId\":1,\"Proveedor\":\"A\"}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ListarCorredorTest()
        {
            proveedorManagerMock.Setup(x => x.ListarCorredor("A"))
                .Returns(new List<ProveedorDto>() { new ProveedorDto()});
            var result = target.ListarCorredor("A");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            proveedorManagerMock.Verify(x => x.ListarCorredor(It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"CorredorId\":0,\"Corredor\":null}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
    }
}
