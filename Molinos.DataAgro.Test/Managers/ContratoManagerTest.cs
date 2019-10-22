using Autofac.Extras.NLog;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq.Expressions;
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
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
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
            diasHabilesAgentMock = new Mock<IDiasHabilesAgent>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

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
                diasHabilesAgentMock.Object);
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
             .Returns(new List<CentroQry>() { new CentroQry { Id = 1, Descripcion = "SAN LORENZO" } });

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


            var result = target.TraerDatosCombo(8);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Provincia, ProvinciaQry>>>(), It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Campaña, CampañaQry>>>(), It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Material, MaterialQry>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Moneda, MonedaQry>>>(), It.IsAny<Expression<Func<Moneda, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Exactly(4));
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
                Sustentable = false,
                Calidad = new List<Calidad>()

            };
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ProveedorEstado, bool>>>())).Returns(new ProveedorEstado { EstadoId= 1 });
            configuracionManagermock.Setup(y => y.TraerConfiguraciones()).Returns(new Configuracion { CantidadDias=10});
            capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI"
            });
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
                Sustentable = false,
                Calidad = new List<Calidad>()

            };
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ProveedorEstado, bool>>>())).Returns(new ProveedorEstado { EstadoId = 1 });
            repositorioMock.Setup(y => y.ObtenerMayor<RangoConfirmacionAutomatica, DateTime>(It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny<Expression<Func<RangoConfirmacionAutomatica, DateTime>>>()))
                    .Returns(new RangoConfirmacionAutomatica() { MaterialId = 1, MonedaId = "ARS ", PrecioMinimo = 1, PrecioMaximo = 2000 });
            diferencialManagerMock.Setup(y => y.ValidarComprasDiferencial(It.IsAny<int>())).Throws(new Exception("Error diferencial"));
            configuracionManagermock.Setup(y => y.TraerConfiguraciones()).Returns(new Configuracion { CantidadDias = 10 });
            capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI"
            });
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
                Sustentable = false,
                Calidad = new List<Calidad>()
            };
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ProveedorEstado, bool>>>())).Returns(new ProveedorEstado { EstadoId = 1 });

            repositorioMock.Setup(y => y.ObtenerMayor(It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny<Expression<Func<RangoConfirmacionAutomatica, DateTime>>>()))
                    .Returns(new RangoConfirmacionAutomatica() { MaterialId = 1, MonedaId = "ARS ", PrecioMinimo = 1, PrecioMaximo = 2000 });
            configuracionManagermock.Setup(y => y.TraerConfiguraciones()).Returns(new Configuracion { CantidadDias = 10 });
            capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI"
            });

            var resultado = target.GrabarContrato(oContrato);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Contrato>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void UpdateContratoOk()
        {
            var oContrato = new Contrato()
            {
                ContratoId = 1,
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
                PagoDiferido = true,
                DiasPesificado=10,
                Sustentable = false,
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
                ContratoId = 1,
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
                Sustentable = false,
                EstadoId = (int)EnumEstadoContrato.Confirmado
            };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContratoBase);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<DescuentoBonificacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<DescuentoBonificacion>() { new DescuentoBonificacion { Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Calidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Calidad>() { new Calidad { Id = 2 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<AperturaPrecio>() { new AperturaPrecio { Id = 3 } });
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
                Nosis = "SI"
            });
            configuracionManagermock.Setup(y => y.TraerConfiguraciones()).Returns(new Configuracion { CantidadDias = 10 });

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
                ContratoId = 1,
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
                ContratoId = 1,
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
                EstadoId = (int)EnumEstadoContrato.Finalizado
            };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1 });
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContratoBase);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<DescuentoBonificacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<DescuentoBonificacion>() { new DescuentoBonificacion { Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Calidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Calidad>() { new Calidad { Id = 2 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<AperturaPrecio>() { new AperturaPrecio { Id = 3 } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>(),It.IsAny<Expression<Func<Centro, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>(),It.IsAny<Expression<Func<Material, string>>>())).Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<Expression<Func<Campaña, string>>>())).Returns("a");
            configuracionManagermock.Setup(y => y.TraerConfiguraciones()).Returns(new Configuracion { CantidadDias = 10 });
            capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI"
            });
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
                ContratoAcuerdoId = 2,
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
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668", RiesgoComercialSap = "a" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 1, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1, Material = new Material { Descripcion = "SOPA" }, Moneda = new Moneda { Descripcion = "PATACON" } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<FACACOP, bool>>>())).Returns(new FACACOP { CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, decimal>>>())).Returns(-3000);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Contrato>() { new Contrato { Cantidad = 10 } });
            contratoAcuerdoManagerMock.Setup(y => y.TraerAcuerdo(It.IsAny<int>())).Returns(new BasicoContrato { Cantidad = 1 });
            configuracionManagermock.Setup(y => y.TraerConfiguraciones()).Returns(new Configuracion { CantidadDias = 10 });
            capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI"
            });

            var resultado = target.GrabarContrato(oContrato);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Contrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.IsTrue(resultado.HayError);
            Assert.AreEqual(26, resultado.ListaErrores.Count);
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
                ContratoAcuerdoId = 2,
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
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668", RiesgoComercialSap = "a" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>())).Returns(new SISA { SituacionCategoria = "AL", EstadoCuit = 3, CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>())).Returns(new RangoPrecio { PrecioMaximo = 50000, PrecioMinimo = 1, Material = new Material { Descripcion = "SOPA" }, Moneda = new Moneda { Descripcion = "PATACON" } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<FACACOP, bool>>>())).Returns(new FACACOP { CUIT = "20358654668" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, decimal>>>())).Returns(-3000);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Contrato>() { new Contrato { Cantidad = 10 } });
            contratoAcuerdoManagerMock.Setup(y => y.TraerAcuerdo(It.IsAny<int>())).Returns(new BasicoContrato { Cantidad = 1 });
            configuracionManagermock.Setup(y => y.TraerConfiguraciones()).Returns(new Configuracion { CantidadDias=10});
            capacidadProductivaAgentMock.Setup(y => y.ObtenerCapacidadProductiva(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            altaTempranaAgentMock.Setup(y => y.ObtenerAlta(It.IsAny<string>())).Returns(new AltaTempranaNRCODto
            {
                AltaTemprana = "SI",
                Bolsa = "SI",
                Carta = "SI",
                FechaActualizacion = "SI",
                Mensaje = "",
                Nosis = "SI"
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

            var resultado = target.ConfirmarContrato(It.IsAny<int>());
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

            var resultado = target.ConfirmarContrato(It.IsAny<int>());
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

        }

        [Test]
        public void ConfirmarContratoErrorContratoNulo()
        {

            var resultado = target.ConfirmarContrato(It.IsAny<int>());
            Assert.That(resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

        }



        [Test]
        public void GrabarAmpliacionContratoOk()
        {
            var oContrato = new Contrato()
            {
                ContratoId = 1,
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
                ContratoId = 1,
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
                ContratoId = 1,
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
                diasHabiles.Add(new DateTime(DateTime.Now.Year, DateTime.Now.Month, i));
            }
            diasHabilesAgentMock.Setup(y => y.ObtenerDiasHabiles()).Returns(diasHabiles);

            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContrato);
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
            for(var i =1; i<= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);i++)
            {
                diasHabiles.Add(new DateTime(DateTime.Now.Year, DateTime.Now.Month, i));
            }
            for (var i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month); i++)
            {
                diasHabiles.Add(new DateTime(DateTime.Now.Year, DateTime.Now.Month, i));
            }
            diasHabilesAgentMock.Setup(y => y.ObtenerDiasHabiles()).Returns(diasHabiles);
            relacionCorredorProveedorAgentMock.Setup(y => y.ObtenerRelacionCorredorProveedor(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContrato);
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
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(new Contrato() { EstadoId = (int)EnumEstadoContrato.Finalizado });
            var resultado = target.FinalizarContrato(It.IsAny<int>(), It.IsAny<string>());
            Assert.That(resultado.HayError);
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(new Contrato() { EstadoId = (int)EnumEstadoContrato.Rechazado });
            resultado = target.FinalizarContrato(It.IsAny<int>(), It.IsAny<string>());
            Assert.That(resultado.HayError);
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(new Contrato() { EstadoId = (int)EnumEstadoContrato.Pendiente });
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
                diasHabiles.Add(new DateTime(DateTime.Now.Year, DateTime.Now.Month, i));
            }
            diasHabilesAgentMock.Setup(y => y.ObtenerDiasHabiles()).Returns(diasHabiles);

            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContrato);

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<DescuentoBonificacion, DescuentoBonificacionDto>>>(), It.IsAny<Expression<Func<DescuentoBonificacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
              .Throws(new Exception("Error generico"));

            var resultado = target.FinalizarContrato(It.IsAny<int>(), It.IsAny<string>());
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
            Assert.That(resultado.HayError);
        }




        [Test]
        public void BorrarContratoOk()
        {
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(new Contrato() { EstadoId = (int)EnumEstadoContrato.Pendiente, Comercial = new Comercial() { ComercialId = 1 } });
            comercialManagerMock.Setup(y => y.CadenaComerciales(It.IsAny<int>())).Returns(new List<int>());
            var resultado = target.BorrarContrato(new Contrato() { ContratoId = 1, EstadoId = (int)EnumEstadoContrato.Pendiente });

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void BorrarContratoError()
        {
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(new Contrato() { EstadoId = (int)EnumEstadoContrato.Finalizado });
            var resultado = target.BorrarContrato(new Contrato() { ContratoId = 1 });

            Assert.That(resultado.HayErrores);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
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

            var resultado = target.TraerCalidadesPorContrato(1);

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

            var resultado = target.TraerContrato(1);

            Assert.That(resultado.AperturaPrecios.Count == 1);
            Assert.That(resultado.Calidades.Count == 1);
            Assert.That(resultado.Descuentos.Count == 1);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void EnviarMailPendienteOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, AvisoContratoDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
              .Returns(new List<AvisoContratoDto>() { new AvisoContratoDto {
                    ContratoId = 1,
                RazonSocial = "ALA",
                Cantidad = 2,
                Precio = 1,
                Moneda = "ARS  ",
                Fecha = "01-02-2019",
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
            proveedorManagerMock.Setup(y => y.GetEmailUserActiveDirectory(It.IsAny<string>())).Returns("mparisi@baufest.com");

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, string>>>()))
                .Returns("marcos ignacio");

            ConfigurationManager.AppSettings["CredentialUserName"] = "dataagro.baufest@gmail.com";
            ConfigurationManager.AppSettings["SmtpServerPort"] = "587";
            ConfigurationManager.AppSettings["SmtpServer"] = "smtp.gmail.com";
            ConfigurationManager.AppSettings["SmtpAnonimo"] = "N";
            ConfigurationManager.AppSettings["EnableSSL"] = "S";

            //FALTA -> Preguntar a Ale
            target.EnviarMailPendiente();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Contrato, AvisoContratoDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);

        }

        [Test]
        public void EnviarMailPendienteError()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, AvisoContratoDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
              .Returns(new List<AvisoContratoDto>() { new AvisoContratoDto {
                    ContratoId = 1,
                RazonSocial = "ALA",
                Cantidad = 2,
                Precio = 1,
                Moneda = "ARS  ",
                Fecha = "01-02-2019",
                ComercialCreadorAD = "pari",
                NombreApellido = "si eme"
              } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, ComercialDto>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
             .Returns(new List<ComercialDto>() {
                 new ComercialDto {
                    ComercialId = 1,
                    IdActiveDirectory = "mparisi"
              }});
            proveedorManagerMock.Setup(y => y.GetEmailUserActiveDirectory(It.IsAny<string>())).Throws(new Exception("Error GetEmailUserActiveDirectory"));

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, string>>>()))
                .Returns("marcos ignacio");

            ConfigurationManager.AppSettings["CredentialUserName"] = "dataagro.baufest@gmail.com";
            ConfigurationManager.AppSettings["SmtpServerPort"] = "587";
            ConfigurationManager.AppSettings["SmtpServer"] = "smtp.gmail.com";
            ConfigurationManager.AppSettings["SmtpAnonimo"] = "N";
            ConfigurationManager.AppSettings["EnableSSL"] = "S";

            //FALTA -> Preguntar a Ale
            target.EnviarMailPendiente();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Contrato, AvisoContratoDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);

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
                diasHabiles.Add(new DateTime(DateTime.Now.Year, DateTime.Now.Month, i));
            }
            diasHabilesAgentMock.Setup(y => y.ObtenerDiasHabiles()).Returns(diasHabiles);


            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(oContrato);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<CorredorProveedor, bool>>>())).Returns(true);
            ConfigurationManager.AppSettings["ValorPruebaSap"] = "1";
            relacionCorredorProveedorAgentMock.Setup(y => y.ObtenerRelacionCorredorProveedor(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<AperturaPrecio>() { new AperturaPrecio { ConceptoAperturaPrecio = new ConceptoAperturaPrecio { CodigoSap = "FI", Descripcion = "FINANCIERO", Id = 1 } } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<Contrato>() {
                    oContrato
                 });

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

            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Throws(new Exception("Error obtener contrato"));

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<Contrato>() {
                    oContrato
                 });

            target.FinalizacionAutomatica(It.IsAny<string>());
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }


        [Test]
        public void BorradoAutomaticoOk()
        {
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(new Contrato() { EstadoId = (int)EnumEstadoContrato.Pendiente, Comercial = new Comercial() { ComercialId = 1 } });
            comercialManagerMock.Setup(y => y.CadenaComerciales(It.IsAny<int>())).Returns(new List<int>());
            var oContrato = new Contrato() { ContratoId = 1, EstadoId = (int)EnumEstadoContrato.Pendiente };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<Contrato>() {
                    oContrato
                });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FechaFeriado, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<FechaFeriado>() { });
            diasHabilesMock.Setup(y => y.ObtenerDiasHabiles()).Returns(new List<DateTime>() { DateTime.Today.AddDays(-2) });

            target.BorradoAutomatico();
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void BorradoAutomaticoError()
        {
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Throws(new Exception("Error obtener contrato"));
            var oContrato = new Contrato() { ContratoId = 1, EstadoId = (int)EnumEstadoContrato.Pendiente };
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
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, AvisoContratoDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<AvisoContratoDto>() { new AvisoContratoDto {
                    ContratoId = 1,
                RazonSocial = "ALA",
                Cantidad = 2,
                Precio = 1,
                Moneda = "ARS  ",
                Fecha = "01-02-2019",
                ComercialCreadorAD = "pari",
                NombreApellido = "si eme"
              },
               new AvisoContratoDto {
                    ContratoId = 1,
                RazonSocial = "MER",
                Cantidad = 2,
                Precio = 1,
                Moneda = "ARS  ",
                Fecha = "01-02-2012",
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
                ContratoId = 1,
                EstadoId = (int)EnumEstadoContrato.Finalizado
            };

            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(contrato);

            eliminarContratoAgentMock.Setup(y => y.Eliminar(It.IsAny<Contrato>())).Returns("OK");

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<DescuentoBonificacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<DescuentoBonificacion>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Calidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Calidad>());

            var resultado = target.AnularContrato(contrato, It.IsAny<string>());

            Assert.IsNotNull(resultado);
            Assert.That(!resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void AnularContratoError()
        {
            var contrato = new Contrato
            {
                ContratoId = 1,
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
        public void AnularContratoErrorSio()
        {
            var contrato = new Contrato
            {
                ContratoId = 1,
                EstadoId = (int)EnumEstadoContrato.Finalizado
            };

            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<int>())).Returns(contrato);
            eliminarContratoAgentMock.Setup(y => y.Eliminar(It.IsAny<Contrato>())).Returns("Error SIO");
            repositorioMock.Setup(y => y.Listar<Comercial>(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                    .Returns(new List<Comercial> { new Comercial { ComercialId = 70, Apellido = "Parisi", Nombres = "Marcos", PerfilId = 4 } });

            var resultado = target.AnularContrato(contrato, It.IsAny<string>());

            Assert.IsNotNull(resultado);
            Assert.That(resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void AnularContratoErrorEstado()
        {
            var contrato = new Contrato
            {
                ContratoId = 1,
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
                ContratoId = 1,
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

    }

}
