using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.Containers;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Moq;
using NUnit.Framework;
using System;
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
    public class CompraNetControllerTest
    {
        private CompraNetController target;
        private Mock<IHomeManager> homeManagerMock;
        private Mock<INegocioManager> negocioManagerMock;
        private Mock<ICompraNetManager> compranetManagerMock;
        private Mock<IContratoManager> contratoManagerMock;
        private Mock<IFijacionDePrecioContratoManager> fijacionManagerMock;
        private Mock<IComercialManager> comercialManagerMock;
        private Mock<ICampañaManager> campanaManagerMock;
        private Mock<ILocalidadManager> localidadManagerMock;
        private Mock<IMaterialManager> materialManagerMock;
        private Mock<IProveedorManager> proveedorManagerMock;
        private Mock<IOperadorManager> operadorManagerMock;
        private Mock<ILogger> logger;
        private Mock<IFasonManager> fasonManagerMock;
        private Mock<IAgenteCompraManager> agenteManagerMock;
        private Mock<IContratoAcuerdoManager> acuerdoManagerMock;
        private Mock<IConfiguracionInternaManager> configuracionInternaMock;
        private Mock<IConfiguracionManager> configuracionMock;
        private Mock<ITipoDeCambioAgent> tipoDeCambioAgentMock;
        private Mock<ICentroManager> centroManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            homeManagerMock = new Mock<IHomeManager>();
            compranetManagerMock = new Mock<ICompraNetManager>();
            negocioManagerMock = new Mock<INegocioManager>();
            contratoManagerMock = new Mock<IContratoManager>();
            fijacionManagerMock = new Mock<IFijacionDePrecioContratoManager>();
            comercialManagerMock = new Mock<IComercialManager>();
            campanaManagerMock = new Mock<ICampañaManager>();
            localidadManagerMock = new Mock<ILocalidadManager>();
            materialManagerMock = new Mock<IMaterialManager>();
            proveedorManagerMock = new Mock<IProveedorManager>();
            operadorManagerMock = new Mock<IOperadorManager>();
            fasonManagerMock = new Mock<IFasonManager>();
            agenteManagerMock = new Mock<IAgenteCompraManager>();
            acuerdoManagerMock = new Mock<IContratoAcuerdoManager>();
            configuracionInternaMock = new Mock<IConfiguracionInternaManager>();
            configuracionMock = new Mock<IConfiguracionManager>();
            tipoDeCambioAgentMock = new Mock<ITipoDeCambioAgent>();
            centroManagerMock = new Mock<ICentroManager>();
            logger = new Mock<ILogger>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new CompraNetController(homeManagerMock.Object, localidadManagerMock.Object, proveedorManagerMock.Object,
                materialManagerMock.Object, contratoManagerMock.Object, fijacionManagerMock.Object, compranetManagerMock.Object,
                comercialManagerMock.Object, campanaManagerMock.Object, logger.Object, fasonManagerMock.Object, agenteManagerMock.Object,
                acuerdoManagerMock.Object, configuracionInternaMock.Object, configuracionMock.Object, operadorManagerMock.Object,
                negocioManagerMock.Object, tipoDeCambioAgentMock.Object, centroManagerMock.Object);
            HttpContext.Current.Session["comercialId"] = 1;
        }

        [Test]
        public void IndexOk()
        {
            HttpContext.Current.Session["perfil"] = 1;
            HttpContext.Current.Session["equipo"] = new List<int> { 1, 2 };
            HttpContext.Current.Session["comercialId"] = 1;
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
        [Test]
        public void CrearContratoOk()
        {
            HttpContext.Current.Session["perfil"] = 1;
            HttpContext.Current.Session["comercialId"] = 1;
            HttpContext.Current.Session["equipo"] = new List<int> { 1, 2 };
            configuracionMock.Setup(x => x.TraerConfiguraciones()).Returns(new Configuracion());
            contratoManagerMock.Setup(x => x.NegocioABasicoContrato(new Negocio())).Returns(new BasicoContrato());
            contratoManagerMock.Setup(x => x.DevolverNamespaceNegocio(It.IsAny<int>())).Returns(new TipoNegocio());
            var result = target.CrearContrato(1, 1, "", "") as ViewResult;

            Assert.NotNull(result);
        }

        [Test]
        public void InicializarTest()
        {
            HttpContext.Current.Session["equipo"] = new List<int> { 1, 2 };
            compranetManagerMock.Setup(x => x.TraerDatosIniciales(GlobalVariables.Equipo)).Returns(new DatosIniCompraNet());
            var result = target.Inicializar();
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            compranetManagerMock.Verify(x => x.TraerDatosIniciales(It.IsAny<List<int>>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":{\"Proveedor\":null,\"Comercial\":null,\"Material\":null,\"Campaña\":null,\"Provincia\":null,\"Localidad\":null,\"MotivoAnterior\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void InicializarContratoTest()
        {
            HttpContext.Current.Session["perfil"] = 7;
            contratoManagerMock.Setup(x => x.TraerDatosCombo(It.IsAny<int?>())).Returns(new DatosIniContrato());
            var result = target.InicializarContrato(null);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.TraerDatosCombo(It.IsAny<int?>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":{\"localidad\":null,\"moneda\":[],\"tiponegocio\":[],\"material\":[],\"prov\":[],\"loc\":[],\"comercial\":[],\"campaña\":[],\"proveedor\":null,\"monedaSustentable\":[],\"estadoContrato\":[],\"Clasificacion\":[],\"Bolsa\":[],\"Destino\":[],\"Condicion\":[],\"Standard\":[],\"TipoDB\":[],\"TipoPeriodoDB\":[],\"MonedaDescuento\":[],\"TipoFason\":[],\"TipoAgenteCompra\":[],\"Operador\":null,\"Zona\":[],\"NivelTarifa\":[],\"MotivoAnterior\":null,\"TipoPosicionCBOT\":null,\"Camara\":[],\"ComisionAFavor\":[],\"CondicionDePagoFijacionVenta\":[],\"CondicionDePagoPesificadoVenta\":[],\"FleteACargo\":[],\"KgBalanza\":[],\"Pago\":[],\"CondicionPago\":[],\"BoletoVenta\":[]},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void InicializarFijacionTest()
        {
            fijacionManagerMock.Setup(x => x.TraerDatosIniciales()).Returns(new DatosIniAbmFijacionDePrecioContrato());
            var result = target.InicializarFijacion();
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fijacionManagerMock.Verify(x => x.TraerDatosIniciales(), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":{\"material\":[],\"proveedor\":[],\"moneda\":[],\"comercial\":[]},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void GrabarContratoConComercialTest()
        {
            var contrato = new Contrato { ComercialId = 1, MaterialId = 1, CampanaId = 1, ComercialCreadorId = 1, Base = true, NoInformaSio = true, TrigoEspecial = true };

            comercialManagerMock.Setup(x => x.TraerComercial(1)).Returns(new ComercialDto { GrupoDeComprasId = 1 });
            contratoManagerMock.Setup(x => x.GrabarContrato(contrato)).Returns(new GrabarContratoResult { ContratoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.GrabarContrato(contrato);

            Assert.NotNull(result);
            Assert.AreEqual(contrato.GrupoCompra, 1);
            Assert.AreEqual(contrato.Base.Value, true);
            Assert.AreEqual(contrato.NoInformaSio.Value, true);
            Assert.AreEqual(contrato.TrigoEspecial.Value, true);
            var a = serializer.Serialize(result);

            comercialManagerMock.Verify(x => x.TraerComercial(It.IsAny<int>()), Times.Once);
            contratoManagerMock.Verify(x => x.GrabarContrato(It.IsAny<Contrato>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ContratoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void GrabarContratoSinComercialTest()
        {
            var contrato = new Contrato { MaterialId = 1, CampanaId = 1, ComercialCreadorId = 1 };
            contratoManagerMock.Setup(x => x.GrabarContrato(contrato)).Returns(new GrabarContratoResult { ContratoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.GrabarContrato(contrato);
            Assert.NotNull(result);
            Assert.IsNull(contrato.GrupoCompra);
            Assert.AreEqual(contrato.Base.Value, false);
            Assert.AreEqual(contrato.NoInformaSio.Value, false);
            Assert.AreEqual(contrato.TrigoEspecial.Value, false);
            var a = serializer.Serialize(result);

            comercialManagerMock.Verify(x => x.TraerComercial(It.IsAny<int>()), Times.Never);
            contratoManagerMock.Verify(x => x.GrabarContrato(It.IsAny<Contrato>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ContratoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void FinalizarContratoTest()
        {
            contratoManagerMock.Setup(x => x.FinalizarContrato(1, GlobalVariables.IdActiveDirectory)).Returns(new GrabarContratoResult { ContratoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.FinalizarContrato(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.FinalizarContrato(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ContratoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ConfirmarContratoTest()
        {
            contratoManagerMock.Setup(x => x.ConfirmarContrato(1, 1)).Returns(new GrabarContratoResult { ContratoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.ConfirmarContrato(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.ConfirmarContrato(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ContratoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void BorrarContratoTest()
        {
            var contrato = new Contrato { ComercialId = 1, MaterialId = 1, CampanaId = 1, ComercialCreadorId = 1 };
            contratoManagerMock.Setup(x => x.BorrarContrato(contrato)).Returns(new GrabarContratoResult { ContratoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.BorrarContrato(contrato);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.BorrarContrato(It.IsAny<Contrato>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ContratoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void BorrarFijacionTest()
        {
            var fijacion = new FijacionDePrecioContrato { Id = 1, ContratoId = 1, ComercialId = 1, MaterialId = 1, ProveedorId = 1 };
            fijacionManagerMock.Setup(x => x.BorrarFijacion(fijacion)).Returns(new GrabarContratoResult { ContratoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.BorrarFijacion(fijacion);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fijacionManagerMock.Verify(x => x.BorrarFijacion(It.IsAny<FijacionDePrecioContrato>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ContratoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void BorrarFasonTest()
        {
            var fason = new Fason { Id = 1, ComercialId = 1, MaterialId = 1 };
            fasonManagerMock.Setup(x => x.BorrarFason(fason)).Returns(new GrabarFasonResult { FasonId = 1, Errores = new List<ErrorMessage>() });
            var result = target.BorrarFason(fason);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fasonManagerMock.Verify(x => x.BorrarFason(It.IsAny<Fason>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"FasonId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void BorrarAgenteTest()
        {
            var agente = new AgenteCompra { Id = 1, ComercialId = 1, MaterialId = 1 };
            agenteManagerMock.Setup(x => x.BorrarAgente(agente)).Returns(new GrabarAgenteResult { AgenteId = 1, Errores = new List<ErrorMessage>() });
            var result = target.BorrarAgente(agente);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            agenteManagerMock.Verify(x => x.BorrarAgente(It.IsAny<AgenteCompra>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"AgenteId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ConfirmarFijacionTest()
        {
            fijacionManagerMock.Setup(x => x.ConfirmarFijacion(1, 1)).Returns(new GrabarFijacionResult { FijacionDePrecioContratoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.ConfirmarFijacion(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fijacionManagerMock.Verify(x => x.ConfirmarFijacion(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"FijacionDePrecioContratoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void FinalizarFijacionTest()
        {
            fijacionManagerMock.Setup(x => x.FinalizarFijacion(1, GlobalVariables.IdActiveDirectory)).Returns(new GrabarFijacionResult { FijacionDePrecioContratoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.FinalizarFijacion(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fijacionManagerMock.Verify(x => x.FinalizarFijacion(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"FijacionDePrecioContratoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ReenviarMailsTest()
        {
            var result = target.ReenviarMails(new Contrato());
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            Assert.AreEqual(
                 "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ContratoId\":null,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                 a);
        }
        [Test]
        public void GrabarAmpliacionContratoTest()
        {
            var contrato = new Contrato { Id = 1, ComercialId = 1, MaterialId = 1, CampanaId = 1, ComercialCreadorId = 1 };
            contratoManagerMock.Setup(x => x.GrabarAmpliacionContrato(contrato)).Returns(new GrabarContratoResult { ContratoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.GrabarAmpliacionContrato(contrato);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.GrabarAmpliacionContrato(It.IsAny<Contrato>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ContratoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void GrabarFijacionTest()
        {
            var fijacion = new FijacionDePrecioContrato { Id = 1, ContratoId = 1, ComercialId = 1, MaterialId = 1, ProveedorId = 1 };
            fijacionManagerMock.Setup(x => x.GrabarFijacionDePrecio(fijacion)).Returns(new GrabarFijacionResult { FijacionDePrecioContratoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.GrabarFijacion(fijacion);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fijacionManagerMock.Verify(x => x.GrabarFijacionDePrecio(It.IsAny<FijacionDePrecioContrato>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"FijacionDePrecioContratoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void BuscaDatosTablaSinSortTest()
        {
            HttpContext.Current.Session["equipo"] = new List<int> { 1, 2 };
            HttpContext.Current.Session["perfil"] = 7;
            var request = new DataSourceRequest();
            contratoManagerMock.Setup(x => x.TraerTodosContratos(request, It.IsAny<bool>(), GlobalVariables.Equipo, GlobalVariables.CorredoresComercial)).Returns(new DataSourceResult { Total = 15, Data = new List<BasicoContrato>() { new BasicoContrato { ContratoId = 1, ComercialId = 1 } } });
            var result = target.BuscaDatosTabla(request);
            Assert.NotNull(result);
            //var a = serializer.Serialize(result);

            Assert.That(request.Sort.Count() == 2);
            contratoManagerMock.Verify(x => x.TraerTodosContratos(It.IsAny<DataSourceRequest>(), It.IsAny<bool>(), It.IsAny<List<int>>(), It.IsAny<List<int>>()), Times.Once);
            //Assert.AreEqual(
            //"{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Data\":[{\"ChequeElectronicoValor\":null,\"Id\":0,\"Cuit\":null,\"ContratoId\":1,\"MaterialId\":0,\"NivelTarifaId\":null,\"TipoNegocioId\":0,\"Cantidad\":0,\"Precio\":0,\"PrecioPlazo\":null,\"FechaEntrega\":null,\"CampanaId\":null,\"FechaDesde\":null,\"FechaDesdeFormateado\":null,\"FechaHasta\":null,\"FechaHastaFormateado\":null,\"ProveedorId\":0,\"MonedaId\":null,\"Moneda\":null,\"Fecha\":null,\"FechaFormateado\":null,\"Hora\":null,\"NivelTarifa\":null,\"TarifaFlete\":null,\"GrupoCompra\":0,\"GrupoCompraDescripcion\":null,\"ComercialId\":1,\"ComercialCreadorId\":null,\"ProvinciaId\":null,\"LocalidadId\":null,\"Base\":null,\"Importe_Sustentable\":null,\"MonedaId_Sustentable\":null,\"Moneda_Sustentable\":null,\"Fecha_Dolarizado\":null,\"Fecha_DolarizadoFormateado\":null,\"Dias_Pesificado\":null,\"NoInformaSIO\":null,\"TrigoEspecial\":null,\"Estado\":null,\"UsuarioId\":null,\"ContratoSAP\":null,\"Ampliaciones\":null,\"TipoNegocio\":null,\"Proveedor\":null,\"Corredor\":null,\"CUITCorredor\":null,\"Comercial\":null,\"ComercialCreador\":null,\"Material\":null,\"Campania\":null,\"Provincia\":null,\"Localidad\":null,\"Estado_Contrato\":null,\"Fecha_Order\":\"\\/Date(-62135586000000)\\/\",\"Estado_Order\":0,\"Observacion\":null,\"FijacionDePrecioContratoId\":null,\"Sustentable\":null,\"Dolarizado\":null,\"Pesificado\":null,\"Negocio\":null,\"ClasificacionId\":null,\"ClasificacionDescripcion\":null,\"DestinoId\":null,\"DestinoDescripcion\":null,\"CantidadCamiones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"CondicionFijacion\":null,\"CD\":null,\"Warrant\":null,\"PagoDirectoVendedor\":null,\"CalidadDescripcion\":null,\"EstablecimientoPropio\":null,\"BoletoId\":null,\"BolsaId\":null,\"BoletoDescripcion\":null,\"BolsaDescripcion\":null,\"DesdeFijacion\":null,\"DesdeFijacionFormateado\":null,\"HastaFijacion\":null,\"HastaFijacionFormateado\":null,\"CondicionFijacionDescripcion\":null,\"Descuentos\":null,\"Calidades\":null,\"MercsDeposito\":null,\"CorredorId\":0,\"DatosFijacion\":null,\"PorcentajeComision\":null,\"ContratoCorredor\":null,\"ContratoVendedor\":null,\"SelCargoMOA\":null,\"SelCargoVendedor\":null,\"Madre\":null,\"EsFason\":null,\"ContratoMadre\":null,\"Posicion\":null,\"TipoFason\":null,\"TipoFasonId\":0,\"FasonId\":0,\"Operador\":null,\"OperadorId\":0,\"AgenteId\":0,\"AperturaPrecios\":null,\"PreciosPactados\":null,\"PrecioNeto\":null,\"Pizarra\":null,\"StandardCalidadId\":null,\"StandardDeCalidadDescripcion\":null,\"PagoDiferido\":null,\"ZonaId\":null,\"ZonaDescripcion\":null,\"AcuerdoId\":null,\"ImporteFinanciero\":null,\"ImporteRedespacho\":null,\"ImporteComision\":null,\"ImporteBonificacion\":null,\"PorcentajeBonificacion\":null,\"Compensacion\":null,\"Acuerdo\":null,\"Rechazo\":null,\"ComercialZonaId\":null,\"ComercialZonaDescripcion\":null,\"OcultarEnTablero\":false,\"FechaCiertaFormateado\":null,\"FechaCierta\":null,\"MonedaBonificacion\":null,\"MesPosicion\":null,\"CampanaMaterialId\":null,\"ContratoAcuerdoId\":null,\"PorcentajeDePago\":null,\"TipoAgenteCompraId\":null,\"CaratulaExtension\":null,\"CaratulaMAT\":null,\"PrecioAjusteComision\":null,\"MonedaAjusteComisionId\":null,\"FechaOperacion\":null,\"FechaOperacionFormateado\":null,\"MotivoOperacionAnterior\":null,\"UsuarioConfirmador\":null,\"FechaConfirmacion\":null,\"CantidadMaximaCupo\":0,\"ChequeElectronico\":null,\"DolarizadoExpress\":null,\"DolarizadoExpressValor\":null,\"PagoCBU\":null,\"DolarizadoValor\":null}],\"Total\":15,\"Aggregates\":null},\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
            //a);
            var data = (List<BasicoContrato>)((DataSourceResult)((JsonResult)result).Data).Data;
            Assert.AreEqual(data.Count, 1);
        }
        [Test]
        public void BuscaDatosTablaConSortTest()
        {
            HttpContext.Current.Session["equipo"] = new List<int> { 1, 2 };
            HttpContext.Current.Session["perfil"] = 7;
            var request = new DataSourceRequest() { Sort = new List<Sort> { new Sort { Field = "Fecha", Dir = "asc" }, new Sort { Field = "ContratoId", Dir = "asc" } } };
            contratoManagerMock.Setup(x => x.TraerTodosContratos(request, It.IsAny<bool>(), GlobalVariables.Equipo, GlobalVariables.CorredoresComercial))
                .Returns(new DataSourceResult { Total = 15, Data = new List<BasicoContrato>() { new BasicoContrato { ContratoId = 1, ComercialId = 1 } } });
            var result = target.BuscaDatosTabla(request);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            Assert.That(request.Sort.Count() > 2);
            contratoManagerMock.Verify(x => x.TraerTodosContratos(It.IsAny<DataSourceRequest>(), It.IsAny<bool>(), It.IsAny<List<int>>(), It.IsAny<List<int>>()), Times.Once);
            //Assert.AreEqual(
            //"{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Data\":[{\"ChequeElectronicoValor\":null,\"Id\":0,\"Cuit\":null,\"ContratoId\":1,\"MaterialId\":0,\"NivelTarifaId\":null,\"TipoNegocioId\":0,\"Cantidad\":0,\"Precio\":0,\"PrecioPlazo\":null,\"FechaEntrega\":null,\"CampanaId\":null,\"FechaDesde\":null,\"FechaDesdeFormateado\":null,\"FechaHasta\":null,\"FechaHastaFormateado\":null,\"ProveedorId\":0,\"MonedaId\":null,\"Moneda\":null,\"Fecha\":null,\"FechaFormateado\":null,\"Hora\":null,\"NivelTarifa\":null,\"TarifaFlete\":null,\"GrupoCompra\":0,\"GrupoCompraDescripcion\":null,\"ComercialId\":1,\"ComercialCreadorId\":null,\"ProvinciaId\":null,\"LocalidadId\":null,\"Base\":null,\"Importe_Sustentable\":null,\"MonedaId_Sustentable\":null,\"Moneda_Sustentable\":null,\"Fecha_Dolarizado\":null,\"Fecha_DolarizadoFormateado\":null,\"Dias_Pesificado\":null,\"NoInformaSIO\":null,\"TrigoEspecial\":null,\"Estado\":null,\"UsuarioId\":null,\"ContratoSAP\":null,\"Ampliaciones\":null,\"TipoNegocio\":null,\"Proveedor\":null,\"Corredor\":null,\"CUITCorredor\":null,\"Comercial\":null,\"ComercialCreador\":null,\"Material\":null,\"Campania\":null,\"Provincia\":null,\"Localidad\":null,\"Estado_Contrato\":null,\"Fecha_Order\":\"\\/Date(-62135586000000)\\/\",\"Estado_Order\":0,\"Observacion\":null,\"FijacionDePrecioContratoId\":null,\"Sustentable\":null,\"Dolarizado\":null,\"Pesificado\":null,\"Negocio\":null,\"ClasificacionId\":null,\"ClasificacionDescripcion\":null,\"DestinoId\":null,\"DestinoDescripcion\":null,\"CantidadCamiones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"CondicionFijacion\":null,\"CD\":null,\"Warrant\":null,\"PagoDirectoVendedor\":null,\"CalidadDescripcion\":null,\"EstablecimientoPropio\":null,\"BoletoId\":null,\"BolsaId\":null,\"BoletoDescripcion\":null,\"BolsaDescripcion\":null,\"DesdeFijacion\":null,\"DesdeFijacionFormateado\":null,\"HastaFijacion\":null,\"HastaFijacionFormateado\":null,\"CondicionFijacionDescripcion\":null,\"Descuentos\":null,\"Calidades\":null,\"MercsDeposito\":null,\"CorredorId\":0,\"DatosFijacion\":null,\"PorcentajeComision\":null,\"ContratoCorredor\":null,\"ContratoVendedor\":null,\"SelCargoMOA\":null,\"SelCargoVendedor\":null,\"Madre\":null,\"EsFason\":null,\"ContratoMadre\":null,\"Posicion\":null,\"TipoFason\":null,\"TipoFasonId\":0,\"FasonId\":0,\"Operador\":null,\"OperadorId\":0,\"AgenteId\":0,\"AperturaPrecios\":null,\"PreciosPactados\":null,\"PrecioNeto\":null,\"Pizarra\":null,\"StandardCalidadId\":null,\"StandardDeCalidadDescripcion\":null,\"PagoDiferido\":null,\"ZonaId\":null,\"ZonaDescripcion\":null,\"AcuerdoId\":null,\"ImporteFinanciero\":null,\"ImporteRedespacho\":null,\"ImporteComision\":null,\"ImporteBonificacion\":null,\"PorcentajeBonificacion\":null,\"Compensacion\":null,\"Acuerdo\":null,\"Rechazo\":null,\"ComercialZonaId\":null,\"ComercialZonaDescripcion\":null,\"OcultarEnTablero\":false,\"FechaCiertaFormateado\":null,\"FechaCierta\":null,\"MonedaBonificacion\":null,\"MesPosicion\":null,\"CampanaMaterialId\":null,\"ContratoAcuerdoId\":null,\"PorcentajeDePago\":null,\"TipoAgenteCompraId\":null,\"CaratulaExtension\":null,\"CaratulaMAT\":null,\"PrecioAjusteComision\":null,\"MonedaAjusteComisionId\":null,\"FechaOperacion\":null,\"FechaOperacionFormateado\":null,\"MotivoOperacionAnterior\":null,\"UsuarioConfirmador\":null,\"FechaConfirmacion\":null,\"CantidadMaximaCupo\":0,\"ChequeElectronico\":null,\"DolarizadoExpress\":null,\"DolarizadoExpressValor\":null,\"PagoCBU\":null,\"DolarizadoValor\":null}],\"Total\":15,\"Aggregates\":null},\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
            //a);
            var data = (List<BasicoContrato>)((DataSourceResult)((JsonResult)result).Data).Data;
            Assert.AreEqual(data.Count, 1);
        }
        [Test]
        public void TraerCampanaPorMaterialTest()
        {
            campanaManagerMock.Setup(x => x.TraerCampañaPorMaterial(1)).Returns(new List<CampañaDto>() { new CampañaDto { CampañaId = 1, Descripcion = "18-19" } });
            var result = target.TraerCampanaPorMaterial(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            campanaManagerMock.Verify(x => x.TraerCampañaPorMaterial(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"CampañaId\":1,\"Descripcion\":\"18-19\"}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerLocalidadPorProvinciaTest()
        {
            localidadManagerMock.Setup(x => x.TraerLocalidadPorProvincia(1)).Returns(new ResultIniLocalidad { Localidad = new List<LocalidadIni> { new LocalidadIni { Nombre = "A", LocalidadId = 1, CodLocalidad = "A" } } });
            var result = target.TraerLocalidadPorProvincia(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            localidadManagerMock.Verify(x => x.TraerLocalidadPorProvincia(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Localidad\":[{\"LocalidadId\":1,\"CodLocalidad\":\"A\",\"Nombre\":\"A\",\"ProNombre\":null}]},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerCampanaActualMaterialTest()
        {
            materialManagerMock.Setup(x => x.TraerMaterial(1)).Returns(new MaterialDto { CampañaId = 1, Codigo = "A", Descripcion = "A", MaterialId = 1 });
            var result = target.TraerCampanaActualMaterial(1);
            Assert.NotNull(result);

            materialManagerMock.Verify(x => x.TraerMaterial(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(1, result);
        }
        [Test]
        public void TraerCalidadesPorMaterialTest()
        {
            campanaManagerMock.Setup(x => x.TraerCalidadPorMaterial(1)).Returns(new List<CalidadEspecialDto>() { new CalidadEspecialDto { MaterialId = 1, Descripcion = "A", CodigoSap = "A", Id = 1 } });
            var result = target.TraerCalidadesPorMaterial(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            campanaManagerMock.Verify(x => x.TraerCalidadPorMaterial(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Id\":1,\"Descripcion\":\"A\",\"CodigoSap\":\"A\",\"MaterialId\":1}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ObtenerProveedorIdTest()
        {
            proveedorManagerMock.Setup(x => x.TraerProveedorPorCuit("a", false)).Returns(new ProveedorQry { ProveedorId = 1, Descripcion = "A" });
            var result = target.ObtenerProveedorId("a", false);
            Assert.NotNull(result);

            proveedorManagerMock.Verify(x => x.TraerProveedorPorCuit(It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
            Assert.AreEqual(1, result);
        }
        [Test]
        public void ObtenerLocalidadIdTest()
        {
            localidadManagerMock.Setup(x => x.TraerLocalidadProvincia("A", "B")).Returns(new LocalidadQry { CodLocalidad = "A", LocalidadId = 1, Nombre = "B", ProvinciaId = 2 });
            var result = target.ObtenerLocalidadId("A", "B");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            localidadManagerMock.Verify(x => x.TraerLocalidadProvincia(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"LocalidadId\":1,\"CodLocalidad\":\"A\",\"Nombre\":\"B\",\"ProvinciaId\":2},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ObtenerProvinciaLocalidadTest()
        {
            proveedorManagerMock.Setup(x => x.TraerLocalidadProveedorPorCuit("201")).Returns(new DatosLocalidadProvincia { ProvinciaId = 1, LocalidadId = 1, CUIT = "201", RazonSocial = "A" });
            var result = target.ObtenerProvinciaLocalidad("201");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            proveedorManagerMock.Verify(x => x.TraerLocalidadProveedorPorCuit(It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ProveedorId\":0,\"CUIT\":\"201\",\"RazonSocial\":\"A\",\"LocalidadId\":1,\"ProvinciaId\":1,\"Localidad\":\"\",\"Provincia\":\"\",\"ClasificacionId\":0,\"Consignatario\":false,\"BoletoId\":0,\"BolsaId\":0,\"Corredor\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ObtenerProvinciaLocalidadProvTest()
        {
            var datos = new DatosLocalidadProvinciaFiltro { CUIT = "201", CampanaId = 1, MaterialId = 1 };
            proveedorManagerMock.Setup(x => x.TraerLocalidadProveedorPorCuit(datos)).Returns(new DatosLocalidadProvincia { ProvinciaId = 1, LocalidadId = 1, CUIT = "201", RazonSocial = "A" });
            var result = target.ObtenerProvinciaLocalidadProv(datos);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            proveedorManagerMock.Verify(x => x.TraerLocalidadProveedorPorCuit(It.IsAny<DatosLocalidadProvinciaFiltro>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ProveedorId\":0,\"CUIT\":\"201\",\"RazonSocial\":\"A\",\"LocalidadId\":1,\"ProvinciaId\":1,\"Localidad\":\"\",\"Provincia\":\"\",\"ClasificacionId\":0,\"Consignatario\":false,\"BoletoId\":0,\"BolsaId\":0,\"Corredor\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void BuscarCuitPorIdTest()
        {
            proveedorManagerMock.Setup(x => x.TraerProveedor(1)).Returns(new ProveedorDto { CUIT = "201", RazonSocial = "A", ProveedorId = 1 });
            var result = target.BuscarCuitPorId(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            proveedorManagerMock.Verify(x => x.TraerProveedor(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ProveedorId\":1,\"CUIT\":\"201\",\"RazonSocial\":\"A\",\"Localidad\":null,\"Provincia\":null,\"LocalidadId\":null,\"ProvinciaId\":null,\"Direccion\":null,\"CodigoPostal\":null,\"LocalidadCompraNetId\":null,\"ProvinciaCompraNetId\":null,\"LocalidadCompraNet\":null,\"ProvinciaCompraNet\":null,\"ClasificacionCompraNetId\":null,\"ClasificacionDescripcion\":null,\"ComisionPorcentaje\":null,\"Consignatario\":null,\"SegmentacionId\":0,\"Deshabilitado\":null,\"Alias\":null},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ListarProveedorTest()
        {
            proveedorManagerMock.Setup(x => x.ListarProveedor("A")).Returns(new List<ProveedorDto>() { new ProveedorDto { CUIT = "201", RazonSocial = "A", ProveedorId = 1 } });
            operadorManagerMock.Setup(x => x.ListarOperador("A")).Returns(new List<OperadorIni>());
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
            proveedorManagerMock.Setup(x => x.ListarCorredor("A")).Returns(new List<ProveedorDto>() { new ProveedorDto { CUIT = "201", RazonSocial = "A", ProveedorId = 1 } });
            var result = target.ListarCorredor("A");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            proveedorManagerMock.Verify(x => x.ListarCorredor(It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"ProveedorId\":1,\"Proveedor\":\"A\"}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
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
        public void TraerCalidadesPorContratoTest()
        {
            contratoManagerMock.Setup(x => x.TraerCalidadesPorContrato(1, 0)).Returns(new List<CalidadDto>() { new CalidadDto { ContratoId = 1, CalidadEspecialDesc = "A", CalidadEspecialId = 1, Valor = 1, Id = 1 } });
            var result = target.TraerCalidadesPorContrato(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.TraerCalidadesPorContrato(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Id\":1,\"CalidadEspecialId\":1,\"CalidadEspecialDesc\":\"A\",\"Valor\":1,\"ContratoId\":1,\"AcuerdoId\":null,\"PorcentajeDesde\":null,\"PorcentajeHasta\":null}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerContratoCompletoTest()
        {
            contratoManagerMock.Setup(x => x.TraerContrato(1)).Returns(new BasicoContrato { ContratoId = 1, Precio = 1, MaterialId = 1, ProveedorId = 1, BoletoId = 3 });
            var result = target.TraerContratoCompleto(1, "");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.TraerContrato(It.IsAny<int>()), Times.Once);
            //Assert.AreEqual(
            //"{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ChequeElectronicoValor\":null,\"Id\":0,\"Cuit\":null,\"ContratoId\":1,\"MaterialId\":1,\"NivelTarifaId\":null,\"TipoNegocioId\":0,\"Cantidad\":0,\"Precio\":1,\"PrecioPlazo\":null,\"FechaEntrega\":null,\"CampanaId\":null,\"FechaDesde\":null,\"FechaDesdeFormateado\":null,\"FechaHasta\":null,\"FechaHastaFormateado\":null,\"ProveedorId\":1,\"MonedaId\":null,\"Moneda\":null,\"Fecha\":null,\"FechaFormateado\":null,\"Hora\":null,\"NivelTarifa\":null,\"TarifaFlete\":null,\"GrupoCompra\":0,\"GrupoCompraDescripcion\":null,\"ComercialId\":null,\"ComercialCreadorId\":null,\"ProvinciaId\":null,\"LocalidadId\":null,\"Base\":null,\"Importe_Sustentable\":null,\"MonedaId_Sustentable\":null,\"Moneda_Sustentable\":null,\"Fecha_Dolarizado\":null,\"Fecha_DolarizadoFormateado\":null,\"Dias_Pesificado\":null,\"NoInformaSIO\":null,\"TrigoEspecial\":null,\"Estado\":null,\"UsuarioId\":null,\"ContratoSAP\":null,\"Ampliaciones\":null,\"TipoNegocio\":null,\"Proveedor\":null,\"Corredor\":null,\"CUITCorredor\":null,\"Comercial\":null,\"ComercialCreador\":null,\"Material\":null,\"Campania\":null,\"Provincia\":null,\"Localidad\":null,\"Estado_Contrato\":null,\"Fecha_Order\":\"\\/Date(-62135586000000)\\/\",\"Estado_Order\":0,\"Observacion\":null,\"FijacionDePrecioContratoId\":null,\"Sustentable\":null,\"Dolarizado\":null,\"Pesificado\":null,\"Negocio\":null,\"ClasificacionId\":null,\"ClasificacionDescripcion\":null,\"DestinoId\":null,\"DestinoDescripcion\":null,\"CantidadCamiones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"CondicionFijacion\":null,\"CD\":null,\"Warrant\":null,\"PagoDirectoVendedor\":null,\"CalidadDescripcion\":null,\"EstablecimientoPropio\":null,\"BoletoId\":3,\"BolsaId\":null,\"BoletoDescripcion\":null,\"BolsaDescripcion\":null,\"DesdeFijacion\":null,\"DesdeFijacionFormateado\":null,\"HastaFijacion\":null,\"HastaFijacionFormateado\":null,\"CondicionFijacionDescripcion\":null,\"Descuentos\":null,\"Calidades\":null,\"MercsDeposito\":null,\"CorredorId\":0,\"DatosFijacion\":null,\"PorcentajeComision\":null,\"ContratoCorredor\":null,\"ContratoVendedor\":null,\"SelCargoMOA\":null,\"SelCargoVendedor\":null,\"Madre\":null,\"EsFason\":null,\"ContratoMadre\":null,\"Posicion\":null,\"TipoFason\":null,\"TipoFasonId\":0,\"FasonId\":0,\"Operador\":null,\"OperadorId\":0,\"AgenteId\":0,\"AperturaPrecios\":null,\"PreciosPactados\":null,\"PrecioNeto\":null,\"Pizarra\":null,\"StandardCalidadId\":null,\"StandardDeCalidadDescripcion\":null,\"PagoDiferido\":null,\"ZonaId\":null,\"ZonaDescripcion\":null,\"AcuerdoId\":null,\"ImporteFinanciero\":null,\"ImporteRedespacho\":null,\"ImporteComision\":null,\"ImporteBonificacion\":null,\"PorcentajeBonificacion\":null,\"Compensacion\":null,\"Acuerdo\":null,\"Rechazo\":null,\"ComercialZonaId\":null,\"ComercialZonaDescripcion\":null,\"OcultarEnTablero\":false,\"FechaCiertaFormateado\":null,\"FechaCierta\":null,\"MonedaBonificacion\":null,\"MesPosicion\":null,\"CampanaMaterialId\":null,\"ContratoAcuerdoId\":null,\"PorcentajeDePago\":null,\"TipoAgenteCompraId\":null,\"CaratulaExtension\":null,\"CaratulaMAT\":null,\"PrecioAjusteComision\":null,\"MonedaAjusteComisionId\":null,\"FechaOperacion\":null,\"FechaOperacionFormateado\":null,\"MotivoOperacionAnterior\":null,\"UsuarioConfirmador\":null,\"FechaConfirmacion\":null,\"CantidadMaximaCupo\":0,\"ChequeElectronico\":null,\"DolarizadoExpress\":null,\"DolarizadoExpressValor\":null,\"PagoCBU\":null,\"DolarizadoValor\":null},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
            //a);
            var data = (BasicoContrato)((System.Web.Mvc.JsonResult)result).Data;
            Assert.AreEqual(data.ContratoId, 1);
        }
        [Test]
        public void TraerFijacionCompletoTest()
        {
            fijacionManagerMock.Setup(x => x.TraerFijacion(1)).Returns(new BasicoContrato { ContratoId = 1, Precio = 1, MaterialId = 1, ProveedorId = 1, BoletoId = 3 });
            var result = target.TraerFijacionCompleto(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fijacionManagerMock.Verify(x => x.TraerFijacion(It.IsAny<int>()), Times.Once);
            //Assert.AreEqual(
            //"{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ChequeElectronicoValor\":null,\"Id\":0,\"Cuit\":null,\"ContratoId\":1,\"MaterialId\":1,\"NivelTarifaId\":null,\"TipoNegocioId\":0,\"Cantidad\":0,\"Precio\":1,\"PrecioPlazo\":null,\"FechaEntrega\":null,\"CampanaId\":null,\"FechaDesde\":null,\"FechaDesdeFormateado\":null,\"FechaHasta\":null,\"FechaHastaFormateado\":null,\"ProveedorId\":1,\"MonedaId\":null,\"Moneda\":null,\"Fecha\":null,\"FechaFormateado\":null,\"Hora\":null,\"NivelTarifa\":null,\"TarifaFlete\":null,\"GrupoCompra\":0,\"GrupoCompraDescripcion\":null,\"ComercialId\":null,\"ComercialCreadorId\":null,\"ProvinciaId\":null,\"LocalidadId\":null,\"Base\":null,\"Importe_Sustentable\":null,\"MonedaId_Sustentable\":null,\"Moneda_Sustentable\":null,\"Fecha_Dolarizado\":null,\"Fecha_DolarizadoFormateado\":null,\"Dias_Pesificado\":null,\"NoInformaSIO\":null,\"TrigoEspecial\":null,\"Estado\":null,\"UsuarioId\":null,\"ContratoSAP\":null,\"Ampliaciones\":null,\"TipoNegocio\":null,\"Proveedor\":null,\"Corredor\":null,\"CUITCorredor\":null,\"Comercial\":null,\"ComercialCreador\":null,\"Material\":null,\"Campania\":null,\"Provincia\":null,\"Localidad\":null,\"Estado_Contrato\":null,\"Fecha_Order\":\"\\/Date(-62135586000000)\\/\",\"Estado_Order\":0,\"Observacion\":null,\"FijacionDePrecioContratoId\":null,\"Sustentable\":null,\"Dolarizado\":null,\"Pesificado\":null,\"Negocio\":null,\"ClasificacionId\":null,\"ClasificacionDescripcion\":null,\"DestinoId\":null,\"DestinoDescripcion\":null,\"CantidadCamiones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"CondicionFijacion\":null,\"CD\":null,\"Warrant\":null,\"PagoDirectoVendedor\":null,\"CalidadDescripcion\":null,\"EstablecimientoPropio\":null,\"BoletoId\":3,\"BolsaId\":null,\"BoletoDescripcion\":null,\"BolsaDescripcion\":null,\"DesdeFijacion\":null,\"DesdeFijacionFormateado\":null,\"HastaFijacion\":null,\"HastaFijacionFormateado\":null,\"CondicionFijacionDescripcion\":null,\"Descuentos\":null,\"Calidades\":null,\"MercsDeposito\":null,\"CorredorId\":0,\"DatosFijacion\":null,\"PorcentajeComision\":null,\"ContratoCorredor\":null,\"ContratoVendedor\":null,\"SelCargoMOA\":null,\"SelCargoVendedor\":null,\"Madre\":null,\"EsFason\":null,\"ContratoMadre\":null,\"Posicion\":null,\"TipoFason\":null,\"TipoFasonId\":0,\"FasonId\":0,\"Operador\":null,\"OperadorId\":0,\"AgenteId\":0,\"AperturaPrecios\":null,\"PreciosPactados\":null,\"PrecioNeto\":null,\"Pizarra\":null,\"StandardCalidadId\":null,\"StandardDeCalidadDescripcion\":null,\"PagoDiferido\":null,\"ZonaId\":null,\"ZonaDescripcion\":null,\"AcuerdoId\":null,\"ImporteFinanciero\":null,\"ImporteRedespacho\":null,\"ImporteComision\":null,\"ImporteBonificacion\":null,\"PorcentajeBonificacion\":null,\"Compensacion\":null,\"Acuerdo\":null,\"Rechazo\":null,\"ComercialZonaId\":null,\"ComercialZonaDescripcion\":null,\"OcultarEnTablero\":false,\"FechaCiertaFormateado\":null,\"FechaCierta\":null,\"MonedaBonificacion\":null,\"MesPosicion\":null,\"CampanaMaterialId\":null,\"ContratoAcuerdoId\":null,\"PorcentajeDePago\":null,\"TipoAgenteCompraId\":null,\"CaratulaExtension\":null,\"CaratulaMAT\":null,\"PrecioAjusteComision\":null,\"MonedaAjusteComisionId\":null,\"FechaOperacion\":null,\"FechaOperacionFormateado\":null,\"MotivoOperacionAnterior\":null,\"UsuarioConfirmador\":null,\"FechaConfirmacion\":null,\"CantidadMaximaCupo\":0,\"ChequeElectronico\":null,\"DolarizadoExpress\":null,\"DolarizadoExpressValor\":null,\"PagoCBU\":null,\"DolarizadoValor\":null},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
            //a);
            var data = (BasicoContrato)((System.Web.Mvc.JsonResult)result).Data;
            Assert.AreEqual(data.ContratoId, 1);
        }
        [Test]
        public void SuscripcionNotificacionesTest()
        {
            HttpContext.Current.Session["comercialId"] = 1;
            compranetManagerMock.Setup(x => x.GrabarSuscripcion("a", 1)).Returns(new GrabarSuscripcionResult { SuscripcionId = 1, Errores = new List<ErrorMessage>() });
            var result = target.SuscripcionNotificaciones("a");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            compranetManagerMock.Verify(x => x.GrabarSuscripcion(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"SuscripcionId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void UsuarioSuscriptoTest()
        {
            HttpContext.Current.Session["comercialId"] = 1;
            compranetManagerMock.Setup(x => x.UsuarioSuscripto(1)).Returns(true);
            var result = target.UsuarioSuscripto();
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            compranetManagerMock.Verify(x => x.UsuarioSuscripto(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":true,\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerContratosPendientesTest()
        {
            HttpContext.Current.Session["equipo"] = new List<int> { 1, 2 };
            contratoManagerMock.Setup(x => x.TraerContratosPendientes(GlobalVariables.Equipo)).Returns(new List<AvisoContratoDto>() { new AvisoContratoDto { Cantidad = 1, ContratoId = 1, ComercialCreadorAD = "ad", Moneda = "a", Precio = 12 } });
            var result = target.TraerContratosPendientes();
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.TraerContratosPendientes(It.IsAny<List<int>>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"ContratoId\":1,\"RazonSocial\":null,\"Cantidad\":1,\"Precio\":12,\"Moneda\":\"a\",\"Fecha\":null,\"FechaDb\":\"\\/Date(-62135586000000)\\/\",\"ComercialCreadorAD\":\"ad\",\"NombreApellido\":null}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ObtenerDatosCompraNetTest()
        {
            contratoManagerMock.Setup(x => x.TraerDatosCompraNet(1)).Returns(new DatosCompraNetDto { LocalidadId = 1, ProvinciaId = 1, ProveedorId = 1, BoletoCompraNetId = 1, BolsaCompraNetId = 1, ClasificacionCompraNetId = 1, Consignatario = true, Localidad = "a", Provincia = "b" });
            var result = target.ObtenerDatosCompraNet(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.TraerDatosCompraNet(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ProveedorId\":1,\"LocalidadId\":1,\"Localidad\":\"a\",\"ProvinciaId\":1,\"Provincia\":\"b\",\"ClasificacionCompraNetId\":1,\"Consignatario\":true,\"BoletoCompraNetId\":1,\"BolsaCompraNetId\":1,\"ComisionPorcentaje\":null,\"PlanCanje\":null},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ObtenerFijacionesAutomaticasTest()
        {
            fijacionManagerMock.Setup(x => x.TraerDatosFijacion("201", "202", 1, "a", 0)).Returns(new List<DatosFijacionDeContratoDto>() { new DatosFijacionDeContratoDto { ContratoId = "1", Filtro = "a|aa", KilosAplicados = "12", KilosContrato = "200", KilosPendiente = "20" } });
            var result = target.ObtenerFijacionesAutomaticas("201", "202", 1, "a", 0, false);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fijacionManagerMock.Verify(x => x.TraerDatosFijacion(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"ContratoId\":\"1\",\"KilosAplicados\":\"12\",\"KilosPendiente\":\"20\",\"KilosContrato\":\"200\",\"FechaDesde\":null,\"FechaHasta\":null,\"Filtro\":\"a|aa\",\"DesdeEntrega\":null,\"HastaEntrega\":null,\"Posicion\":null,\"Calidad\":null,\"Campana\":null,\"PagoDiferido\":null,\"Centro\":null,\"CentroDescripcion\":null,\"ARecibirSinPrecio\":null,\"RecibidoSinFijar\":null,\"Color\":null,\"ImporteAPrecio\":0,\"ImporteSobrePrecio\":0,\"MonedaAPrecio\":null,\"MonedaSobrePrecio\":null,\"PorcentajeAPrecio\":0,\"PorcentajeSobrePrecio\":0,\"CondicionFijacionCod\":null,\"CondicionFijacionDescripcion\":null,\"CondicionPagoCod\":null,\"CondicionPagoDescripcion\":null,\"ChequeElectronico\":null,\"Calidades\":null,\"CampanaId\":0,\"Clasificacion\":null,\"Cesion\":false,\"Anticipo\":false,\"FijacionSap\":null,\"Aperturas\":null,\"Bonificaciones\":null,\"Virtual\":false,\"KgContratoTotal\":0,\"Pase\":false}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerContratoMadreTest()
        {
            contratoManagerMock.Setup(x => x.TraerContratoMadre("1")).Returns(new ContratoResult { Contrato = new BasicoContrato(), Errores = new List<ErrorMessage>() });
            var result = target.TraerContratoMadre("1");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.TraerContratoMadre(It.IsAny<string>()), Times.Once);
            //Assert.AreEqual(
            //"{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Contrato\":{\"ChequeElectronicoValor\":null,\"Id\":0,\"Cuit\":null,\"ContratoId\":0,\"MaterialId\":0,\"NivelTarifaId\":null,\"TipoNegocioId\":0,\"Cantidad\":0,\"Precio\":0,\"PrecioPlazo\":null,\"FechaEntrega\":null,\"CampanaId\":null,\"FechaDesde\":null,\"FechaDesdeFormateado\":null,\"FechaHasta\":null,\"FechaHastaFormateado\":null,\"ProveedorId\":0,\"MonedaId\":null,\"Moneda\":null,\"Fecha\":null,\"FechaFormateado\":null,\"Hora\":null,\"NivelTarifa\":null,\"TarifaFlete\":null,\"GrupoCompra\":0,\"GrupoCompraDescripcion\":null,\"ComercialId\":null,\"ComercialCreadorId\":null,\"ProvinciaId\":null,\"LocalidadId\":null,\"Base\":null,\"Importe_Sustentable\":null,\"MonedaId_Sustentable\":null,\"Moneda_Sustentable\":null,\"Fecha_Dolarizado\":null,\"Fecha_DolarizadoFormateado\":null,\"Dias_Pesificado\":null,\"NoInformaSIO\":null,\"TrigoEspecial\":null,\"Estado\":null,\"UsuarioId\":null,\"ContratoSAP\":null,\"Ampliaciones\":null,\"TipoNegocio\":null,\"Proveedor\":null,\"Corredor\":null,\"CUITCorredor\":null,\"Comercial\":null,\"ComercialCreador\":null,\"Material\":null,\"Campania\":null,\"Provincia\":null,\"Localidad\":null,\"Estado_Contrato\":null,\"Fecha_Order\":\"\\/Date(-62135586000000)\\/\",\"Estado_Order\":0,\"Observacion\":null,\"FijacionDePrecioContratoId\":null,\"Sustentable\":null,\"Dolarizado\":null,\"Pesificado\":null,\"Negocio\":null,\"ClasificacionId\":null,\"ClasificacionDescripcion\":null,\"DestinoId\":null,\"DestinoDescripcion\":null,\"CantidadCamiones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"CondicionFijacion\":null,\"CD\":null,\"Warrant\":null,\"PagoDirectoVendedor\":null,\"CalidadDescripcion\":null,\"EstablecimientoPropio\":null,\"BoletoId\":null,\"BolsaId\":null,\"BoletoDescripcion\":null,\"BolsaDescripcion\":null,\"DesdeFijacion\":null,\"DesdeFijacionFormateado\":null,\"HastaFijacion\":null,\"HastaFijacionFormateado\":null,\"CondicionFijacionDescripcion\":null,\"Descuentos\":null,\"Calidades\":null,\"MercsDeposito\":null,\"CorredorId\":0,\"DatosFijacion\":null,\"PorcentajeComision\":null,\"ContratoCorredor\":null,\"ContratoVendedor\":null,\"SelCargoMOA\":null,\"SelCargoVendedor\":null,\"Madre\":null,\"EsFason\":null,\"ContratoMadre\":null,\"Posicion\":null,\"TipoFason\":null,\"TipoFasonId\":0,\"FasonId\":0,\"Operador\":null,\"OperadorId\":0,\"AgenteId\":0,\"AperturaPrecios\":null,\"PreciosPactados\":null,\"PrecioNeto\":null,\"Pizarra\":null,\"StandardCalidadId\":null,\"StandardDeCalidadDescripcion\":null,\"PagoDiferido\":null,\"ZonaId\":null,\"ZonaDescripcion\":null,\"AcuerdoId\":null,\"ImporteFinanciero\":null,\"ImporteRedespacho\":null,\"ImporteComision\":null,\"ImporteBonificacion\":null,\"PorcentajeBonificacion\":null,\"Compensacion\":null,\"Acuerdo\":null,\"Rechazo\":null,\"ComercialZonaId\":null,\"ComercialZonaDescripcion\":null,\"OcultarEnTablero\":false,\"FechaCiertaFormateado\":null,\"FechaCierta\":null,\"MonedaBonificacion\":null,\"MesPosicion\":null,\"CampanaMaterialId\":null,\"ContratoAcuerdoId\":null,\"PorcentajeDePago\":null,\"TipoAgenteCompraId\":null,\"CaratulaExtension\":null,\"CaratulaMAT\":null,\"PrecioAjusteComision\":null,\"MonedaAjusteComisionId\":null,\"FechaOperacion\":null,\"FechaOperacionFormateado\":null,\"MotivoOperacionAnterior\":null,\"UsuarioConfirmador\":null,\"FechaConfirmacion\":null,\"CantidadMaximaCupo\":0,\"ChequeElectronico\":null,\"DolarizadoExpress\":null,\"DolarizadoExpressValor\":null,\"PagoCBU\":null,\"DolarizadoValor\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
            //a);
            var data = (BasicoContrato)((ContratoResult)((JsonResult)result).Data).Contrato;
            Assert.NotNull(data);
        }

        [Test]
        public void GrabarFasonTest()
        {
            var fason = new Fason { Id = 1, EstadoId = 3 };
            fasonManagerMock.Setup(x => x.GrabarFason(fason)).Returns(new GrabarFasonResult { FasonId = 1, Errores = new List<ErrorMessage>() });
            var result = target.GrabarFason(fason);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fasonManagerMock.Verify(x => x.GrabarFason(It.IsAny<Fason>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"FasonId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerFasonCompletoTest()
        {
            fasonManagerMock.Setup(x => x.TraerFason(1)).Returns(new BasicoContrato { FasonId = 1, Estado = 3 });
            var result = target.TraerFasonCompleto(1);
            Assert.NotNull(result);

            var a = serializer.Serialize(result);

            fasonManagerMock.Verify(x => x.TraerFason(It.IsAny<int>()), Times.Once);
            //Assert.AreEqual(
            //"{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ChequeElectronicoValor\":null,\"Id\":0,\"Cuit\":null,\"ContratoId\":0,\"MaterialId\":0,\"NivelTarifaId\":null,\"TipoNegocioId\":0,\"Cantidad\":0,\"Precio\":0,\"PrecioPlazo\":null,\"FechaEntrega\":null,\"CampanaId\":null,\"FechaDesde\":null,\"FechaDesdeFormateado\":null,\"FechaHasta\":null,\"FechaHastaFormateado\":null,\"ProveedorId\":0,\"MonedaId\":null,\"Moneda\":null,\"Fecha\":null,\"FechaFormateado\":null,\"Hora\":null,\"NivelTarifa\":null,\"TarifaFlete\":null,\"GrupoCompra\":0,\"GrupoCompraDescripcion\":null,\"ComercialId\":null,\"ComercialCreadorId\":null,\"ProvinciaId\":null,\"LocalidadId\":null,\"Base\":null,\"Importe_Sustentable\":null,\"MonedaId_Sustentable\":null,\"Moneda_Sustentable\":null,\"Fecha_Dolarizado\":null,\"Fecha_DolarizadoFormateado\":null,\"Dias_Pesificado\":null,\"NoInformaSIO\":null,\"TrigoEspecial\":null,\"Estado\":3,\"UsuarioId\":null,\"ContratoSAP\":null,\"Ampliaciones\":null,\"TipoNegocio\":null,\"Proveedor\":null,\"Corredor\":null,\"CUITCorredor\":null,\"Comercial\":null,\"ComercialCreador\":null,\"Material\":null,\"Campania\":null,\"Provincia\":null,\"Localidad\":null,\"Estado_Contrato\":null,\"Fecha_Order\":\"\\/Date(-62135586000000)\\/\",\"Estado_Order\":0,\"Observacion\":null,\"FijacionDePrecioContratoId\":null,\"Sustentable\":null,\"Dolarizado\":null,\"Pesificado\":null,\"Negocio\":null,\"ClasificacionId\":null,\"ClasificacionDescripcion\":null,\"DestinoId\":null,\"DestinoDescripcion\":null,\"CantidadCamiones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"CondicionFijacion\":null,\"CD\":null,\"Warrant\":null,\"PagoDirectoVendedor\":null,\"CalidadDescripcion\":null,\"EstablecimientoPropio\":null,\"BoletoId\":null,\"BolsaId\":null,\"BoletoDescripcion\":null,\"BolsaDescripcion\":null,\"DesdeFijacion\":null,\"DesdeFijacionFormateado\":null,\"HastaFijacion\":null,\"HastaFijacionFormateado\":null,\"CondicionFijacionDescripcion\":null,\"Descuentos\":null,\"Calidades\":null,\"MercsDeposito\":null,\"CorredorId\":0,\"DatosFijacion\":null,\"PorcentajeComision\":null,\"ContratoCorredor\":null,\"ContratoVendedor\":null,\"SelCargoMOA\":null,\"SelCargoVendedor\":null,\"Madre\":null,\"EsFason\":null,\"ContratoMadre\":null,\"Posicion\":null,\"TipoFason\":null,\"TipoFasonId\":0,\"FasonId\":1,\"Operador\":null,\"OperadorId\":0,\"AgenteId\":0,\"AperturaPrecios\":null,\"PreciosPactados\":null,\"PrecioNeto\":null,\"Pizarra\":null,\"StandardCalidadId\":null,\"StandardDeCalidadDescripcion\":null,\"PagoDiferido\":null,\"ZonaId\":null,\"ZonaDescripcion\":null,\"AcuerdoId\":null,\"ImporteFinanciero\":null,\"ImporteRedespacho\":null,\"ImporteComision\":null,\"ImporteBonificacion\":null,\"PorcentajeBonificacion\":null,\"Compensacion\":null,\"Acuerdo\":null,\"Rechazo\":null,\"ComercialZonaId\":null,\"ComercialZonaDescripcion\":null,\"OcultarEnTablero\":false,\"FechaCiertaFormateado\":null,\"FechaCierta\":null,\"MonedaBonificacion\":null,\"MesPosicion\":null,\"CampanaMaterialId\":null,\"ContratoAcuerdoId\":null,\"PorcentajeDePago\":null,\"TipoAgenteCompraId\":null,\"CaratulaExtension\":null,\"CaratulaMAT\":null,\"PrecioAjusteComision\":null,\"MonedaAjusteComisionId\":null,\"FechaOperacion\":null,\"FechaOperacionFormateado\":null,\"MotivoOperacionAnterior\":null,\"UsuarioConfirmador\":null,\"FechaConfirmacion\":null,\"CantidadMaximaCupo\":0,\"ChequeElectronico\":null,\"DolarizadoExpress\":null,\"DolarizadoExpressValor\":null,\"PagoCBU\":null,\"DolarizadoValor\":null},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
            //a);
            var data = (BasicoContrato)((JsonResult)result).Data;
            Assert.NotNull(data);
            Assert.AreEqual(data.FasonId, 1);
        }
        [Test]
        public void FinalizarFasonTest()
        {
            fasonManagerMock.Setup(x => x.FinalizarFason(1)).Returns(new GrabarFasonResult { FasonId = 1, Errores = new List<ErrorMessage>() });
            var result = target.FinalizarFason(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fasonManagerMock.Verify(x => x.FinalizarFason(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"FasonId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GrabarAmpliacionFasonTest()
        {
            var fason = new Fason { Id = 1, ComercialId = 1, MaterialId = 1, CampanaId = 1 };
            fasonManagerMock.Setup(x => x.GrabarAmpliacionFason(fason)).Returns(new GrabarContratoResult { ContratoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.GrabarAmpliacionFason(fason);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fasonManagerMock.Verify(x => x.GrabarAmpliacionFason(It.IsAny<Fason>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ContratoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GrabarAgenteTest()
        {
            var agente = new AgenteCompra { Id = 1, EstadoId = 3 };
            agenteManagerMock.Setup(x => x.GrabarAgente(agente)).Returns(new GrabarAgenteResult { AgenteId = 1, Errores = new List<ErrorMessage>() });
            var result = target.GrabarAgente(agente);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            agenteManagerMock.Verify(x => x.GrabarAgente(It.IsAny<AgenteCompra>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"AgenteId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerAgenteCompletoTest()
        {
            agenteManagerMock.Setup(x => x.TraerAgente(1)).Returns(new BasicoContrato { AgenteId = 1, Estado = 3 });
            var result = target.TraerAgenteCompleto(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            agenteManagerMock.Verify(x => x.TraerAgente(It.IsAny<int>()), Times.Once);
            //Assert.AreEqual(
            //"{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ChequeElectronicoValor\":null,\"Id\":0,\"Cuit\":null,\"ContratoId\":0,\"MaterialId\":0,\"NivelTarifaId\":null,\"TipoNegocioId\":0,\"Cantidad\":0,\"Precio\":0,\"PrecioPlazo\":null,\"FechaEntrega\":null,\"CampanaId\":null,\"FechaDesde\":null,\"FechaDesdeFormateado\":null,\"FechaHasta\":null,\"FechaHastaFormateado\":null,\"ProveedorId\":0,\"MonedaId\":null,\"Moneda\":null,\"Fecha\":null,\"FechaFormateado\":null,\"Hora\":null,\"NivelTarifa\":null,\"TarifaFlete\":null,\"GrupoCompra\":0,\"GrupoCompraDescripcion\":null,\"ComercialId\":null,\"ComercialCreadorId\":null,\"ProvinciaId\":null,\"LocalidadId\":null,\"Base\":null,\"Importe_Sustentable\":null,\"MonedaId_Sustentable\":null,\"Moneda_Sustentable\":null,\"Fecha_Dolarizado\":null,\"Fecha_DolarizadoFormateado\":null,\"Dias_Pesificado\":null,\"NoInformaSIO\":null,\"TrigoEspecial\":null,\"Estado\":3,\"UsuarioId\":null,\"ContratoSAP\":null,\"Ampliaciones\":null,\"TipoNegocio\":null,\"Proveedor\":null,\"Corredor\":null,\"CUITCorredor\":null,\"Comercial\":null,\"ComercialCreador\":null,\"Material\":null,\"Campania\":null,\"Provincia\":null,\"Localidad\":null,\"Estado_Contrato\":null,\"Fecha_Order\":\"\\/Date(-62135586000000)\\/\",\"Estado_Order\":0,\"Observacion\":null,\"FijacionDePrecioContratoId\":null,\"Sustentable\":null,\"Dolarizado\":null,\"Pesificado\":null,\"Negocio\":null,\"ClasificacionId\":null,\"ClasificacionDescripcion\":null,\"DestinoId\":null,\"DestinoDescripcion\":null,\"CantidadCamiones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"CondicionFijacion\":null,\"CD\":null,\"Warrant\":null,\"PagoDirectoVendedor\":null,\"CalidadDescripcion\":null,\"EstablecimientoPropio\":null,\"BoletoId\":null,\"BolsaId\":null,\"BoletoDescripcion\":null,\"BolsaDescripcion\":null,\"DesdeFijacion\":null,\"DesdeFijacionFormateado\":null,\"HastaFijacion\":null,\"HastaFijacionFormateado\":null,\"CondicionFijacionDescripcion\":null,\"Descuentos\":null,\"Calidades\":null,\"MercsDeposito\":null,\"CorredorId\":0,\"DatosFijacion\":null,\"PorcentajeComision\":null,\"ContratoCorredor\":null,\"ContratoVendedor\":null,\"SelCargoMOA\":null,\"SelCargoVendedor\":null,\"Madre\":null,\"EsFason\":null,\"ContratoMadre\":null,\"Posicion\":null,\"TipoFason\":null,\"TipoFasonId\":0,\"FasonId\":0,\"Operador\":null,\"OperadorId\":0,\"AgenteId\":1,\"AperturaPrecios\":null,\"PreciosPactados\":null,\"PrecioNeto\":null,\"Pizarra\":null,\"StandardCalidadId\":null,\"StandardDeCalidadDescripcion\":null,\"PagoDiferido\":null,\"ZonaId\":null,\"ZonaDescripcion\":null,\"AcuerdoId\":null,\"ImporteFinanciero\":null,\"ImporteRedespacho\":null,\"ImporteComision\":null,\"ImporteBonificacion\":null,\"PorcentajeBonificacion\":null,\"Compensacion\":null,\"Acuerdo\":null,\"Rechazo\":null,\"ComercialZonaId\":null,\"ComercialZonaDescripcion\":null,\"OcultarEnTablero\":false,\"FechaCiertaFormateado\":null,\"FechaCierta\":null,\"MonedaBonificacion\":null,\"MesPosicion\":null,\"CampanaMaterialId\":null,\"ContratoAcuerdoId\":null,\"PorcentajeDePago\":null,\"TipoAgenteCompraId\":null,\"CaratulaExtension\":null,\"CaratulaMAT\":null,\"PrecioAjusteComision\":null,\"MonedaAjusteComisionId\":null,\"FechaOperacion\":null,\"FechaOperacionFormateado\":null,\"MotivoOperacionAnterior\":null,\"UsuarioConfirmador\":null,\"FechaConfirmacion\":null,\"CantidadMaximaCupo\":0,\"ChequeElectronico\":null,\"DolarizadoExpress\":null,\"DolarizadoExpressValor\":null,\"PagoCBU\":null,\"DolarizadoValor\":null},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
            //a);
            var data = (BasicoContrato)((JsonResult)result).Data;
            Assert.NotNull(data);
            Assert.AreEqual(data.AgenteId, 1);
        }
        [Test]
        public void FinalizarAgenteTest()
        {
            agenteManagerMock.Setup(x => x.FinalizarAgente(1)).Returns(new GrabarAgenteResult { AgenteId = 1, Errores = new List<ErrorMessage>() });
            var result = target.FinalizarAgente(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            agenteManagerMock.Verify(x => x.FinalizarAgente(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"AgenteId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GrabarAmpliacionAgenteTest()
        {
            var agente = new AgenteCompra { Id = 1, ComercialId = 1, MaterialId = 1 };
            agenteManagerMock.Setup(x => x.GrabarAmpliacionAgente(agente)).Returns(new GrabarAgenteResult { AgenteId = 1, Errores = new List<ErrorMessage>() });
            var result = target.GrabarAmpliacionAgente(agente);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            agenteManagerMock.Verify(x => x.GrabarAmpliacionAgente(It.IsAny<AgenteCompra>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"AgenteId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void BorrarAcuerdoTest()
        {
            var agente = new AgenteCompra { Id = 1, ComercialId = 1, MaterialId = 1 };
            acuerdoManagerMock.Setup(x => x.BorrarAcuerdo(It.IsAny<ContratoAcuerdo>()))
                .Returns(new GrabarAcuerdoResult { AcuerdoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.BorrarAcuerdo(new ContratoAcuerdo());
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            acuerdoManagerMock.Verify(x => x.BorrarAcuerdo(It.IsAny<ContratoAcuerdo>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"AcuerdoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void GrabarAmpliacionFijacionTest()
        {
            var agente = new AgenteCompra { Id = 1, ComercialId = 1, MaterialId = 1 };
            fijacionManagerMock.Setup(x => x.GrabarAmpliacionFijacion(It.IsAny<FijacionDePrecioContrato>()))
                .Returns(new GrabarContratoResult { ContratoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.GrabarAmpliacionFijacion(new FijacionDePrecioContrato());
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fijacionManagerMock.Verify(x => x.GrabarAmpliacionFijacion(It.IsAny<FijacionDePrecioContrato>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ContratoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerDescuentosPorContratoTest()
        {
            var agente = new AgenteCompra { Id = 1, ComercialId = 1, MaterialId = 1 };
            contratoManagerMock.Setup(x => x.TraerDescuentosPorContrato(It.IsAny<int>()))
                .Returns(new List<DescuentoBonificacionDto> { new DescuentoBonificacionDto() { Id=1,ContratoId=1,
                    FechaDesde="01/10/2019",
                    FechaHasta= "01/10/2019",
                    Importe=1,
                    MonedaId="a",
                    Moneda="a",
                    Porcentaje=1,
                    TipoDBDesc="a",
                    TipoDBId=1,
                    TipoPeriodoDBDesc="a",
                    TipoPeriodoDBId=1 } });
            var result = target.TraerDescuentosPorContrato(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.TraerDescuentosPorContrato(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Id\":1,\"FechaDesde\":\"01/10/2019\",\"FechaHasta\":\"01/10/2019\",\"Importe\":1,\"MonedaId\":\"a\",\"Moneda\":\"a\",\"Porcentaje\":1,\"TipoDBDesc\":\"a\",\"TipoDBId\":1,\"TipoPeriodoDBDesc\":\"a\",\"TipoPeriodoDBId\":1,\"ContratoId\":1}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerAperturaPrecioPorContratoTestNoFijacion()
        {
            var agente = new AgenteCompra { Id = 1, ComercialId = 1, MaterialId = 1 };
            contratoManagerMock.Setup(x => x.TraerAperturaDePrecioPorContrato(It.IsAny<int>()))
                .Returns(new List<AperturaPrecioDto> { new AperturaPrecioDto() { contratoId=1,
                    ConceptoAperturaPrecio="a",
                    ConceptoAperturaPrecioId=1,
                    FijacionId=null,Id=1,
                    Importe=1,Moneda="a",
                    MonedaId="a",
                    Porcentaje=1} });
            var result = target.TraerAperturaPrecioPorContrato(1, "A");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.TraerAperturaDePrecioPorContrato(It.IsAny<int>()), Times.Once);
            fijacionManagerMock.Verify(x => x.TraerAperturaDePrecioPorFijacion(It.IsAny<int>()), Times.Never);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Id\":1,\"contratoId\":1,\"FijacionId\":null,\"ConceptoAperturaPrecioId\":1,\"Importe\":1,\"Porcentaje\":1,\"MonedaId\":\"a\",\"ConceptoAperturaPrecio\":\"a\",\"Moneda\":\"a\"}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerAperturaPrecioPorContratoTestFijacion()
        {
            var agente = new AgenteCompra { Id = 1, ComercialId = 1, MaterialId = 1 };
            fijacionManagerMock.Setup(x => x.TraerAperturaDePrecioPorFijacion(It.IsAny<int>()))
                .Returns(new List<AperturaPrecioDto> { new AperturaPrecioDto() { contratoId=1,
                    ConceptoAperturaPrecio="a",
                    ConceptoAperturaPrecioId=1,
                    FijacionId=null,Id=1,
                    Importe=1,Moneda="a",
                    MonedaId="a",
                    Porcentaje=1} });
            var result = target.TraerAperturaPrecioPorContrato(1, "Fijacion");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.TraerAperturaDePrecioPorContrato(It.IsAny<int>()), Times.Never);
            fijacionManagerMock.Verify(x => x.TraerAperturaDePrecioPorFijacion(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Id\":1,\"contratoId\":1,\"FijacionId\":null,\"ConceptoAperturaPrecioId\":1,\"Importe\":1,\"Porcentaje\":1,\"MonedaId\":\"a\",\"ConceptoAperturaPrecio\":\"a\",\"Moneda\":\"a\"}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ConfirmarAcuerdoTest()
        {
            acuerdoManagerMock.Setup(x => x.GrabarAcuerdo(It.IsAny<ContratoAcuerdo>()))
                .Returns(new GrabarAcuerdoResult { AcuerdoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.GrabarAcuerdo(new ContratoAcuerdo());
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            acuerdoManagerMock.Verify(x => x.GrabarAcuerdo(It.IsAny<ContratoAcuerdo>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"AcuerdoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerAcuerdoCompletoTest()
        {
            acuerdoManagerMock.Setup(x => x.TraerAcuerdo(It.IsAny<int>()))
                .Returns(new BasicoContrato());
            var result = target.TraerAcuerdoCompleto(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            acuerdoManagerMock.Verify(x => x.TraerAcuerdo(It.IsAny<int>()), Times.Once);
            //Assert.AreEqual(
            //"{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ChequeElectronicoValor\":null,\"Id\":0,\"Cuit\":null,\"ContratoId\":0,\"MaterialId\":0,\"NivelTarifaId\":null,\"TipoNegocioId\":0,\"Cantidad\":0,\"Precio\":0,\"PrecioPlazo\":null,\"FechaEntrega\":null,\"CampanaId\":null,\"FechaDesde\":null,\"FechaDesdeFormateado\":null,\"FechaHasta\":null,\"FechaHastaFormateado\":null,\"ProveedorId\":0,\"MonedaId\":null,\"Moneda\":null,\"Fecha\":null,\"FechaFormateado\":null,\"Hora\":null,\"NivelTarifa\":null,\"TarifaFlete\":null,\"GrupoCompra\":0,\"GrupoCompraDescripcion\":null,\"ComercialId\":null,\"ComercialCreadorId\":null,\"ProvinciaId\":null,\"LocalidadId\":null,\"Base\":null,\"Importe_Sustentable\":null,\"MonedaId_Sustentable\":null,\"Moneda_Sustentable\":null,\"Fecha_Dolarizado\":null,\"Fecha_DolarizadoFormateado\":null,\"Dias_Pesificado\":null,\"NoInformaSIO\":null,\"TrigoEspecial\":null,\"Estado\":null,\"UsuarioId\":null,\"ContratoSAP\":null,\"Ampliaciones\":null,\"TipoNegocio\":null,\"Proveedor\":null,\"Corredor\":null,\"CUITCorredor\":null,\"Comercial\":null,\"ComercialCreador\":null,\"Material\":null,\"Campania\":null,\"Provincia\":null,\"Localidad\":null,\"Estado_Contrato\":null,\"Fecha_Order\":\"\\/Date(-62135586000000)\\/\",\"Estado_Order\":0,\"Observacion\":null,\"FijacionDePrecioContratoId\":null,\"Sustentable\":null,\"Dolarizado\":null,\"Pesificado\":null,\"Negocio\":null,\"ClasificacionId\":null,\"ClasificacionDescripcion\":null,\"DestinoId\":null,\"DestinoDescripcion\":null,\"CantidadCamiones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"CondicionFijacion\":null,\"CD\":null,\"Warrant\":null,\"PagoDirectoVendedor\":null,\"CalidadDescripcion\":null,\"EstablecimientoPropio\":null,\"BoletoId\":null,\"BolsaId\":null,\"BoletoDescripcion\":null,\"BolsaDescripcion\":null,\"DesdeFijacion\":null,\"DesdeFijacionFormateado\":null,\"HastaFijacion\":null,\"HastaFijacionFormateado\":null,\"CondicionFijacionDescripcion\":null,\"Descuentos\":null,\"Calidades\":null,\"MercsDeposito\":null,\"CorredorId\":0,\"DatosFijacion\":null,\"PorcentajeComision\":null,\"ContratoCorredor\":null,\"ContratoVendedor\":null,\"SelCargoMOA\":null,\"SelCargoVendedor\":null,\"Madre\":null,\"EsFason\":null,\"ContratoMadre\":null,\"Posicion\":null,\"TipoFason\":null,\"TipoFasonId\":0,\"FasonId\":0,\"Operador\":null,\"OperadorId\":0,\"AgenteId\":0,\"AperturaPrecios\":null,\"PreciosPactados\":null,\"PrecioNeto\":null,\"Pizarra\":null,\"StandardCalidadId\":null,\"StandardDeCalidadDescripcion\":null,\"PagoDiferido\":null,\"ZonaId\":null,\"ZonaDescripcion\":null,\"AcuerdoId\":null,\"ImporteFinanciero\":null,\"ImporteRedespacho\":null,\"ImporteComision\":null,\"ImporteBonificacion\":null,\"PorcentajeBonificacion\":null,\"Compensacion\":null,\"Acuerdo\":null,\"Rechazo\":null,\"ComercialZonaId\":null,\"ComercialZonaDescripcion\":null,\"OcultarEnTablero\":false,\"FechaCiertaFormateado\":null,\"FechaCierta\":null,\"MonedaBonificacion\":null,\"MesPosicion\":null,\"CampanaMaterialId\":null,\"ContratoAcuerdoId\":null,\"PorcentajeDePago\":null,\"TipoAgenteCompraId\":null,\"CaratulaExtension\":null,\"CaratulaMAT\":null,\"PrecioAjusteComision\":null,\"MonedaAjusteComisionId\":null,\"FechaOperacion\":null,\"FechaOperacionFormateado\":null,\"MotivoOperacionAnterior\":null,\"UsuarioConfirmador\":null,\"FechaConfirmacion\":null,\"CantidadMaximaCupo\":0,\"ChequeElectronico\":null,\"DolarizadoExpress\":null,\"DolarizadoExpressValor\":null,\"PagoCBU\":null,\"DolarizadoValor\":null},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
            //a);
            var data = (BasicoContrato)((JsonResult)result).Data;
            Assert.NotNull(data);
        }
        [Test]
        public void FinalizarAcuerdoTest()
        {
            acuerdoManagerMock.Setup(x => x.FinalizarAcuerdo(It.IsAny<int>()))
                .Returns(new GrabarAcuerdoResult { AcuerdoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.FinalizarAcuerdo(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            acuerdoManagerMock.Verify(x => x.FinalizarAcuerdo(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"AcuerdoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void AnularContrato()
        {
            contratoManagerMock.Setup(x => x.AnularContrato(It.IsAny<Contrato>(), It.IsAny<string>()))
                .Returns(new GrabarContratoResult { ContratoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.AnularContrato(new Contrato());
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.AnularContrato(It.IsAny<Contrato>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ContratoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ObtenerContratosParaCopiarTest()
        {
            contratoManagerMock.Setup(x => x.TraerContratosPorSap(It.IsAny<string>()))
                .Returns(new List<ContratoCopiar>() { new ContratoCopiar { CantidadD = 1, Comercial = "", ContratoSap = "a", Fecha = "a", Filtro = "a", Id = 1, Material = "a", RazonSocial = "a", tipoNegocio = "a" } });
            var result = target.ObtenerContratosParaCopiar("a");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.TraerContratosPorSap(It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Id\":1,\"RazonSocial\":\"a\",\"Comercial\":\"\",\"Material\":\"a\",\"Fecha\":\"a\",\"ContratoSap\":\"a\",\"Filtro\":\"a\",\"tipoNegocio\":\"a\",\"CantidadD\":1,\"Cantidad\":\"1\",\"Precio\":\"0\",\"NegocioDescripcion\":null,\"PrecioD\":0,\"FechaDesde\":null,\"FechaHasta\":null,\"Moneda\":null,\"MonedaId\":null}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ObtenerContratosAcuerdoTest()
        {
            contratoManagerMock.Setup(x => x.TraerContratosAcuerdo(It.IsAny<string>()))
                .Returns(new List<ContratoCopiar>() { new ContratoCopiar { CantidadD = 1, Comercial = "", ContratoSap = "a", Fecha = "a", Filtro = "a", Id = 1, Material = "a", RazonSocial = "a", tipoNegocio = "a" } });
            var result = target.ObtenerContratosAcuerdo("a");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.TraerContratosAcuerdo(It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                  "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Id\":1,\"RazonSocial\":\"a\",\"Comercial\":\"\",\"Material\":\"a\",\"Fecha\":\"a\",\"ContratoSap\":\"a\",\"Filtro\":\"a\",\"tipoNegocio\":\"a\",\"CantidadD\":1,\"Cantidad\":\"1\",\"Precio\":\"0\",\"NegocioDescripcion\":null,\"PrecioD\":0,\"FechaDesde\":null,\"FechaHasta\":null,\"Moneda\":null,\"MonedaId\":null}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                  a);
        }
        [Test]
        public void BuscarGrupoDeComprasTest()
        {
            comercialManagerMock.Setup(x => x.ListarGrupoDeCompras(It.IsAny<string>()))
                .Returns(new List<GrupoDeCompras>() { new GrupoDeCompras { Id = 1, Descripcion = "a" } });
            var result = target.BuscarGrupoDeCompras("a");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            comercialManagerMock.Verify(x => x.ListarGrupoDeCompras(It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Id\":1,\"Descripcion\":\"a\"}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void BuscarTotalesTest()
        {
            contratoManagerMock.Setup(x => x.TraerTotalesPesosDolares(It.IsAny<DataSourceRequest>(), It.IsAny<List<int>>(), It.IsAny<List<int>>()))
                .Returns(new TotalPesosDolares
                {
                    Ampliaciones = 1,
                    Campania = "a",
                    Cantidad = 1,
                    Comercial = "a",
                    ComercialCreador = "a",
                    ComercialId = 1,
                    ComercialCreadorId = 1,
                    Corredor = "a",
                    CorredorId = 1,
                    DestinoDescripcion = "a",
                    Estado_Contrato = "a",
                    Fecha = new DateTime(2019, 10, 1),
                    FechaDesde = new DateTime(2019, 10, 1),
                    FechaHasta = new DateTime(2019, 10, 1),
                    GrupoCompraDescripcion = "a",
                    Material = "a",
                    MaterialId = 1,
                    Negocio = "a",
                    Proveedor = "a",
                    ProveedorId = 1,
                    TipoNegocio = "a",
                    TotalDolares = 1,
                    TotalGirasol = 2,
                    TotalGirasolAlto = 1,
                    TotalMaiz = 1,
                    TotalPesos = 1,
                    TotalSoja = 1,
                    TotalTrigo = 1,
                    ContratoSAP = "a",
                    ContratoCorredor = "a"

                });


            var result = target.BuscarTotales(new Kendo.DynamicLinq.Filter());
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.TraerTotalesPesosDolares(It.IsAny<DataSourceRequest>(), It.IsAny<List<int>>(), It.IsAny<List<int>>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Proveedor\":\"a\",\"ProveedorId\":1,\"Corredor\":\"a\",\"CorredorId\":1,\"FechaDesde\":\"\\/Date(1569898800000)\\/\",\"FechaHasta\":\"\\/Date(1569898800000)\\/\",\"TipoNegocio\":\"a\",\"Material\":\"a\",\"MaterialId\":1,\"Cantidad\":1,\"Ampliaciones\":1,\"Campania\":\"a\",\"Negocio\":\"a\",\"Fecha\":\"\\/Date(1569898800000)\\/\",\"GrupoCompraDescripcion\":\"a\",\"Comercial\":\"a\",\"ComercialCreador\":\"a\",\"DestinoDescripcion\":\"a\",\"ComercialId\":1,\"Estado_Contrato\":\"a\",\"TotalPesos\":1,\"TotalDolares\":1,\"TotalTrigo\":1,\"TotalMaiz\":1,\"TotalSoja\":1,\"TotalGirasol\":2,\"TotalGirasolAlto\":1,\"Id\":0,\"ComercialCreadorId\":1,\"ContratoSAP\":\"a\",\"ContratoCorredor\":\"a\"},\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ValidarProveedorTest()
        {
            contratoManagerMock.Setup(x => x.ValidarProveedor(It.IsAny<int>()))
                .Returns(new AltaTempranaNRCODto
                {
                    AltaTemprana = "SI",
                    Bolsa = "SI",
                    Carta = "SI",
                    Consignatario = "SI",
                    FechaActualizacion = "SI",
                    Mensaje = "",
                    Nosis = "SI",
                    PlanCanje = "SI",
                    Ruca = new Ruca
                    {
                        Acopiador = new ValoresRuca { PlanCanje = "SI", Consignatario = "SI", Directo = "SI" },
                        Corredor = "SI",
                        Fason = "SI",
                        Otros = new ValoresRuca { Directo = "SI", Consignatario = "SI", PlanCanje = "SI" }
                    }
                });
            var result = target.ValidarProveedor(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.ValidarProveedor(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"AltaTemprana\":\"SI\",\"FechaActualizacion\":\"SI\",\"Nosis\":\"SI\",\"Bolsa\":\"SI\",\"PlanCanje\":\"SI\",\"Consignatario\":\"SI\",\"Ruca\":{\"Otros\":{\"Consignatario\":\"SI\",\"PlanCanje\":\"SI\",\"Directo\":\"SI\"},\"Acopiador\":{\"Consignatario\":\"SI\",\"PlanCanje\":\"SI\",\"Directo\":\"SI\"},\"Corredor\":\"SI\",\"Fason\":\"SI\"},\"Carta\":\"SI\",\"Mensaje\":\"\",\"ProveedorGrano\":null,\"BoletoFisico\":null},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void NoMostrarEnTableroTest()
        {
            negocioManagerMock.Setup(x => x.OcultarEnTablero(It.IsAny<Negocio>()))
                .Returns(new Resultado());
            var result = target.NoMostrarEnTablero(new Negocio { Id = 1, OcultarEnTablero = true });
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            negocioManagerMock.Verify(x => x.OcultarEnTablero(It.IsAny<Negocio>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ValidarModificarFinalizadoTest()
        {
            contratoManagerMock.Setup(x => x.ValidarStatus(It.IsAny<int>()))
                .Returns(new EstadoSAPDto { NumeroSio = 0, Status = "OK" });
            var result = target.ValidarModificarFinalizado(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            contratoManagerMock.Verify(x => x.ValidarStatus(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Status\":\"OK\",\"NumeroSio\":0},\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void PreAnularContratoTest()
        {
            var contrato = new Contrato { ComercialId = 1, MaterialId = 1, CampanaId = 1, ComercialCreadorId = 1 };
            contratoManagerMock.Setup(x => x.PreAnularContrato(It.IsAny<int>(), It.IsAny<string>())).Returns(new GrabarContratoResult { ContratoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.PreAnularContrato(It.IsAny<int>(), It.IsAny<string>());
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.PreAnularContrato(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ContratoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void RechazarPreAnularContratoTest()
        {
            contratoManagerMock.Setup(x => x.RechazarPreAnularContrato(It.IsAny<int>()/*, It.IsAny<string>()*/)).Returns(new GrabarContratoResult { ContratoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.RechazarPreAnularContrato(It.IsAny<int>()/*, It.IsAny<string>()*/);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.RechazarPreAnularContrato(It.IsAny<int>()/*, It.IsAny<string>()*/), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ContratoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void ReconfirmarFinalizadoTest()
        {
            contratoManagerMock.Setup(x => x.ReconfirmarFinalizado(It.IsAny<int>(), It.IsAny<string>())).Returns(new GrabarContratoResult { ContratoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.ReconfirmarFinalizado(It.IsAny<int>());
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.ReconfirmarFinalizado(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ContratoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void AnularContratoPreAnuladoTest()
        {
            contratoManagerMock.Setup(x => x.AnularContratoPreAnulado(It.IsAny<int>(), It.IsAny<string>())).Returns(new GrabarContratoResult { ContratoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.AnularContratoPreAnulado(It.IsAny<int>());
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.AnularContratoPreAnulado(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ContratoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void CompararNegocioReconfirmadoTest()
        {
            contratoManagerMock.Setup(x => x.CompararNegocioReconfirmado(It.IsAny<int>())).Returns(new List<BasicoContrato>());
            var result = target.CompararNegocioReconfirmado(It.IsAny<int>());
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.CompararNegocioReconfirmado(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void ValidarCalidadesTest()
        {
            contratoManagerMock.Setup(x => x.DiferenciaEnCalidades(It.IsAny<int>())).Returns(true);
            var result = target.ValidarCalidades(It.IsAny<int>());
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.DiferenciaEnCalidades(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":true,\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void FechaFeriadosTest()
        {
            fijacionManagerMock.Setup(x => x.FechaFeriados()).Returns(new List<DateTime>() { new DateTime(2020, 01, 20) });
            var result = target.FechaFeriados();
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fijacionManagerMock.Verify(x => x.FechaFeriados(), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[\"\\/Date(1579489200000)\\/\"],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void UltimoDiaHabilTest()
        {
            fijacionManagerMock.Setup(x => x.UltimoDiaHabil()).Returns(new DateTime(2020, 01, 20));
            var result = target.UltimoDiaHabil();
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fijacionManagerMock.Verify(x => x.UltimoDiaHabil(), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":\"\\/Date(1579489200000)\\/\",\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }


        [Test]
        public void BorrarContratoPreAprobacionTest()
        {
            var contrato = new Contrato { ComercialId = 1, MaterialId = 1, CampanaId = 1, ComercialCreadorId = 1 };
            contratoManagerMock.Setup(x => x.BorrarContratoPreAprobacion(It.IsAny<int>(), It.IsAny<string>())).Returns(new GrabarContratoResult { ContratoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.BorrarContratoPreAprobacion(1, "ok");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.BorrarContratoPreAprobacion(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ContratoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void AprobarContratoTest()
        {
            var contrato = new Contrato { ComercialId = 1, MaterialId = 1, CampanaId = 1, ComercialCreadorId = 1 };
            contratoManagerMock.Setup(x => x.AprobarContrato(It.IsAny<int>())).Returns(new GrabarContratoResult { ContratoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.AprobarContrato(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.AprobarContrato(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ContratoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void ValidarCreditoTest()
        {

            contratoManagerMock.Setup(x => x.ValidarCredito(It.IsAny<string>(), It.IsAny<double>(), It.IsAny<decimal>(), It.IsAny<string>())).Returns("");
            var result = target.ValidarCredito("", 1, 1, "");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.ValidarCredito(It.IsAny<string>(), It.IsAny<double>(), It.IsAny<decimal>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":\"\",\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void ListarCartasDePortePendienteAplicarTest()
        {

            contratoManagerMock.Setup(x => x.ListarCartasDePortePendienteAplicar(It.IsAny<CcPpPerndienteAplicarDto>())).Returns(new List<CcPpPerndienteAplicarDto>());
            var result = target.ListarCartasDePortePendienteAplicar(It.IsAny<CcPpPerndienteAplicarDto>());
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            contratoManagerMock.Verify(x => x.ListarCartasDePortePendienteAplicar(It.IsAny<CcPpPerndienteAplicarDto>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void TraerPagosDiferidoTest()
        {

            configuracionInternaMock.Setup(x => x.TraerPagosDiferido()).Returns(new List<HabilitacionPagoDiferidoDto>());
            var result = target.TraerPagosDiferido(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            configuracionInternaMock.Verify(x => x.TraerPagosDiferido(), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Id\":0,\"MaterialId\":0,\"Material\":null,\"TipoNegocioId\":0,\"TipoNegocio\":null,\"CantidadDia\":0,\"Tasa\":0,\"DesdeVigencia\":\"\\/Date(-62135586000000)\\/\",\"HastaVigencia\":\"\\/Date(-62135586000000)\\/\",\"Habilitado\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        //[Test]
        //public void ObtenerAcuerdoParaFasonTest()
        //{

        //    fijacionManagerMock.Setup(x => x.TraerDatosFijacion(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
        //        .Returns(new List<DatosFijacionDeContratoDto>());
        //    var result = target.ObtenerAcuerdoParaFason(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), false);
        //    Assert.NotNull(result);
        //    var a = serializer.Serialize(result);

        //    fijacionManagerMock.Verify(x => x.TraerDatosFijacion(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        //    Assert.AreEqual(
        //        "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
        //        a);
        //}
        [Test]
        public void ObtenerCapacidadProductivaPendienteTest()
        {

            contratoManagerMock.Setup(x => x.ObtenerCapacidadProductivaPendiente(It.IsAny<int>())).Returns(new List<CapacidadProductivaPendienteDto> { new CapacidadProductivaPendienteDto { MATERIAL = "Soja" } });
            var result = target.ObtenerCapacidadProductivaPendiente(1) as JsonResult;
            var model = serializer.Deserialize<List<CapacidadProductivaPendienteDto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(1, model.Count());
            Assert.AreEqual("Soja", model.First().MATERIAL);
            contratoManagerMock.Verify(x => x.ObtenerCapacidadProductivaPendiente(It.IsAny<int>()), Times.Once);
            Assert.NotNull(result);

        }
        [Test]
        public void AnularFijacionVirtualTest()
        {

            fijacionManagerMock.Setup(x => x.AnularFijacionVirtual(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(new GrabarFijacionResult());
            var result = target.AnularFijacionVirtual(It.IsAny<int>());
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fijacionManagerMock.Verify(x => x.AnularFijacionVirtual(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"FijacionDePrecioContratoId\":null,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void PreAnularFijacionVirtualTest()
        {
            fijacionManagerMock.Setup(x => x.PreAnularFijacion(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(new GrabarFijacionResult());
            var result = target.PreAnularFijacionVirtual(It.IsAny<int>(), It.IsAny<string>());
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fijacionManagerMock.Verify(x => x.PreAnularFijacion(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"FijacionDePrecioContratoId\":null,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void RechazarPreAnularFijacionVirtualTest()
        {
            fijacionManagerMock.Setup(x => x.RechazarPreAnularFijacionVirtual(It.IsAny<int>()))
                .Returns(new GrabarFijacionResult());
            var result = target.RechazarPreAnularFijacionVirtual(It.IsAny<int>());
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fijacionManagerMock.Verify(x => x.RechazarPreAnularFijacionVirtual(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"FijacionDePrecioContratoId\":null,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void DevolverKilosPendientesAnularFijacionCanjeTest()
        {
            fijacionManagerMock.Setup(x => x.DevolverKilosPendientesAnularFijacionCanje(It.IsAny<int>()))
                .Returns(new ResultadoDevolverKilosPendientesAnularFijacionCanjeDto());
            var result = target.DevolverKilosPendientesAnularFijacionCanje(It.IsAny<int>());
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fijacionManagerMock.Verify(x => x.DevolverKilosPendientesAnularFijacionCanje(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"KilosPendientes\":0,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void ValidacionesAnulaYReemplazaTest()
        {

            contratoManagerMock.Setup(x => x.ValidacionesAnulaYReemplaza(It.IsAny<string>())).Returns(new Resultado());
            var result = target.ValidacionesAnulaYReemplaza("") as JsonResult;
            contratoManagerMock.Verify(x => x.ValidacionesAnulaYReemplaza(It.IsAny<string>()), Times.Once);
            Assert.NotNull(result);
        }

        [Test]
        public void DevolverContratosParaAsociarTest()
        {

            contratoManagerMock.Setup(x => x.DevolverContratosParaAsociar(It.IsAny<int>(), It.IsAny<string>())).Returns(new List<NegocioAsociadoDto>());
            var result = target.DevolverContratosParaAsociar(1,"") as JsonResult;
            contratoManagerMock.Verify(x => x.DevolverContratosParaAsociar(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            Assert.NotNull(result);
        }

        [Test]
        public void DevolverContratoAsociadosPaseTest()
        {

            contratoManagerMock.Setup(x => x.DevolverContratoAsociadosPase(It.IsAny<int>())).Returns(new List<NegocioAsociadoDto>());
            var result = target.DevolverContratoAsociadosPase(1) as JsonResult;
            contratoManagerMock.Verify(x => x.DevolverContratoAsociadosPase(It.IsAny<int>()), Times.Once);
            Assert.NotNull(result);
        }

        [Test]
        public void ActualizarNegociosAsociadosTest()
        {

            contratoManagerMock.Setup(x => x.GrabarNegociosAsociados(It.IsAny<List<NegocioAsociadoDto>>(), It.IsAny<int>(), It.IsAny<decimal>())).Returns(new Resultado());
            var result = target.ActualizarNegociosAsociados(null, 1, 1) as JsonResult;
            contratoManagerMock.Verify(x => x.GrabarNegociosAsociados(It.IsAny<List<NegocioAsociadoDto>>(), It.IsAny<int>(), It.IsAny<decimal>()), Times.Once);
            Assert.NotNull(result);
        }

        [Test]
        public void EsUnContratoAsociadoTest()
        {

            contratoManagerMock.Setup(x => x.EsUnContratoAsociado(It.IsAny<int>())).Returns(true);
            var result = target.EsUnContratoAsociado(1) as JsonResult;
            contratoManagerMock.Verify(x => x.EsUnContratoAsociado(It.IsAny<int>()), Times.Once);
            Assert.NotNull(result);
        }

        [Test]
        public void ValidarProveedorSISATest()
        {

            contratoManagerMock.Setup(x => x.ValidarProveedor(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns("");
            var result = target.ValidarProveedorSISA(1, 1) as JsonResult;
            contratoManagerMock.Verify(x => x.ValidarProveedor(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
            Assert.NotNull(result);
        }

        [Test]
        public void EstaConfirmadoEnSAPTest()
        {

            contratoManagerMock.Setup(x => x.EstaConfirmadoEnSAP(It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            var result = target.EstaConfirmadoEnSAP("", 1) as JsonResult;
            contratoManagerMock.Verify(x => x.EstaConfirmadoEnSAP(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            Assert.NotNull(result);
        }

        [Test]
        public void ValidarCopiarContratoTest()
        {

            contratoManagerMock.Setup(x => x.ValidarCopiarContrato(It.IsAny<int>())).Returns(false);
            var result = target.ValidarCopiarContrato(1) as JsonResult;
            contratoManagerMock.Verify(x => x.ValidarCopiarContrato( It.IsAny<int>()), Times.Once);
            Assert.NotNull(result);
        }

        [Test]
        public void ValidarComisionEnCentroTest()
        {

            centroManagerMock.Setup(x => x.TraerCentro(It.IsAny<int>())).Returns(new CentroDto());
            var result = target.ValidarComisionEnCentro(1) as JsonResult;
            centroManagerMock.Verify(x => x.TraerCentro(It.IsAny<int>()), Times.Once);
            Assert.NotNull(result);
        }

        [Test]
        public void Tiene2doCondicionalAsociadoTest()
        {

            contratoManagerMock.Setup(x => x.Tiene2doCondicionalAsociado(It.IsAny<int>())).Returns(true);
            var result = target.EsUnContratoAsociado(1) as JsonResult;
            contratoManagerMock.Verify(x => x.EsUnContratoAsociado(It.IsAny<int>()), Times.Once);
            Assert.NotNull(result);
        }
    }
}
