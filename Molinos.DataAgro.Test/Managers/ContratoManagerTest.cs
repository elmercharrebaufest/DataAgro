using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using Molinos.DataAgro.Test.Mock;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ContratoManagerTest
    {
        private ContratoManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<IComercialManager> comercialManagerMock;
        private Mock<IMaterialManager> materialManagerMock;
        private Mock<ITipoNegocioManager> tipoNegocioManagerMock;
        private Mock<ICampañaManager> oMSCampaniaManagerMock;
        private Mock<IProvinciaManager> provinciaManagerMock;
        private Mock<ILocalidadManager> localidadManagerMock;
        private Mock<IPushNotificationManager> pushNotificacionManagerMock;
        private Mock<IDiferencialManager> diferencialManagerMock;
        private Mock<IContratoAcuerdoManager> contratoAcuerdoManagerMock;
        private Mock<IProveedorManager> proveedorManagerMock;
        private Mock<IFinalizarContratoAgent> finalizarContratoAgentMock;
        private Mock<IDiasHabilesAgent> diasHabilesMock;
        private Mock<IRelacionCorredorProveedorAgent> relacionCorredorProveedorAgentMock;
        private Mock<IEliminarContratoAgent> eliminarContratoAgentMock;
        private Mock<IConfiguracionManager> configuracionManagermock;
        private Mock<IAltaTempranaAgent> altaTempranaAgentMock;
        private Mock<ICapacidadProductivaAgent> capacidadProductivaAgentMock;
        private Mock<IDiasHabilesAgent> diasHabilesAgentMock;
        private Mock<IModificarContratoAgent> modificarContratoAgentMock;
        private Mock<IMailManager> mailManagerMock;
        private JavaScriptSerializer serializer;
        private Mock<IStatusContratoAgent> status;
        private Mock<ILogDataAgroManager> logDataAgroManagerMock;
        private Mock<IValidarDocProcPagoAgent> validarPagoAgente;
        private Mock<IListaCBUProveedorAgent> cbuAgentMock;
        private Mock<IModificarFijacionAgent> modificarFijacionAgentMock;
        private Mock<ICartasDePortePendienteAplicarAgent> ccppPendienteAplicarAgentMock;
        private Mock<IHttpContextManager> contextoMock;
        private Mock<ITipoDeCambioAgent> tipoDeCambioAgentMock;
        private Mock<IValidacionCreditoAgent> validacionCreditoAgent;
        private Mock<ICapacidadProductivaDisponibleAgent> capacidadProductivaDisponibleAgent;
        private Mock<INegocioManager> negocioManagerMock;
        private Mock<IContratosParaFijacionAgent> contratosParaFijacionAgent;


        [SetUp]
        public void SetUp()
        {
            ConfigurationManager.AppSettings["CredentialUserName"] = "dataagro.baufest@gmail.com";
            ConfigurationManager.AppSettings["SmtpServerPort"] = "587";
            ConfigurationManager.AppSettings["SmtpServer"] = "smtp.gmail.com";
            ConfigurationManager.AppSettings["UseDefaultCredentials"] = "S";
            ConfigurationManager.AppSettings["EnableSSL"] = "S";
            ConfigurationManager.AppSettings["CredentialPassword"] = "Hola1234";
            this.serializer = new JavaScriptSerializer();
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            comercialManagerMock = new Mock<IComercialManager>();
            materialManagerMock = new Mock<IMaterialManager>();
            tipoNegocioManagerMock = new Mock<ITipoNegocioManager>();
            oMSCampaniaManagerMock = new Mock<ICampañaManager>();
            provinciaManagerMock = new Mock<IProvinciaManager>();
            localidadManagerMock = new Mock<ILocalidadManager>();
            pushNotificacionManagerMock = new Mock<IPushNotificationManager>();
            diferencialManagerMock = new Mock<IDiferencialManager>();
            contratoAcuerdoManagerMock = new Mock<IContratoAcuerdoManager>();
            proveedorManagerMock = new Mock<IProveedorManager>();
            finalizarContratoAgentMock = new Mock<IFinalizarContratoAgent>();
            diasHabilesMock = new Mock<IDiasHabilesAgent>();
            relacionCorredorProveedorAgentMock = new Mock<IRelacionCorredorProveedorAgent>();
            eliminarContratoAgentMock = new Mock<IEliminarContratoAgent>();
            configuracionManagermock = new Mock<IConfiguracionManager>();
            altaTempranaAgentMock = new Mock<IAltaTempranaAgent>();
            capacidadProductivaAgentMock = new Mock<ICapacidadProductivaAgent>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            diasHabilesAgentMock = new Mock<IDiasHabilesAgent>();
            modificarContratoAgentMock = new Mock<IModificarContratoAgent>();
            mailManagerMock = new Mock<IMailManager>();
            status = new Mock<IStatusContratoAgent>();
            logDataAgroManagerMock = new Mock<ILogDataAgroManager>();
            validarPagoAgente = new Mock<IValidarDocProcPagoAgent>();
            cbuAgentMock = new Mock<IListaCBUProveedorAgent>();
            modificarFijacionAgentMock = new Mock<IModificarFijacionAgent>();
            ccppPendienteAplicarAgentMock = new Mock<ICartasDePortePendienteAplicarAgent>();
            contextoMock = new Mock<IHttpContextManager>();
            tipoDeCambioAgentMock = new Mock<ITipoDeCambioAgent>();
            validacionCreditoAgent = new Mock<IValidacionCreditoAgent>();
            capacidadProductivaDisponibleAgent = new Mock<ICapacidadProductivaDisponibleAgent>();
            negocioManagerMock = new Mock<INegocioManager>();
            contratosParaFijacionAgent = new Mock<IContratosParaFijacionAgent>();



            target = new ContratoManager(logger.Object, repositorioMock.Object,
                materialManagerMock.Object, tipoNegocioManagerMock.Object,
                oMSCampaniaManagerMock.Object, provinciaManagerMock.Object,
                localidadManagerMock.Object, proveedorManagerMock.Object,
                comercialManagerMock.Object, pushNotificacionManagerMock.Object,
                diferencialManagerMock.Object, contratoAcuerdoManagerMock.Object,
                finalizarContratoAgentMock.Object,
                diasHabilesMock.Object,
                relacionCorredorProveedorAgentMock.Object,
                eliminarContratoAgentMock.Object,
                configuracionManagermock.Object,
                capacidadProductivaAgentMock.Object,
                altaTempranaAgentMock.Object,
                diasHabilesAgentMock.Object, modificarContratoAgentMock.Object,
                mailManagerMock.Object, status.Object, logDataAgroManagerMock.Object,
                validarPagoAgente.Object, cbuAgentMock.Object,
                modificarFijacionAgentMock.Object, ccppPendienteAplicarAgentMock.Object,
                contextoMock.Object, validacionCreditoAgent.Object, tipoDeCambioAgentMock.Object,
                capacidadProductivaDisponibleAgent.Object, negocioManagerMock.Object,
                contratosParaFijacionAgent.Object);
        }

        [Test]
        public void TraerDatosComboOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Provincia, ProvinciaQry>>>(), It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<ProvinciaQry>() { new ProvinciaQry { Provinciaid = 1, Nombre = "Buenos Aires", Orden = 1 } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Campaña, CampañaQry>>>(), It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
              .Returns(new List<CampañaQry>() { new CampañaQry { CampañaId = 1, Descripcion = "17-19" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, MaterialQry>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<MaterialQry>() { new MaterialQry { MaterialId = 1, Descripcion = "TRIGO" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Moneda, MonedaQry>>>(), It.IsAny<Expression<Func<Moneda, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<MonedaQry>() { new MonedaQry { MonedaId = "AUS ", Descripcion = "AUSTRAL" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, ComercialQry>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<ComercialQry>() { new ComercialQry { ComercialId = 1, Comercial = "MARCO ANTONIO" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TipoNegocio, TipoNegocioQry>>>(), It.IsAny<Expression<Func<TipoNegocio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<TipoNegocioQry>() { new TipoNegocioQry { TipoNegocioId = 1, Descripcion = "A PRECIO" }, new TipoNegocioQry { TipoNegocioId = 4, Descripcion = "FASON" }, new TipoNegocioQry { TipoNegocioId = 5, Descripcion = "AGENTE" }, new TipoNegocioQry { TipoNegocioId = 6, Descripcion = "ACUERDO" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ClasificacionCompraNet, ClasificacionCompraNetQry>>>(), It.IsAny<Expression<Func<ClasificacionCompraNet, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<ClasificacionCompraNetQry>() { new ClasificacionCompraNetQry { Id = 1, Descripcion = "PRODUCTOR" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<BolsaCompraNet, BolsaCompraNetQry>>>(), It.IsAny<Expression<Func<BolsaCompraNet, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
              .Returns(new List<BolsaCompraNetQry>() { new BolsaCompraNetQry { Id = 1, Descripcion = "ROSARIO" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Centro, CentroQry>>>(), It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
             .Returns(new List<CentroQry>() { new CentroQry { Id = 1, Descripcion = "S. Lorenzo" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CondicionFijacion, CondicionFijacionQry>>>(), It.IsAny<Expression<Func<CondicionFijacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
            .Returns(new List<CondicionFijacionQry>() { new CondicionFijacionQry { Id = 1, Descripcion = "HASTA QUE DIGA YA" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<StandardDeCalidad, StandardDeCalidadQry>>>(), It.IsAny<Expression<Func<StandardDeCalidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
           .Returns(new List<StandardDeCalidadQry>() { new StandardDeCalidadQry { Id = 1, Descripcion = "CAMARA" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TipoDB, TipoDBQry>>>(), It.IsAny<Expression<Func<TipoDB, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
           .Returns(new List<TipoDBQry>() { new TipoDBQry { Id = 1, Descripcion = "CAMARA" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TipoPeriodoDB, TipoPeriodoDBQry>>>(), It.IsAny<Expression<Func<TipoPeriodoDB, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
           .Returns(new List<TipoPeriodoDBQry>() { new TipoPeriodoDBQry { Id = 1, Descripcion = "CAMARA" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TipoFason, TipoFasonQry>>>(), It.IsAny<Expression<Func<TipoFason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
          .Returns(new List<TipoFasonQry>() { new TipoFasonQry { Id = 1, Descripcion = "RAS" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TipoAgenteCompra, TipoAgenteCompraQry>>>(), It.IsAny<Expression<Func<TipoAgenteCompra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
        .Returns(new List<TipoAgenteCompraQry>() { new TipoAgenteCompraQry { Id = 1, Descripcion = "ZAR" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Operador, OperadorQry>>>(), It.IsAny<Expression<Func<Operador, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
        .Returns(new List<OperadorQry>() { new OperadorQry { Id = 1, Descripcion = "OP SD" } });


            var result = target.TraerDatosCombo();

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Provincia, ProvinciaQry>>>(), It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Campaña, CampañaQry>>>(), It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Material, MaterialQry>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Moneda, MonedaQry>>>(), It.IsAny<Expression<Func<Moneda, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Exactly(3));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Comercial, ComercialQry>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<TipoNegocio, TipoNegocioQry>>>(), It.IsAny<Expression<Func<TipoNegocio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ClasificacionCompraNet, ClasificacionCompraNetQry>>>(), It.IsAny<Expression<Func<ClasificacionCompraNet, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<BolsaCompraNet, BolsaCompraNetQry>>>(), It.IsAny<Expression<Func<BolsaCompraNet, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Centro, CentroQry>>>(), It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CondicionFijacion, CondicionFijacionQry>>>(), It.IsAny<Expression<Func<CondicionFijacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<StandardDeCalidad, StandardDeCalidadQry>>>(), It.IsAny<Expression<Func<StandardDeCalidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<TipoDB, TipoDBQry>>>(), It.IsAny<Expression<Func<TipoDB, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<TipoPeriodoDB, TipoPeriodoDBQry>>>(), It.IsAny<Expression<Func<TipoPeriodoDB, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<TipoFason, TipoFasonQry>>>(), It.IsAny<Expression<Func<TipoFason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<TipoAgenteCompra, TipoAgenteCompraQry>>>(), It.IsAny<Expression<Func<TipoAgenteCompra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Operador, OperadorQry>>>(), It.IsAny<Expression<Func<Operador, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);

            Assert.NotNull(result);
        }

        [Test]
        public void TraerDatosComboExternoOk()
        {
            Thread.CurrentPrincipal = new TestPrincipal(new Claim[] {
            new Claim(ClaimTypes.Role, PermisosDataAgro.IngresoExterno.ToString()) });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Provincia, ProvinciaQry>>>(), It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<ProvinciaQry>() { new ProvinciaQry { Provinciaid = 1, Nombre = "Buenos Aires", Orden = 1 } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Campaña, CampañaQry>>>(), It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
              .Returns(new List<CampañaQry>() { new CampañaQry { CampañaId = 1, Descripcion = "17-19" } });


            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Moneda, MonedaQry>>>(), It.IsAny<Expression<Func<Moneda, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<MonedaQry>() { new MonedaQry { MonedaId = "AUS ", Descripcion = "AUSTRAL" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, ComercialQry>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<ComercialQry>() { new ComercialQry { ComercialId = 1, Comercial = "MARCO ANTONIO" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TipoNegocio, TipoNegocioQry>>>(), It.IsAny<Expression<Func<TipoNegocio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<TipoNegocioQry>() { new TipoNegocioQry { TipoNegocioId = 1, Descripcion = "A PRECIO" }, new TipoNegocioQry { TipoNegocioId = 4, Descripcion = "FASON" }, new TipoNegocioQry { TipoNegocioId = 5, Descripcion = "AGENTE" }, new TipoNegocioQry { TipoNegocioId = 6, Descripcion = "ACUERDO" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ClasificacionCompraNet, ClasificacionCompraNetQry>>>(), It.IsAny<Expression<Func<ClasificacionCompraNet, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<ClasificacionCompraNetQry>() { new ClasificacionCompraNetQry { Id = 1, Descripcion = "PRODUCTOR" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<BolsaCompraNet, BolsaCompraNetQry>>>(), It.IsAny<Expression<Func<BolsaCompraNet, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
              .Returns(new List<BolsaCompraNetQry>() { new BolsaCompraNetQry { Id = 1, Descripcion = "ROSARIO" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Centro, CentroQry>>>(), It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
             .Returns(new List<CentroQry>() { new CentroQry { Id = 1, Descripcion = "S. Lorenzo" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CondicionFijacion, CondicionFijacionQry>>>(), It.IsAny<Expression<Func<CondicionFijacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
            .Returns(new List<CondicionFijacionQry>() { new CondicionFijacionQry { Id = 1, Descripcion = "HASTA QUE DIGA YA" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<StandardDeCalidad, StandardDeCalidadQry>>>(), It.IsAny<Expression<Func<StandardDeCalidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
           .Returns(new List<StandardDeCalidadQry>() { new StandardDeCalidadQry { Id = 1, Descripcion = "CAMARA" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TipoDB, TipoDBQry>>>(), It.IsAny<Expression<Func<TipoDB, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
           .Returns(new List<TipoDBQry>() { new TipoDBQry { Id = 1, Descripcion = "CAMARA" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TipoPeriodoDB, TipoPeriodoDBQry>>>(), It.IsAny<Expression<Func<TipoPeriodoDB, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
           .Returns(new List<TipoPeriodoDBQry>() { new TipoPeriodoDBQry { Id = 1, Descripcion = "CAMARA" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TipoFason, TipoFasonQry>>>(), It.IsAny<Expression<Func<TipoFason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
          .Returns(new List<TipoFasonQry>() { new TipoFasonQry { Id = 1, Descripcion = "RAS" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TipoAgenteCompra, TipoAgenteCompraQry>>>(), It.IsAny<Expression<Func<TipoAgenteCompra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
        .Returns(new List<TipoAgenteCompraQry>() { new TipoAgenteCompraQry { Id = 1, Descripcion = "ZAR" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Operador, OperadorQry>>>(), It.IsAny<Expression<Func<Operador, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
        .Returns(new List<OperadorQry>() { new OperadorQry { Id = 1, Descripcion = "OP SD" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<HabilitacionPizarra, MaterialQry>>>(), It.IsAny<Expression<Func<HabilitacionPizarra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                 .Returns(new List<MaterialQry>() { new MaterialQry { MaterialId = 1, Descripcion = "OP SD" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PrecioMoa, MaterialQry>>>(), It.IsAny<Expression<Func<PrecioMoa, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
     .Returns(new List<MaterialQry>() { new MaterialQry { MaterialId = 1, Descripcion = "OP SD" } });

            var result = target.TraerDatosCombo();

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Provincia, ProvinciaQry>>>(), It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Campaña, CampañaQry>>>(), It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Moneda, MonedaQry>>>(), It.IsAny<Expression<Func<Moneda, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Exactly(3));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Comercial, ComercialQry>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<TipoNegocio, TipoNegocioQry>>>(), It.IsAny<Expression<Func<TipoNegocio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ClasificacionCompraNet, ClasificacionCompraNetQry>>>(), It.IsAny<Expression<Func<ClasificacionCompraNet, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<BolsaCompraNet, BolsaCompraNetQry>>>(), It.IsAny<Expression<Func<BolsaCompraNet, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Centro, CentroQry>>>(), It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CondicionFijacion, CondicionFijacionQry>>>(), It.IsAny<Expression<Func<CondicionFijacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<StandardDeCalidad, StandardDeCalidadQry>>>(), It.IsAny<Expression<Func<StandardDeCalidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<TipoDB, TipoDBQry>>>(), It.IsAny<Expression<Func<TipoDB, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<TipoPeriodoDB, TipoPeriodoDBQry>>>(), It.IsAny<Expression<Func<TipoPeriodoDB, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<TipoFason, TipoFasonQry>>>(), It.IsAny<Expression<Func<TipoFason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<TipoAgenteCompra, TipoAgenteCompraQry>>>(), It.IsAny<Expression<Func<TipoAgenteCompra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Operador, OperadorQry>>>(), It.IsAny<Expression<Func<Operador, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);

            Assert.NotNull(result);
        }


        [Test]
        public void GrabarContratoOk()
        {
            var oContrato = new Contrato()
            {
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                PrecioNeto = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Sustentable = false,
                PorcentajeDePago = 95,
                Calidad = new List<Calidad>(),
                AperturaPrecio = new List<AperturaPrecio>()
            };
            repositorioMock.Setup(y => y.Obtener<Proveedor, string>(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>())).Returns("30209034560");
            validacionCreditoAgent.Setup(x => x.ValidarCredito(It.IsAny<string>())).Returns(new ValidarCreditoDto() { Moneda = "ARP" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ProveedorEstado, bool>>>())).Returns(new ProveedorEstado { EstadoId = 1 });
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { CantidadDias = 10, ImporteSustentable = 10, CantidadMaxima = 1000 });
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Provincia, bool>>>())).Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Localidad, bool>>>())).Returns(true); capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI",
                Ruca = new Ruca
                {
                    Acopiador = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Otros = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Corredor = "SI"
                }
            });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(new Centro { ValidaRedespacho = false, Descripcion = "bandera", Acopio = false, CodigoSap = "1127", Id = 1 });

            var resultado = target.GrabarContrato(oContrato);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Contrato>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

        }

        [Test]
        public void GrabarContratoOkDiferencial()
        {
            var oContrato = new Contrato()
            {
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                PrecioNeto = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaOperacion = DateTime.Now.Date,
                FechaEntrega = DateTime.Now,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Sustentable = false,
                PorcentajeDePago = 95,
                Calidad = new List<Calidad>(),
                AperturaPrecio = new List<AperturaPrecio>()

            };
            repositorioMock.Setup(y => y.Obtener<Proveedor, string>(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>())).Returns("30209034560");
            validacionCreditoAgent.Setup(x => x.ValidarCredito(It.IsAny<string>())).Returns(new ValidarCreditoDto() { Moneda = "ARP" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ProveedorEstado, bool>>>())).Returns(new ProveedorEstado { EstadoId = 1 });
            repositorioMock.Setup(y => y.ObtenerMayor<RangoConfirmacionAutomatica, DateTime>(It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny<Expression<Func<RangoConfirmacionAutomatica, DateTime>>>()))
                    .Returns(new RangoConfirmacionAutomatica() { MaterialId = 1, MonedaId = "ARS ", PrecioMinimo = 1, PrecioMaximo = 2000 });
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Provincia, bool>>>())).Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Localidad, bool>>>())).Returns(true); diferencialManagerMock.Setup(y => y.ValidarComprasDiferencial(It.IsAny<int>())).Throws(new Exception("Error diferencial"));
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { CantidadDias = 10, ImporteSustentable = 10, CantidadMaxima = 1000 });
            capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI",
                Ruca = new Ruca
                {
                    Acopiador = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Otros = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Corredor = "SI"
                }
            });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(new Centro { ValidaRedespacho = false, Descripcion = "bandera", Acopio = false, CodigoSap = "1127", Id = 1 });

            var resultado = target.GrabarContrato(oContrato);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Contrato>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

        }

        [Test]
        public void GrabarContratoConfirmacionAutomaticaOk()
        {
            var oContrato = new Contrato()
            {
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                PrecioNeto = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = new DateTime(2019, 11, 11),
                FechaOperacion = new DateTime(2019, 11, 11),
                FechaDesde = new DateTime(2019, 11, 11),
                FechaHasta = new DateTime(2019, 12, 11),
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                NoInformaSio = true,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Sustentable = false,
                Calidad = new List<Calidad>(),
                Comercial = new Comercial { GrupoDeComprasId = 1 },
                PorcentajeDePago = 95,
                MotivoOperacionAnterior = "aaaaaa",
                AperturaPrecio = new List<AperturaPrecio>(),
                DescripcionOperacionAnterior = "aaaaaaa"
            };
            repositorioMock.Setup(y => y.Obtener<Proveedor, string>(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>())).Returns("30209034560");
            validacionCreditoAgent.Setup(x => x.ValidarCredito(It.IsAny<string>())).Returns(new ValidarCreditoDto() { Moneda = "ARP" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ProveedorEstado, bool>>>())).Returns(new ProveedorEstado { EstadoId = 1 });
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Provincia, bool>>>())).Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Localidad, bool>>>())).Returns(true);
            repositorioMock.Setup(y => y.ObtenerMayor(It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny<Expression<Func<RangoConfirmacionAutomatica, DateTime>>>()))
                    .Returns(new RangoConfirmacionAutomatica()
                    {
                        MaterialId = 1,
                        MonedaId = "ARS ",
                        PrecioMinimo = 1,
                        PrecioMaximo = 2000,
                        Cantidad = 1000,
                        DesdeAnio = 2019,
                        DesdeMes = 10,
                        FechaDesde = new DateTime(2018, 11, 11),
                        FechaHasta = new DateTime(2020, 11, 11),
                        HastaAnio = 2021,
                        HastaMes = 12,
                        ZonaId = 1
                    });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, double>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()));
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { CantidadDias = 10, ImporteSustentable = 10, CantidadMaxima = 1000 });
            capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI",
                Ruca = new Ruca
                {
                    Acopiador = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Otros = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Corredor = "SI"
                }
            });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(new Centro { ValidaRedespacho = false, Descripcion = "bandera", Acopio = false, CodigoSap = "1127", Id = 1 });

            var resultado = target.GrabarContrato(oContrato);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Contrato>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void UpdateContratoOk()
        {
            var oContrato = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                PrecioNeto = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = new DateTime(2020, 02, 23),
                FechaHasta = new DateTime(2021, 02, 23),
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                PagoDiferido = true,
                DiasPesificado = 10,
                Sustentable = false,
                PorcentajeDePago = 95,
                Descuentos = new List<DescuentoBonificacion>()
                {
                    new DescuentoBonificacion()
                    {
                        Id= 0
                    }
                },
                Calidad = new List<Calidad>()
                {
                    new Calidad()
                    {
                        Id= 0,
                    }
                },
                AperturaPrecio = new List<AperturaPrecio>()
                {
                    new AperturaPrecio { ConceptoAperturaPrecioId = (int)EnumConceptoApertura.Financiero, Importe =100 }
                }
            };
            var oContratoBase = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 3,
                Precio = 500,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = new DateTime(2020, 02, 23),
                FechaHasta = new DateTime(2021, 02, 23),
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Sustentable = false,
                EstadoId = (int)EnumEstadoContrato.Confirmado,
                Descuentos = new List<DescuentoBonificacion>(),
                Calidad = new List<Calidad>(),
                AperturaPrecio = new List<AperturaPrecio>(),
                PrecioPactado = new List<PrecioPactado>()
            };
            repositorioMock.Setup(y => y.Obtener<Proveedor, string>(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>())).Returns("30209034560");
            validacionCreditoAgent.Setup(x => x.ValidarCredito(It.IsAny<string>())).Returns(new ValidarCreditoDto() { Moneda = "ARP" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContratoBase);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ProveedorEstado, bool>>>())).Returns(new ProveedorEstado { EstadoId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<Expression<Func<Centro, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<Expression<Func<Campaña, string>>>())).Returns("20-21");
            repositorioMock.Setup(y => y.Obtener<Negocio>(It.IsAny<int>())).Returns(new Negocio { Fecha = DateTime.Now });
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Provincia, bool>>>())).Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Localidad, bool>>>())).Returns(true);
            capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI",
                Ruca = new Ruca
                {
                    Acopiador = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Otros = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Corredor = "SI"
                }
            });
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { CantidadDias = 10, ImporteSustentable = 10, CantidadMaxima = 1000 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(new Centro { ValidaRedespacho = false, Descripcion = "bandera", Acopio = false, CodigoSap = "1127", Id = 1 });

            var resultado = target.GrabarContrato(oContrato);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<DescuentoBonificacion>()), Times.Exactly(1));
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Calidad>()), Times.Exactly(1));
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

        }

        [Test]
        public void UpdateContratoError()
        {
            var oContrato = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 5,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 0,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaDesde = new DateTime(2020, 02, 23),
                FechaHasta = new DateTime(2021, 02, 23),
                MonedaSustentableId = "USDM ",
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = null,
                BoletoId = 3,
                StandardDeCalidadId = 2,
                ImporteSustentable = 10000,
                Consignatario = true,
                PlanCanje = true,
                PagoDiferido = true,
                ZonaId = 0,
                TipoAgenteCompraId = 1,
                CaratulaMAT = null,
                PrecioAjusteComision = null,
                MonedaAjusteComisionId = null,
                DolarizadoExpress = true,
                FechaDolarizado = DateTime.Now,

                Descuentos = new List<DescuentoBonificacion>()
                {
                    new DescuentoBonificacion()
                    {
                        Id= 0
                    }
                },
                AperturaPrecio = new List<AperturaPrecio>()
                {
                    new AperturaPrecio()
                    {
                        ConceptoAperturaPrecioId= (int)EnumConceptoApertura.Financiero,
                        Importe = 300,
                        Porcentaje = 200
                    }
                }
            };
            var oContratoBase = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 3,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaDesde = new DateTime(2020, 02, 23),
                FechaHasta = new DateTime(2021, 02, 23),
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 4,
                StandardDeCalidadId = 1,
                EstadoId = (int)EnumEstadoContrato.Finalizado,
                Descuentos = new List<DescuentoBonificacion>(),
                Calidad = new List<Calidad>(),
                AperturaPrecio = new List<AperturaPrecio>(),
                PrecioPactado = new List<PrecioPactado>(),
            };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668", EstadoId = 4 });

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContratoBase);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<Expression<Func<Centro, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<Expression<Func<Campaña, string>>>())).Returns("20-21");
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { CantidadDias = 10, ImporteSustentable = 10 });
            repositorioMock.Setup(y => y.Obtener<Negocio>(It.IsAny<int>())).Returns(new Negocio { Fecha = DateTime.Now });

            capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("error");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "NO",
                Carta = "NO",
                FechaActualizacion = "SI",
                Mensaje = "",
                Consignatario = "NO",
                Nosis = "NO",
                PlanCanje = "NO",
                Ruca = new Ruca
                {
                    Acopiador = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Otros = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Corredor = "SI"
                }
            });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(new Centro { ValidaRedespacho = false, Descripcion = "bandera", Acopio = false, CodigoSap = "1127", Id = 1 });

            var resultado = target.GrabarContrato(oContrato);
            Assert.That(resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

        }
        [Test]
        public void UpdateContratoErrores()
        {
            var oContrato = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaDesde = new DateTime(2020, 02, 23),
                FechaHasta = new DateTime(2021, 02, 23),
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Descuentos = new List<DescuentoBonificacion>()
                {
                    new DescuentoBonificacion()
                    {
                        Id= 0
                    }
                },
                Calidad = new List<Calidad>()
                {
                    new Calidad()
                    {
                        Id= 0,
                    }
                },
                AperturaPrecio = new List<AperturaPrecio>()
                {
                    new AperturaPrecio()
                    {
                        ConceptoAperturaPrecioId= (int)EnumConceptoApertura.Redespacho,
                        Importe = 300
                    }
                }
            };
            var oContratoBase = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaDesde = new DateTime(2020, 02, 23),
                FechaHasta = new DateTime(2021, 02, 23),
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                EstadoId = (int)EnumEstadoContrato.Finalizado,
                Descuentos = new List<DescuentoBonificacion>(),
                Calidad = new List<Calidad>(),
                AperturaPrecio = new List<AperturaPrecio>(),
                PrecioPactado = new List<PrecioPactado>()
            };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContratoBase);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<Expression<Func<Centro, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<Expression<Func<Campaña, string>>>())).Returns("20-21");
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { CantidadDias = 10, ImporteSustentable = 10 });
            repositorioMock.Setup(y => y.Obtener<Negocio>(It.IsAny<int>())).Returns(new Negocio { Fecha = DateTime.Now });

            capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI",
                Ruca = new Ruca
                {
                    Acopiador = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Otros = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Corredor = "SI"
                }
            });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(new Centro { ValidaRedespacho = false, Descripcion = "bandera", Acopio = false, CodigoSap = "1127", Id = 1 });

            var resultado = target.GrabarContrato(oContrato);
            Assert.That(resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

        }

        [Test]
        public void GrabarContratoErrorProveedor1()
        {
            var oContrato = new Contrato()
            {
                ProveedorId = 0,

            };
            var resultado = target.GrabarContrato(oContrato);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Contrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

        }
        [Test]
        public void GrabarContratoErrorProveedor2()
        {
            var oContrato = new Contrato()
            {
                ProveedorId = -1,

            };

            var resultado = target.GrabarContrato(oContrato);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Contrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

        }

        [Test]
        public void GrabarContratoError()
        {
            var oContrato = new Contrato()
            {
                ProveedorId = 1,
                ClasificacionId = 0,
                CorredorId = 2,
                MaterialId = 0,
                Cantidad = 0,
                ContratoMadre = "001010101010",
                Precio = 0,
                TipoNegocioId = 2,
                DestinoId = 0,
                LocalidadId = 0,
                ProvinciaId = null,
                CampanaId = 0,
                ComercialId = null,
                EstablecimientoPropio = true,
                BoletoId = null,
                StandardDeCalidadId = null,
                Sustentable = true,
                PorcentajeDePago = 95,
                Calidad = new List<Calidad>(),
                AperturaPrecio = new List<AperturaPrecio>()
                {
                    new AperturaPrecio{
                        ConceptoAperturaPrecioId = 3,
                        Porcentaje = 2,
                        Importe = 0
                    },
                    new AperturaPrecio{
                        ConceptoAperturaPrecioId = 1,
                        Importe = 0
                    }
                }
            };
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(new Centro { ValidaRedespacho = false, Descripcion = "bandera", Acopio = false, CodigoSap = "1127", Id = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668", RiesgoComercialSap = "a" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1, Material = new Material { Descripcion = "SOPA" }, Moneda = new Moneda { Descripcion = "PATACON" } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<FACACOP, bool>>>())).Returns(new FACACOP { CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, decimal>>>())).Returns(-3000);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Contrato>() { new Contrato { Cantidad = 10 } });
            contratoAcuerdoManagerMock.Setup(y => y.TraerAcuerdo(It.IsAny<int>())).Returns(new BasicoContrato { Cantidad = 1 });
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { CantidadDias = 10, ImporteSustentable = 10, CantidadAcuerdo = 39 });
            capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI",
                Ruca = new Ruca
                {
                    Acopiador = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Otros = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Corredor = "SI"
                }
            });

            var resultado = target.GrabarContrato(oContrato);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Contrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            //Assert.IsTrue(resultado.HayError);
            //Assert.AreEqual(29, resultado.ListaErrores.Count);
        }

        [Test]
        public void GrabarContratoErrorDos()
        {
            var oContrato = new Contrato()
            {
                ProveedorId = 1,
                ClasificacionId = 2,
                CorredorId = 2,
                MaterialId = 0,
                Cantidad = -1,
                ContratoMadre = "001010101010",
                Precio = 0,
                TipoNegocioId = 1,
                DestinoId = 0,
                LocalidadId = -1,
                ProvinciaId = null,
                CampanaId = 0,
                ComercialId = null,
                EstablecimientoPropio = true,
                BoletoId = 4,
                BolsaId = 0,
                StandardDeCalidadId = null,
                Calidad = new List<Calidad>(),
                AperturaPrecio = new List<AperturaPrecio>()
                {
                    new AperturaPrecio{
                        ConceptoAperturaPrecioId = 3,
                        Porcentaje = 2,
                        Importe = 0
                    }
                },
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now.AddDays(-1),
                DesdeFijacion = DateTime.Now,
                HastaFijacion = DateTime.Now.AddDays(-1)
            };

            ConfigurationManager.AppSettings["RiesgoComercialAltoSap"] = "a";
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(new Centro { ValidaRedespacho = false, Descripcion = "bandera", Acopio = false, CodigoSap = "1127", Id = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668", RiesgoComercialSap = "a" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 3, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1, Material = new Material { Descripcion = "SOPA" }, Moneda = new Moneda { Descripcion = "PATACON" } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<FACACOP, bool>>>())).Returns(new FACACOP { CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, decimal>>>())).Returns(-3000);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Contrato>() { new Contrato { Cantidad = 10 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<BoletoCompraNetProvincia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<BoletoCompraNetProvincia>() { new BoletoCompraNetProvincia { BoletoCompraNetId = 4, ProvinciaId = 1 } });
            contratoAcuerdoManagerMock.Setup(y => y.TraerAcuerdo(It.IsAny<int>())).Returns(new BasicoContrato { Cantidad = 1 });
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { CantidadDias = 10, ImporteSustentable = 10, CantidadAcuerdo = 20 });
            capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI",
                Ruca = new Ruca
                {
                    Acopiador = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Otros = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Corredor = "SI"
                }
            });
            var resultado = target.GrabarContrato(oContrato);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Contrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

        }


        [Test]
        public void ConfirmarContratoOk()
        {
            var oContrato = new Contrato()
            {
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                EstadoId = 1
            };

            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContrato);

            var resultado = target.ConfirmarContrato(It.IsAny<int>(), It.IsAny<int>());
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

        }


        [Test]
        public void ConfirmarContratoError()
        {
            var oContrato = new Contrato()
            {
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 1,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                EstadoId = 1
            };

            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContrato);
            diferencialManagerMock.Setup(y => y.ValidarComprasDiferencial(It.IsAny<int>())).Throws(new Exception("Error diferencial"));
            comercialManagerMock.Setup(y => y.CadenaComerciales(It.IsAny<int>())).Returns(new List<int>() { 1, 2 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SuscripcionComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<SuscripcionComercial>() { new SuscripcionComercial { Id = 1, ComercialId = 1, Key = "ala" } });

            var resultado = target.ConfirmarContrato(It.IsAny<int>(), It.IsAny<int>());
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

        }

        [Test]
        public void ConfirmarContratoErrorContratoNulo()
        {

            var resultado = target.ConfirmarContrato(It.IsAny<int>(), It.IsAny<int>());
            Assert.That(resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

        }



        [Test]
        public void GrabarAmpliacionContratoOk()
        {
            var oContrato = new Contrato()
            {
                Id = 1,
                Ampliaciones = 5
            };
            var oContratoSave = new Contrato()
            {
                ContratoMadre = "0010101010",
                Cantidad = 1,
                EstadoId = 2,
                CantidadCamiones = 1
            };

            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContratoSave);

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, double>>>())).Returns(3000);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Contrato>() { new Contrato { Cantidad = 10 } });

            var resultado = target.GrabarAmpliacionContrato(oContrato);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void GrabarAmpliacionContratoError()
        {
            var oContrato = new Contrato()
            {
                Id = 1,
                Ampliaciones = 5
            };
            var oContratoSave = new Contrato()
            {
                ContratoMadre = "0010101010",
                Cantidad = 1,
                EstadoId = 2,
                CantidadCamiones = 1
            };

            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContratoSave);

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, double>>>())).Returns(-3000);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Contrato>() { new Contrato { Cantidad = 10 } });

            var resultado = target.GrabarAmpliacionContrato(oContrato);
            Assert.That(resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void GrabarAmpliacionContratoErrorNoSePuedeAmpliar()
        {
            var oContrato = new Contrato()
            {
                Id = 1,
                Ampliaciones = 5
            };
            var oContratoSave = new Contrato()
            {
                ContratoMadre = null,
                Cantidad = 1,
                EstadoId = 1,
                CantidadCamiones = 1
            };

            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContratoSave);

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, double>>>())).Returns(-3000);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Contrato>() { new Contrato { Cantidad = 10 } });

            var resultado = target.GrabarAmpliacionContrato(oContrato);
            Assert.That(resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }


        [Test]
        public void FinalizarContratoSinSAp()
        {
            var oContrato = new Contrato()
            {
                ProveedorId = 1,
                Proveedor = new Proveedor { CUIT = "1234" },
                CorredorId = 2,
                Corredor = new Proveedor { CUIT = "2345" },
                ClasificacionId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                Fecha = DateTime.Now,
                FechaEntrega = DateTime.Now,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                EstadoId = (int)EnumEstadoContrato.Confirmado,
                Comercial = new Comercial { ComercialId = 1 }
            };
            var diasHabiles = new List<DateTime>();
            for (var i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); i++)
            {
                diasHabiles.Add(new DateTime(DateTime.Now.Year, DateTime.Now.Month, i));
            }
            for (var i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month); i++)
            {
                diasHabiles.Add(new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, i));
            }
            diasHabilesAgentMock.Setup(y => y.ObtenerDiasHabiles()).Returns(diasHabiles);

            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(oContrato);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<CorredorProveedor, bool>>>())).Returns(true);
            ConfigurationManager.AppSettings["ValorPruebaSap"] = "1";
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<AperturaPrecio>() { new AperturaPrecio { ConceptoAperturaPrecio = new ConceptoAperturaPrecio { CodigoSap = "FI", Descripcion = "FINANCIERO", Id = 1 } } });
            relacionCorredorProveedorAgentMock.Setup(y => y.ObtenerRelacionCorredorProveedor(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            comercialManagerMock.Setup(y => y.CadenaComerciales(It.IsAny<int>())).Returns(new List<int>());

            var resultado = target.FinalizarContrato(It.IsAny<int>(), It.IsAny<string>());
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
            Assert.That(!resultado.HayError);
        }

        [Test]
        public void FinalizarContratoSinSapNiApertura()
        {
            var oContrato = new Contrato()
            {
                ProveedorId = 1,
                Proveedor = new Proveedor { CUIT = "1234" },
                CorredorId = 2,
                Corredor = new Proveedor { CUIT = "2345" },
                ClasificacionId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                Fecha = DateTime.Now,
                FechaEntrega = DateTime.Now,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                EstadoId = (int)EnumEstadoContrato.Confirmado,
                Comercial = new Comercial { ComercialId = 1 }
            };
            var diasHabiles = new List<DateTime>();
            for (var i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); i++)
            {
                diasHabiles.Add(new DateTime(DateTime.Now.Year, DateTime.Now.Month, i));
            }
            for (var i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month); i++)
            {
                diasHabiles.Add(new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, i));
            }
            diasHabilesAgentMock.Setup(y => y.ObtenerDiasHabiles()).Returns(diasHabiles);
            relacionCorredorProveedorAgentMock.Setup(y => y.ObtenerRelacionCorredorProveedor(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(oContrato);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<CorredorProveedor, bool>>>())).Returns(true);
            ConfigurationManager.AppSettings["ValorPruebaSap"] = "1";
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ConceptoAperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<ConceptoAperturaPrecio>() { new ConceptoAperturaPrecio { CodigoSap = "FI", Descripcion = "FINANCIERO", Id = 1 } });
            comercialManagerMock.Setup(y => y.CadenaComerciales(It.IsAny<int>())).Returns(new List<int>() { 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SuscripcionComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<SuscripcionComercial>() { new SuscripcionComercial { Id = 1, ComercialId = 1, Key = "ala" } });

            var resultado = target.FinalizarContrato(It.IsAny<int>(), It.IsAny<string>());
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
            Assert.That(!resultado.HayError);
        }

        [Test]
        public void FinalizarContratoErrorEstado()
        {
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(new Contrato() { EstadoId = (int)EnumEstadoContrato.Finalizado });
            var resultado = target.FinalizarContrato(It.IsAny<int>(), It.IsAny<string>());
            Assert.That(resultado.HayError);
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(new Contrato() { EstadoId = (int)EnumEstadoContrato.Rechazado });
            resultado = target.FinalizarContrato(It.IsAny<int>(), It.IsAny<string>());
            Assert.That(resultado.HayError);
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(new Contrato() { EstadoId = (int)EnumEstadoContrato.Pendiente });
            resultado = target.FinalizarContrato(It.IsAny<int>(), It.IsAny<string>());
            Assert.That(resultado.HayError);

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(3));
        }

        [Test]
        public void FinalizarContratoError()
        {
            var oContrato = new Contrato()
            {
                ProveedorId = 1,
                Proveedor = new Proveedor { CUIT = "1234" },
                CorredorId = null,
                Corredor = new Proveedor { CUIT = "2345" },
                ClasificacionId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                EstadoId = (int)EnumEstadoContrato.Confirmado,
                Comercial = new Comercial { ComercialId = 1 },
                Fecha = DateTime.Now
            };
            var diasHabiles = new List<DateTime>();
            for (var i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); i++)
            {
                diasHabiles.Add(new DateTime(DateTime.Now.Year, DateTime.Now.Month, i));
            }
            for (var i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month); i++)
            {
                diasHabiles.Add(new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, i));
            }
            diasHabilesAgentMock.Setup(y => y.ObtenerDiasHabiles()).Returns(diasHabiles);

            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(oContrato);

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<DescuentoBonificacion, DescuentoBonificacionDto>>>(), It.IsAny<Expression<Func<DescuentoBonificacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
              .Throws(new Exception("Error generico"));

            var resultado = target.FinalizarContrato(It.IsAny<int>(), It.IsAny<string>());
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
            Assert.That(resultado.HayError);
        }




        [Test]
        public void BorrarContratoOk()
        {
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(new Contrato() { EstadoId = (int)EnumEstadoContrato.Pendiente, Comercial = new Comercial() { ComercialId = 1 }, MotivoRechazo = "test" });
            comercialManagerMock.Setup(y => y.CadenaComerciales(It.IsAny<int>())).Returns(new List<int>());
            var resultado = target.BorrarContrato(new Contrato() { Id = 1, EstadoId = (int)EnumEstadoContrato.Pendiente, MotivoRechazo = "test" });

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
        }

        [Test]
        public void BorrarContratoError()
        {

            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(new Contrato() { EstadoId = (int)EnumEstadoContrato.Finalizado });
            var resultado = target.BorrarContrato(new Contrato() { Id = 1, MotivoRechazo = "test" });

            Assert.That(resultado.HayErrores);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void BorrarContratoEstadoOk()
        {
            var contratoParaSerializar = new Contrato()
            {
                ProveedorId = 1,
                Proveedor = new Proveedor { CUIT = "1234" },
                CorredorId = null,
                Corredor = new Proveedor { CUIT = "2345" },
                ClasificacionId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                EstadoId = (int)EnumEstadoContrato.ReconfirmarFinalizado,
                Comercial = new Comercial { ComercialId = 1 },
                Fecha = DateTime.Now,
                Base = true,
                ImporteSustentable = 1,
                FechaDolarizado = DateTime.Now,
                DiasPesificado = 2,
                NoInformaSio = false,
                TrigoEspecial = false,
                UsuarioId = "",
                Ampliaciones = 1,
                Observacion = "",
                CantidadCamiones = 2,
                Consignatario = false,
                PlanCanje = true,
                CD = false,
                Warrant = false,
                CondicionFijacionId = 1,
                PagoDirectoVendedor = false,
                BolsaId = 1,
                MercsDeposito = true,
                ComercialCreadorId = 1,
                SelCargoMOA = false,
                SelCargoVendedor = true,
                Madre = false,
                ContratoMadre = "",
                PrecioNeto = 100,
                Pizarra = true,
                PagoDiferido = false,
                ZonaId = 1,
                Compensacion = false,
                Sustentable = false,
                FechaCierta = DateTime.Now,
                ContratoAcuerdoId = 1,
                DolarizadoExpress = false,
                Dolarizado = true,

                PrecioPactado = new List<PrecioPactado>()
                {
                    new PrecioPactado()
                    {
                        Id = 1,
                        ContratoId = 1,
                        FechaDesde = DateTime.Now,
                        FechaHasta = DateTime.Now,
                    }
                },
                Descuentos = new List<DescuentoBonificacion>()
                {
                    new DescuentoBonificacion()
                    {
                        Id = 1,
                        ContratoId = 1,
                        FechaDesde = DateTime.Now,
                        FechaHasta = DateTime.Now,
                        Importe = 1
                    }
                },
                Calidad = new List<Calidad>()
                {
                    new Calidad()
                    {
                        Id = 1,
                        CalidadEspecialId = 1,
                        NegocioId = 1,
                        PorcentajeDesde = 1,
                        PorcentajeHasta = 2
                    }
                },
                AperturaPrecio = new List<AperturaPrecio>()
                {
                    new AperturaPrecio()
                    {
                        Id = 1,
                        NegocioId = 1,
                    }
                },
            };
            var datos = JsonConvert.SerializeObject(contratoParaSerializar, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });
            var oContrato = new Contrato()
            {
                ProveedorId = 1,
                Proveedor = new Proveedor { CUIT = "1234" },
                CorredorId = null,
                Corredor = new Proveedor { CUIT = "2345" },
                ClasificacionId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                EstadoId = 7,
                Comercial = new Comercial { ComercialId = 1 },
                Fecha = DateTime.Now,
                Base = true,
                ImporteSustentable = 1,
                FechaDolarizado = DateTime.Now,
                Dolarizado = true,
                Ampliaciones = 1,
                NegocioHistorico = new List<NegocioHistorico>()
                {
                    new NegocioHistorico()
                    {
                    ComercialId = 1,
                    TipoNegocioId = 1,
                    Datos = datos
                  }
                },
                PrecioPactado = new List<PrecioPactado>()
                {
                    new PrecioPactado()
                    {
                        Id = 1,
                        ContratoId = 1,
                        FechaDesde = DateTime.Now,
                        FechaHasta = DateTime.Now,
                    }
                },
                Descuentos = new List<DescuentoBonificacion>()
                {
                    new DescuentoBonificacion()
                    {
                        Id = 1,
                        ContratoId = 1,
                        FechaDesde = DateTime.Now,
                        FechaHasta = DateTime.Now,
                        Importe = 1
                    }
                },
                Calidad = new List<Calidad>()
                {
                    new Calidad()
                    {
                        Id = 1,
                        CalidadEspecialId = 1,
                        NegocioId = 1,
                        PorcentajeDesde = 1,
                        PorcentajeHasta = 2
                    }
                },
                AperturaPrecio = new List<AperturaPrecio>()
                {
                    new AperturaPrecio()
                    {
                        Id = 1,
                        NegocioId = 1,
                    }
                },
                MotivoRechazo = "Test"
            };
            comercialManagerMock.Setup(y => y.CadenaComerciales(It.IsAny<int>())).Returns(new List<int>());
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContrato);
            var resultado = target.BorrarContrato(oContrato);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
        }
        [Test]
        public void BorrarContratoEstadoSinAmpliacionOk()
        {
            var contratoParaSerializar = new Contrato()
            {
                ProveedorId = 1,
                Proveedor = new Proveedor { CUIT = "1234" },
                CorredorId = null,
                Corredor = new Proveedor { CUIT = "2345" },
                ClasificacionId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                EstadoId = (int)EnumEstadoContrato.Reconfirmar,
                Comercial = new Comercial { ComercialId = 1 },
                Fecha = DateTime.Now,
                Base = true,
                ImporteSustentable = 1,
                FechaDolarizado = DateTime.Now,
                DiasPesificado = 2,
                NoInformaSio = false,
                TrigoEspecial = false,
                UsuarioId = "",
                Ampliaciones = 0,
                Observacion = "",
                CantidadCamiones = 2,
                Consignatario = false,
                PlanCanje = true,
                CD = false,
                Warrant = false,
                CondicionFijacionId = 1,
                PagoDirectoVendedor = false,
                BolsaId = 1,
                MercsDeposito = true,
                ComercialCreadorId = 1,
                SelCargoMOA = false,
                SelCargoVendedor = true,
                Madre = false,
                ContratoMadre = "",
                PrecioNeto = 100,
                Pizarra = true,
                PagoDiferido = false,
                ZonaId = 1,
                Compensacion = false,
                Sustentable = false,
                FechaCierta = DateTime.Now,
                ContratoAcuerdoId = 1,
                DolarizadoExpress = false,
                Dolarizado = true,

                PrecioPactado = new List<PrecioPactado>()
                {
                    new PrecioPactado()
                    {
                        Id = 1,
                        ContratoId = 1,
                        FechaDesde = DateTime.Now,
                        FechaHasta = DateTime.Now,
                    }
                },
                Descuentos = new List<DescuentoBonificacion>()
                {
                    new DescuentoBonificacion()
                    {
                        Id = 1,
                        ContratoId = 1,
                        FechaDesde = DateTime.Now,
                        FechaHasta = DateTime.Now,
                        Importe = 1
                    }
                },
                Calidad = new List<Calidad>()
                {
                    new Calidad()
                    {
                        Id = 1,
                        CalidadEspecialId = 1,
                        NegocioId = 1,
                        PorcentajeDesde = 1,
                        PorcentajeHasta = 2
                    }
                },
                AperturaPrecio = new List<AperturaPrecio>()
                {
                    new AperturaPrecio()
                    {
                        Id = 1,
                        NegocioId = 1,
                    }
                },
            };
            var datos = JsonConvert.SerializeObject(contratoParaSerializar, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });
            var oContrato = new Contrato()
            {
                ProveedorId = 1,
                Proveedor = new Proveedor { CUIT = "1234" },
                CorredorId = null,
                Corredor = new Proveedor { CUIT = "2345" },
                ClasificacionId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                EstadoId = 7,
                Comercial = new Comercial { ComercialId = 1 },
                Fecha = DateTime.Now,
                Base = true,
                ImporteSustentable = 1,
                FechaDolarizado = DateTime.Now,
                Dolarizado = true,
                Ampliaciones = 0,
                NegocioHistorico = new List<NegocioHistorico>()
                {
                    new NegocioHistorico()
                    {
                    ComercialId = 1,
                    TipoNegocioId = 1,
                    Datos = datos
                  }
                },
                PrecioPactado = new List<PrecioPactado>()
                {
                    new PrecioPactado()
                    {
                        Id = 1,
                        ContratoId = 1,
                        FechaDesde = DateTime.Now,
                        FechaHasta = DateTime.Now,
                    }
                },
                Descuentos = new List<DescuentoBonificacion>()
                {
                    new DescuentoBonificacion()
                    {
                        Id = 1,
                        ContratoId = 1,
                        FechaDesde = DateTime.Now,
                        FechaHasta = DateTime.Now,
                        Importe = 1
                    }
                },
                Calidad = new List<Calidad>()
                {
                    new Calidad()
                    {
                        Id = 1,
                        CalidadEspecialId = 1,
                        NegocioId = 1,
                        PorcentajeDesde = 1,
                        PorcentajeHasta = 2
                    }
                },
                AperturaPrecio = new List<AperturaPrecio>()
                {
                    new AperturaPrecio()
                    {
                        Id = 1,
                        NegocioId = 1,
                    }
                },
                MotivoRechazo = "Test"
            };
            comercialManagerMock.Setup(y => y.CadenaComerciales(It.IsAny<int>())).Returns(new List<int>());
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContrato);
            var resultado = target.BorrarContrato(oContrato);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
        }
        [Test]
        public void TraerDescuentosPorContratoOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<DescuentoBonificacion, DescuentoBonificacionDto>>>(), It.IsAny<Expression<Func<DescuentoBonificacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<DescuentoBonificacionDto>() { new DescuentoBonificacionDto { ContratoId=1,Id = 1, FechaDesde = DateTime.Now.ToString(), FechaHasta = DateTime.Now.ToString(),
                                                                Importe = 100, Porcentaje = 10, MonedaId = "ARS  ", TipoDBId = 1, TipoDBDesc = "111",
                                                                TipoPeriodoDBDesc = "222",TipoPeriodoDBId = 2} });

            var resultado = target.TraerDescuentosPorContrato(1);

            Assert.That(resultado.Count == 1);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void TraerCalidadesPorContratoOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Calidad, CalidadDto>>>(), It.IsAny<Expression<Func<Calidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<CalidadDto>() { new CalidadDto { ContratoId=1,Id = 1,CalidadEspecialDesc = "FABRICA",CalidadEspecialId = 2,
                                                                PorcentajeDesde = 0 , PorcentajeHasta =1 ,Valor = 20} });

            var resultado = target.TraerCalidadesPorContrato(1, 1);

            Assert.That(resultado.Count == 1);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void TraerAperturaDePrecioPorContratoOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, AperturaPrecioDto>>>(), It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
           .Returns(new List<AperturaPrecioDto>() { new AperturaPrecioDto { Id = 1,ConceptoAperturaPrecioId = 1, ConceptoAperturaPrecio = "FINANCIERO",contratoId = 2, Importe =200, Moneda = "ARS  ",MonedaId = "ARS  ",Porcentaje = 0
              } });


            var resultado = target.TraerAperturaDePrecioPorContrato(1);

            Assert.That(resultado.Count == 1);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void TraerContratoOk()
        {
            var ap = new List<AperturaPrecioDto>() { new AperturaPrecioDto { Id = 1,ConceptoAperturaPrecioId = 1, ConceptoAperturaPrecio = "FINANCIERO",contratoId = 2, Importe =200, Moneda = "ARS  ",MonedaId = "ARS  ",Porcentaje = 0
              } };
            var cal = new List<CalidadDto>() { new CalidadDto { ContratoId=1,Id = 1,CalidadEspecialDesc = "FABRICA",CalidadEspecialId = 2,
                                                                PorcentajeDesde = 0 , PorcentajeHasta =1 ,Valor = 20} };
            var desc = new List<DescuentoBonificacionDto>() { new DescuentoBonificacionDto { ContratoId=1,Id = 1, FechaDesde = DateTime.Now.ToString(), FechaHasta = DateTime.Now.ToString(),
                                                                Importe = 100, Porcentaje = 10, MonedaId = "ARS  ", TipoDBId = 1, TipoDBDesc = "111",
                                                                TipoPeriodoDBDesc = "222",TipoPeriodoDBId = 2} };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<DescuentoBonificacion, DescuentoBonificacionDto>>>(), It.IsAny<Expression<Func<DescuentoBonificacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<DescuentoBonificacionDto>() { new DescuentoBonificacionDto { ContratoId=1,Id = 1, FechaDesde = DateTime.Now.ToString(), FechaHasta = DateTime.Now.ToString(),
                                                                Importe = 100, Porcentaje = 10, MonedaId = "ARS  ", TipoDBId = 1, TipoDBDesc = "111",
                                                                TipoPeriodoDBDesc = "222",TipoPeriodoDBId = 2} });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Calidad, CalidadDto>>>(), It.IsAny<Expression<Func<Calidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
              .Returns(new List<CalidadDto>() { new CalidadDto { ContratoId=1,Id = 1,CalidadEspecialDesc = "FABRICA",CalidadEspecialId = 2,
                                                                PorcentajeDesde = 0 , PorcentajeHasta =1 ,Valor = 20} });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, AperturaPrecioDto>>>(), It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
              .Returns(new List<AperturaPrecioDto>() { new AperturaPrecioDto { Id = 1,ConceptoAperturaPrecioId = 1, ConceptoAperturaPrecio = "FINANCIERO",contratoId = 2, Importe =200, Moneda = "ARS  ",MonedaId = "ARS  ",Porcentaje = 0
              } });


            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, BasicoContrato>>>()))
                .Returns(new BasicoContrato
                {
                    ContratoId = 1,
                    ProveedorId = 1,
                    Proveedor = "HAA",
                    Corredor = "HEE",
                    ComercialId = 1,
                    FechaDesdeFormateado = "10101",
                    FechaHastaFormateado = "02020",
                    FechaFormateado = "0403030",
                    TipoNegocioId = 2,
                    MaterialId = 1,
                    Cantidad = 100,
                    Ampliaciones = 1,
                    Precio = 111,
                    MonedaId = "ARS  ",
                    CampanaId = 5,
                    ProvinciaId = 1,
                    Provincia = "BS AS",
                    LocalidadId = 2,
                    Localidad = "SAN MARTIN",
                    ContratoSAP = "010101",
                    Base = true,
                    Observacion = "HHAAA",
                    Estado = 2,
                    Importe_Sustentable = 0,
                    Moneda_Sustentable = "ARS  ",
                    Fecha_DolarizadoFormateado = "848484",
                    Dias_Pesificado = 3,
                    NoInformaSIO = true,
                    TrigoEspecial = false,
                    ClasificacionId = 4,
                    DestinoId = 2,
                    PlanCanje = false,
                    Consignatario = false,
                    CantidadCamiones = 0,
                    BoletoId = 3,
                    BolsaId = 3,
                    DesdeFijacionFormateado = "87454",
                    HastaFijacionFormateado = "7564",
                    CondicionFijacion = 2,
                    CD = false,
                    Warrant = false,
                    PagoDirectoVendedor = false,
                    EstablecimientoPropio = false,
                    MercsDeposito = false,
                    PorcentajeComision = 2,
                    ContratoCorredor = "33",
                    ContratoVendedor = "44",
                    SelCargoMOA = true,
                    SelCargoVendedor = true,
                    Madre = false,
                    ContratoMadre = "",
                    Pizarra = false,
                    StandardCalidadId = 2,
                    Calidades = cal,
                    Descuentos = desc,
                    AperturaPrecios = ap
                });

            var resultado = target.TraerContrato(1);

            Assert.That(resultado.AperturaPrecios.Count == 1);
            Assert.That(resultado.Calidades.Count == 1);
            Assert.That(resultado.Descuentos.Count == 1);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void EnviarMailPendienteOk()
        {
            var comercial = new Comercial
            {
                IdActiveDirectory = "bmelgarejo",
                Nombres = "Brisa",
                Apellido = "Melgarejo"
            };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Negocio, AvisoContratoDto>>>(), It.IsAny<Expression<Func<Negocio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
              .Returns(new List<AvisoContratoDto>() { new AvisoContratoDto {
                    ContratoId = 1,
                RazonSocial = "ALA",
                Cantidad = 2,
                Precio = 1,
                Moneda = "ARS  ",
                FechaDb = new DateTime(2019,2,1),
                ComercialCreadorAD = "pari",
                NombreApellido = "si eme"
              } });



            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, ComercialDto>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
             .Returns(new List<ComercialDto>() {
                 new ComercialDto {
                    ComercialId = 1,
                    IdActiveDirectory = "mparisi"
              },
                 new ComercialDto {
                    ComercialId = 1,
                    IdActiveDirectory = "dsanchez"
              }});
            mailManagerMock.Setup(y => y.GetEmailUserActiveDirectory(It.IsAny<string>())).Returns("mparisi@baufest.com");

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, string>>>()))
                .Returns("marcos ignacio");

            ConfigurationManager.AppSettings["CredentialUserName"] = "dataagro.baufest@gmail.com";
            ConfigurationManager.AppSettings["SmtpServerPort"] = "587";
            ConfigurationManager.AppSettings["SmtpServer"] = "smtp.gmail.com";
            ConfigurationManager.AppSettings["SmtpAnonimo"] = "N";
            ConfigurationManager.AppSettings["EnableSSL"] = "S";
            contextoMock.Setup(x => x.ObtenerPathLogoMail()).Returns(TestContext.CurrentContext.TestDirectory + "\\Util\\MolinosAgro.png");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, string>>>())).Returns("Brisa Melgarejo");
            //FALTA -> Preguntar a Ale
            target.EnviarMailPendiente();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Negocio, AvisoContratoDto>>>(), It.IsAny<Expression<Func<Negocio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);

        }

        [Test]
        public void EnviarMailPendienteError()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Negocio, AvisoContratoDto>>>(), It.IsAny<Expression<Func<Negocio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
              .Returns(new List<AvisoContratoDto>() { new AvisoContratoDto {
                    ContratoId = 1,
                RazonSocial = "ALA",
                Cantidad = 2,
                Precio = 1,
                Moneda = "ARS  ",
                FechaDb = new DateTime(2019,2,1),
                ComercialCreadorAD = "pari",
                NombreApellido = "si eme"
              } });


            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, ComercialDto>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
             .Returns(new List<ComercialDto>() {
                 new ComercialDto {
                    ComercialId = 1,
                    IdActiveDirectory = "mparisi"
              }});
            mailManagerMock.Setup(y => y.GetEmailUserActiveDirectory(It.IsAny<string>())).Throws(new Exception("Error GetEmailUserActiveDirectory"));

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Negocio, bool>>>(), It.IsAny<Expression<Func<Negocio, string>>>()))
                .Returns("marcos ignacio");

            ConfigurationManager.AppSettings["CredentialUserName"] = "dataagro.baufest@gmail.com";
            ConfigurationManager.AppSettings["SmtpServerPort"] = "587";
            ConfigurationManager.AppSettings["SmtpServer"] = "smtp.gmail.com";
            ConfigurationManager.AppSettings["SmtpAnonimo"] = "N";
            ConfigurationManager.AppSettings["EnableSSL"] = "S";
            contextoMock.Setup(x => x.ObtenerPathLogoMail()).Returns(TestContext.CurrentContext.TestDirectory + "\\Util\\MolinosAgro.png");

            //FALTA -> Preguntar a Ale
            target.EnviarMailPendiente();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Negocio, AvisoContratoDto>>>(), It.IsAny<Expression<Func<Negocio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);

        }

        [Test]
        public void FinalizacionAutomaticaOk()
        {
            var oContrato = new Contrato()
            {
                ProveedorId = 1,
                Proveedor = new Proveedor { CUIT = "1234" },
                CorredorId = 2,
                Corredor = new Proveedor { CUIT = "2345" },
                ClasificacionId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                EstadoId = (int)EnumEstadoContrato.Confirmado,
                Fecha = DateTime.Now
            };
            var diasHabiles = new List<DateTime>();
            for (var i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); i++)
            {
                diasHabiles.Add(new DateTime(DateTime.Now.Year, DateTime.Now.Month, i));
            }
            for (var i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month); i++)
            {
                diasHabiles.Add(new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, i));
            }
            diasHabilesAgentMock.Setup(y => y.ObtenerDiasHabiles()).Returns(diasHabiles);


            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(oContrato);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<CorredorProveedor, bool>>>())).Returns(true);
            ConfigurationManager.AppSettings["ValorPruebaSap"] = "1";
            relacionCorredorProveedorAgentMock.Setup(y => y.ObtenerRelacionCorredorProveedor(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<AperturaPrecio>() { new AperturaPrecio { ConceptoAperturaPrecio = new ConceptoAperturaPrecio { CodigoSap = "FI", Descripcion = "FINANCIERO", Id = 1 } } });

            repositorioMock.Setup(y => y.Listar<Contrato, int>(It.IsAny<Expression<Func<Contrato, int>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<int>() { 1 });

            target.FinalizacionAutomatica(It.IsAny<string>());
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
        }

        [Test]
        public void FinalizacionAutomaticaError()
        {
            var oContrato = new Contrato()
            {
                ProveedorId = 1,
                Proveedor = new Proveedor { CUIT = "1234" },
                CorredorId = 2,
                Corredor = new Proveedor { CUIT = "2345" },
                ClasificacionId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                EstadoId = (int)EnumEstadoContrato.Confirmado
            };
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<Expression<Func<Contrato, bool>>>())).Throws(new Exception("Error obtener contrato"));


            repositorioMock.Setup(y => y.Listar<Contrato, int>(It.IsAny<Expression<Func<Contrato, int>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<int>() { 1 });

            target.FinalizacionAutomatica(It.IsAny<string>());
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }


        [Test]
        public void BorradoAutomaticoOk()
        {
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(new Contrato() { EstadoId = (int)EnumEstadoContrato.Pendiente, Comercial = new Comercial() { ComercialId = 1 }, MotivoRechazo = "test" });
            comercialManagerMock.Setup(y => y.CadenaComerciales(It.IsAny<int>())).Returns(new List<int>());
            var oContrato = new Contrato() { Id = 1, EstadoId = (int)EnumEstadoContrato.Pendiente, MotivoRechazo = "test" };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<Contrato>() {
                    oContrato
                });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FechaFeriado, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<FechaFeriado>() { });
            diasHabilesMock.Setup(y => y.ObtenerDiasHabiles()).Returns(new List<DateTime>() { DateTime.Today.AddDays(-2) });

            target.BorradoAutomatico();
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
        }

        [Test]
        public void BorradoAutomaticoError()
        {
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Throws(new Exception("Error obtener contrato"));
            var oContrato = new Contrato() { Id = 1, EstadoId = (int)EnumEstadoContrato.Pendiente };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<Contrato>() {
                    oContrato
                });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FechaFeriado, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<FechaFeriado>() { });
            diasHabilesMock.Setup(y => y.ObtenerDiasHabiles()).Returns(new List<DateTime>() { DateTime.Today.AddDays(-2) });

            target.BorradoAutomatico();
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }


        [Test]
        public void TraerContratosPendientesOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Negocio, AvisoContratoDto>>>(), It.IsAny<Expression<Func<Negocio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<AvisoContratoDto>() { new AvisoContratoDto {
                    ContratoId = 1,
                RazonSocial = "ALA",
                Cantidad = 2,
                Precio = 1,
                Moneda = "ARS  ",
                FechaDb = new DateTime(2019,2,1),
                ComercialCreadorAD = "pari",
                NombreApellido = "si eme"
              },
               new AvisoContratoDto {
                    ContratoId = 1,
                RazonSocial = "MER",
                Cantidad = 2,
                Precio = 1,
                Moneda = "ARS  ",
                FechaDb = new DateTime(2019,2,1),
                ComercialCreadorAD = "palacios",
                NombreApellido = "claudio"
              }});

            var resultado = target.TraerContratosPendientes(It.IsAny<List<int>>());

            Assert.That(resultado.Count == 2);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void TraerDatosCompraNetOk()
        {
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosCompraNetDto>>>()))
               .Returns(new DatosCompraNetDto
               {
                   ProveedorId = 1,
                   BoletoCompraNetId = 1,
                   BolsaCompraNetId = 1,
                   ClasificacionCompraNetId = 1,
                   Consignatario = false,
                   LocalidadId = 1,
                   ProvinciaId = 1,
                   Localidad = "MORON",
                   Provincia = "BS AS"
               });

            var resultado = target.TraerDatosCompraNet(It.IsAny<int>());

            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void TraerContratoMadreOk()
        {
            repositorioMock.Setup(y => y.Obtener<Contrato, int>(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, int>>>())).Returns(1);

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<DescuentoBonificacion, DescuentoBonificacionDto>>>(), It.IsAny<Expression<Func<DescuentoBonificacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
              .Returns(new List<DescuentoBonificacionDto>() { new DescuentoBonificacionDto { ContratoId=1,Id = 1, FechaDesde = DateTime.Now.ToString(), FechaHasta = DateTime.Now.ToString(),
                                                                Importe = 100, Porcentaje = 10, MonedaId = "ARS  ", TipoDBId = 1, TipoDBDesc = "111",
                                                                TipoPeriodoDBDesc = "222",TipoPeriodoDBId = 2} });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Calidad, CalidadDto>>>(), It.IsAny<Expression<Func<Calidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
              .Returns(new List<CalidadDto>() { new CalidadDto { ContratoId=1,Id = 1,CalidadEspecialDesc = "FABRICA",CalidadEspecialId = 2,
                                                                PorcentajeDesde = 0 , PorcentajeHasta =1 ,Valor = 20} });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, AperturaPrecioDto>>>(), It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
              .Returns(new List<AperturaPrecioDto>() { new AperturaPrecioDto { Id = 1,ConceptoAperturaPrecioId = 1, ConceptoAperturaPrecio = "FINANCIERO",contratoId = 2, Importe =200, Moneda = "ARS  ",MonedaId = "ARS  ",Porcentaje = 0
              } });


            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, BasicoContrato>>>()))
                .Returns(new BasicoContrato
                {
                    ContratoId = 1,
                    ProveedorId = 1,
                    Proveedor = "HAA",
                    Corredor = "HEE",
                    ComercialId = 1,
                    FechaDesdeFormateado = "10101",
                    FechaHastaFormateado = "02020",
                    FechaFormateado = "0403030",
                    TipoNegocioId = 2,
                    MaterialId = 1,
                    Cantidad = 100,
                    Ampliaciones = 1,
                    Precio = 111,
                    MonedaId = "ARS  ",
                    CampanaId = 5,
                    ProvinciaId = 1,
                    Provincia = "BS AS",
                    LocalidadId = 2,
                    Localidad = "SAN MARTIN",
                    ContratoSAP = "010101",
                    Base = true,
                    Observacion = "HHAAA",
                    Estado = 2,
                    Importe_Sustentable = 0,
                    Moneda_Sustentable = "ARS  ",
                    Fecha_DolarizadoFormateado = "848484",
                    Dias_Pesificado = 3,
                    NoInformaSIO = true,
                    TrigoEspecial = false,
                    ClasificacionId = 4,
                    DestinoId = 2,
                    PlanCanje = false,
                    Consignatario = false,
                    CantidadCamiones = 0,
                    BoletoId = 3,
                    BolsaId = 3,
                    DesdeFijacionFormateado = "87454",
                    HastaFijacionFormateado = "7564",
                    CondicionFijacion = 2,
                    CD = false,
                    Warrant = false,
                    PagoDirectoVendedor = false,
                    EstablecimientoPropio = false,
                    MercsDeposito = false,
                    PorcentajeComision = 2,
                    ContratoCorredor = "33",
                    ContratoVendedor = "44",
                    SelCargoMOA = true,
                    SelCargoVendedor = true,
                    Madre = false,
                    ContratoMadre = "",
                    Pizarra = false,
                    StandardCalidadId = 2
                });


            var resultado = target.TraerContratoMadre("0020202020");

            Assert.IsNotNull(resultado);
            Assert.That(!resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }


        [Test]
        public void TraerContratoMadreError()
        {
            repositorioMock.Setup(y => y.Obtener<Contrato, int>(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, int>>>())).Returns(0);
            var resultado = target.TraerContratoMadre("0020202020");

            Assert.IsNotNull(resultado);
            Assert.That(resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void AnularContratoOk()
        {
            var contrato = new Contrato
            {
                Id = 1,
                EstadoId = (int)EnumEstadoContrato.Finalizado,
                MotivoRechazo = "test"
            };

            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(contrato);

            eliminarContratoAgentMock.Setup(y => y.Eliminar(It.IsAny<Contrato>())).Returns("OK");

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<DescuentoBonificacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<DescuentoBonificacion>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Calidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Calidad>());

            var resultado = target.AnularContrato(contrato, It.IsAny<string>());

            Assert.IsNotNull(resultado);
            Assert.That(!resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
        }

        [Test]
        public void AnularContratoError()
        {
            var contrato = new Contrato
            {
                Id = 1,
                EstadoId = (int)EnumEstadoContrato.Finalizado
            };

            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(contrato);
            eliminarContratoAgentMock.Setup(y => y.Eliminar(It.IsAny<Contrato>())).Returns("Error");

            var resultado = target.AnularContrato(contrato, It.IsAny<string>());

            Assert.IsNotNull(resultado);
            Assert.That(resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void AnularContratoErrorAlEliminar()
        {
            var contrato = new Contrato
            {
                Id = 5,
                EstadoId = 1,
                MotivoRechazo = "Test"
            };

            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(contrato);
            eliminarContratoAgentMock.Setup(y => y.Eliminar(It.IsAny<Contrato>())).Returns("Error SIO");
            repositorioMock.Setup(y => y.Listar<Comercial>(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                    .Returns(new List<Comercial> { new Comercial { ComercialId = 70, Apellido = "Parisi", Nombres = "Marcos", PerfilId = 4 } });

            var resultado = target.AnularContrato(contrato, It.IsAny<string>());

            Assert.IsNotNull(resultado);
            Assert.That(resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
        [Test]
        public void AnularContratoErrorSio()
        {
            var contrato = new Contrato
            {
                Id = 5,
                EstadoId = (int)EnumEstadoContrato.Finalizado,
                MotivoRechazo = "Test"
            };

            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(contrato);
            eliminarContratoAgentMock.Setup(y => y.Eliminar(It.IsAny<Contrato>())).Returns("Error SIO");
            repositorioMock.Setup(y => y.Listar<Comercial>(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                    .Returns(new List<Comercial> { new Comercial { ComercialId = 70, Apellido = "Parisi", Nombres = "Marcos", PerfilId = 4 } });

            var resultado = target.AnularContrato(contrato, It.IsAny<string>());

            Assert.IsNotNull(resultado);
            Assert.That(resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void AnularContratoErrorEstado()
        {
            var contrato = new Contrato
            {
                Id = 1,
                EstadoId = (int)EnumEstadoContrato.Pendiente
            };

            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(contrato);
            eliminarContratoAgentMock.Setup(y => y.Eliminar(It.IsAny<Contrato>())).Returns("Error");

            var resultado = target.AnularContrato(contrato, It.IsAny<string>());

            Assert.IsNotNull(resultado);
            Assert.That(resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void AnularContratoErrorGuardar()
        {
            var contrato = new Contrato
            {
                Id = 1,
                EstadoId = (int)EnumEstadoContrato.Finalizado
            };

            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(contrato);
            eliminarContratoAgentMock.Setup(y => y.Eliminar(It.IsAny<Contrato>())).Returns("Ok");
            repositorioMock.Setup(y => y.GuardarCambios()).Throws(new Exception("Error guardar cambios"));

            var resultado = target.AnularContrato(contrato, It.IsAny<string>());

            Assert.IsNotNull(resultado);
            Assert.That(resultado.HayError);
        }

        [Test]
        public void TraerContratoAcuerdoACopiarOk()
        {
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, BasicoContrato>>>()))
                 .Returns(new BasicoContrato
                 {
                     ContratoId = 1,
                     ProveedorId = 1,
                     Proveedor = "HAA",
                     Corredor = "HEE",
                     ComercialId = 1,
                     FechaDesdeFormateado = "10101",
                     FechaHastaFormateado = "02020",
                     FechaFormateado = "0403030",
                     TipoNegocioId = 2,
                     MaterialId = 1,
                     Cantidad = 100,
                     Ampliaciones = 1,
                     Precio = 111,
                     MonedaId = "ARS  ",
                     CampanaId = 5,
                     ProvinciaId = 1,
                     Provincia = "BS AS",
                     LocalidadId = 2,
                     Localidad = "SAN MARTIN",
                     ContratoSAP = "010101",
                     Base = true,
                     Observacion = "HHAAA",
                     Estado = 2,
                     Importe_Sustentable = 0,
                     Moneda_Sustentable = "ARS  ",
                     Fecha_DolarizadoFormateado = "848484",
                     Dias_Pesificado = 3,
                     NoInformaSIO = true,
                     TrigoEspecial = false,
                     ClasificacionId = 4,
                     DestinoId = 2,
                     PlanCanje = false,
                     Consignatario = false,
                     CantidadCamiones = 0,
                     BoletoId = 3,
                     BolsaId = 3,
                     DesdeFijacionFormateado = "87454",
                     HastaFijacionFormateado = "7564",
                     CondicionFijacion = 2,
                     CD = false,
                     Warrant = false,
                     PagoDirectoVendedor = false,
                     EstablecimientoPropio = false,
                     MercsDeposito = false,
                     PorcentajeComision = 2,
                     ContratoCorredor = "33",
                     ContratoVendedor = "44",
                     SelCargoMOA = true,
                     SelCargoVendedor = true,
                     Madre = false,
                     ContratoMadre = "",
                     Pizarra = false,
                     StandardCalidadId = 2
                 });

            var resultado = target.TraerContratoAcuerdoACopiar(1);

            Assert.IsNotNull(resultado);

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void TraerAperturaDePrecioPorContratoTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, AperturaPrecioDto>>>(), It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<AperturaPrecioDto>());
            var resultado = target.TraerAperturaDePrecioPorContrato(1);
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }
        [Test]
        public void TraerTodoLosEstadosTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<EstadoContrato, EstadoContratoDto>>>(), It.IsAny<Expression<Func<EstadoContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<EstadoContratoDto>());
            var resultado = target.TraerTodoLosEstados();
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }
        [Test]
        public void TraerTodosLosBoletosTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<BoletoCompraNet, BoletoCompraNetDto>>>(), It.IsAny<Expression<Func<BoletoCompraNet, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<BoletoCompraNetDto>());
            var resultado = target.TraerTodosLosBoletos();
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void TraerTodoGrupoDeComprasTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<GrupoDeCompras, GrupoDeComprasDto>>>(), It.IsAny<Expression<Func<GrupoDeCompras, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<GrupoDeComprasDto>());
            var resultado = target.TraerTodoGrupoDeCompras();
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }
        [Test]
        public void ValidarProveedorTest()
        {
            var proveedor = new Proveedor
            {
                ProveedorId = 1,
                CUIT = "1123422"
            };

            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>())).Returns(proveedor);
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto());
            var resultado = target.ValidarProveedor(It.IsAny<int>());
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void ActualizarContratoSAPConErrorTest()
        {
            var contrato = new Contrato
            {
                Id = 1,
                ProveedorId = 1,
                ComercialId = 1,
                TipoNegocioId = 1,
                DestinoId = 2,
                MaterialId = 1,
                Cantidad = 100,
                Ampliaciones = 1,
                Precio = 111,
                MonedaId = "ARS  ",
                CampanaId = 5,
                ProvinciaId = 1,
                LocalidadId = 2,
                ContratoSAP = "010101",
                Base = true,
                Observacion = "HHAAA",
                TrigoEspecial = false,
                ClasificacionId = 4,
                PlanCanje = false,
                Consignatario = false,
                CantidadCamiones = 0,
                BoletoId = 3,
                BolsaId = 3,
                CD = false,
                Warrant = false,
                PagoDirectoVendedor = false,
                EstablecimientoPropio = false,
                MercsDeposito = false,
                PorcentajeComision = 2,
                ContratoCorredor = "33",
                ContratoVendedor = "44",
                SelCargoMOA = true,
                SelCargoVendedor = true,
                Madre = false,
                ContratoMadre = "",
                Pizarra = true,
                TipoAgenteCompraId = 1,
                Dolarizado = true,
                FechaDolarizado = DateTime.Now,
                CaratulaMAT = null,
                EstadoId = 5,


                Descuentos = new List<DescuentoBonificacion>()
                {
                    new DescuentoBonificacion()
                    {
                        Id= 0
                    }
                },
                AperturaPrecio = new List<AperturaPrecio>()
                {
                    new AperturaPrecio()
                    {
                        ConceptoAperturaPrecioId= (int)EnumConceptoApertura.Financiero,
                        Importe = 300,
                        Porcentaje = 200
                    }
                }
            };
            var oContratoBase = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 3,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 4,
                StandardDeCalidadId = 1,
                EstadoId = (int)EnumEstadoContrato.Finalizado,
                Descuentos = new List<DescuentoBonificacion>(),
                Calidad = new List<Calidad>(),
                AperturaPrecio = new List<AperturaPrecio>(),
                PrecioPactado = new List<PrecioPactado>(),
            };
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668", EstadoId = 4 });
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContratoBase);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<Expression<Func<Centro, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(new Centro { ValidaRedespacho = false, Descripcion = "bandera", Acopio = false, CodigoSap = "1127", Id = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<Expression<Func<Campaña, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { CantidadDias = 10, ImporteSustentable = 10 });
            capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("error");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "NO",
                Carta = "NO",
                FechaActualizacion = "SI",
                Mensaje = "",
                Consignatario = "NO",
                Nosis = "NO",
                PlanCanje = "NO",
                Ruca = new Ruca
                {
                    Acopiador = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Otros = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Corredor = "SI"
                }
            });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(contrato);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Calidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Calidad>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<DescuentoBonificacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<DescuentoBonificacion>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<AperturaPrecio>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PrecioPactado, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<PrecioPactado>());

            var resultado = target.ActualizarContratoSAP(contrato, true);
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }
        [Test]
        public void ActualizarContratoSapOk()
        {
            var oContrato = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                PrecioNeto = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = new DateTime(2020, 02, 23),
                FechaHasta = new DateTime(2021, 02, 23),
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                PagoDiferido = true,
                DiasPesificado = 10,
                Sustentable = false,
                PorcentajeDePago = 95,
                Descuentos = new List<DescuentoBonificacion>()
                {
                    new DescuentoBonificacion()
                    {
                        Id= 0
                    }
                },
                Calidad = new List<Calidad>()
                {
                    new Calidad()
                    {
                        Id= 0,
                    }
                },
                AperturaPrecio = new List<AperturaPrecio>()
                {
                    new AperturaPrecio { ConceptoAperturaPrecioId = (int)EnumConceptoApertura.Financiero, Importe =100 }
                }
            };
            var oContratoBase = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 3,
                Precio = 500,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = new DateTime(2020, 02, 23),
                FechaHasta = new DateTime(2021, 02, 23),
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Sustentable = false,
                EstadoId = (int)EnumEstadoContrato.Confirmado,
                Descuentos = new List<DescuentoBonificacion>(),
                Calidad = new List<Calidad>(),
                AperturaPrecio = new List<AperturaPrecio>(),
                PrecioPactado = new List<PrecioPactado>()
            };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContratoBase);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ProveedorEstado, bool>>>())).Returns(new ProveedorEstado { EstadoId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<Expression<Func<Centro, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<Expression<Func<Campaña, string>>>())).Returns("20-21");
            repositorioMock.Setup(y => y.Obtener<Negocio>(It.IsAny<int>())).Returns(new Negocio { Fecha = DateTime.Now });
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Provincia, bool>>>())).Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Localidad, bool>>>())).Returns(true);

            capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI",
                Ruca = new Ruca
                {
                    Acopiador = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Otros = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Corredor = "SI"
                }
            });
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { CantidadDias = 10, ImporteSustentable = 10, CantidadMaxima = 1000 });

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(oContrato);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Calidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Calidad>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<DescuentoBonificacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<DescuentoBonificacion>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<AperturaPrecio>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PrecioPactado, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<PrecioPactado>());
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(new Centro { ValidaRedespacho = false, Descripcion = "bandera", Acopio = false, CodigoSap = "1127", Id = 1 });

            var resultado = target.ActualizarContratoSAP(oContrato, false);
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void PreanularContratoOk()
        {
            var oContratoBase = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 3,
                Precio = 500,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Sustentable = false,
                EstadoId = (int)EnumEstadoContrato.Finalizado,
                Descuentos = new List<DescuentoBonificacion>(),
                Calidad = new List<Calidad>(),
                AperturaPrecio = new List<AperturaPrecio>(),
                PrecioPactado = new List<PrecioPactado>(),
                ContratoSAP = "23422343"
            };
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContratoBase);
            status.Setup(x => x.ValidarEstado(oContratoBase.ContratoSAP)).Returns(new EstadoSAPDto { NumeroSio = 0, Status = "" });
            var resultado = target.PreAnularContrato(1, "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void PreanularContratoError()
        {
            var oContratoBase = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 3,
                Precio = 500,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Sustentable = false,
                EstadoId = (int)EnumEstadoContrato.Confirmado,
                Descuentos = new List<DescuentoBonificacion>(),
                Calidad = new List<Calidad>(),
                AperturaPrecio = new List<AperturaPrecio>(),
                PrecioPactado = new List<PrecioPactado>(),
                ContratoSAP = "23422343"
            };
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContratoBase);
            status.Setup(x => x.ValidarEstado(oContratoBase.ContratoSAP)).Returns(new EstadoSAPDto { NumeroSio = 0, Status = "OK" });
            var resultado = target.PreAnularContrato(1, "");
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void RechazarPreAnularContratoOk()
        {
            var oContratoBase = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 3,
                Precio = 500,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Sustentable = false,
                EstadoId = (int)EnumEstadoContrato.PreAnulado,
                Descuentos = new List<DescuentoBonificacion>(),
                Calidad = new List<Calidad>(),
                AperturaPrecio = new List<AperturaPrecio>(),
                PrecioPactado = new List<PrecioPactado>(),
                ContratoSAP = "23422343"
            };
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContratoBase);

            var resultado = target.RechazarPreAnularContrato(1/*, "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"*/);
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
        [Test]
        public void RechazarPreAnularContratoError()
        {
            var oContratoBase = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 3,
                Precio = 500,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Sustentable = false,
                EstadoId = (int)EnumEstadoContrato.Confirmado,
                Descuentos = new List<DescuentoBonificacion>(),
                Calidad = new List<Calidad>(),
                AperturaPrecio = new List<AperturaPrecio>(),
                PrecioPactado = new List<PrecioPactado>(),
                ContratoSAP = "23422343"
            };
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContratoBase);

            var resultado = target.RechazarPreAnularContrato(1/*, "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"*/);
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void AnularContratoPreAnuladoOk()
        {
            var oContratoBase = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 3,
                Precio = 500,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Sustentable = false,
                EstadoId = (int)EnumEstadoContrato.PreAnulado,
                Descuentos = new List<DescuentoBonificacion>(),
                Calidad = new List<Calidad>(),
                AperturaPrecio = new List<AperturaPrecio>(),
                PrecioPactado = new List<PrecioPactado>(),
                ContratoSAP = "23422343"
            };
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContratoBase);
            eliminarContratoAgentMock.Setup(x => x.Eliminar(oContratoBase)).Returns("");
            status.Setup(x => x.ValidarEstado(It.IsAny<string>())).Returns(new EstadoSAPDto { NumeroSio = 0, Status = "" });
            var resultado = target.AnularContratoPreAnulado(1, "");

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Calidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Calidad>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<DescuentoBonificacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<DescuentoBonificacion>());
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void AnularContratoPreAnuladoError()
        {
            var oContratoBase = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 3,
                Precio = 500,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Sustentable = false,
                EstadoId = (int)EnumEstadoContrato.PreAnulado,
                Descuentos = new List<DescuentoBonificacion>(),
                Calidad = new List<Calidad>(),
                AperturaPrecio = new List<AperturaPrecio>(),
                PrecioPactado = new List<PrecioPactado>(),
                ContratoSAP = "23422343",
                Proveedor = new Proveedor { RazonSocial = "Parisi", CUIT = "00023832" }
            };
            var comercial = new Comercial
            {
                IdActiveDirectory = "bmelgarejo",
                Nombres = "Brisa",
                Apellido = "Melgarejo"
            };
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContratoBase);
            status.Setup(x => x.ValidarEstado(It.IsAny<string>())).Returns(new EstadoSAPDto { NumeroSio = 0, Status = "OK" });
            eliminarContratoAgentMock.Setup(x => x.Eliminar(It.IsAny<Contrato>())).Returns("Error SIO");
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
                Returns(new List<Comercial>() { comercial });
            mailManagerMock.Setup(x => x.GetEmailUserActiveDirectory(It.IsAny<string>())).Returns("bmelgarejooo@baufest.com");
            contextoMock.Setup(x => x.ObtenerPathLogoMail()).Returns(TestContext.CurrentContext.TestDirectory + "\\Util\\MolinosAgro.png");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>())).Returns(comercial);

            var resultado = target.AnularContratoPreAnulado(1, "");

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Calidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Calidad>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<DescuentoBonificacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<DescuentoBonificacion>());
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void ActualizarContratoFinalizadoOk()
        {
            var oContrato = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                PrecioNeto = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = new DateTime(2020, 02, 23),
                FechaHasta = new DateTime(2021, 02, 23),
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                PagoDiferido = true,
                DiasPesificado = 10,
                Sustentable = false,
                PorcentajeDePago = 95,
                ContratoSAP = "23422343",
                EstadoId = (int)EnumEstadoContrato.Finalizado,                
                Descuentos = new List<DescuentoBonificacion>()
                {
                    new DescuentoBonificacion()
                    {
                        Id= 0
                    }
                },
                Calidad = new List<Calidad>()
                {
                    new Calidad()
                    {
                        Id= 0,
                    }
                }              
            };
            var oContratoBase = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 3,
                Precio = 500,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = new DateTime(2020, 02, 23),
                FechaHasta = new DateTime(2021, 02, 23),
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Sustentable = false,
                EstadoId = (int)EnumEstadoContrato.Finalizado,
                Descuentos = new List<DescuentoBonificacion>(),
                Calidad = new List<Calidad>(),
                AperturaPrecio = new List<AperturaPrecio>(),
                PrecioPactado = new List<PrecioPactado>(),

                ContratoSAP = "434343",
                ChequeElectronico = true
            };
            repositorioMock.Setup(y => y.Obtener<Proveedor, string>(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>())).Returns("30209034560");
            validacionCreditoAgent.Setup(x => x.ValidarCredito(It.IsAny<string>())).Returns(new ValidarCreditoDto() { Moneda = "ARP" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContratoBase);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ProveedorEstado, bool>>>())).Returns(new ProveedorEstado { EstadoId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<Expression<Func<Centro, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<Expression<Func<Campaña, string>>>())).Returns("20-21");
            repositorioMock.Setup(y => y.Obtener<Negocio>(It.IsAny<int>())).Returns(new Negocio { Fecha = DateTime.Now });
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Provincia, bool>>>())).Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Localidad, bool>>>())).Returns(true);
          
            capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI",
                Ruca = new Ruca
                {
                    Acopiador = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Otros = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Corredor = "SI"
                }
            });
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { CantidadDias = 10, ImporteSustentable = 10, CantidadMaxima = 1000 });
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(oContratoBase);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(new Centro { ValidaRedespacho = false, Descripcion = "bandera", Acopio = false, CodigoSap = "1127", Id = 1 });

            var resultado = target.ActualizarContratoFinalizado(oContrato);

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Calidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Calidad>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<DescuentoBonificacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<DescuentoBonificacion>());
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

        }

        [Test]
        public void ActualizarContratoFinalizadoSinCambioOk()
        {
            var oContrato = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                PrecioNeto = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = new DateTime(2020, 02, 23),
                FechaHasta = new DateTime(2021, 02, 23),
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                PagoDiferido = true,
                DiasPesificado = 10,
                Sustentable = false,
                PorcentajeDePago = 95,
                ContratoSAP = "23422343",
                EstadoId = (int)EnumEstadoContrato.Finalizado,
                Descuentos = new List<DescuentoBonificacion>()
                {
                    new DescuentoBonificacion()
                    {
                        Id= 0
                    }
                },
                Calidad = new List<Calidad>()
                {
                    new Calidad()
                    {
                        Id= 0,
                    }
                },
                AperturaPrecio = new List<AperturaPrecio>()
                {
                    new AperturaPrecio { ConceptoAperturaPrecioId = (int)EnumConceptoApertura.Financiero, Importe =100 }
                }
            };
            var oContratoBase = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = new DateTime(2020, 02, 23),
                FechaHasta = new DateTime(2021, 02, 23),
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Sustentable = false,
                EstadoId = (int)EnumEstadoContrato.Finalizado,
                Descuentos = new List<DescuentoBonificacion>(),
                AperturaPrecio = new List<AperturaPrecio>(),
                PrecioPactado = new List<PrecioPactado>(),
                ContratoSAP = "23422343",
                Calidad = new List<Calidad>()
                {
                    new Calidad()
                    {
                        Id= 0,
                    }
                },
            };
            repositorioMock.Setup(y => y.Obtener<Proveedor, string>(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>())).Returns("30209034560");
            validacionCreditoAgent.Setup(x => x.ValidarCredito(It.IsAny<string>())).Returns(new ValidarCreditoDto() { Moneda = "ARP" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContratoBase);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ProveedorEstado, bool>>>())).Returns(new ProveedorEstado { EstadoId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<Expression<Func<Centro, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<Expression<Func<Campaña, string>>>())).Returns("20-21");
            repositorioMock.Setup(y => y.Obtener<Negocio>(It.IsAny<int>())).Returns(new Negocio { Fecha = DateTime.Now });
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Provincia, bool>>>())).Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Localidad, bool>>>())).Returns(true);

            capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI",
                Ruca = new Ruca
                {
                    Acopiador = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Otros = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Corredor = "SI"
                }
            });
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { CantidadDias = 10, ImporteSustentable = 10, CantidadMaxima = 1000 });
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(oContratoBase);
            modificarContratoAgentMock.Setup(x => x.Modificar(oContrato, oContratoBase)).Returns("Ok");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(new Centro { ValidaRedespacho = false, Descripcion = "bandera", Acopio = false, CodigoSap = "1127", Id = 1 });

            var resultado = target.ActualizarContratoFinalizado(oContrato);

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Calidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Calidad>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<DescuentoBonificacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<DescuentoBonificacion>());
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

        }


        [Test]
        public void ActualizarContratoFinalizadoSinCambioError()
        {
            var oContrato = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                PagoDiferido = true,
                DiasPesificado = 10,
                Sustentable = false,
                PorcentajeDePago = 95,
                ContratoSAP = "23422343",
                EstadoId = (int)EnumEstadoContrato.Rechazado,
                Descuentos = new List<DescuentoBonificacion>()
                {
                    new DescuentoBonificacion()
                    {
                        Id= 0
                    }
                },
                Calidad = new List<Calidad>()
                {
                    new Calidad()
                    {
                        Id= 0,
                    }
                },
                AperturaPrecio = new List<AperturaPrecio>()
                {
                    new AperturaPrecio()
                    {
                        ConceptoAperturaPrecioId= (int)EnumConceptoApertura.Redespacho,
                        Importe = 300
                    },
                    new AperturaPrecio { ConceptoAperturaPrecioId = (int)EnumConceptoApertura.Financiero, Importe =100 }
                }
            };
            var oContratoBase = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Sustentable = false,
                EstadoId = (int)EnumEstadoContrato.Rechazado,
                Descuentos = new List<DescuentoBonificacion>(),
                AperturaPrecio = new List<AperturaPrecio>(),
                PrecioPactado = new List<PrecioPactado>(),
                ContratoSAP = "23422343",
                Calidad = new List<Calidad>()
                {
                    new Calidad()
                    {
                        Id= 0,
                    }
                },
            };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContratoBase);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ProveedorEstado, bool>>>())).Returns(new ProveedorEstado { EstadoId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<Expression<Func<Centro, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<Expression<Func<Campaña, string>>>())).Returns("a");
            capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI",
                Ruca = new Ruca
                {
                    Acopiador = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Otros = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Corredor = "SI"
                }
            });
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { CantidadDias = 10, ImporteSustentable = 10 });
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(oContratoBase);

            var resultado = target.ActualizarContratoFinalizado(oContrato);

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Calidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Calidad>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<DescuentoBonificacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<DescuentoBonificacion>());
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

        }

        [Test]
        public void ReconfirmarFinalizadoOk()
        {
            var contratoParaSerializar = new Contrato()
            {
                ProveedorId = 1,
                Proveedor = new Proveedor { CUIT = "1234", RazonSocial = "hernanbio" },
                CorredorId = null,
                Corredor = new Proveedor { CUIT = "2345", RazonSocial = "hernanbio" },
                Clasificacion = new ClasificacionCompraNet { Descripcion = "aa" },
                Localidad = new Localidad { Nombre = "bs" },
                Provincia = new Provincia { Nombre = "bs" },
                Campana = new Campaña { Descripcion = "20-21" },
                Boleto = new BoletoCompraNet { Descripcion = "Ninguno" },
                MonedaSustentable = new Moneda { Descripcion = "USD", MonedaId = "USD" },
                ClasificacionId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                EstadoId = (int)EnumEstadoContrato.Reconfirmar,
                Comercial = new Comercial { ComercialId = 1 },
                Fecha = DateTime.Now,
                Base = true,
                ImporteSustentable = 1,
                FechaDolarizado = DateTime.Now,
                DiasPesificado = 2,
                NoInformaSio = false,
                TrigoEspecial = false,
                UsuarioId = "",
                Ampliaciones = 1,
                Observacion = "",
                CantidadCamiones = 2,
                Consignatario = false,
                PlanCanje = true,
                CD = false,
                Warrant = false,
                CondicionFijacionId = 1,
                PagoDirectoVendedor = false,
                BolsaId = 1,
                MercsDeposito = true,
                ComercialCreadorId = 1,
                SelCargoMOA = false,
                SelCargoVendedor = true,
                Madre = false,
                ContratoMadre = "",
                PrecioNeto = 100,
                Pizarra = true,
                PagoDiferido = false,
                ZonaId = 1,
                Compensacion = false,
                Sustentable = false,
                FechaCierta = DateTime.Now,
                ContratoAcuerdoId = 1,
                DolarizadoExpress = false,
                Moneda = new Moneda { Descripcion = "aaa" },
                Dolarizado = true,
                ContratoAcuerdo = new ContratoAcuerdo { Fecha = DateTime.Now },
                Material = new Material { Descripcion = "Soja" },
                Destino = new Centro { Descripcion = "123" },
                PrecioPactado = new List<PrecioPactado>()
                {
                    new PrecioPactado()
                    {
                        Id = 1,
                        ContratoId = 1,
                        FechaDesde = DateTime.Now,
                        FechaHasta = DateTime.Now,
                        MonedaImportePactado = new Moneda {Descripcion = "usd"},
                        MonedaPactado = new Moneda {Descripcion = "usd"},

                    }
                },
                Descuentos = new List<DescuentoBonificacion>()
                {
                    new DescuentoBonificacion()
                    {
                        Id = 1,
                        ContratoId = 1,
                        FechaDesde = DateTime.Now,
                        FechaHasta = DateTime.Now,
                        Importe = 1,
                        TipoDB = new TipoDB{Descripcion = "aaa"},
                        Moneda = new Moneda{Descripcion ="sss"}
                    }
                },
                Calidad = new List<Calidad>()
                {
                    new Calidad()
                    {
                        Id = 1,
                        CalidadEspecialId = 1,
                        NegocioId = 1,
                        PorcentajeDesde = 1,
                        PorcentajeHasta = 2,
                        CalidadEspecial = new CalidadEspecial{Descripcion = "aaa"}
                    }
                },
                AperturaPrecio = new List<AperturaPrecio>()
                {
                    new AperturaPrecio()
                    {
                        Id = 1,
                        NegocioId = 1,
                    }
                },
            };
            var datos = JsonConvert.SerializeObject(contratoParaSerializar, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<NegocioHistorico, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<NegocioHistorico>() { new NegocioHistorico { Datos = datos } });
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(contratoParaSerializar);
            modificarContratoAgentMock.Setup(x => x.Modificar(It.IsAny<Contrato>(), It.IsAny<Contrato>())).Returns("Ok");
            repositorioMock.Setup(x => x.GuardarCambios()).Verifiable();
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>())).Returns(new Comercial { ComercialId = 1, IdActiveDirectory = "bmelga@bf.com" });
            comercialManagerMock.Setup(x => x.ListarComercialesCorredor()).Returns(new List<Comercial>() { new Comercial { ComercialId = 2, IdActiveDirectory = "hernanbio@bf.com" } });
            comercialManagerMock.Setup(x => x.ListarComercialesSinRecibirMail()).Returns(new List<Comercial>() { new Comercial { ComercialId = 2, IdActiveDirectory = "hernanbio@bf.com" } });
            contextoMock.Setup(x => x.ObtenerPathLogoMail()).Returns(TestContext.CurrentContext.TestDirectory + "\\Util\\MolinosAgro.png");
            mailManagerMock.Setup(y => y.EnviarMail(It.IsAny<Comercial>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>(),
                 It.IsAny<List<string>>(), It.IsAny<AlternateView>(), It.IsAny<byte[]>(), It.IsAny<string>())).Verifiable();
            repositorioMock.Setup(x => x.AgregarTodos(It.IsAny<List<Cupo>>(), null)).Verifiable();

            var resultado = target.ReconfirmarFinalizado(1, "");
            ConfigurationManager.AppSettings["AmbientePruebas"] = "1";
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
        [Test]
        public void ValidarCalidadModificadaOk()
        {
            var oContrato = new Contrato()
            {
                Id = 1,
                Calidad = new List<Calidad>()
                {
                    new Calidad()
                    {
                        Id= 0,
                        PorcentajeDesde = 2
                    }
                },
                StandardDeCalidadId = 1
            };
            var oContratoBase = new Contrato()
            {
                Id = 1,
                Calidad = new List<Calidad>()
                {
                    new Calidad()
                    {
                        Id= 0,
                        PorcentajeDesde = 3
                    }
                },
                StandardDeCalidadId = 2
            };
            var resultado = target.ValidarCalidadModificada(oContrato, oContratoBase);

            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void ValidarCalidadModificada()
        {
            var oContrato = new Contrato()
            {
                Id = 1,
                StandardDeCalidadId = 1
            };
            var oContratoBase = new Contrato()
            {
                Id = 1,
                Calidad = new List<Calidad>()
                {
                    new Calidad()
                    {
                        Id= 0,
                    }
                },
                StandardDeCalidadId = 2
            };
            var resultado = target.ValidarCalidadModificada(oContrato, oContratoBase);

            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void AltaContratoSapOk()
        {
            var oContrato = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                PrecioNeto = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = new DateTime(2020, 02, 23),
                FechaHasta = new DateTime(2021, 02, 23),
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                PagoDiferido = true,
                DiasPesificado = 10,
                Sustentable = false,
                PorcentajeDePago = 95,
                Descuentos = new List<DescuentoBonificacion>()
                {
                    new DescuentoBonificacion()
                    {
                        Id= 0
                    }
                },
                Calidad = new List<Calidad>()
                {
                    new Calidad()
                    {
                        Id= 0,
                    }
                },
                AperturaPrecio = new List<AperturaPrecio>()
                {
                    new AperturaPrecio { ConceptoAperturaPrecioId = (int)EnumConceptoApertura.Financiero, Importe =100 }
                }
            };
            var oContratoBase = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 3,
                Precio = 500,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = new DateTime(2020, 02, 23),
                FechaHasta = new DateTime(2021, 02, 23),
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Sustentable = false,
                EstadoId = (int)EnumEstadoContrato.Confirmado,
                Descuentos = new List<DescuentoBonificacion>(),
                Calidad = new List<Calidad>(),
                AperturaPrecio = new List<AperturaPrecio>(),
                PrecioPactado = new List<PrecioPactado>()
            };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContratoBase);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ProveedorEstado, bool>>>())).Returns(new ProveedorEstado { EstadoId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<Expression<Func<Centro, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<Expression<Func<Campaña, string>>>())).Returns("20-21");
            repositorioMock.Setup(y => y.Obtener<Negocio>(It.IsAny<int>())).Returns(new Negocio { Fecha = DateTime.Now });
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Provincia, bool>>>())).Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Localidad, bool>>>())).Returns(true);

            capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI",
                Ruca = new Ruca
                {
                    Acopiador = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Otros = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Corredor = "SI"
                }
            });
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { CantidadDias = 10, ImporteSustentable = 10, CantidadMaxima = 1000 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(oContrato);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Calidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Calidad>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<DescuentoBonificacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<DescuentoBonificacion>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<AperturaPrecio>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PrecioPactado, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<PrecioPactado>());
            repositorioMock.Setup(y => y.Agregar(oContratoBase));
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(new Centro { ValidaRedespacho = false, Descripcion = "bandera", Acopio = false, CodigoSap = "1127", Id = 1 });

            var resultado = target.AltaContratoSAP(oContrato, false);
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }



        //[Test]
        //public void ModificarFijacionErrorSap()
        //{
        //    var contratoSave = new FijacionDePrecioContrato
        //    {
        //        ContratoSAP = "434343",
        //        ChequeElectronico = false
        //    };
        //    var contrato = new FijacionDePrecioContrato
        //    {
        //        ContratoSAP = "434343",
        //        ChequeElectronico = false,
        //        Id = 1
        //    };
        //    repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(contrato);
        //    repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>()))
        //        .Returns(new FijacionDePrecioContrato { ContratoSAP = "434343", Id = 1 });
        //    modificarFijacionAgentMock.Setup(x => x.Modificar(It.IsAny<FijacionDePrecioContrato>(), It.IsAny<FijacionDePrecioContrato>())).Returns("Error");
        //    var resultado = target.ActualizarFijacion(contrato);
        //    Assert.IsNotNull(resultado);
        //    repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

        //}

        [Test]
        public void ObtenerContratoTest()
        {
            repositorioMock.Setup(y => y.Obtener<Contrato, string>(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, string>>>())).Returns("0003456334");
            var resultado = target.ObtenerSapContrato(It.IsAny<int>());
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

        }
        [Test]
        public void ObtenerFijacionTest()
        {
            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato, string>(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, string>>>())).Returns("3456");
            var resultado = target.ObtenerSapFijacion(It.IsAny<int>());
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }
        [Test]
        public void TraerContratosPorSapTest()
        {
            var contrato = new ContratoCopiar
            {
                Material = "Soja",
                RazonSocial = "",
                Id = 1
            };
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<DevolverContratos>())).Returns(new List<ContratoCopiar>() { contrato });
            var resultado = target.TraerContratosPorSap(It.IsAny<string>());
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void TraerContratosAcuerdoTest()
        {
            var contrato = new ContratoCopiar
            {
                Material = "Soja",
                RazonSocial = "",
                Id = 1
            };
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<DevolverContratosAcuerdo>())).Returns(new List<ContratoCopiar>() { contrato });
            var resultado = target.TraerContratosAcuerdo(It.IsAny<string>());
            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<DevolverContratosAcuerdo>()), Times.Once);
        }
        [Test]
        public void TraerTotalesPesosDolaresTest()
        {
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<TraerTotalesPesosDolares>())).Returns(new TotalPesosDolares());
            var resultado = target.TraerTotalesPesosDolares(It.IsAny<DataSourceRequest>(), It.IsAny<List<int>>(), It.IsAny<List<int>>());
            repositorioMock.Verify(x => x.ObtenerConsultaEscalar(It.IsAny<TraerTotalesPesosDolares>()), Times.Once);
        }
        [Test]
        public void TraerTodosContratosTest()
        {
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<TraerTodosContratos>())).Returns(new DataSourceResult());
            var resultado = target.TraerTodosContratos(It.IsAny<DataSourceRequest>(), It.IsAny<bool>(), It.IsAny<List<int>>(), It.IsAny<List<int>>());
            repositorioMock.Verify(x => x.ObtenerConsultaEscalar(It.IsAny<TraerTodosContratos>()), Times.Once);
        }

        [Test]
        public void TraerContratosFiltradosTest()
        {
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<TraerContratosPorFiltro>())).Returns(new DataSourceResult());
            var resultado = target.TraerContratosFiltrados(It.IsAny<DataSourceRequest>(), It.IsAny<List<int>>());
            repositorioMock.Verify(x => x.ObtenerConsultaEscalar(It.IsAny<TraerContratosPorFiltro>()), Times.Once);
        }
        [Test]
        public void TraerDatosDeContratoAcuerdoTest()
        {
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, DatosContratoDto>>>()))
              .Returns(new DatosContratoDto());
            var resultado = target.TraerDatosDeContratoAcuerdo(It.IsAny<int>());
            repositorioMock.Verify(y => y.Obtener(It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, DatosContratoDto>>>()), Times.Once);

        }

        [Test]
        public void TraerContratosSAPTest()
        {
            repositorioMock.Setup(y => y.ObtenerMayor(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, int>>>(), It.IsAny<Expression<Func<Contrato, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, ContratoIdDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<ContratoIdDto>());
            var res = target.TraerContratosSAP(It.IsAny<string>(), It.IsAny<string>());
            repositorioMock.Verify(y => y.ObtenerMayor(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, int>>>(), It.IsAny<Expression<Func<Contrato, int>>>()), Times.Once);
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<Contrato, ContratoIdDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);

        }
        [Test]
        public void ValidarStatusTest()
        {
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, BasicoContrato>>>()))
              .Returns(new BasicoContrato()
              {
                  ContratoSAP = "23443",
                  Estado = 5
              });
            status.Setup(x => x.ValidarEstado(It.IsAny<string>())).Returns(new EstadoSAPDto { NumeroSio = 0, Status = "OK" });
            var res = target.ValidarStatus(It.IsAny<int>());
            Assert.NotNull(res);
            status.Verify(x => x.ValidarEstado(It.IsAny<string>()), Times.Once);
            repositorioMock.Verify(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, BasicoContrato>>>()), Times.Once);
        }
        [Test]
        public void CompararNegocioReconfirmadoTest()
        {
            var contratoParaSerializar = new Contrato()
            {
                ProveedorId = 1,
                Proveedor = new Proveedor { CUIT = "1234" },
                CorredorId = null,
                Corredor = new Proveedor { CUIT = "2345" },
                ClasificacionId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                EstadoId = (int)EnumEstadoContrato.ReconfirmarFinalizado,
                Comercial = new Comercial { ComercialId = 1 },
                Fecha = DateTime.Now,
                Base = true,
                ImporteSustentable = 1,
                FechaDolarizado = DateTime.Now,
                DiasPesificado = 2,
                NoInformaSio = false,
                TrigoEspecial = false,
                UsuarioId = "",
                Ampliaciones = 1,
                Observacion = "",
                CantidadCamiones = 2,
                Consignatario = false,
                PlanCanje = true,
                CD = false,
                Warrant = false,
                CondicionFijacionId = 1,
                PagoDirectoVendedor = false,
                BolsaId = 1,
                MercsDeposito = true,
                ComercialCreadorId = 1,
                SelCargoMOA = false,
                SelCargoVendedor = true,
                Madre = false,
                ContratoMadre = "",
                PrecioNeto = 100,
                Pizarra = true,
                PagoDiferido = false,
                ZonaId = 1,
                Compensacion = false,
                Sustentable = false,
                FechaCierta = DateTime.Now,
                ContratoAcuerdoId = 1,
                DolarizadoExpress = false,
                Dolarizado = true,
                StandardDeCalidad = new StandardDeCalidad { Descripcion = "aa" },
                PrecioPactado = new List<PrecioPactado>()
                {
                    new PrecioPactado()
                    {
                        Id = 1,
                        ContratoId = 1,
                        FechaDesde = DateTime.Now,
                        FechaHasta = DateTime.Now,
                    }
                },
                Descuentos = new List<DescuentoBonificacion>()
                {
                    new DescuentoBonificacion()
                    {
                        Id = 1,
                        ContratoId = 1,
                        FechaDesde = DateTime.Now,
                        FechaHasta = DateTime.Now,
                        Importe = 1
                    }
                },
                Calidad = new List<Calidad>()
                {
                    new Calidad()
                    {
                        Id = 1,
                        CalidadEspecialId = 1,
                        NegocioId = 1,
                        PorcentajeDesde = 1,
                        PorcentajeHasta = 2,
                        StandardDeCalidad = new StandardDeCalidad { Descripcion = "aaa"},
                        CalidadEspecial = new CalidadEspecial {Descripcion = "aaa"}
                    }
                },
                AperturaPrecio = new List<AperturaPrecio>()
                {
                    new AperturaPrecio()
                    {
                        Id = 1,
                        NegocioId = 1,
                    }
                },
            };
            var datos = JsonConvert.SerializeObject(contratoParaSerializar, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<NegocioHistorico, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
               Returns(new List<NegocioHistorico>() { new NegocioHistorico { Datos = datos } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, BasicoContrato>>>()))
             .Returns(new BasicoContrato()
             {
                 ProveedorId = 1,
                 ClasificacionId = 1,
                 MaterialId = 1,
                 Cantidad = 1,
                 Precio = 1000,
                 TipoNegocioId = 2,
                 DestinoId = 1,
                 LocalidadId = 1,
                 ProvinciaId = 1,
                 FechaEntrega = DateTime.Now,
                 FechaDesde = DateTime.Now,
                 FechaHasta = DateTime.Now,
                 MonedaId = "ARS ",
                 CampanaId = 1,
                 ComercialId = 70,
                 EstablecimientoPropio = true,
                 BoletoId = 3,
                 Fecha = DateTime.Now,
                 Base = true,
                 TrigoEspecial = false,
                 UsuarioId = "",
                 Ampliaciones = 1,
                 Observacion = "",
                 CantidadCamiones = 2,
                 Consignatario = false,
                 PlanCanje = true,
                 CD = false,
                 Warrant = false,
                 PagoDirectoVendedor = false,
                 BolsaId = 1,
                 MercsDeposito = true,
                 ComercialCreadorId = 1,
                 SelCargoMOA = false,
                 SelCargoVendedor = true,
                 Madre = false,
                 ContratoMadre = "",
                 PrecioNeto = 100,
                 Pizarra = true,
                 PagoDiferido = false,
                 ZonaId = 1,
                 Compensacion = false,
                 Sustentable = false,
                 FechaCierta = DateTime.Now,
                 ContratoAcuerdoId = 1,
                 DolarizadoExpress = false,
                 Dolarizado = true,
                 Estado = 5,
                 ContratoSAP = "112323",
                 StandardDeCalidadDescripcion = "",
                 Calidades = new List<CalidadDto>()
                {
                    new CalidadDto()
                    {
                        Id = 1,
                        CalidadEspecialId = 1,
                        PorcentajeDesde = 1,
                        PorcentajeHasta = 2,
                        Valor = 1,

                    }
                },
             });
            var res = target.CompararNegocioReconfirmado(It.IsAny<int>());
            Assert.NotNull(res);
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<NegocioHistorico, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, BasicoContrato>>>()), Times.Once);

        }
        [Test]
        public void DiferenciaEnCalidades()
        {
            var contratoParaSerializar = new Contrato()
            {
                ProveedorId = 1,
                Proveedor = new Proveedor { CUIT = "1234" },
                CorredorId = null,
                Corredor = new Proveedor { CUIT = "2345" },
                ClasificacionId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                EstadoId = (int)EnumEstadoContrato.ReconfirmarFinalizado,
                Comercial = new Comercial { ComercialId = 1 },
                Fecha = DateTime.Now,
                Base = true,
                ImporteSustentable = 1,
                FechaDolarizado = DateTime.Now,
                DiasPesificado = 2,
                NoInformaSio = false,
                TrigoEspecial = false,
                UsuarioId = "",
                Ampliaciones = 1,
                Observacion = "",
                CantidadCamiones = 2,
                Consignatario = false,
                PlanCanje = true,
                CD = false,
                Warrant = false,
                CondicionFijacionId = 1,
                PagoDirectoVendedor = false,
                BolsaId = 1,
                MercsDeposito = true,
                ComercialCreadorId = 1,
                SelCargoMOA = false,
                SelCargoVendedor = true,
                Madre = false,
                ContratoMadre = "",
                PrecioNeto = 100,
                Pizarra = true,
                PagoDiferido = false,
                ZonaId = 1,
                Compensacion = false,
                Sustentable = false,
                FechaCierta = DateTime.Now,
                ContratoAcuerdoId = 1,
                DolarizadoExpress = false,
                Dolarizado = true,
                StandardDeCalidad = new StandardDeCalidad { Descripcion = "aa" },
                PrecioPactado = new List<PrecioPactado>()
                {
                    new PrecioPactado()
                    {
                        Id = 1,
                        ContratoId = 1,
                        FechaDesde = DateTime.Now,
                        FechaHasta = DateTime.Now,
                    }
                },
                Descuentos = new List<DescuentoBonificacion>()
                {
                    new DescuentoBonificacion()
                    {
                        Id = 1,
                        ContratoId = 1,
                        FechaDesde = DateTime.Now,
                        FechaHasta = DateTime.Now,
                        Importe = 1
                    }
                },
                Calidad = new List<Calidad>()
                {
                    new Calidad()
                    {
                        Id = 1,
                        CalidadEspecialId = 1,
                        NegocioId = 1,
                        PorcentajeDesde = 1,
                        PorcentajeHasta = 2,
                        StandardDeCalidad = new StandardDeCalidad { Descripcion = "aaa"},
                        CalidadEspecial = new CalidadEspecial {Descripcion = "aaa"}
                    }
                },
                AperturaPrecio = new List<AperturaPrecio>()
                {
                    new AperturaPrecio()
                    {
                        Id = 1,
                        NegocioId = 1,
                    }
                },
            };
            var datos = JsonConvert.SerializeObject(contratoParaSerializar, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<NegocioHistorico, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
               Returns(new List<NegocioHistorico>() { new NegocioHistorico { Datos = datos } });
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(contratoParaSerializar);
            var res = target.DiferenciaEnCalidades(It.IsAny<int>());
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<NegocioHistorico, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(y => y.Obtener<Contrato>(It.IsAny<int>()), Times.Once);

        }
        [Test]
        public void AnularContratoSapTest()
        {
            var contrato = new ContratoSAP
            {
                CodigoSap = "000454543",
                Cantidad = "1000"
            };
            repositorioMock.Setup(y => y.ObtenerMayor(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, int>>>()))
                .Returns(new Contrato { ContratoSAP = "000454543", Cantidad = 10, EstadoId = 5 });
            repositorioMock.Setup(x => x.GuardarCambios()).Verifiable();
            var res = target.AnularContratoSAP(contrato);
            repositorioMock.Verify(y => y.ObtenerMayor(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, int>>>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
        [Test]
        public void AnularContratoSapnNegativoTest()
        {
            var contrato = new ContratoSAP
            {
                CodigoSap = "000454543",
                Cantidad = "-1000"
            };
            repositorioMock.Setup(y => y.ObtenerMayor(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, int>>>()))
                .Returns(new Contrato { ContratoSAP = "000454543", Cantidad = 10, EstadoId = 5 });
            repositorioMock.Setup(x => x.GuardarCambios()).Verifiable();
            var res = target.AnularContratoSAP(contrato);
            repositorioMock.Verify(y => y.ObtenerMayor(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, int>>>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
        [Test]
        public void ListarCBUTest()
        {
            cbuAgentMock.Setup(x => x.ListarCBU(It.IsAny<string>(), It.IsAny<string>())).Returns(new List<PagoCBUDto>());
            var res = target.ListarCBU(It.IsAny<string>(), It.IsAny<string>());
            cbuAgentMock.Verify(x => x.ListarCBU(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }


        [Test]
        public void AprobarContratoTest()
        {
            var contrato = new Contrato()
            {
                Id = 1,
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                PrecioNeto = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                Fecha = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Sustentable = false,
                PorcentajeDePago = 95,
                Calidad = new List<Calidad>(),
                EstadoId = 9,
                Estado = new EstadoContrato { EstadoContratoId = 9 },
                AperturaPrecio = new List<AperturaPrecio>()
            };
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(contrato);
            repositorioMock.Setup(y => y.Obtener<Negocio>(It.IsAny<int>())).Returns(contrato);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ProveedorEstado, bool>>>())).Returns(new ProveedorEstado { EstadoId = 1 });
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { CantidadDias = 10, ImporteSustentable = 10, CantidadMaxima = 1000 });
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Provincia, bool>>>())).Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Localidad, bool>>>())).Returns(true); capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI",
                Ruca = new Ruca
                {
                    Acopiador = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Otros = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Corredor = "SI"
                }
            });
            repositorioMock.Setup(x => x.GuardarCambios()).Verifiable();
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(new Centro { ValidaRedespacho = false, Descripcion = "bandera", Acopio = false, CodigoSap = "1127", Id = 1 });

            var res = target.AprobarContrato(1);

            repositorioMock.Verify(y => y.Obtener<Contrato>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void BorrarContratoPreAprobacionTest()
        {
            var contrato = new Contrato
            {
                Id = 1,
                Cantidad = 1000,
                EstadoId = 9,
                Estado = new EstadoContrato { EstadoContratoId = 9 }
            };
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(contrato);
            repositorioMock.Setup(x => x.GuardarCambios()).Verifiable();

            var res = target.BorrarContratoPreAprobacion(1, "ok");

            repositorioMock.Verify(y => y.Obtener<Contrato>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void TraerContratosAcuerdoPorCorredorTest()
        {
            var contrato = new ContratoCopiar
            {
                Material = "Soja",
                RazonSocial = "",
                Id = 1
            };
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<DevolverContratosAcuerdoPorCorredor>())).Returns(new List<ContratoCopiar>() { contrato });
            var resultado = target.TraerContratosAcuerdoPorCorredor(It.IsAny<int>());
            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<DevolverContratosAcuerdoPorCorredor>()), Times.Once);
        }

        [Test]
        public void GrabarContratoMasivoTest()
        {
            var contratos = new List<BasicoContrato>{ new BasicoContrato
            {
                ContratoAcuerdoId = 1,
                MaterialId =1,
                ProveedorId = 1,
                Cantidad = 1,
                Precio =1,
                PrecioNeto =1,
                Id = 0,
                ContratoCorredor="1",
                ContratoVendedor="1",
                CampanaId=1,
                ClasificacionId=1,
                PlanCanje=false,
                Consignatario= false,
                DestinoId=1,
                LocalidadId=1,
                ProvinciaId=1,
                Observacion="0",
                CantidadAmpliado = 10
            } };
            repositorioMock.Setup(y => y.Obtener<ContratoAcuerdo>(It.IsAny<int>()))
                           .Returns(new ContratoAcuerdo
                           {
                               FechaOperacion = DateTime.Now.Date,
                               FechaDesde = DateTime.Now.Date,
                               FechaHasta = DateTime.Now.Date,
                               DesdeFijacion = DateTime.Now.Date,
                               HastaFijacion = DateTime.Now.Date,
                               ProveedorId = 1,
                               ComercialId = 1,
                               TipoNegocioId = 2,
                               MaterialId = 1,
                               Cantidad = 1000,
                               Ampliaciones = 1,
                               Precio = 111,
                               PrecioNeto = 111,
                               MonedaId = "ARS  ",
                               CampanaId = 5,
                               ContratoSAP = "010101",
                               Observacion = "HHAAA",
                               CD = false,
                               Warrant = false,
                           });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, BasicoContrato>>>()))
                            .Returns(new BasicoContrato
                            {
                                FechaOperacion = DateTime.Now.Date,
                                FechaDesde = DateTime.Now.Date,
                                FechaHasta = DateTime.Now.Date,
                                DesdeFijacion = DateTime.Now.Date,
                                HastaFijacion = DateTime.Now.Date,
                                ContratoId = 1,
                                ProveedorId = 1,
                                Proveedor = "HAA",
                                Corredor = "HEE",
                                ComercialId = 1,
                                FechaDesdeFormateado = "10101",
                                FechaHastaFormateado = "02020",
                                FechaFormateado = "0403030",
                                TipoNegocioId = 2,
                                MaterialId = 1,
                                Cantidad = 1000,
                                Ampliaciones = 1,
                                Precio = 111,
                                PrecioNeto = 111,
                                MonedaId = "ARS  ",
                                CampanaId = 5,
                                ProvinciaId = 1,
                                Provincia = "BS AS",
                                LocalidadId = 2,
                                Localidad = "SAN MARTIN",
                                ContratoSAP = "010101",
                                Base = true,
                                Observacion = "HHAAA",
                                Estado = 2,
                                Importe_Sustentable = 0,
                                Moneda_Sustentable = "ARS  ",
                                Fecha_DolarizadoFormateado = "848484",
                                //Dias_Pesificado = 3,
                                NoInformaSIO = true,
                                TrigoEspecial = false,
                                ClasificacionId = 4,
                                DestinoId = 2,
                                PlanCanje = false,
                                Consignatario = false,
                                CantidadCamiones = 0,
                                BoletoId = 3,
                                BolsaId = 3,
                                DesdeFijacionFormateado = "87454",
                                HastaFijacionFormateado = "7564",
                                CondicionFijacion = 2,
                                CD = false,
                                Warrant = false,
                                PagoDirectoVendedor = false,
                                EstablecimientoPropio = false,
                                MercsDeposito = false,
                                PorcentajeComision = 2,
                                ContratoCorredor = "33",
                                ContratoVendedor = "44",
                                SelCargoMOA = true,
                                SelCargoVendedor = true,
                                Madre = false,
                                ContratoMadre = "",
                                Pizarra = false,
                                StandardCalidadId = 2,
                                AperturaPrecios = new List<AperturaPrecioDto> { new AperturaPrecioDto { Importe = 0, Porcentaje = 0, MonedaId = null, ConceptoAperturaPrecioId = 1 }, new AperturaPrecioDto { Importe = 0, Porcentaje = 0, MonedaId = null, ConceptoAperturaPrecioId = 2 }, new AperturaPrecioDto { Importe = 0, Porcentaje = 0, MonedaId = null, ConceptoAperturaPrecioId = 3 }, new AperturaPrecioDto { Importe = 0, Porcentaje = 0, MonedaId = null, ConceptoAperturaPrecioId = 4 } },
                                Descuentos = new List<DescuentoBonificacionDto>(),
                                Calidades = new List<CalidadDto>(),
                                PreciosPactados = new List<PrecioPactadosDto>(),

                            });
            repositorioMock.Setup(y => y.Obtener<Proveedor, string>(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>())).Returns("30209034560");
            validacionCreditoAgent.Setup(x => x.ValidarCredito(It.IsAny<string>())).Returns(new ValidarCreditoDto() { Moneda = "ARP" });
            comercialManagerMock.Setup(y => y.TraerComercial(It.IsAny<int>())).Returns(new ComercialDto { IdActiveDirectory = "a", GrupoDeComprasId = 1 });
            materialManagerMock.Setup(y => y.TraerTodoMaterial()).Returns(new ResultIniMaterial { Material = new List<MaterialIni> { new MaterialIni { MaterialId = 1, Descripcion = "a" } } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Moneda, MonedaQry>>>(), It.IsAny<Expression<Func<Moneda, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                            .Returns(new List<MonedaQry>() { new MonedaQry { Descripcion = "ARS  ", MonedaId = "ARS  " } });
            proveedorManagerMock.Setup(y => y.TraerProveedor(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<int>>())).Returns(new StoredPorProveedorResult { BasicoProveedorTraerPorProveedores = new List<BasicoProveedor> { new BasicoProveedor { RazonSocial = "a" } } });
            proveedorManagerMock.Setup(y => y.TraerBoletoBolsa(It.IsAny<int>())).Returns(new DatosCompraNetDto { BoletoCompraNetId = 1, BolsaCompraNetId = 1 });
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(false);
            proveedorManagerMock.Setup(y => y.ObtenerIdProveedorPorCuit(It.IsAny<string>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ProveedorEstado, bool>>>())).Returns(new ProveedorEstado { EstadoId = 1 });
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { CantidadDias = 10, ImporteSustentable = 10, CantidadMaxima = 1000, CantidadAcuerdo = 10000, CantidadDiasDolarizadoLimiteMaximo = 11, DiasDiferimiento = 111 });
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Provincia, bool>>>())).Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Localidad, bool>>>())).Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<CorredorProveedor, bool>>>())).Returns(true);
            capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI",
                Ruca = new Ruca
                {
                    Acopiador = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Otros = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Corredor = "SI"
                }
            });
            contratoAcuerdoManagerMock.Setup(y => y.TraerAcuerdo(It.IsAny<int>())).Returns(new BasicoContrato { Cantidad = 1000000 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Contrato>() { new Contrato { Cantidad = 10 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ContactoComercial, string>>>(), It.IsAny<Expression<Func<ContactoComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                            .Returns(new List<string> { "a" });
            contextoMock.Setup(x => x.ObtenerPathLogoMail()).Returns(TestContext.CurrentContext.TestDirectory + "\\Util\\MolinosAgro.png");
            mailManagerMock.Setup(y => y.EnviarMail(It.IsAny<Comercial>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>(),
         It.IsAny<List<string>>(), It.IsAny<AlternateView>(), It.IsAny<byte[]>(), It.IsAny<string>())).Verifiable();
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(new Centro { ValidaRedespacho = false, Descripcion = "bandera", Acopio = false, CodigoSap = "1127", Id = 1 });

            var resultado = target.GrabarContratoMasivo(contratos);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Contrato>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }


        [Test]
        public void ObtenerCapacidadProductivaPendienteTest()
        {
            repositorioMock.Setup(y => y.Obtener<Proveedor, string>(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>())).Returns("30209034560");
            capacidadProductivaDisponibleAgent.Setup(y => y.ObtenerCapacidadProductivaPendiente(It.IsAny<string>())).Returns(new List<CapacidadProductivaPendienteDto>());

            var resultado = target.ObtenerCapacidadProductivaPendiente(It.IsAny<int>());

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

        }

        [Test]
        public void AnularContratoCargaTest()
        {
            var contrato = new Contrato
            {
                Id = 1,
                Cantidad = 1000,
                EstadoId = 9,
                Estado = new EstadoContrato { EstadoContratoId = 9 }
            };
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(contrato);
            repositorioMock.Setup(x => x.GuardarCambios()).Verifiable();

            var res = target.AnularContratoCarga(1, "ok");

            repositorioMock.Verify(y => y.Obtener<Contrato>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void GrabarContratoTestErrorAFijarRedespachoARP()
        {
            var oContrato = new Contrato()
            {
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                PrecioNeto = 1000,
                TipoNegocioId = 1,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                DesdeFijacion = DateTime.Now,
                HastaFijacion = DateTime.Now,
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Sustentable = false,
                PorcentajeDePago = 95,
                Calidad = new List<Calidad>(),
                AperturaPrecio = new List<AperturaPrecio> { new AperturaPrecio { ConceptoAperturaPrecioId = (int)EnumConceptoApertura.Redespacho, Importe = -61, MonedaId = "ARP  " } },
                CondicionFijacionId = 1
            };
            repositorioMock.Setup(y => y.Obtener<Proveedor, string>(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>())).Returns("30209034560");
            validacionCreditoAgent.Setup(x => x.ValidarCredito(It.IsAny<string>())).Returns(new ValidarCreditoDto() { Moneda = "ARP" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ProveedorEstado, bool>>>())).Returns(new ProveedorEstado { EstadoId = 1 });
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { CantidadDias = 10, ImporteSustentable = 10, CantidadMaxima = 1000, RedespachoMaximoUSDM = 60 });
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Provincia, bool>>>())).Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Localidad, bool>>>())).Returns(true); capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI",
                Ruca = new Ruca
                {
                    Acopiador = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Otros = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Corredor = "SI"
                }
            });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(new Centro { ValidaRedespacho = true, Descripcion = "bandera", Acopio = false, CodigoSap = "1127", Id = 1 });

            var resultado = target.GrabarContrato(oContrato);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Contrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.AreEqual(2, resultado.Errores.Count);
        }

        [Test]
        public void GrabarContratoTestErrorAPrecioRedespachoARP()
        {
            var oContrato = new Contrato()
            {
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                PrecioNeto = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                DesdeFijacion = DateTime.Now,
                HastaFijacion = DateTime.Now,
                MonedaId = "ARP  ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Sustentable = false,
                PorcentajeDePago = 95,
                Calidad = new List<Calidad>(),
                AperturaPrecio = new List<AperturaPrecio> { new AperturaPrecio { ConceptoAperturaPrecioId = (int)EnumConceptoApertura.Redespacho, Importe = -200, MonedaId = "ARP  " } },
                CondicionFijacionId = 1
            };
            repositorioMock.Setup(y => y.Obtener<Proveedor, string>(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>())).Returns("30209034560");
            validacionCreditoAgent.Setup(x => x.ValidarCredito(It.IsAny<string>())).Returns(new ValidarCreditoDto() { Moneda = "ARP" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ProveedorEstado, bool>>>())).Returns(new ProveedorEstado { EstadoId = 1 });
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { CantidadDias = 10, ImporteSustentable = 10, CantidadMaxima = 1000, RedespachoMaximoUSDM = 60, RedespachoMaximoARP = 1000 });
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Provincia, bool>>>())).Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Localidad, bool>>>())).Returns(true); capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI",
                Ruca = new Ruca
                {
                    Acopiador = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Otros = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Corredor = "SI"
                }
            });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(new Centro { ValidaRedespacho = true, Descripcion = "bandera", Acopio = false, CodigoSap = "1127", Id = 1 });

            var resultado = target.GrabarContrato(oContrato);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Contrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.AreEqual(1, resultado.Errores.Count);
        }

        [Test]
        public void GrabarContratoTestErrorAPrecioRedespachoUSDM()
        {
            var oContrato = new Contrato()
            {
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                PrecioNeto = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                DesdeFijacion = DateTime.Now,
                HastaFijacion = DateTime.Now,
                MonedaId = "USDM ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Sustentable = false,
                PorcentajeDePago = 95,
                Calidad = new List<Calidad>(),
                AperturaPrecio = new List<AperturaPrecio> { new AperturaPrecio { ConceptoAperturaPrecioId = (int)EnumConceptoApertura.Redespacho, Importe = -200, MonedaId = "USDM " } },
                CondicionFijacionId = 1
            };
            repositorioMock.Setup(y => y.Obtener<Proveedor, string>(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>())).Returns("30209034560");
            validacionCreditoAgent.Setup(x => x.ValidarCredito(It.IsAny<string>())).Returns(new ValidarCreditoDto() { Moneda = "ARP" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ProveedorEstado, bool>>>())).Returns(new ProveedorEstado { EstadoId = 1 });
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { CantidadDias = 10, ImporteSustentable = 10, CantidadMaxima = 1000, RedespachoMaximoUSDM = 60, RedespachoMaximoARP = 1000 });
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Provincia, bool>>>())).Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Localidad, bool>>>())).Returns(true); capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI",
                Ruca = new Ruca
                {
                    Acopiador = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Otros = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Corredor = "SI"
                }
            });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(new Centro { ValidaRedespacho = true, Descripcion = "bandera", Acopio = false, CodigoSap = "1127", Id = 1 });

            var resultado = target.GrabarContrato(oContrato);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Contrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.AreEqual(1, resultado.Errores.Count);
        }


        [Test]
        public void ActualizarCesionContratoSAPOk()
        {
            var oContrato = new Contrato()
            {
                Id = 1,
                ContratoSAP = "0001234567",
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                PrecioNeto = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = new DateTime(2020, 02, 23),
                FechaHasta = new DateTime(2021, 02, 23),
                MonedaId = "ARS ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                PagoDiferido = true,
                DiasPesificado = 10,
                Sustentable = false,
                PorcentajeDePago = 95,
                Descuentos = new List<DescuentoBonificacion>()
                {
                    new DescuentoBonificacion()
                    {
                        Id= 0
                    }
                },
                Calidad = new List<Calidad>()
                {
                    new Calidad()
                    {
                        Id= 0,
                    }
                },
                AperturaPrecio = new List<AperturaPrecio>()
                {
                    new AperturaPrecio()
                    {
                        ConceptoAperturaPrecioId= (int)EnumConceptoApertura.Redespacho,
                        Importe = 300
                    },
                    new AperturaPrecio { ConceptoAperturaPrecioId = (int)EnumConceptoApertura.Financiero, Importe =100 }
                }
            };
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(oContrato);

            var resultado = target.ActualizarCesionContratoSAP(oContrato.ContratoSAP, false);
            Assert.IsNotNull(resultado);
            Assert.AreEqual(false, resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        

        [Test]
        public void TraerContratosReporteAFijarPaseTestOk()
        {
            var result = new DataSourceResult();
            result.Data = new List<ReporteAfijarPaseDto> { new ReporteAfijarPaseDto { Negocio = "1", Cantidad = 10 } };
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<TraerContratosReporteAFijarPase>())).Returns(result);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, BasicoContrato>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<BasicoContrato>() { new BasicoContrato { ContratoSAP = "1", Cantidad = 1 } });

            var resultado = target.TraerContratosReporteAFijarPase(It.IsAny<DataSourceRequest>(), It.IsAny<List<int>>());

            repositorioMock.Verify(x => x.ObtenerConsultaEscalar(It.IsAny<TraerContratosReporteAFijarPase>()), Times.Once);

        }

        [Test]
        public void GrabarContratoErrorDolarizadoExpress()
        {
            var oContrato = new Contrato()
            {
                ProveedorId = 1,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                PrecioNeto = 1000,
                TipoNegocioId = 2,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaHasta = DateTime.Now,
                MonedaId = "USDM ",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Sustentable = false,
                PorcentajeDePago = 95,
                Calidad = new List<Calidad>(),
                AperturaPrecio = new List<AperturaPrecio>(),
                DolarizadoExpress = true,
                FechaDolarizado = DateTime.Now.Date.AddDays(31),
                FechaDesde = DateTime.Now.Date
            };
            repositorioMock.Setup(y => y.Obtener<Proveedor, string>(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>())).Returns("30209034560");
            validacionCreditoAgent.Setup(x => x.ValidarCredito(It.IsAny<string>())).Returns(new ValidarCreditoDto() { Moneda = "ARP" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ProveedorEstado, bool>>>())).Returns(new ProveedorEstado { EstadoId = 1 });
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { CantidadDias = 90, CantidadDiasDolarizadoLimiteMaximo = 90, ImporteSustentable = 10, CantidadMaxima = 1000 });
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Provincia, bool>>>())).Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Localidad, bool>>>())).Returns(true); capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI",
                Ruca = new Ruca
                {
                    Acopiador = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Otros = new ValoresRuca { Consignatario = "SI", Directo = "SI", PlanCanje = "SI" },
                    Corredor = "SI"
                }
            });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(new Centro { ValidaRedespacho = false, Descripcion = "bandera", Acopio = false, CodigoSap = "1127", Id = 1 });

            var resultado = target.GrabarContrato(oContrato);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Contrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.HayError);
            Assert.AreEqual(1, resultado.ListaErrores.Count);

        }
    }
}
