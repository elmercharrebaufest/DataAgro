using Autofac.Extras.NLog;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Criterios;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;


namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class CupoManagerTest
    {
        private CupoManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<ICrearCupoAgent> crearCupoAgentMock;
        private Mock<IEliminarCupoAgent> eliminarCupoAgentMock;
        private Mock<IClienteStopAgent> clienteStopMock;
        private Mock<IModificarCupoAgent> modificarCupoAgentMock;
        private Mock<IProveedorManager> proveedorManagerMock;
        private Mock<IMailManager> mailManagerMock;
        private Mock<IServicioCriterios> servicioCriterioMock;
        private Mock<IDisponibilidadCuposAgent> disponibilidadCuposAgentMock;
        private Mock<ICriterioCDWarrantAgent> criterioCDWarrantAgentMock;
        private Mock<ILogDataAgroManager> logDataAgroManagerMock;
        private Mock<IComercialManager> comercialManagerMock;
        private Mock<IServicioRepositorioScatoAgent> servicioScato;
        private Mock<IHttpContextManager> contextoManager;
        private Mock<IAltaTempranaAgent> altaTempranaAgent;
        private Mock<ICumplimientoCuposAgent> cumplimientoCuposAgent;
        private Mock<IContratoKgPendienteAgent> contratoKgPendienteAgent;
        private Mock<IContratoManager> contratoManager;

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            crearCupoAgentMock = new Mock<ICrearCupoAgent>();
            eliminarCupoAgentMock = new Mock<IEliminarCupoAgent>();
            clienteStopMock = new Mock<IClienteStopAgent>();
            modificarCupoAgentMock = new Mock<IModificarCupoAgent>();
            proveedorManagerMock = new Mock<IProveedorManager>();
            mailManagerMock = new Mock<IMailManager>();
            servicioCriterioMock = new Mock<IServicioCriterios>();
            disponibilidadCuposAgentMock = new Mock<IDisponibilidadCuposAgent>();
            criterioCDWarrantAgentMock = new Mock<ICriterioCDWarrantAgent>();
            logDataAgroManagerMock = new Mock<ILogDataAgroManager>();
            comercialManagerMock = new Mock<IComercialManager>();
            servicioScato = new Mock<IServicioRepositorioScatoAgent>();
            contextoManager = new Mock<IHttpContextManager>();
            altaTempranaAgent = new Mock<IAltaTempranaAgent>();
            cumplimientoCuposAgent = new Mock<ICumplimientoCuposAgent>();
            contratoKgPendienteAgent = new Mock<IContratoKgPendienteAgent>();
            contratoManager = new Mock<IContratoManager>();
            ConfigurationManager.AppSettings["ValorPruebaSap"] = "1";
            target = new CupoManager(repositorioMock.Object, logger.Object, crearCupoAgentMock.Object,
                eliminarCupoAgentMock.Object, clienteStopMock.Object, modificarCupoAgentMock.Object, proveedorManagerMock.Object,
                mailManagerMock.Object, servicioCriterioMock.Object, disponibilidadCuposAgentMock.Object, criterioCDWarrantAgentMock.Object,
                logDataAgroManagerMock.Object, comercialManagerMock.Object, servicioScato.Object, contextoManager.Object, altaTempranaAgent.Object,
                cumplimientoCuposAgent.Object, contratoKgPendienteAgent.Object, contratoManager.Object);
            repositorioMock.Setup(x => x.Obtener<Configuracion>(1)).Returns(new Configuracion { ConexionABMStop = true });
        }

        [Test]
        public void GrabarCupoTestOk()
        {
            var cupo = new Cupo
            {
                Id = 0,
                ProveedorId = 1,
                MaterialId = 1,
                CentroId = 1,
                ZonaCupoId = 1,
                FechaIngreso = new DateTime(2099, 11, 19)
            };
            var configuracion = new ConfiguracionCupoDto()
            {
                LimiteCupo = 100,
                CentroId = 1,
                CierreCupera = false

            };
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<ConfiguracionCupo, ConfiguracionCupoDto>>>(), It.IsAny<Expression<Func<ConfiguracionCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                  .Returns(new List<ConfiguracionCupoDto>() { new ConfiguracionCupoDto { Id = 1 } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<LimiteCupo, ConfiguracionCupoDto>>>(), It.IsAny<Expression<Func<LimiteCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
           .Returns(new List<ConfiguracionCupoDto>());
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
               .Returns(new List<Cupo>() { new Cupo { Id = 1 } });
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<Expression<Func<Cupo, CupoDto>>>()))
                .Returns(new CupoDto { Id = 1 });
            repositorioMock.Setup(x => x.Obtener<Cupo>(It.IsAny<Expression<Func<Cupo, bool>>>())).Returns(new Cupo { Id = 1 });

            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { ProveedorId = 1 });
            repositorioMock.Setup(y => y.Obtener<Material>(It.IsAny<int>()))
                .Returns(new Material { MaterialId = 1 });
            repositorioMock.Setup(y => y.Obtener<Centro>(It.IsAny<int>()))
                .Returns(new Centro { Id = 1 });
            repositorioMock.Setup(y => y.Obtener<ZonaCupo>(It.IsAny<int>()))
                .Returns(new ZonaCupo { Id = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<LimiteCupo, bool>>>())).Returns(new LimiteCupo { ConfiguracionCupoId = 1, ZonaCupoId = 1, CantidadCupo = 1 });
            repositorioMock.Setup(y => y.Contar(It.IsAny<Expression<Func<Cupo, bool>>>())).Returns(0);

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ContactoComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ContactoComercial>() { new ContactoComercial { Email1 = "b@b.com" } });
            mailManagerMock.Setup(y => y.GetEmailUserActiveDirectory(It.IsAny<string>())).Returns("a@a.com");
            crearCupoAgentMock.Setup(x => x.Crear(It.IsAny<Cupo>(), It.IsAny<int>()))
                .Returns(new List<string>() { "a" });
            repositorioMock.Setup(y => y.ObtenerMayor<Cupo, int>(It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<Expression<Func<Cupo, int>>>()))
                    .Returns(new Cupo() { MaterialId = 1, Id = 1 });
            repositorioMock.Setup(x => x.Agregar(It.IsAny<Cupo>()));
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ConfiguracionCupo, bool>>>(), It.IsAny<Expression<Func<ConfiguracionCupo, int>>>()))
               .Returns(20);

            var result = target.GrabarCupo(cupo, new List<DiaCupo>() {
                new DiaCupo { Cantidad=1, Fecha = new DateTime(2099,11,10)},
                new DiaCupo { Fecha= new DateTime(2099,11,10),Cantidad=0},
            new DiaCupo { Fecha= new DateTime(2099,11,10),Cantidad=1}});

            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Material>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Centro>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<ZonaCupo>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.AgregarTodos(It.IsAny<List<Cupo>>(), null), Times.Exactly(2));
            crearCupoAgentMock.Verify(x => x.Crear(It.IsAny<Cupo>(), It.IsAny<int>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }

        [Test]
        public void ModificarCupoTestOk()
        {
            var cupo = new Cupo
            {
                Id = 1,
                ProveedorId = 1,
                MaterialId = 1,
                CentroId = 1,
                Centro = new Centro { Acopio = false },
                ZonaCupoId = 1,
                CupoStop = 1,
                CupoSap = "aa",
                FechaIngreso = new DateTime(2019, 11, 19)
            };
            var configuracion = new ConfiguracionCupoDto()
            {
                LimiteCupo = 100,
                CentroId = 1,
                CierreCupera = false

            };
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<ConfiguracionCupo, ConfiguracionCupoDto>>>(), It.IsAny<Expression<Func<ConfiguracionCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ConfiguracionCupoDto>() { new ConfiguracionCupoDto { Id = 1 } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<LimiteCupo, ConfiguracionCupoDto>>>(), It.IsAny<Expression<Func<LimiteCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
           .Returns(new List<ConfiguracionCupoDto>());
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
               .Returns(new List<Cupo>() { new Cupo { Id = 1 } });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { ProveedorId = 1 });
            repositorioMock.Setup(y => y.Obtener<Material>(It.IsAny<int>()))
                .Returns(new Material { MaterialId = 1 });
            repositorioMock.Setup(y => y.Obtener<Centro>(It.IsAny<int>()))
                .Returns(new Centro { Id = 1 });
            repositorioMock.Setup(y => y.Obtener<ZonaCupo>(It.IsAny<int>()))
                .Returns(new ZonaCupo { Id = 1 });
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { ConexionABMStop = true });
            repositorioMock.Setup(y => y.Obtener<Cupo>(It.IsAny<int>())).Returns(cupo);

            modificarCupoAgentMock.Setup(x => x.Modificar(It.IsAny<Cupo>()))
                .Returns("Ok");

            repositorioMock.Setup(x => x.Agregar(It.IsAny<Cupo>()));
            var result = target.GrabarCupo(cupo, new List<DiaCupo>());

            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Material>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Centro>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<ZonaCupo>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Configuracion>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Cupo>(It.IsAny<int>()), Times.Once);
            modificarCupoAgentMock.Verify(x => x.Modificar(It.IsAny<Cupo>()), Times.Once);
            clienteStopMock.Verify(x => x.ModificarCupo(It.IsAny<Cupo>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }

        [Test]
        public void EliminarCupoTest()
        {
            repositorioMock.Setup(x => x.Obtener<Cupo>(It.IsAny<int>())).Returns(new Cupo { EstadoCupoId = 1, CupoStop = 1, CupoSap = "a", Centro = new Centro { Acopio = false } });
            eliminarCupoAgentMock.Setup(x => x.Eliminar(It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            clienteStopMock.Setup(x => x.EliminarCupo(It.IsAny<Cupo>())).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            repositorioMock.Setup(x => x.GuardarCambios());

            var result = target.EliminarCupo(1, "a", true);
            repositorioMock.Verify(x => x.Obtener<Cupo>(It.IsAny<int>()), Times.Once);
            clienteStopMock.Verify(x => x.EliminarCupo(It.IsAny<Cupo>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            eliminarCupoAgentMock.Verify(x => x.Eliminar(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        }

        [Test]
        public void ValidarCupoTestError()
        {
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>()))
                .Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>()))
                .Returns(new SISA());
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668", Deshabilitado = false });
            altaTempranaAgent.Setup(y => y.ObtenerAlta(It.IsAny<string>(), It.IsAny<string>())).Returns(new AltaTempranaNRCODto { ProveedorGrano = "No" });
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(new Centro { Id = 1, NoPropio = false });
            var result = target.Validar(new Cupo() { Fason = true, MaterialId = 3, ProveedorId = 1, FechaIngreso = new DateTime(2019, 12, 30) }, 0, new DateTime(2019, 12, 1));

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<SISA, bool>>>()), Times.Once);

            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual(7, result.Errores.Count);
        }
        //[Test]
        //public void TraerCuposTablaTestOk()
        //{
        //    repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<TraerTodosCupos>())).Returns(
        //        new KendoGrid<CupoDto>(new List<CupoDto>() { new CupoDto { Acopio= false,Calidad="a",Centro="a",CentroId=1,Comercial="a",ComercialId=1,CupoSap="a",CupoStop="a",Destinatario="a",EstadoCupo="a", EstadoCupoId=1,Fason=false,
        //        FechaGeneracion = new DateTime(2019,1,1),FechaIngreso=new DateTime(2019,1,1),FleteProcedencia=false,HoraIngreso=new DateTime(2019,1,1),Id=1,
        //        Material="a",MaterialId=1,MensajeError="",Observaciones="a",Proveedor="a",ProveedorId=1,ZonaCupo="a",ZonaCupoId=1} }, 1));

        //    var result = target.TraerCuposTabla(new KendoGridMvcRequest(), new List<int>() { 1, 2 });

        //    Assert.NotNull(result);
        //    repositorioMock.Verify(x => x.ObtenerConsultaEscalar(It.IsAny<TraerTodosCupos>()), Times.Once);
        //}
        [Test]
        public void EliminarCupoTestOk()
        {
            repositorioMock.Setup(y => y.Obtener<Cupo>(It.IsAny<int>()))
                .Returns(new Cupo() { EstadoCupoId = 1, CupoStop = 1, CupoSap = "a", Centro = new Centro { Acopio = false } });
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { ConexionABMStop = true });
            eliminarCupoAgentMock.Setup(y => y.Eliminar(It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            clienteStopMock.Setup(y => y.EliminarCupo(It.IsAny<Cupo>())).Returns(new Resultado() { Errores = new List<ErrorMessage>() });
            var result = target.EliminarCupo(1, "a", false);

            repositorioMock.Verify(x => x.Obtener<Cupo>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Configuracion>(It.IsAny<int>()), Times.Once);
            eliminarCupoAgentMock.Verify(y => y.Eliminar(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            clienteStopMock.Verify(y => y.EliminarCupo(It.IsAny<Cupo>()), Times.Once);

            Assert.NotNull(result);

        }
        [Test]
        public void TransmitirCuposTest()
        {
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { ConexionABMStop = true });
            eliminarCupoAgentMock.Setup(x => x.Eliminar(It.IsAny<string>(), It.IsAny<string>())).Returns("OK");

            var result = target.TransmitirCupos(new List<string>());
            repositorioMock.Verify(x => x.Obtener<Configuracion>(It.IsAny<int>()), Times.Once);
            clienteStopMock.Verify(x => x.CrearCupo(It.IsAny<List<string>>()), Times.Once);
            Assert.IsFalse(result.HayError);
        }

        [Test]
        public void TransmitirJobCuposTest()
        {
            target.TransmitirCupos();

            clienteStopMock.Verify(x => x.TransmitirJobCupos(), Times.Once);
        }
        [Test]
        public void ConsultarCuposDiariosTest()
        {
            clienteStopMock.Setup(x => x.ConsultarCuposDiarios()).Returns(new List<RespuestaCupoStop>() { new RespuestaCupoStop { token="a",
            codGrano=1,codLocalidadDestino=1,
             creado="a",cuitCorredorC="a",cuitDestinatario="a",desvio="n",
            cuitDestino="a",estado="a",fecha="a",idCupo=1,idCupoEstado=1,idCupoTerminal="a",idTerminal=1 } });
            var result = target.ConsultarCuposDiarios();

            clienteStopMock.Verify(x => x.ConsultarCuposDiarios(), Times.Once);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }
        [Test]
        public void ObtenerCupoTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<Expression<Func<Cupo, CupoDto>>>())).Returns(new CupoDto
            {
                Id = 1,
                Calidad = "a",
                Centro = "a",
                CentroId = 1,
                ComercialId = 1,
                Destinatario = "a",
                Fason = true,
                FleteProcedencia = true,
                FechaIngreso = DateTime.Now,
                MaterialId = 1,
                Material = "a",
                Observaciones = "a",
                ProveedorId = 1,
                Proveedor = "a",
                ZonaCupoId = 2,
                ZonaCupo = "a"
            });
            var result = target.ObtenerCupo(1, null);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<Expression<Func<Cupo, CupoDto>>>()), Times.Once);
            Assert.IsNotNull(result);
        }
        [Test]
        public void EliminarVariosCupoTestOk()
        {
            repositorioMock.Setup(y => y.Obtener<Cupo>(1))
                .Returns(new Cupo() { EstadoCupoId = 1, CupoStop = 1, CupoSap = "a", Centro = new Centro { Acopio = false } });
            repositorioMock.Setup(y => y.Obtener<Cupo>(2))
                .Returns(new Cupo() { EstadoCupoId = 1, CupoStop = 1, CupoSap = "a", Centro = new Centro { Acopio = false } });
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { ConexionABMStop = true });
            eliminarCupoAgentMock.Setup(y => y.Eliminar(It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            clienteStopMock.Setup(y => y.EliminarCupo(It.IsAny<Cupo>())).Returns(new Resultado() { Errores = new List<ErrorMessage>() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Cupo, CupoDto>>>(), It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
               .Returns(new List<CupoDto>() {new CupoDto
                {
                    Id = 1,
                    CentroId = 1,
                    ComercialId = 1,
                    Destinatario = "a",
                    MaterialId = 1,
                    ProveedorId = 1,
                    ZonaCupoId = 2,
                }});
            var result = target.EliminarVarios(new List<int>() { 1, 2 }, "a");

            repositorioMock.Verify(x => x.Obtener<Cupo>(It.IsAny<int>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Obtener<Configuracion>(It.IsAny<int>()), Times.Exactly(2));
            eliminarCupoAgentMock.Verify(y => y.Eliminar(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            clienteStopMock.Verify(y => y.EliminarCupo(It.IsAny<Cupo>()), Times.Exactly(2));

            Assert.NotNull(result);
        }
        [Test]
        public void ObtenerCodigoSapTestOk()
        {
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<Expression<Func<Cupo, string>>>()))
                .Returns("a");
            var result = target.ObtenerCodigoSap(1);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<Expression<Func<Cupo, string>>>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual("a", result);
        }

        [Test]
        public void ObtenerSugerenciaCupoTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, MaterialIni>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
              .Returns(new List<MaterialIni>() { new MaterialIni { MaterialId = 1, Descripcion = "Soja" } });
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<ObtenerUltimaFormula>()))
               .Returns(new Formula
               {
                   Id = 1,
                   CuposDesde = DateTime.Now.Date,
                   CuposHasta = DateTime.Now.Date,
                   NegociosDesde = DateTime.Now.Date,
                   NegociosHasta = DateTime.Now.Date,
                   Fecha = DateTime.Now.Date,
                   CentroId = 1,
                   CriterioId = 1,
                   Criterio =
               new CriterioRaiz
               {
                   Id = 1,
                   Prioridad = 100,
                   Hijos = new List<Criterio> {
                       new CriterioEsContratoAFijar { Id = 3, PadreId = 2, Prioridad = 60 },
                        new CriterioEsContratoAPrecio { Id = 4, PadreId = 2, Prioridad = 40 },
                    }
               }
               });

            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<ObtenerSugerencias>()))
               .Returns(new List<SugerenciaCupoDto>() {new SugerenciaCupoDto
                {
                    Id = 1,
                    CentroId = 1,
                    ComercialId = 1,
                    Destinatario = "a",
                    MaterialId = 1,
                    ProveedorId = 1,
                    ZonaCupoId = 2,
                    PuntuacionesString = "{\"CriterioRaiz\":0.4}",
                    ProveedorDesc = "Parisi",
                    CantidadDeCupos = 1,
                    Aceptado = true,
                    MonedaDesc = "USDM",
                    ProveedorCUIT = "23343434",
                    TipoNegocioDesc = "ESPACIO DINAMICO",
                    ContratoSAP = "3343434",
               }
               });

            var result = target.ObtenerSugerenciaCupo(1, null);
            Assert.IsNotNull(result);
        }

        [Test]
        public void RechazarSugerenciaCupoTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SugerenciaCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<SugerenciaCupo>() {new SugerenciaCupo
                {
                    Id = 1,
                    CentroId = 1,
                    ComercialId = 1,
                    Destinatario = "a",
                    MaterialId = 1,
                    ProveedorId = 1,
                    ZonaCupoId = 2,
                    Puntuaciones = "{\"CriterioRaiz\":0.4}",
                    FechaSugerida = DateTime.Now.Date
                }});
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CierreCupera, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                        .Returns(new List<CierreCupera>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SugerenciaPorComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                        .Returns(new List<SugerenciaPorComercial> { new SugerenciaPorComercial { Total = 99, CentroId = 1, ComercialId = 1, Fecha = DateTime.Now.Date, MaterialId = 1, Id = 1 } });
            var result = target.RechazarSugerenciaCupo(new List<int> { 1 }, "");

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<SugerenciaCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>())
                , Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }

        [Test]
        public void AceptarSugerenciaCupo()
        {


            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<ZonaCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
               .Returns(new List<ZonaCupo>() { new ZonaCupo { Id = 1, Descripcion = "Bs As" } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
               .Returns(new List<Comercial>() { new Comercial { ComercialId = 1, GrupoDeCompras = new GrupoDeCompras { Descripcion = "Bs As" } } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SugerenciaPorComercial, bool>>>())).Returns(new SugerenciaPorComercial { Id = 1, Total = 10 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CierreCupera, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<CierreCupera>());
            //sugerencias
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SugerenciaCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<SugerenciaCupo>() {new SugerenciaCupo
                {
                    Id = 1,
                    CentroId = 1,
                    ComercialId = 1,
                    Destinatario = "a",
                    MaterialId = 1,
                    ProveedorId = 1,
                    ZonaCupoId = 1,
                    ZonaCupo =  new ZonaCupo {CodigoSap="",Descripcion="",Id=1 } ,
                    Puntuaciones = "{\"CriterioRaiz\":0.4}",
                    FechaSugerida = DateTime.Now.Date,
                    Centro = new Centro { CodigoSap = "0002321"},
                    CantidadDeCupos = 10,
                    Proveedor = new Proveedor {RazonSocial = "PARISI"}
                }});
            var configuracion = new ConfiguracionCupoDto()
            {
                LimiteCupo = 100,
                CentroId = 1,
                CierreCupera = false

            };
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<ConfiguracionCupo, ConfiguracionCupoDto>>>(), It.IsAny<Expression<Func<ConfiguracionCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                 .Returns(new List<ConfiguracionCupoDto>() { new ConfiguracionCupoDto { Id = 1 } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<LimiteCupo, ConfiguracionCupoDto>>>(), It.IsAny<Expression<Func<LimiteCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
           .Returns(new List<ConfiguracionCupoDto>());
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
               .Returns(new List<Cupo>() { new Cupo { Id = 1 } });
            //disponibilidad en planta
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ConfiguracionCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ConfiguracionCupo>() {new ConfiguracionCupo
                {
                    Id = 1,
                    CentroId = 1,
                    MaterialId = 1,
                    Fecha=DateTime.Now.Date,
                    LimiteCupo=10,
                    CantidadCupo = new List<LimiteCupo>(){ new LimiteCupo {CantidadCupo=10,ZonaCupoId=1, ZonaCupo = new ZonaCupo {CodigoSap="",Descripcion="",Id=1 } } }
                }});
            //cupo
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<Cupo>() {new Cupo
                {
                    Id = 1,
                    CentroId = 1,
                    ComercialId = 1,
                    Destinatario = "a",
                    MaterialId = 1,
                    ProveedorId = 1,
                    ZonaCupoId = 1,
                    FechaIngreso = DateTime.Now.Date,
                    FechaGeneracion = DateTime.Now.Date
                }});
            //formula
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<ObtenerUltimaFormula>()))
               .Returns(new Formula
               {
                   Id = 1,
                   CuposDesde = DateTime.Now.Date,
                   CuposHasta = DateTime.Now.Date,
                   NegociosDesde = DateTime.Now.Date,
                   NegociosHasta = DateTime.Now.Date,
                   Fecha = DateTime.Now.Date,
                   CentroId = 1,
                   CriterioId = 1,
                   Criterio =
               new CriterioRaiz
               {
                   Id = 1,
                   Prioridad = 100,
                   Hijos = new List<Criterio> {
                       new CriterioEsContratoAFijar { Id = 3, PadreId = 2, Prioridad = 60 },
                        new CriterioEsContratoAPrecio { Id = 4, PadreId = 2, Prioridad = 40 },
                    }
               }
               });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
            .Returns(new Proveedor { ProveedorId = 1, RazonSocial = "Parisi" });
            repositorioMock.Setup(y => y.Obtener<Material>(It.IsAny<int>()))
                .Returns(new Material { MaterialId = 1 });
            repositorioMock.Setup(y => y.Obtener<Centro>(It.IsAny<int>()))
                .Returns(new Centro { Id = 1 });
            repositorioMock.Setup(y => y.Obtener<ZonaCupo>(It.IsAny<int>()))
                .Returns(new ZonaCupo { Id = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<LimiteCupo, bool>>>())).Returns(new LimiteCupo { ConfiguracionCupoId = 1, ZonaCupoId = 1, CantidadCupo = 1 });
            repositorioMock.Setup(y => y.Contar(It.IsAny<Expression<Func<Cupo, bool>>>())).Returns(0);

            crearCupoAgentMock.Setup(x => x.Crear(It.IsAny<Cupo>(), It.IsAny<int>()))
                .Returns(new List<string>() { "a" });
            repositorioMock.Setup(x => x.Agregar(It.IsAny<Cupo>()));

            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<Expression<Func<Cupo, CupoDto>>>()))
                .Returns(new CupoDto { Id = 1 });
            repositorioMock.Setup(x => x.Obtener<Cupo>(It.IsAny<Expression<Func<Cupo, bool>>>())).Returns(new Cupo { Id = 1 });

            var result = target.AceptarSugerenciaCupo(new List<SugerenciaCupoDto> { new SugerenciaCupoDto { Id = 1, CantidadDeCupos = 1, CantidadFleteProcedencia = 1, MaterialId = 1, FechaSugerida = DateTime.Now.Date, CentroId = 1, ZonaCupoId = 1, ZonaDescrip = "" } });


            //repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<SugerenciaCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>())
            //    , Times.Once);
            //repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.NotNull(result);
        }

        [Test]
        public void CrearSugerenciaCupo()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SugerenciaCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<SugerenciaCupo>() {new SugerenciaCupo
                {
                    Id = 1,
                    CentroId = 1,
                    ComercialId = 1,
                    Destinatario = "a",
                    MaterialId = 1,
                    ProveedorId = 1,
                    ZonaCupoId = 1,
                    ZonaCupo =  new ZonaCupo {CodigoSap="",Descripcion="",Id=1 } ,
                    Puntuaciones = "{\"CriterioRaiz\":0.4}",
                    FechaSugerida = DateTime.Now.Date
                }});
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ConfiguracionCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ConfiguracionCupo>() {new ConfiguracionCupo
                {
                    Id = 1,
                    CentroId = 1,
                    MaterialId = 1,
                    Fecha=DateTime.Now.Date,
                    LimiteCupo=10,
                    CantidadCupo = new List<LimiteCupo>(){ new LimiteCupo {CantidadCupo=10,ZonaCupoId=1, ZonaCupo = new ZonaCupo {CodigoSap="",Descripcion="",Id=1 } } },
                    LimiteAlgoritmo = 10
                } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<Cupo>() {new Cupo
                {
                    Id = 1,
                    CentroId = 1,
                    ComercialId = 1,
                    Destinatario = "a",
                    MaterialId = 1,
                    ProveedorId = 1,
                    ZonaCupoId = 1,
                    NegocioId = 44,
                    ConfiguracionEspacioDinamicoId=66,
                    FechaIngreso = DateTime.Now.Date,
                    FechaGeneracion = DateTime.Now.Date
                }});
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Cupo, CupoDto>>>(), It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<CupoDto>() {new CupoDto
                {
                    Id = 1,
                    CentroId = 1,
                    ComercialId = 1,
                    Destinatario = "a",
                    MaterialId = 1,
                    ProveedorId = 1,
                    ZonaCupoId = 1,
                    FechaIngreso = DateTime.Now.Date,
                    FechaGeneracion = DateTime.Now.Date,
                    NegocioId = 56,
                    Proveedor = "",
                }});
            var formula = new Formula
            {
                Id = 1,
                CuposDesde = DateTime.Now.Date,
                CuposHasta = DateTime.Now.Date,
                NegociosDesde = DateTime.Now.Date,
                NegociosHasta = DateTime.Now.Date,
                Fecha = DateTime.Now.Date,
                CentroId = 1,
                CriterioId = 1,
                Criterio =
               new CriterioRaiz
               {
                   Id = 1,
                   Prioridad = 100,
                   Hijos = new List<Criterio> {
                        new CriterioEsContratoAFijar { Id = 3, PadreId = 2, Prioridad = 100 }
                    }
               }
            };
            var formulaDto = new FormulaDto
            {
                Id = 1,
                CuposDesde = DateTime.Now.Date,
                CuposHasta = DateTime.Now.Date,
                NegociosDesde = DateTime.Now.Date,
                NegociosHasta = DateTime.Now.Date,
                Fecha = DateTime.Now.Date,
                CentroId = 1,
                CriterioId = 1,
                Criterio =
              new CriterioRaiz
              {
                  Id = 1,
                  Prioridad = 100,
                  Hijos = new List<Criterio> {
                        new CriterioEsContratoAFijar { Id = 3, PadreId = 2, Prioridad = 100 }
                   }
              }
            };
            criterioCDWarrantAgentMock.Setup(x => x.ConsultarContratoWarrant(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(new List<BasicoContrato>());
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<ObtenerUltimaFormula>()))
               .Returns(formula);
            repositorioMock.Setup(y => y.Obtener<Formula>(It.IsAny<int>()))
           .Returns(formula);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PrecioPizarra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
            .Returns(new List<PrecioPizarra>() { new PrecioPizarra { Id = 1, FechaDesde = DateTime.Now.Date, FechaHasta = DateTime.Now.Date.AddDays(1), MaterialId = 1, MonedaId = "ARP  ", PizarraId = 1, Precio = 600, UnidadMedida = "Tons" } });
            repositorioMock.Setup(y => y.Obtener<Material>(It.IsAny<int>()))
                .Returns(new Material { MaterialId = 1 });
            repositorioMock.Setup(y => y.ObtenerPrimero<TipoNegocio>(It.IsAny<Expression<Func<TipoNegocio, bool>>>()))
                .Returns(new TipoNegocio { TipoNegocioId = 7, Descripcion = "ESPACIO DINAMICO" });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ZonaCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ZonaCupo>() { new ZonaCupo { Id = 1, Descripcion = "" } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<LimiteCupo, bool>>>())).Returns(new LimiteCupo { ConfiguracionCupoId = 1, ZonaCupoId = 1, CantidadCupo = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, SugerenciaCupoDto>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<SugerenciaCupoDto>() {new SugerenciaCupoDto
                {
                    StandardDeCalidad =  "",
                    ComercialId =1,
                    Destinatario = "b",
                    ProveedorId =1,
                    ZonaDescrip ="",
                    CantidadDeCupos = 2,
                    DestinoId = 1,
                    NegocioId = 1,
                    MaterialId = 1,
                    MonedaId = "ARP  ",
                    Precio = 500,
                    FechaDesde = DateTime.Now.Date,
                    FechaHasta = DateTime.Now.Date.AddDays(1),
                    TipoNegocioId =2,
                    ContratoSAP = "aa",
                    KgNegocio = 60000,
                    KgPendienteAplicar = 60000
                }});
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ConfiguracionEspacioDinamico, SugerenciaCupoDto>>>(), It.IsAny<Expression<Func<ConfiguracionEspacioDinamico, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
              .Returns(new List<SugerenciaCupoDto>() {new SugerenciaCupoDto
                {
                    StandardDeCalidad =  "",
                    ComercialId =1,
                    Destinatario = "a",
                    ProveedorId =1,
                    ZonaDescrip ="",
                    CantidadDeCupos = 22,
                    DestinoId = 1,
                    ConfiguracionEspacioDinamicoId = 1,
                    MaterialId = 1,
                    MonedaId = "ARP  ",
                    Precio = 300,
                    FechaDesde = DateTime.Now.Date,
                    FechaHasta = DateTime.Now.Date.AddDays(1),
                    TipoNegocioId =2,
                    ContratoSAP = ""
                }});

            servicioCriterioMock.SetupSequence(y => y.Calcular(It.IsAny<Criterio>())).Returns(4).Returns(1);

            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                          .Returns(new List<Comercial>() { new Comercial { ComercialId = 1, GrupoDeComprasId = 44, GrupoDeCompras = new GrupoDeCompras { Descripcion = "a" } } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, int>>>())).Returns(1);
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                          .Returns(new List<Centro>() { new Centro { Id = 1, CodigoSap = "1600" } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                          .Returns(new List<Material>() { new Material { MaterialId = 1, Codigo = "1" } });


            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "1" });
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { AlgoritmoKilosMinimosParaSugerencia = 20 });
            repositorioMock.Setup(y => y.Obtener<Centro>(It.IsAny<int>())).Returns(new Centro { NoPropio = false, Id = 1 });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "1" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>()))
                .Returns(new SISA { EstadoCuit = 1 });
            altaTempranaAgent.Setup(y => y.ObtenerAlta(It.IsAny<string>(), It.IsAny<string>())).Returns(new AltaTempranaNRCODto { ProveedorGrano = "No" });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AdministracionCupo, int>>>(), It.IsAny<Expression<Func<AdministracionCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
               .Returns(new List<int>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AdministracionCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<AdministracionCupo>());
            contratoKgPendienteAgent.Setup(x => x.Consultar(It.IsAny<List<ContratoKgPendiente>>())).Returns(new List<ContratoKgPendiente> { new ContratoKgPendiente { ContratoSAP = "aa", KgPendiente = 60000, ContratoId = 1 } });

            target.CrearSugerenciaCupo(It.IsAny<int>(), formulaDto, It.IsAny<ConfiguracionCupo>());
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            repositorioMock.Verify(x => x.AgregarTodos(It.Is<List<SugerenciaCupo>>(y => y.First().NegocioId == 1 && y.First().CantidadDeCupos == 2), null), Times.Once);
            repositorioMock.Verify(x => x.AgregarTodos(It.Is<List<SugerenciaCupo>>(y => y.Last().ConfiguracionEspacioDinamicoId == 1 && y.Last().CantidadDeCupos == 7), null), Times.Once);

            repositorioMock.Verify(x => x.AgregarTodos(It.IsAny<List<SugerenciaCupo>>(), It.IsAny<List<KeyValuePair<string, string>>>()), Times.Once);

        }

        [Test]
        public void TraerCupoDisponibilidadTest()
        {
            disponibilidadCuposAgentMock.Setup(y => y.TraerDisponibilidadCupos(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(new List<DisponibilidadCuposDto>() {new DisponibilidadCuposDto
                {
                    MaterialNombre  ="Soja"
                }});
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                        .Returns(new List<Centro>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ConfiguracionCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                        .Returns(new List<ConfiguracionCupo>());
            var result = target.TraerCupoDisponibilidad(DateTime.Now.Date, DateTime.Now.Date, "", new List<string>(), "");

            Assert.AreEqual(1, result.Count());

        }

        [Test]
        public void BuscarCupoTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Cupo, CupoDto>>>(), It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<CupoDto>(){new CupoDto
            {
                Id = 1,
                Calidad = "a",
                Centro = "a",
                CentroId = 1,
                ComercialId = 1,
                Destinatario = "a",
                Fason = true,
                FleteProcedencia = true,
                FechaIngreso = DateTime.Now,
                MaterialId = 1,
                Material = "a",
                Observaciones = "a",
                ProveedorId = 1,
                Proveedor = "a",
                ZonaCupoId = 2,
                ZonaCupo = "a"
            } });
            var result = target.ListarCupo("asd");

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Cupo, CupoDto>>>(), It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
        }
        [Test]
        public void TraerTodoLosEstadosTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<EstadoCupo, EstadoCupoDto>>>(), It.IsAny<Expression<Func<EstadoCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>())).Returns(new List<EstadoCupoDto>());
            var result = target.TraerTodoLosEstados();
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<EstadoCupo, EstadoCupoDto>>>(), It.IsAny<Expression<Func<EstadoCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.IsNotNull(result);

        }
        [Test]
        public void AltaCupoSAPOk()
        {
            var cupoSave = new Cupo
            {
                FechaIngreso = DateTime.Now,
                MaterialId = 1,
                ProveedorId = 1,
                CentroId = 1,
                ZonaCupoId = 1,
                Observaciones = "",
                Destinatario = "",
                FleteProcedencia = true,
                Calidad = "",
                CupoSap = "MOL",
                EstadoCupoId = 1
            };
            repositorioMock.Setup(x => x.Agregar(It.IsAny<Cupo>()));
            repositorioMock.Setup(x => x.GuardarCambios());

            var resultado = target.AltaCupoSAP(cupoSave);

            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Cupo>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void ActualizarCupoSAPOk()
        {
            var cupoSave = new Cupo
            {
                FechaIngreso = DateTime.Now,
                MaterialId = 1,
                ProveedorId = 1,
                CentroId = 1,
                ZonaCupoId = 1,
                Observaciones = "",
                Destinatario = "",
                FleteProcedencia = true,
                Calidad = "",
                CupoSap = "MOL",
                EstadoCupoId = 1
            };
            repositorioMock.Setup(x => x.GuardarCambios());
            repositorioMock.Setup(y => y.Obtener<Cupo>(It.IsAny<int>())).Returns(new Cupo() { CupoSap = "Mol", EstadoCupoId = 1 });

            var resultado = target.ActualizarCupoSAP(cupoSave);

            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void AceptarCupo()
        {
            repositorioMock.Setup(y => y.Obtener<Cupo>(It.IsAny<int>())).Returns(new Cupo() { CupoSap = "Mol", EstadoCupoId = 1 });
            crearCupoAgentMock.Setup(x => x.Crear(It.IsAny<Cupo>(), It.IsAny<int>())).Returns(new List<string>() { "MOL" });

            repositorioMock.Setup(x => x.GuardarCambios());

            var resultado = target.AceptarCupo(new Cupo() { CupoSap = "Mol", EstadoCupoId = 1, Id = 1 });

            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void TraerTodoConfiguracionCupoTest()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<ConfiguracionCupo, ConfiguracionCupoDto>>>(), It.IsAny<Expression<Func<ConfiguracionCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ConfiguracionCupoDto>() { new ConfiguracionCupoDto { Id = 1 } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<LimiteCupo, ConfiguracionCupoDto>>>(), It.IsAny<Expression<Func<LimiteCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
           .Returns(new List<ConfiguracionCupoDto>());
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
               .Returns(new List<Cupo>() { new Cupo { Id = 1 } });
            var resultado = target.TraerTodaConfiguracionCupoPorDia(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>());
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ConfiguracionCupo, ConfiguracionCupoDto>>>(), It.IsAny<Expression<Func<ConfiguracionCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

        }

        //[Test]
        //public void AnulacionMasivaTest()
        //{
        //    var cupo = new Cupo
        //    {
        //        Id = 1,
        //        ZonaCupo = new ZonaCupo { Descripcion = "BS AS" },
        //        Proveedor = new Proveedor { RazonSocial = "PARISI" },
        //        CupoSap = "MOL",
        //        Material = new Material { Descripcion = "Soja" }
        //    };
        //    var data = new List<CupoDto> { new CupoDto { Id = 1 } };
        //    repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
        //        .Returns(new List<Cupo>(){
        //            cupo
        //        });
        //    repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion() { ConexionABMStop = true });
        //    eliminarCupoAgentMock.Setup(x => x.Eliminar(It.IsAny<string>(), It.IsAny<string>())).Returns("Ok");
        //    clienteStopMock.Setup(x => x.EliminarCupo(cupo)).Returns(new Resultado { });
        //    repositorioMock.Setup(x => x.GuardarCambios());
        //    repositorioMock.Setup(y => y.Obtener<Comercial>(It.IsAny<int>())).Returns(new Comercial() { IdActiveDirectory = "bmelgarejo" });

        //    target.AnulacionMasiva(new List<int> { 1, 2, 3}, It.IsAny<string>(), new DataSourceResult { Data = data }, It.IsAny<string>());
        //    repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
        //          .Returns(new List<Comercial>() {
        //            new Comercial
        //            {
        //                IdActiveDirectory = "bmelgarejo"
        //            }
        //          });

        //    repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);


        //}

        [Test]
        public void GenerarSolicitudExtraordinariaTest()
        {
            var solicitud = new AdministracionCupoDto
            {
                CantidadCupo = 1,
                CantidadFleteProcedencia = 0,
                CantidadDeCupoOriginal = 1,
                CantidadFleteProcedenciaOriginal = 0,
                CentroId = 1,
                ComercialCreadorId = 1,
                ComercialId = 1,
                EstadoId = (int)EnumEstadoAdministracionCupo.Pendiente,
                Excedente = true,
                MaterialId = 3,
                Observacion = "",
                TipoAdministracionCupoId = (int)EnumTipoAdministracionCupo.Extraordinaria,
                ProveedorId = 1,
                ZonaId = 1,
                Fecha = DateTime.Now.Date,
                Dias = new List<DiaCupo> { new DiaCupo { Cantidad = 1, Fecha = DateTime.Now.Date } }
            };

            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                          .Returns(new List<Comercial>() { new Comercial { ComercialId = 1, GrupoDeComprasId = 44, GrupoDeCompras = new GrupoDeCompras { Descripcion = "a" } } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, int>>>())).Returns(1);
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<ZonaCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                         .Returns(new List<ZonaCupo>() { new ZonaCupo { Id = 1, Descripcion = "a" } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                        .Returns(new List<Cupo>() { new Cupo { Centro = new Centro { CodigoSap = "" } } });
            repositorioMock.Setup(y => y.Obtener<Centro>(It.IsAny<int>()))
                .Returns(new Centro { CodigoSap = "" });
            //repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
            //          .Returns(new List<Material>() { new Material { Codigo = "" } });
            crearCupoAgentMock.Setup(x => x.Crear(It.IsAny<Cupo>(), It.IsAny<int>())).Returns(new List<string> { });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
               .Returns(new Proveedor { ProveedorId = 1, CUIT = "1" });
            repositorioMock.Setup(y => y.Obtener<ConfiguracionCupo>(It.IsAny<Expression<Func<ConfiguracionCupo, bool>>>()))
                .Returns(new ConfiguracionCupo { LiberarCupera = false, Id = 1, Fecha = DateTime.Now.Date, MaterialId = 1, CentroId = 1, LimiteCupo = 90 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ConfiguracionCupo, bool>>>(), It.IsAny<Expression<Func<ConfiguracionCupo, int>>>())).Returns(90);

            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<ConfiguracionCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
               .Returns(new List<ConfiguracionCupo>() { new ConfiguracionCupo { LiberarCupera = false, Id = 1, Fecha = DateTime.Now.Date, MaterialId = 1, CentroId = 1, LimiteCupo = 90 } });

            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<ConfiguracionCupo, ConfiguracionCupoDto>>>(), It.IsAny<Expression<Func<ConfiguracionCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ConfiguracionCupoDto>() { new ConfiguracionCupoDto { LiberarCupera = false, Id = 1, Fecha = DateTime.Now.Date, MaterialId = 1, CentroId = 1, LimiteCupo = 90 } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<LimiteCupo, ConfiguracionCupoDto>>>(), It.IsAny<Expression<Func<LimiteCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
               .Returns(new List<ConfiguracionCupoDto>());

            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<Expression<Func<Proveedor, bool>>>()))
              .Returns(new Proveedor { ProveedorId = 1, CUIT = "1" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>()))
                .Returns(new SISA { EstadoCuit = 1 });
            altaTempranaAgent.Setup(y => y.ObtenerAlta(It.IsAny<string>(), It.IsAny<string>())).Returns(new AltaTempranaNRCODto { ProveedorGrano = "No" });
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(new Centro { Id = 1, NoPropio = false });
            var resultado = target.GenerarSolicitudExtraordinaria(solicitud);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado.HayError, false);
            Assert.AreEqual(resultado.Errores.Count(), 0);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<AdministracionCupo>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(1));
        }

        [Test]
        public void ActualizarCumplimientoCuposTest()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
               .Returns(new List<Cupo>() { new Cupo { Id = 1, CupoSap = "1", FechaIngreso = DateTime.Now.Date } });

            cumplimientoCuposAgent.Setup(x => x.Ejecutar(It.IsAny<List<string>>(), It.IsAny<DateTime>()))
                .Returns(new List<CumplimientoCupoDto> { new CumplimientoCupoDto { Codigo = "1", Cumplimiento = true } });

            target.ActualizarCumplimientoCupos(It.IsAny<DateTime>());
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void ConsultarMisTurnosActivosTest()
        {
            target.ConsultarMisTurnosActivos();

            clienteStopMock.Verify(x => x.ConsultarMisTurnosActivos(), Times.Once);
        }

        [Test]
        public void DevolverSugerenciasMasivoTest()
        {
            List<DevolucionSugerenciaCupoDto> sugerenciasADevolver = new List<DevolucionSugerenciaCupoDto> { new DevolucionSugerenciaCupoDto { IdSugerencia = 1, Cantidad = 2 } };
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<CierreCupera, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<CierreCupera>());
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, int>>>())).Returns(1);
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<SugerenciaCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<SugerenciaCupo>() {new SugerenciaCupo
                {
                    Id = 1,
                    CentroId = 1,
                    ComercialId = 1,
                    Destinatario = "a",
                    MaterialId = 1,
                    ProveedorId = 1,
                    ZonaCupoId = 2,
                    Puntuaciones = "{\"CriterioRaiz\":0.4}",
                    FechaSugerida = DateTime.Now.Date
                }});
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<SugerenciaPorComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                        .Returns(new List<SugerenciaPorComercial> { new SugerenciaPorComercial { Total = 99, CentroId = 1, ComercialId = 1, Fecha = DateTime.Now.Date, MaterialId = 1, Id = 1 } });
            repositorioMock.Setup(x => x.AgregarTodos(It.IsAny<List<AdministracionCupo>>(), null)).Verifiable();
            var result = target.DevolverSugerenciasMasivo(sugerenciasADevolver);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CierreCupera, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<SugerenciaCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<SugerenciaPorComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, int>>>()), Times.Once);
            repositorioMock.Verify(x => x.AgregarTodos(It.IsAny<List<AdministracionCupo>>(), null), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }
    }
}