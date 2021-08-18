using Autofac.Extras.NLog;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using Molinos.DataAgro.Test.Mock;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Web.Script.Serialization;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ProveedorManagerTest
    {
        private ProveedorManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<IComercialManager> comercialManagerMock;
        private Mock<IRiesgoComercialAgent> riesgoComercialAgentMock;
        private Mock<IDatosProveedorAgent> datosProveedorMock;
        private Mock<IMailManager> mailManagerMock;
        private Mock<ILogDataAgroManager> logDataAgroManagerMock;
        private Mock<IHttpContextManager> httpContextManagerMock;

        private JavaScriptSerializer serializer;


        [SetUp]
        public void SetUp()
        {
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
            riesgoComercialAgentMock = new Mock<IRiesgoComercialAgent>();
            datosProveedorMock = new Mock<IDatosProveedorAgent>();
            mailManagerMock = new Mock<IMailManager>();
            logDataAgroManagerMock = new Mock<ILogDataAgroManager>();
            httpContextManagerMock = new Mock<IHttpContextManager>();
            httpContextManagerMock.Setup(x => x.ObtenerPathLogoMail()).Returns(TestContext.CurrentContext.TestDirectory + "\\Util\\MolinosAgro.png");

            target = new ProveedorManager(logger.Object, repositorioMock.Object, comercialManagerMock.Object, riesgoComercialAgentMock.Object, datosProveedorMock.Object, mailManagerMock.Object, logDataAgroManagerMock.Object, httpContextManagerMock.Object);


            //para pasar el logDataA
            comercialManagerMock.Setup(x => x.ListarEquipo(It.IsAny<string>())).Returns(new EquipoDto { Equipo = new List<int> { 1, 2, 3 } });

            var comercial = new Comercial { ComercialId = 1, Apellido = "a", Nombres = "a", PerfilId = 7, IdActiveDirectory = "a" };
            var campana = new Campaña { CampañaId = 1, Descripcion = "a" };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(comercial);
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()))
                .Returns(new List<BasicoProveedor>() { new BasicoProveedor() { NoOperable = true, Operando = true, EstadoCuit = 2, Facacop = 0 } });
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()))
                .Returns(new List<ActividadTraer>() { new ActividadTraer() });
            repositorioMock.Setup(y => y.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ContactosComerciales>() { new ContactosComerciales() });
            repositorioMock.Setup(y => y.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ObjetivosTraer>() { new ObjetivosTraer() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioMaterialPorProveedor>() { new AcopioMaterialPorProveedor { AcopioId = 1,
                    AcopioMaterialId = 1,
                    CampañaId = 1,
                    Campaña = "a",
                    MaterialId = 1,
                    NroItem = 1,
                    Toneladas = 1,
                    Material = "a",
                    LocalidadId = 1,
                    Localidad = "a",
                    ProvinciaId = 1,
                    Provincia = "a" } });
            repositorioMock.Setup(y => y.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampoProduccionAcopio>() { new CampoProduccionAcopio() { EsCampoProduccion = true, ProveedorId = 1 }, new CampoProduccionAcopio() { EsCampoProduccion = false, ProveedorId = 1, } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()))
                .Returns(new DatosContacto { });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Material>() { new Material { CampañaId = 1, Campaña = campana, Descripcion = "a", MaterialId = 1, Codigo = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                 .Returns(new List<CampañaMaterial>() { new CampañaMaterial { CampañaId = 1, Campaña = campana } });
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });

            //para pasar el logDataA


        }


        [Test]
        public void TraerHistorialActividadOkTest()
        {
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()))
                .Returns(new List<ActividadTraer>() { new ActividadTraer() });
            var result = target.TraerHistorialActividad(new HistorialActiviad(), 1, "a");

            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.ActividadHistoriaTraerPorProveedores.Count);
        }
        [Test]
        public void TraerProveedorOkTest()
        {
            var comercial = new Comercial { ComercialId = 1, Apellido = "a", Nombres = "a", PerfilId = 7, IdActiveDirectory = "a" };
            var campana = new Campaña { CampañaId = 1, Descripcion = "a" };


            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(comercial);
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()))
                .Returns(new List<BasicoProveedor>() { new BasicoProveedor() { NoOperable = true, Operando = true, EstadoCuit = 2, Facacop = 0 } });
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()))
                .Returns(new List<ActividadTraer>() { new ActividadTraer() });
            repositorioMock.Setup(y => y.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ContactosComerciales>() { new ContactosComerciales() });
            repositorioMock.Setup(y => y.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ObjetivosTraer>() { new ObjetivosTraer() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioMaterialPorProveedor>() { new AcopioMaterialPorProveedor { AcopioId = 1,
                    AcopioMaterialId = 1,
                    CampañaId = 1,
                    Campaña = "a",
                    MaterialId = 1,
                    NroItem = 1,
                    Toneladas = 1,
                    Material = "a",
                    LocalidadId = 1,
                    Localidad = "a",
                    ProvinciaId = 1,
                    Provincia = "a" } });
            repositorioMock.Setup(y => y.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampoProduccionAcopio>() { new CampoProduccionAcopio() { EsCampoProduccion = true, ProveedorId = 1 }, new CampoProduccionAcopio() { EsCampoProduccion = false, ProveedorId = 1, } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()))
                .Returns(new DatosContacto { });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Material>() { new Material { CampañaId = 1, Campaña = campana, Descripcion = "a", MaterialId = 1, Codigo = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                 .Returns(new List<CampañaMaterial>() { new CampañaMaterial { CampañaId = 1, Campaña = campana } });
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCorredorDto>() { new ProveedorCorredorDto() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
             .Returns(new List<CampanaMaterialDetallePorMes>() { new CampanaMaterialDetallePorMes() {
                 CampanaId = 1,
                MaterialId = 2,
                ProveedorId = 1,
                Material = new Material
                {
                    Descripcion = "Soja",
                    MaterialId = 1,
                    CampañaId = 1
                },
                Campana = new Campaña
                {
                    Descripcion = "11",
                    CampañaId = 1
                },
                 ClaseDoc = "as",
                 Clasificacion = "PRODUCTOR",
                 ComercialId = 1,
                 CorredorCuit = "2321123",
                 PendienteAFijar = 1,
                 PendienteAplicar = 1,
                 ToneladaAmpliada = 0,
                 ToneladaAnulada = 1,
                 ToneladaAplicada = 3,
                 ToneladaContrato = 2,
                 ToneladaFijada = 2,
                 Fecha = DateTime.Now,
                 CampanaMaterialDetalleId = 1,
                 Contrato = "11233"
             } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Proveedor, int?>>>(), It.IsAny<Expression<Func<Proveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
               .Returns(new List<int?>() { 1 });

            var result = target.TraerProveedor(1, "a", new List<int>() { 1, 2, 3 });

            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()), Times.Once);
            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Acopio.Count);
            Assert.AreEqual(1, result.AcopioMaterialPorProveedores.Count);
            Assert.AreEqual(1, result.ActividadHistoriaTraerPorProveedores.Count);
            Assert.AreEqual(1, result.ActividadTraerPorProveedores.Count);
            Assert.AreEqual(1, result.BasicoProveedorTraerPorProveedores.Count);
            Assert.AreEqual(1, result.CampoProduccionAcopioPorProveedores.Count);
            Assert.AreEqual(1, result.CanalesDeOperacion.Count);
            Assert.AreEqual(1, result.ContactosComercialesTraerPorProveedores.Count);
            Assert.IsNull(result.DatosContacto.CodigoPostal);
            Assert.IsNull(result.DatosContacto.Direccion);
            Assert.IsNull(result.DatosContacto.Intermediario);
            Assert.AreEqual(1, result.Historial.HistorialGrano.Count);
            Assert.AreEqual(2, result.Historial.camp.Count);
            Assert.AreEqual(1, result.ObjetivosTraerPorProveedorId.Count);
            Assert.AreEqual(1, result.ProveedorCondicion.Count);
            Assert.AreEqual(1, result.ProveedorDestinatario.Count);
        }
        [Test]
        public void TraerProveedorConRiesgoSapTest()
        {
            var comercial = new Comercial { ComercialId = 1, Apellido = "a", Nombres = "a", PerfilId = 7, IdActiveDirectory = "a" };
            var campana = new Campaña { CampañaId = 1, Descripcion = "a" };
            ConfigurationManager.AppSettings["RiesgoComercialAltoSap"] = "a";

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(comercial);
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()))
                .Returns(new List<BasicoProveedor>() { new BasicoProveedor() { NoOperable = true, Operando = true, EstadoCuit = 2, Facacop = 0, RiesgoComercialSap = "A" } });
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()))
                .Returns(new List<ActividadTraer>() { new ActividadTraer() });
            repositorioMock.Setup(y => y.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ContactosComerciales>() { new ContactosComerciales() });
            repositorioMock.Setup(y => y.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ObjetivosTraer>() { new ObjetivosTraer() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioMaterialPorProveedor>() { new AcopioMaterialPorProveedor { AcopioId = 1,
                    AcopioMaterialId = 1,
                    CampañaId = 1,
                    Campaña = "a",
                    MaterialId = 1,
                    NroItem = 1,
                    Toneladas = 1,
                    Material = "a",
                    LocalidadId = 1,
                    Localidad = "a",
                    ProvinciaId = 1,
                    Provincia = "a" } });
            repositorioMock.Setup(y => y.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampoProduccionAcopio>() { new CampoProduccionAcopio() { EsCampoProduccion = true, ProveedorId = 1 }, new CampoProduccionAcopio() { EsCampoProduccion = false, ProveedorId = 1, } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()))
                .Returns(new DatosContacto { });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Material>() { new Material { CampañaId = 1, Campaña = campana, Descripcion = "a", MaterialId = 1, Codigo = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                 .Returns(new List<CampañaMaterial>() { new CampañaMaterial { CampañaId = 1, Campaña = campana } });
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCorredorDto>() { new ProveedorCorredorDto() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
             .Returns(new List<CampanaMaterialDetallePorMes>() { new CampanaMaterialDetallePorMes() {
                 CampanaId = 1,
                     MaterialId = 2,
                     ProveedorId = 1,
                     Material = new Material
                     {
                         Descripcion = "Soja",
                         MaterialId = 1,
                         CampañaId = 1
                     },
                     Campana = new Campaña
                     {
                         Descripcion = "11",
                         CampañaId = 1
                     },
                 ClaseDoc = "as",
                 Clasificacion = "PRODUCTOR",
                 ComercialId = 1,
                 CorredorCuit = "2321123",
                 PendienteAFijar = 1,
                 PendienteAplicar = 1,
                 ToneladaAmpliada = 0,
                 ToneladaAnulada = 1,
                 ToneladaAplicada = 3,
                 ToneladaContrato = 2,
                 ToneladaFijada = 2,
                 Fecha = DateTime.Now,
                 CampanaMaterialDetalleId = 1,
                 Contrato = "11233"
             } });
            var result = target.TraerProveedor(1, "a", new List<int>() { 1, 2, 3 });

            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()), Times.Once);
            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.BasicoProveedorTraerPorProveedores.Count);
            Assert.AreEqual("A", result.BasicoProveedorTraerPorProveedores[0].RiesgoComercialSap);
            Assert.AreEqual("Riesgo Comercial Alto", result.BasicoProveedorTraerPorProveedores[0].TooltipNoOperable);
            Assert.IsFalse(result.BasicoProveedorTraerPorProveedores[0].Operando);
            Assert.IsTrue(result.BasicoProveedorTraerPorProveedores[0].NoOperable);
        }
        [Test]
        public void TraerProveedorConEstadoInactivoTest()
        {
            var comercial = new Comercial { ComercialId = 1, Apellido = "a", Nombres = "a", PerfilId = 7, IdActiveDirectory = "a" };
            var campana = new Campaña { CampañaId = 1, Descripcion = "a" };
            ConfigurationManager.AppSettings["RiesgoComercialAltoSap"] = "a";

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(comercial);
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()))
                .Returns(new List<BasicoProveedor>() { new BasicoProveedor() { NoOperable = true, Operando = true, EstadoCuit = 0, Facacop = 0, RiesgoComercialSap = "B" } });
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()))
                .Returns(new List<ActividadTraer>() { new ActividadTraer() });
            repositorioMock.Setup(y => y.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ContactosComerciales>() { new ContactosComerciales() });
            repositorioMock.Setup(y => y.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ObjetivosTraer>() { new ObjetivosTraer() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioMaterialPorProveedor>() { new AcopioMaterialPorProveedor { AcopioId = 1,
                    AcopioMaterialId = 1,
                    CampañaId = 1,
                    Campaña = "a",
                    MaterialId = 1,
                    NroItem = 1,
                    Toneladas = 1,
                    Material = "a",
                    LocalidadId = 1,
                    Localidad = "a",
                    ProvinciaId = 1,
                    Provincia = "a" } });
            repositorioMock.Setup(y => y.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampoProduccionAcopio>() { new CampoProduccionAcopio() { EsCampoProduccion = true, ProveedorId = 1 }, new CampoProduccionAcopio() { EsCampoProduccion = false, ProveedorId = 1, } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()))
                .Returns(new DatosContacto { });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Material>() { new Material { CampañaId = 1, Campaña = campana, Descripcion = "a", MaterialId = 1, Codigo = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                 .Returns(new List<CampañaMaterial>() { new CampañaMaterial { CampañaId = 1, Campaña = campana } });
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCorredorDto>() { new ProveedorCorredorDto() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
             .Returns(new List<CampanaMaterialDetallePorMes>() { new CampanaMaterialDetallePorMes() {
                 CampanaId = 1,
                     MaterialId = 2,
                     ProveedorId = 1,
                     Material = new Material
                     {
                         Descripcion = "Soja",
                         MaterialId = 1,
                         CampañaId = 1
                     },
                     Campana = new Campaña
                     {
                         Descripcion = "11",
                         CampañaId = 1
                     },
                 ClaseDoc = "as",
                 Clasificacion = "PRODUCTOR",
                 ComercialId = 1,
                 CorredorCuit = "2321123",
                 PendienteAFijar = 1,
                 PendienteAplicar = 1,
                 ToneladaAmpliada = 0,
                 ToneladaAnulada = 1,
                 ToneladaAplicada = 3,
                 ToneladaContrato = 2,
                 ToneladaFijada = 2,
                 Fecha = DateTime.Now,
                 CampanaMaterialDetalleId = 1,
                 Contrato = "11233"
             } });
            var result = target.TraerProveedor(1, "a", new List<int>() { 1, 2, 3 });

            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()), Times.Once);
            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.BasicoProveedorTraerPorProveedores.Count);
            Assert.AreEqual("B", result.BasicoProveedorTraerPorProveedores[0].RiesgoComercialSap);
            Assert.AreEqual("Inactivo", result.BasicoProveedorTraerPorProveedores[0].TooltipNoOperable);
            Assert.IsFalse(result.BasicoProveedorTraerPorProveedores[0].Operando);
            Assert.IsTrue(result.BasicoProveedorTraerPorProveedores[0].NoOperable);
        }
        [Test]
        public void TraerProveedorConEstado3Test()
        {
            var comercial = new Comercial { ComercialId = 1, Apellido = "a", Nombres = "a", PerfilId = 7, IdActiveDirectory = "a" };
            var campana = new Campaña { CampañaId = 1, Descripcion = "a" };
            ConfigurationManager.AppSettings["RiesgoComercialAltoSap"] = "a";

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(comercial);
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()))
                .Returns(new List<BasicoProveedor>() { new BasicoProveedor() { NoOperable = true, Operando = true, EstadoCuit = 3, Facacop = 0, RiesgoComercialSap = "B" } });
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()))
                .Returns(new List<ActividadTraer>() { new ActividadTraer() });
            repositorioMock.Setup(y => y.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ContactosComerciales>() { new ContactosComerciales() });
            repositorioMock.Setup(y => y.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ObjetivosTraer>() { new ObjetivosTraer() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioMaterialPorProveedor>() { new AcopioMaterialPorProveedor { AcopioId = 1,
                    AcopioMaterialId = 1,
                    CampañaId = 1,
                    Campaña = "a",
                    MaterialId = 1,
                    NroItem = 1,
                    Toneladas = 1,
                    Material = "a",
                    LocalidadId = 1,
                    Localidad = "a",
                    ProvinciaId = 1,
                    Provincia = "a" } });
            repositorioMock.Setup(y => y.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampoProduccionAcopio>() { new CampoProduccionAcopio() { EsCampoProduccion = true, ProveedorId = 1 }, new CampoProduccionAcopio() { EsCampoProduccion = false, ProveedorId = 1, } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()))
                .Returns(new DatosContacto { });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Material>() { new Material { CampañaId = 1, Campaña = campana, Descripcion = "a", MaterialId = 1, Codigo = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                 .Returns(new List<CampañaMaterial>() { new CampañaMaterial { CampañaId = 1, Campaña = campana } });
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCorredorDto>() { new ProveedorCorredorDto() });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
             .Returns(new List<CampanaMaterialDetallePorMes>() { new CampanaMaterialDetallePorMes() {
                 CampanaId = 1,
                     MaterialId = 2,
                     ProveedorId = 1,
                     Material = new Material
                     {
                         Descripcion = "Soja",
                         MaterialId = 1,
                         CampañaId = 1
                     },
                     Campana = new Campaña
                     {
                         Descripcion = "11",
                         CampañaId = 1
                     },
                 ClaseDoc = "as",
                 Clasificacion = "PRODUCTOR",
                 ComercialId = 1,
                 CorredorCuit = "2321123",
                 PendienteAFijar = 1,
                 PendienteAplicar = 1,
                 ToneladaAmpliada = 0,
                 ToneladaAnulada = 1,
                 ToneladaAplicada = 3,
                 ToneladaContrato = 2,
                 ToneladaFijada = 2,
                 Fecha = DateTime.Now,
                 CampanaMaterialDetalleId = 1,
                 Contrato = "11233"
             } });


            var result = target.TraerProveedor(1, "a", new List<int>() { 1, 2, 3 });

            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()), Times.Once);
            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.BasicoProveedorTraerPorProveedores.Count);
            Assert.AreEqual("B", result.BasicoProveedorTraerPorProveedores[0].RiesgoComercialSap);
            Assert.AreEqual("Estado 3", result.BasicoProveedorTraerPorProveedores[0].TooltipNoOperable);
            Assert.IsFalse(result.BasicoProveedorTraerPorProveedores[0].Operando);
            Assert.IsTrue(result.BasicoProveedorTraerPorProveedores[0].NoOperable);
        }
        [Test]
        public void TraerProveedorConFacacopTest()
        {
            var comercial = new Comercial { ComercialId = 1, Apellido = "a", Nombres = "a", PerfilId = 7, IdActiveDirectory = "a" };
            var campana = new Campaña { CampañaId = 1, Descripcion = "a" };
            ConfigurationManager.AppSettings["RiesgoComercialAltoSap"] = "a";

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(comercial);
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()))
                .Returns(new List<BasicoProveedor>() { new BasicoProveedor() { NoOperable = true, Operando = true, EstadoCuit = 1, Facacop = 1, RiesgoComercialSap = "B" } });
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()))
                .Returns(new List<ActividadTraer>() { new ActividadTraer() });
            repositorioMock.Setup(y => y.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ContactosComerciales>() { new ContactosComerciales() });
            repositorioMock.Setup(y => y.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ObjetivosTraer>() { new ObjetivosTraer() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioMaterialPorProveedor>() { new AcopioMaterialPorProveedor { AcopioId = 1,
                    AcopioMaterialId = 1,
                    CampañaId = 1,
                    Campaña = "a",
                    MaterialId = 1,
                    NroItem = 1,
                    Toneladas = 1,
                    Material = "a",
                    LocalidadId = 1,
                    Localidad = "a",
                    ProvinciaId = 1,
                    Provincia = "a" } });
            repositorioMock.Setup(y => y.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampoProduccionAcopio>() { new CampoProduccionAcopio() { EsCampoProduccion = true, ProveedorId = 1 }, new CampoProduccionAcopio() { EsCampoProduccion = false, ProveedorId = 1, } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()))
                .Returns(new DatosContacto { });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Material>() { new Material { CampañaId = 1, Campaña = campana, Descripcion = "a", MaterialId = 1, Codigo = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                 .Returns(new List<CampañaMaterial>() { new CampañaMaterial { CampañaId = 1, Campaña = campana } });
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCorredorDto>() { new ProveedorCorredorDto() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
             .Returns(new List<CampanaMaterialDetallePorMes>() { new CampanaMaterialDetallePorMes() {
                 CampanaId = 1,
                     MaterialId = 2,
                     ProveedorId = 1,
                     Material = new Material
                     {
                         Descripcion = "Soja",
                         MaterialId = 1,
                         CampañaId = 1
                     },
                     Campana = new Campaña
                     {
                         Descripcion = "11",
                         CampañaId = 1
                     },
                 ClaseDoc = "as",
                 Clasificacion = "PRODUCTOR",
                 ComercialId = 1,
                 CorredorCuit = "2321123",
                 PendienteAFijar = 1,
                 PendienteAplicar = 1,
                 ToneladaAmpliada = 0,
                 ToneladaAnulada = 1,
                 ToneladaAplicada = 3,
                 ToneladaContrato = 2,
                 ToneladaFijada = 2,
                 Fecha = DateTime.Now,
                 CampanaMaterialDetalleId = 1,
                 Contrato = "11233"
             } });
            var result = target.TraerProveedor(1, "a", new List<int>() { 1, 2, 3 });

            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()), Times.Once);
            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.BasicoProveedorTraerPorProveedores.Count);
            Assert.AreEqual("B", result.BasicoProveedorTraerPorProveedores[0].RiesgoComercialSap);
            Assert.AreEqual("Apocrifos", result.BasicoProveedorTraerPorProveedores[0].TooltipNoOperable);
            Assert.IsFalse(result.BasicoProveedorTraerPorProveedores[0].Operando);
            Assert.IsTrue(result.BasicoProveedorTraerPorProveedores[0].NoOperable);
        }
        [Test]
        public void TraerLocalidadProveedorPorCuitOkTest()
        {
            var proveedor = new DatosLocalidadProvincia { ProveedorId = 1, CUIT = "a", RazonSocial = "a", LocalidadId = 1, ProvinciaId = 1, Localidad = "a", Provincia = "a" };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosLocalidadProvincia>>>()))
                .Returns(proveedor);

            var result = target.TraerLocalidadProveedorPorCuit("a");

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosLocalidadProvincia>>>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(proveedor, result);
        }
        [Test]
        public void TraerLocalidadProveedorPorCuitConFiltroTest()
        {
            var prov = new Provincia { Nombre = "a", Orden = 1, ProvinciaId = 1 };
            var loc = new Localidad { ProvinciaId = 1, Nombre = "a", Provincia = prov, CodLocalidad = "a", LocalidadId = 1 };
            var proveedor = new Proveedor { ProveedorId = 1, CUIT = "1", RazonSocial = "a", LocalidadCompraNetId = 1, ProvinciaCompraNetId = 1, ProvinciaCompraNet = prov, LocalidadCompraNet = loc };
            var datos = new DatosLocalidadProvincia { ProveedorId = 1, CUIT = "a", RazonSocial = "a", Localidad = "a", Provincia = "b", LocalidadId = 1, ProvinciaId = 2 };
            var filtro = new DatosLocalidadProvinciaFiltro { CUIT = "a", CampanaId = 1, MaterialId = 1 };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(proveedor);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Localidad, bool>>>(), It.IsAny<Expression<Func<Localidad, DatosLocalidadProvincia>>>()))
                            .Returns(datos);

            var result = target.TraerLocalidadProveedorPorCuit(filtro);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Localidad, bool>>>(), It.IsAny<Expression<Func<Localidad, DatosLocalidadProvincia>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<CampoMaterial, bool>>>(), It.IsAny<Expression<Func<CampoMaterial, DatosLocalidadProvincia>>>()), Times.Never);

            Assert.NotNull(result);
            Assert.AreEqual("1", result.CUIT);
            Assert.AreEqual(1, result.ProveedorId);
            Assert.AreEqual("a", result.RazonSocial);
            Assert.AreEqual(0, result.ClasificacionId);
            Assert.AreEqual(false, result.Consignatario);
            Assert.AreEqual(0, result.BoletoId);
            Assert.AreEqual(0, result.BolsaId);
            Assert.AreEqual(false, result.Corredor);
        }
        [Test]
        public void TraerLocalidadProveedorPorCuitSinFiltroTest()
        {
            var proveedor = new Proveedor { ProveedorId = 1, CUIT = "1", RazonSocial = "a", ClasificacionCompraNetId = 1, Consignatario = true, BoletoCompraNetId = 1, BolsaCompraNetId = 1, SegmentacionId = 5 };
            var datos = new DatosLocalidadProvincia { ProveedorId = 1, CUIT = "a", RazonSocial = "a", Localidad = "a", Provincia = "b", LocalidadId = 1, ProvinciaId = 2 };
            var filtro = new DatosLocalidadProvinciaFiltro { CUIT = "a", CampanaId = 1, MaterialId = 1 };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(proveedor);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CampoMaterial, bool>>>(), It.IsAny<Expression<Func<CampoMaterial, DatosLocalidadProvincia>>>()))
                            .Returns(datos);

            var result = target.TraerLocalidadProveedorPorCuit(filtro);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Localidad, bool>>>(), It.IsAny<Expression<Func<Localidad, DatosLocalidadProvincia>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<CampoMaterial, bool>>>(), It.IsAny<Expression<Func<CampoMaterial, DatosLocalidadProvincia>>>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual("1", result.CUIT);
            Assert.AreEqual(1, result.ProveedorId);
            Assert.AreEqual("a", result.RazonSocial);
            Assert.AreEqual(1, result.ClasificacionId);
            Assert.AreEqual(true, result.Consignatario);
            Assert.AreEqual(1, result.BoletoId);
            Assert.AreEqual(1, result.BolsaId);
            Assert.AreEqual(true, result.Corredor);
        }
        //faltaria GrabarRecordatorio, EnviarMail, EnviarMailFijacion y GetEmailUserActiveDirectory

        [Test]
        public void GrabarRecordatorioSinCitaOkTest()
        {
            ConfigurationManager.AppSettings["AgendaCita"] = "2";
            var fecha = new DateTime(2019, 10, 10);
            var actividad = new ActividadInsetarIni { ProveedorId = 1, ComercialId = 1, fechaYHoraActividad = fecha, fechaYHoraRecordatorio = fecha, tipoactividad = 1, ActividadId = 1 };


            //para pasar el logDataA
            comercialManagerMock.Setup(x => x.ListarEquipo(It.IsAny<string>())).Returns(new EquipoDto { Equipo = new List<int> { 1, 2, 3 } });

            var comercial = new Comercial { ComercialId = 1, Apellido = "a", Nombres = "a", PerfilId = 7, IdActiveDirectory = "a" };
            var campana = new Campaña { CampañaId = 1, Descripcion = "a" };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(comercial);
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()))
                .Returns(new List<BasicoProveedor>() { new BasicoProveedor() { NoOperable = true, Operando = true, EstadoCuit = 2, Facacop = 0 } });
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()))
                .Returns(new List<ActividadTraer>() { new ActividadTraer() });
            repositorioMock.Setup(y => y.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ContactosComerciales>() { new ContactosComerciales() });
            repositorioMock.Setup(y => y.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ObjetivosTraer>() { new ObjetivosTraer() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioMaterialPorProveedor>() { new AcopioMaterialPorProveedor { AcopioId = 1,
                    AcopioMaterialId = 1,
                    CampañaId = 1,
                    Campaña = "a",
                    MaterialId = 1,
                    NroItem = 1,
                    Toneladas = 1,
                    Material = "a",
                    LocalidadId = 1,
                    Localidad = "a",
                    ProvinciaId = 1,
                    Provincia = "a" } });
            repositorioMock.Setup(y => y.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampoProduccionAcopio>() { new CampoProduccionAcopio() { EsCampoProduccion = true, ProveedorId = 1 }, new CampoProduccionAcopio() { EsCampoProduccion = false, ProveedorId = 1, } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()))
                .Returns(new DatosContacto { });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Material>() { new Material { CampañaId = 1, Campaña = campana, Descripcion = "a", MaterialId = 1, Codigo = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                 .Returns(new List<CampañaMaterial>() { new CampañaMaterial { CampañaId = 1, Campaña = campana } });
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCorredorDto>() { new ProveedorCorredorDto() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
            .Returns(new List<CampanaMaterialDetallePorMes>() { new CampanaMaterialDetallePorMes() {
                 CampanaId = 1,
                     MaterialId = 2,
                     ProveedorId = 1,
                     Material = new Material
                     {
                         Descripcion = "Soja",
                         MaterialId = 1,
                         CampañaId = 1
                     },
                     Campana = new Campaña
                     {
                         Descripcion = "11",
                         CampañaId = 1
                     },
                 ClaseDoc = "as",
                 Clasificacion = "PRODUCTOR",
                 ComercialId = 1,
                 CorredorCuit = "2321123",
                 PendienteAFijar = 1,
                 PendienteAplicar = 1,
                 ToneladaAmpliada = 0,
                 ToneladaAnulada = 1,
                 ToneladaAplicada = 3,
                 ToneladaContrato = 2,
                 ToneladaFijada = 2,
                 Fecha = DateTime.Now,
                 CampanaMaterialDetalleId = 1,
                 Contrato = "11233"
             } });
            //para pasar el logDataA


            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Actividad, bool>>>()))
                .Returns(new Actividad());
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Material>() { new Material { CampañaId = 1, Campaña = campana, Descripcion = "a", MaterialId = 1, Codigo = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                 .Returns(new List<CampañaMaterial>() { new CampañaMaterial { CampañaId = 1, Campaña = campana } });
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });


            var result = target.GrabarRecordatorio(actividad);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Actividad, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Exactly(3));
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(result);
            Assert.IsFalse(result.HayErrores);
        }

        [Test]
        public void EliminarRecordatorioOkTest()
        {
            var result = target.EliminarRecordatorio(1);

            repositorioMock.Verify(x => x.Remover<Actividad>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(false, result.HayErrores);
        }
        [Test]
        public void TraerDatosComboOkTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Segmentacion, SegmentacionQry>>>(), It.IsAny<Expression<Func<Segmentacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<SegmentacionQry>() { new SegmentacionQry { Descripcion = "a", Grupo = "1", SegmentacionId = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TipoTelefono, TipoTelefonoQry>>>(), It.IsAny<Expression<Func<TipoTelefono, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<TipoTelefonoQry>() { new TipoTelefonoQry { Descripcion = "a", TipoTelefonoId = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Provincia, ProvinciaQry>>>(), It.IsAny<Expression<Func<Provincia, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProvinciaQry>() { new ProvinciaQry { Nombre = "a", Provinciaid = 1, Orden = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CanalOperacion, CanalOperacionQry>>>(), It.IsAny<Expression<Func<CanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacionQry>() { new CanalOperacionQry { Descripcion = "a", CanalOperacionId = 1, Inhabilitado = false } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, MaterialQry>>>(), It.IsAny<Expression<Func<Material, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<MaterialQry>() { new MaterialQry { Descripcion = "a", CampañaIdActual = 1, Codigo = "a", MaterialId = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Destinatario, DestinatarioQry>>>(), It.IsAny<Expression<Func<Destinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<DestinatarioQry>() { new DestinatarioQry { Descripcion = "a", DestinatarioId = 1, Inhabilitado = false } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Condicion, CondicionQry>>>(), It.IsAny<Expression<Func<Condicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CondicionQry>() { new CondicionQry { Descripcion = "a", CondicionId = 1, Inhabilitado = false } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Interes, InteresQry>>>(), It.IsAny<Expression<Func<Interes, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<InteresQry>() { new InteresQry { Descripcion = "a", InteresId = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TipoActividad, TipoActividadQry>>>(), It.IsAny<Expression<Func<TipoActividad, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<TipoActividadQry>() { new TipoActividadQry { Descripcion = "a", TipoActividadId = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ContactoComercial, ContactoComercialQry>>>(), It.IsAny<Expression<Func<ContactoComercial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ContactoComercialQry>() { new ContactoComercialQry { Nombres = "a", ContactoComercialId = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ClasificacionCompraNet, ClasificacionCompraNetQry>>>(), It.IsAny<Expression<Func<ClasificacionCompraNet, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ClasificacionCompraNetQry>() { new ClasificacionCompraNetQry { Descripcion = "a", Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<BoletoCompraNet, BoletoCompraNetQry>>>(), It.IsAny<Expression<Func<BoletoCompraNet, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<BoletoCompraNetQry>() { new BoletoCompraNetQry { Descripcion = "a", Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<BolsaCompraNet, BolsaCompraNetQry>>>(), It.IsAny<Expression<Func<BolsaCompraNet, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<BolsaCompraNetQry>() { new BolsaCompraNetQry { Descripcion = "a", Id = 1 } });


            var result = target.TraerDatosCombo(1, false);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Segmentacion, SegmentacionQry>>>(), It.IsAny<Expression<Func<Segmentacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Exactly(2));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<TipoTelefono, TipoTelefonoQry>>>(), It.IsAny<Expression<Func<TipoTelefono, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Provincia, ProvinciaQry>>>(), It.IsAny<Expression<Func<Provincia, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CanalOperacion, CanalOperacionQry>>>(), It.IsAny<Expression<Func<CanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Material, MaterialQry>>>(), It.IsAny<Expression<Func<Material, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Destinatario, DestinatarioQry>>>(), It.IsAny<Expression<Func<Destinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Condicion, CondicionQry>>>(), It.IsAny<Expression<Func<Condicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Interes, InteresQry>>>(), It.IsAny<Expression<Func<Interes, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<TipoActividad, TipoActividadQry>>>(), It.IsAny<Expression<Func<TipoActividad, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ContactoComercial, ContactoComercialQry>>>(), It.IsAny<Expression<Func<ContactoComercial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ClasificacionCompraNet, ClasificacionCompraNetQry>>>(), It.IsAny<Expression<Func<ClasificacionCompraNet, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<BoletoCompraNet, BoletoCompraNetQry>>>(), It.IsAny<Expression<Func<BoletoCompraNet, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<BolsaCompraNet, BolsaCompraNetQry>>>(), It.IsAny<Expression<Func<BolsaCompraNet, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.segm.Count);
            Assert.AreEqual(1, result.tiptel.Count);
            Assert.AreEqual(1, result.prov.Count);
            Assert.AreEqual(0, result.loc.Count);
            Assert.AreEqual(1, result.cope.Count);
            Assert.AreEqual(1, result.gran.Count);
            Assert.AreEqual(1, result.dest.Count);
            Assert.AreEqual(1, result.cond.Count);
            Assert.AreEqual(1, result.inte.Count);
            Assert.AreEqual(1, result.tipoact.Count);
            Assert.AreEqual(1, result.concom.Count);
            Assert.AreEqual(1, result.ClasComNet.Count);
            Assert.AreEqual(1, result.BoleComNet.Count);
            Assert.AreEqual(1, result.BolsComNet.Count);

        }
        [Test]
        public void TraerLocalidadOkTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Localidad, LocalidadDto>>>(), It.IsAny<Expression<Func<Localidad, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<LocalidadDto>() { new LocalidadDto { CodLocalidad = "a", LocalidadId = 1, Nombre = "a", ProvinciaId = 1, Provincia_Nombre = "b" } });

            var result = target.TraerLocalidad(1);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Localidad, LocalidadDto>>>(), It.IsAny<Expression<Func<Localidad, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
        }
        [Test]
        public void TraerProveedorPorCuitOkTest()
        {
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, ProveedorQry>>>()))
                .Returns(new ProveedorQry { ProveedorId = 1, Descripcion = "b" });

            var result = target.TraerProveedorPorCuit("a", false);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, ProveedorQry>>>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.ProveedorId);
        }
        [Test]
        public void TraerProveedorPorCuitCorredorOkTest()
        {
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), It.IsAny<Expression<Func<CorredorProveedor, ProveedorQry>>>()))
                .Returns(new ProveedorQry { ProveedorId = 1, Descripcion = "b" });

            var result = target.TraerProveedorPorCuit("a", true);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), It.IsAny<Expression<Func<CorredorProveedor, ProveedorQry>>>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.ProveedorId);
        }

        [Test]
        public void TraerRazonSocialOkTest()
        {
            ConfigurationManager.AppSettings["RiesgoComercialAltoSap"] = "B";
            ConfigurationManager.AppSettings["SinConexionSap"] = "0";
            var fecha = new DateTime(2019, 1, 19);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SISA, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<SISA>() { new SISA { CUIT = "1", EstadoCuit = 1, RazonSocial = "a", FechaVigenciaEstado = fecha } });
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()))
                .Returns(false);
            riesgoComercialAgentMock.Setup(y => y.ObtenerRiesgoComercial(It.IsAny<string>())).Returns("A");
            var result = target.TraerRazonSocial("1");

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<SISA, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()), Times.Once);
            riesgoComercialAgentMock.Verify(x => x.ObtenerRiesgoComercial(It.IsAny<string>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Existe);
            Assert.AreEqual(1, result.Operable);
            Assert.AreEqual("", result.Condicion);
        }
        [Test]
        public void TraerRazonSocialSinRazonSocialTest()
        {
            ConfigurationManager.AppSettings["RiesgoComercialAltoSap"] = "B";
            ConfigurationManager.AppSettings["SinConexionSap"] = "0";
            var fecha = new DateTime(2019, 1, 19);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SISA, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<SISA>());
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()))
                .Returns(false);
            riesgoComercialAgentMock.Setup(y => y.ObtenerRiesgoComercial(It.IsAny<string>())).Returns("A");
            var result = target.TraerRazonSocial("1");

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<SISA, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()), Times.Never);
            riesgoComercialAgentMock.Verify(x => x.ObtenerRiesgoComercial(It.IsAny<string>()), Times.Never);

            Assert.NotNull(result);
            Assert.AreEqual(0, result.Existe);
            Assert.AreEqual(0, result.Operable);
            Assert.AreEqual("no incluido", result.Condicion);
            Assert.AreEqual("No existe Razon Social", result.razonSocial);
        }
        [Test]
        public void TraerRazonSocialApocrifoTest()
        {
            ConfigurationManager.AppSettings["RiesgoComercialAltoSap"] = "B";
            ConfigurationManager.AppSettings["SinConexionSap"] = "0";
            var fecha = new DateTime(2019, 1, 19);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SISA, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<SISA>() { new SISA { CUIT = "1", EstadoCuit = 1, RazonSocial = "a", FechaVigenciaEstado = fecha } });
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()))
                .Returns(true);
            riesgoComercialAgentMock.Setup(y => y.ObtenerRiesgoComercial(It.IsAny<string>())).Returns("A");
            var result = target.TraerRazonSocial("1");

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<SISA, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()), Times.Once);
            riesgoComercialAgentMock.Verify(x => x.ObtenerRiesgoComercial(It.IsAny<string>()), Times.Never);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Existe);
            Assert.AreEqual(0, result.Operable);
            Assert.AreEqual("Apocrifos", result.Condicion);
            Assert.AreEqual("a", result.razonSocial);
        }
        [Test]
        public void TraerRazonSocialInactivoTest()
        {
            var fecha = new DateTime(2019, 1, 19);
            ConfigurationManager.AppSettings["RiesgoComercialAltoSap"] = "A";
            ConfigurationManager.AppSettings["SinConexionSap"] = "0";
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SISA, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<SISA>() { new SISA { CUIT = "1", EstadoCuit = 0, RazonSocial = "a", FechaVigenciaEstado = fecha } });
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()))
                .Returns(false);
            riesgoComercialAgentMock.Setup(y => y.ObtenerRiesgoComercial(It.IsAny<string>())).Returns("B");
            var result = target.TraerRazonSocial("1");

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<SISA, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()), Times.Once);
            riesgoComercialAgentMock.Verify(x => x.ObtenerRiesgoComercial(It.IsAny<string>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Existe);
            Assert.AreEqual(0, result.Operable);
            Assert.AreEqual("Inactivo", result.Condicion);
            Assert.AreEqual("a", result.razonSocial);
        }
        [Test]
        public void TraerRazonSocialEstado3Test()
        {
            var fecha = new DateTime(2019, 1, 19);
            ConfigurationManager.AppSettings["RiesgoComercialAltoSap"] = "A";
            ConfigurationManager.AppSettings["SinConexionSap"] = "0";
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SISA, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<SISA>() { new SISA { CUIT = "1", EstadoCuit = 3, RazonSocial = "a", FechaVigenciaEstado = fecha } });
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()))
                .Returns(false);
            riesgoComercialAgentMock.Setup(y => y.ObtenerRiesgoComercial(It.IsAny<string>())).Returns("B");
            var result = target.TraerRazonSocial("1");

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<SISA, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()), Times.Once);
            riesgoComercialAgentMock.Verify(x => x.ObtenerRiesgoComercial(It.IsAny<string>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Existe);
            Assert.AreEqual(0, result.Operable);
            Assert.AreEqual("Estado 3", result.Condicion);
            Assert.AreEqual("a", result.razonSocial);
        }
        [Test]
        public void TraerRazonSocialRiegoAltoTest()
        {
            var fecha = new DateTime(2019, 1, 19);
            ConfigurationManager.AppSettings["RiesgoComercialAltoSap"] = "A";
            ConfigurationManager.AppSettings["SinConexionSap"] = "0";
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SISA, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<SISA>() { new SISA { CUIT = "1", EstadoCuit = 2, RazonSocial = "a", FechaVigenciaEstado = fecha } });
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()))
                .Returns(false);
            riesgoComercialAgentMock.Setup(y => y.ObtenerRiesgoComercial(It.IsAny<string>())).Returns("A");
            var result = target.TraerRazonSocial("1");

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<SISA, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()), Times.Once);
            riesgoComercialAgentMock.Verify(x => x.ObtenerRiesgoComercial(It.IsAny<string>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Existe);
            Assert.AreEqual(0, result.Operable);
            Assert.AreEqual("Riesgo Comercial Alto", result.Condicion);
            Assert.AreEqual("a", result.razonSocial);
        }

        [Test]
        public void GrabarNuevoProveedorOkTest()
        {
            ConfigurationManager.AppSettings["SinConexionSap"] = "0";
            var fecha = new DateTime(2019, 1, 19);
            var proveedor = new NuevoProveedor
            {
                produccion = new Produccion
                {
                    habilitaoSojaSust = "0",
                    CamposProduccion = new List<CamposProduccion>() {
                        new CamposProduccion { granos = new List<Granos> { new Granos { campañaId = 1, granoId = 1 } } } },
                    objetivos = new List<Objetivos>() {
                        new Objetivos { granoId = 1, campañaId = 1, toneladasObjetivo = "1000" },
                        new Objetivos { granoId = 1, campañaId = 1, toneladasObjetivo = "1000"
                        } }
                },
                almacenamiento = new Almacenamiento
                {
                    CamposAlmacenamiento = new List<CamposAlmacenamiento>() { new CamposAlmacenamiento {
                        granosAlmacenamiento = new List<GranosAlmacenamiento>() { new GranosAlmacenamiento { campañaId = 1 } },
                        granosAlmacenamientoGrano = new List<GranosAlmacenamientoGrano>() { new GranosAlmacenamientoGrano { campañaId = 1, granoId = 1 }
                        } } }
                },
                basicos = new Basico() { cuit = "1", RazonSocial = "1", segmentacion = 1, calificacion = 1 },
                contacto = new Contacto
                {
                    canalesOperacion = new List<int>() { 1 },
                    condPreferentes = new List<int>() { 1 },
                    entregaA = new List<int>() { 1 },
                },
                contactocomercial = new List<ContactosComercial>() {
                    new ContactosComercial { intereses = new List<int> { 1, 2 },
                        telefonos = new List<Telefono> { new Telefono(), new Telefono(), new Telefono() },
                        emails = new List<string>() { "eee", "", "" },
                        CompraNet = false, Cupo = false },
                    new ContactosComercial { intereses = new List<int>(),
                        telefonos = new List<Telefono> { new Telefono(), new Telefono(), new Telefono() },
                        emails = new List<string>() { "eee", "", "" },
                        CompraNet = false, Cupo = false }
                },
                ProveedorCorredorId = 0,
                ProveedorId = 1,
                establecimiento = new List<CampoDetalleDto> { new CampoDetalleDto { archivo = "", archivoFileResult = "", CampoId = 1, comercialId = 1, hcultivables = 1, htotales = 1, ImportId = 1, rinde = 1, latitud = "", longitud = "", nombre = "", localidadId = 1, materialId = 1 } }
            };


            //para pasar el logDataA
            comercialManagerMock.Setup(x => x.ListarEquipo(It.IsAny<string>())).Returns(new EquipoDto { Equipo = new List<int> { 1, 2, 3 } });

            var comercial = new Comercial { ComercialId = 1, Apellido = "a", Nombres = "a", PerfilId = 7, IdActiveDirectory = "a" };
            var campana = new Campaña { CampañaId = 1, Descripcion = "a" };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(comercial);
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()))
                .Returns(new List<BasicoProveedor>() { new BasicoProveedor() { NoOperable = true, Operando = true, EstadoCuit = 2, Facacop = 0 } });
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()))
                .Returns(new List<ActividadTraer>() { new ActividadTraer() });
            repositorioMock.Setup(y => y.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ContactosComerciales>() { new ContactosComerciales() });
            repositorioMock.Setup(y => y.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ObjetivosTraer>() { new ObjetivosTraer() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioMaterialPorProveedor>() { new AcopioMaterialPorProveedor { AcopioId = 1,
                    AcopioMaterialId = 1,
                    CampañaId = 1,
                    Campaña = "a",
                    MaterialId = 1,
                    NroItem = 1,
                    Toneladas = 1,
                    Material = "a",
                    LocalidadId = 1,
                    Localidad = "a",
                    ProvinciaId = 1,
                    Provincia = "a" } });
            repositorioMock.Setup(y => y.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampoProduccionAcopio>() { new CampoProduccionAcopio() { EsCampoProduccion = true, ProveedorId = 1 }, new CampoProduccionAcopio() { EsCampoProduccion = false, ProveedorId = 1, } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()))
                .Returns(new DatosContacto { });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Material>() { new Material { CampañaId = 1, Campaña = campana, Descripcion = "a", MaterialId = 1, Codigo = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                 .Returns(new List<CampañaMaterial>() { new CampañaMaterial { CampañaId = 1, Campaña = campana } });
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCorredorDto>() { new ProveedorCorredorDto() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
            .Returns(new List<CampanaMaterialDetallePorMes>() { new CampanaMaterialDetallePorMes() {
                 CampanaId = 1,
                     MaterialId = 2,
                     ProveedorId = 1,
                     Material = new Material
                     {
                         Descripcion = "Soja",
                         MaterialId = 1,
                         CampañaId = 1
                     },
                     Campana = new Campaña
                     {
                         Descripcion = "11",
                         CampañaId = 1
                     },
                 ClaseDoc = "as",
                 Clasificacion = "PRODUCTOR",
                 ComercialId = 1,
                 CorredorCuit = "2321123",
                 PendienteAFijar = 1,
                 PendienteAplicar = 1,
                 ToneladaAmpliada = 0,
                 ToneladaAnulada = 1,
                 ToneladaAplicada = 3,
                 ToneladaContrato = 2,
                 ToneladaFijada = 2,
                 Fecha = DateTime.Now,
                 CampanaMaterialDetalleId = 1,
                 Contrato = "11233"
             } });
            //para pasar el logDataA

            //validar proveedor
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(false);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<SISA, bool>>>()))
                .Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()))
                .Returns(false);

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(new Comercial { IdActiveDirectory = "a", ComercialId = 1, PerfilId = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Estado>() { new Estado { Descripcion = "a", EstadoId = 1 } });

            datosProveedorMock.Setup(y => y.ObtenerDatosDeProveedor(It.IsAny<List<Datos>>()))
                .Returns(new List<DatosProveedorAgentDto>() { new DatosProveedorAgentDto { USUARIO = "a", CLIENTE_MOA = "X", CUIT = "a", STATUS = "1" } });

            repositorioMock.Setup(y => y.Obtener<Interes>(It.IsAny<int>()))
                .Returns(new Interes { Descripcion = "a", InteresId = 1 });
            var result = target.GrabarNuevoProveedor(proveedor, "1");

            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<SISA, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Exactly(3));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Proveedor>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<ContactoComercial>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Agregar(It.IsAny<ContactoComercialInteres>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Campo>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<CampoMaterial>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Acopio>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<AcopioCampaña>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<AcopioMaterial>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<ProveedorCanalOperacion>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<ProveedorCondicion>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<ProveedorDestinatario>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Objetivo>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.NotNull(result);
            Assert.IsFalse(result.HayErrores);
        }
        [Test]
        public void GrabarNuevoProveedorExistenteTest()
        {
            ConfigurationManager.AppSettings["SinConexionSap"] = "0";
            var fecha = new DateTime(2019, 1, 19);
            var proveedor = new NuevoProveedor
            {
                produccion = new Produccion(),
                almacenamiento = new Almacenamiento(),
                basicos = new Basico() { cuit = "1", RazonSocial = "1", segmentacion = 1, calificacion = 1 },
                contacto = new Contacto(),
                contactocomercial = new List<ContactosComercial>(),
                ProveedorCorredorId = 0,
                ProveedorId = 1
            };

            //validar proveedor
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<SISA, bool>>>()))
                .Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()))
                .Returns(false);

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(new Comercial { IdActiveDirectory = "a", ComercialId = 1, PerfilId = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Estado>() { new Estado { Descripcion = "a", EstadoId = 1 } });

            datosProveedorMock.Setup(y => y.ObtenerDatosDeProveedor(It.IsAny<List<Datos>>()))
                .Returns(new List<DatosProveedorAgentDto>() { new DatosProveedorAgentDto { USUARIO = "a", CLIENTE_MOA = "X", CUIT = "a", STATUS = "1" } });


            var result = target.GrabarNuevoProveedor(proveedor, "1");

            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<SISA, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Proveedor>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayErrores);
        }
        [Test]
        public void GrabarNuevoProveedorExistenteCorredorTest()
        {
            ConfigurationManager.AppSettings["SinConexionSap"] = "0";
            var fecha = new DateTime(2019, 1, 19);
            var proveedor = new NuevoProveedor
            {
                produccion = new Produccion(),
                almacenamiento = new Almacenamiento(),
                basicos = new Basico() { cuit = "1", RazonSocial = "1", segmentacion = 5, calificacion = 1 },
                contacto = new Contacto(),
                contactocomercial = new List<ContactosComercial>(),
                ProveedorCorredorId = 0,
                ProveedorId = 1
            };

            //validar proveedor
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<SISA, bool>>>()))
                .Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()))
                .Returns(false);

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(new Comercial { IdActiveDirectory = "a", ComercialId = 1, PerfilId = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Estado>() { new Estado { Descripcion = "a", EstadoId = 1 } });

            datosProveedorMock.Setup(y => y.ObtenerDatosDeProveedor(It.IsAny<List<Datos>>()))
                .Returns(new List<DatosProveedorAgentDto>() { new DatosProveedorAgentDto { USUARIO = "a", CLIENTE_MOA = "X", CUIT = "a", STATUS = "1" } });


            var result = target.GrabarNuevoProveedor(proveedor, "1");

            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<SISA, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Proveedor>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayErrores);
        }
        [Test]
        public void GrabarNuevoProveedorSinCuitExistenteTest()
        {
            ConfigurationManager.AppSettings["SinConexionSap"] = "0";
            var fecha = new DateTime(2019, 1, 19);
            var proveedor = new NuevoProveedor
            {
                produccion = new Produccion(),
                almacenamiento = new Almacenamiento(),
                basicos = new Basico() { cuit = "1", RazonSocial = "1", segmentacion = 5, calificacion = 1 },
                contacto = new Contacto(),
                contactocomercial = new List<ContactosComercial>(),
                ProveedorCorredorId = 0,
                ProveedorId = 1
            };

            //validar proveedor
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(false);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<SISA, bool>>>()))
                .Returns(false);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()))
                .Returns(false);

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(new Comercial { IdActiveDirectory = "a", ComercialId = 1, PerfilId = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Estado>() { new Estado { Descripcion = "a", EstadoId = 1 } });

            datosProveedorMock.Setup(y => y.ObtenerDatosDeProveedor(It.IsAny<List<Datos>>()))
                .Returns(new List<DatosProveedorAgentDto>() { new DatosProveedorAgentDto { USUARIO = "a", CLIENTE_MOA = "X", CUIT = "a", STATUS = "1" } });


            var result = target.GrabarNuevoProveedor(proveedor, "1");

            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<SISA, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Proveedor>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayErrores);
        }
        [Test]
        public void GrabarNuevoProveedorConFacacopTest()
        {
            ConfigurationManager.AppSettings["SinConexionSap"] = "0";
            var fecha = new DateTime(2019, 1, 19);
            var proveedor = new NuevoProveedor
            {
                produccion = new Produccion(),
                almacenamiento = new Almacenamiento(),
                basicos = new Basico() { cuit = "1", RazonSocial = "1", segmentacion = 5, calificacion = 1 },
                contacto = new Contacto(),
                contactocomercial = new List<ContactosComercial>(),
                ProveedorCorredorId = 0,
                ProveedorId = 1
            };

            //validar proveedor
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(false);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<SISA, bool>>>()))
                .Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()))
                .Returns(true);

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(new Comercial { IdActiveDirectory = "a", ComercialId = 1, PerfilId = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Estado>() { new Estado { Descripcion = "a", EstadoId = 1 } });

            datosProveedorMock.Setup(y => y.ObtenerDatosDeProveedor(It.IsAny<List<Datos>>()))
                .Returns(new List<DatosProveedorAgentDto>() { new DatosProveedorAgentDto { USUARIO = "a", CLIENTE_MOA = "X", CUIT = "a", STATUS = "1" } });


            var result = target.GrabarNuevoProveedor(proveedor, "1");

            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<SISA, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Proveedor>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayErrores);
        }
        [Test]
        public void GrabarNuevoProveedorSinDatosProveedorTest()
        {
            ConfigurationManager.AppSettings["SinConexionSap"] = "0";
            ConfigurationManager.AppSettings["NoCliente"] = "1";
            ConfigurationManager.AppSettings["PotencialCliente"] = "2";
            var fecha = new DateTime(2019, 1, 19);
            var proveedor = new NuevoProveedor
            {
                produccion = new Produccion(),
                almacenamiento = new Almacenamiento(),
                basicos = new Basico() { cuit = "1", RazonSocial = "1", segmentacion = 1, calificacion = 1 },
                contacto = new Contacto(),
                contactocomercial = new List<ContactosComercial>(),
                ProveedorCorredorId = 0,
                ProveedorId = 1
            };


            //para pasar el logDataA
            comercialManagerMock.Setup(x => x.ListarEquipo(It.IsAny<string>())).Returns(new EquipoDto { Equipo = new List<int> { 1, 2, 3 } });

            var comercial = new Comercial { ComercialId = 1, Apellido = "a", Nombres = "a", PerfilId = 7, IdActiveDirectory = "a" };
            var campana = new Campaña { CampañaId = 1, Descripcion = "a" };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(comercial);
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()))
                .Returns(new List<BasicoProveedor>() { new BasicoProveedor() { NoOperable = true, Operando = true, EstadoCuit = 2, Facacop = 0 } });
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()))
                .Returns(new List<ActividadTraer>() { new ActividadTraer() });
            repositorioMock.Setup(y => y.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ContactosComerciales>() { new ContactosComerciales() });
            repositorioMock.Setup(y => y.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ObjetivosTraer>() { new ObjetivosTraer() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioMaterialPorProveedor>() { new AcopioMaterialPorProveedor { AcopioId = 1,
                    AcopioMaterialId = 1,
                    CampañaId = 1,
                    Campaña = "a",
                    MaterialId = 1,
                    NroItem = 1,
                    Toneladas = 1,
                    Material = "a",
                    LocalidadId = 1,
                    Localidad = "a",
                    ProvinciaId = 1,
                    Provincia = "a" } });
            repositorioMock.Setup(y => y.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampoProduccionAcopio>() { new CampoProduccionAcopio() { EsCampoProduccion = true, ProveedorId = 1 }, new CampoProduccionAcopio() { EsCampoProduccion = false, ProveedorId = 1, } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()))
                .Returns(new DatosContacto { });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Material>() { new Material { CampañaId = 1, Campaña = campana, Descripcion = "a", MaterialId = 1, Codigo = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                 .Returns(new List<CampañaMaterial>() { new CampañaMaterial { CampañaId = 1, Campaña = campana } });
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCorredorDto>() { new ProveedorCorredorDto() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
            .Returns(new List<CampanaMaterialDetallePorMes>() { new CampanaMaterialDetallePorMes() {
                 CampanaId = 1,
                     MaterialId = 2,
                     ProveedorId = 1,
                     Material = new Material
                     {
                         Descripcion = "Soja",
                         MaterialId = 1,
                         CampañaId = 1
                     },
                     Campana = new Campaña
                     {
                         Descripcion = "11",
                         CampañaId = 1
                     },
                 ClaseDoc = "as",
                 Clasificacion = "PRODUCTOR",
                 ComercialId = 1,
                 CorredorCuit = "2321123",
                 PendienteAFijar = 1,
                 PendienteAplicar = 1,
                 ToneladaAmpliada = 0,
                 ToneladaAnulada = 1,
                 ToneladaAplicada = 3,
                 ToneladaContrato = 2,
                 ToneladaFijada = 2,
                 Fecha = DateTime.Now,
                 CampanaMaterialDetalleId = 1,
                 Contrato = "11233"
             } });
            //para pasar el logDataA



            //validar proveedor
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(false);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<SISA, bool>>>()))
                .Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()))
                .Returns(false);

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(new Comercial { IdActiveDirectory = "a", ComercialId = 1, PerfilId = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Estado>() { new Estado { Descripcion = "a", EstadoId = 1 } });

            datosProveedorMock.Setup(y => y.ObtenerDatosDeProveedor(It.IsAny<List<Datos>>()))
                .Returns(new List<DatosProveedorAgentDto>());


            var result = target.GrabarNuevoProveedor(proveedor, "1");

            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<SISA, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Proveedor>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.NotNull(result);
            Assert.IsFalse(result.HayErrores);
        }
        [Test]
        public void GrabarNuevoProveedorSinSAPTest()
        {
            ConfigurationManager.AppSettings["SinConexionSap"] = "1";
            ConfigurationManager.AppSettings["NoCliente"] = "1";
            ConfigurationManager.AppSettings["PotencialCliente"] = "2";
            var fecha = new DateTime(2019, 1, 19);
            var proveedor = new NuevoProveedor
            {
                produccion = new Produccion(),
                almacenamiento = new Almacenamiento(),
                basicos = new Basico() { cuit = "1", RazonSocial = "1", segmentacion = 1, calificacion = 1 },
                contacto = new Contacto(),
                contactocomercial = new List<ContactosComercial>(),
                ProveedorCorredorId = 0,
                ProveedorId = 1
            };


            //para pasar el logDataA
            comercialManagerMock.Setup(x => x.ListarEquipo(It.IsAny<string>())).Returns(new EquipoDto { Equipo = new List<int> { 1, 2, 3 } });

            var comercial = new Comercial { ComercialId = 1, Apellido = "a", Nombres = "a", PerfilId = 7, IdActiveDirectory = "a" };
            var campana = new Campaña { CampañaId = 1, Descripcion = "a" };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(comercial);
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()))
                .Returns(new List<BasicoProveedor>() { new BasicoProveedor() { NoOperable = true, Operando = true, EstadoCuit = 2, Facacop = 0 } });
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()))
                .Returns(new List<ActividadTraer>() { new ActividadTraer() });
            repositorioMock.Setup(y => y.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ContactosComerciales>() { new ContactosComerciales() });
            repositorioMock.Setup(y => y.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ObjetivosTraer>() { new ObjetivosTraer() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioMaterialPorProveedor>() { new AcopioMaterialPorProveedor { AcopioId = 1,
                    AcopioMaterialId = 1,
                    CampañaId = 1,
                    Campaña = "a",
                    MaterialId = 1,
                    NroItem = 1,
                    Toneladas = 1,
                    Material = "a",
                    LocalidadId = 1,
                    Localidad = "a",
                    ProvinciaId = 1,
                    Provincia = "a" } });
            repositorioMock.Setup(y => y.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampoProduccionAcopio>() { new CampoProduccionAcopio() { EsCampoProduccion = true, ProveedorId = 1 }, new CampoProduccionAcopio() { EsCampoProduccion = false, ProveedorId = 1, } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()))
                .Returns(new DatosContacto { });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Material>() { new Material { CampañaId = 1, Campaña = campana, Descripcion = "a", MaterialId = 1, Codigo = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                 .Returns(new List<CampañaMaterial>() { new CampañaMaterial { CampañaId = 1, Campaña = campana } });
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCorredorDto>() { new ProveedorCorredorDto() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
            .Returns(new List<CampanaMaterialDetallePorMes>() { new CampanaMaterialDetallePorMes() {
                 CampanaId = 1,
                     MaterialId = 2,
                     ProveedorId = 1,
                     Material = new Material
                     {
                         Descripcion = "Soja",
                         MaterialId = 1,
                         CampañaId = 1
                     },
                     Campana = new Campaña
                     {
                         Descripcion = "11",
                         CampañaId = 1
                     },
                 ClaseDoc = "as",
                 Clasificacion = "PRODUCTOR",
                 ComercialId = 1,
                 CorredorCuit = "2321123",
                 PendienteAFijar = 1,
                 PendienteAplicar = 1,
                 ToneladaAmpliada = 0,
                 ToneladaAnulada = 1,
                 ToneladaAplicada = 3,
                 ToneladaContrato = 2,
                 ToneladaFijada = 2,
                 Fecha = DateTime.Now,
                 CampanaMaterialDetalleId = 1,
                 Contrato = "11233"
             } });
            //para pasar el logDataA



            //validar proveedor
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(false);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<SISA, bool>>>()))
                .Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()))
                .Returns(false);

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(new Comercial { IdActiveDirectory = "a", ComercialId = 1, PerfilId = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Estado>() { new Estado { Descripcion = "a", EstadoId = 1 } });

            datosProveedorMock.Setup(y => y.ObtenerDatosDeProveedor(It.IsAny<List<Datos>>()))
                .Returns(new List<DatosProveedorAgentDto>());


            var result = target.GrabarNuevoProveedor(proveedor, "1");

            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<SISA, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Proveedor>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.NotNull(result);
            Assert.IsFalse(result.HayErrores);
        }

        [Test]
        public void UpdateProveedorOkTest()
        {
            ConfigurationManager.AppSettings["SinConexionSap"] = "0";
            var fecha = new DateTime(2019, 1, 19);
            var proveedor = new NuevoProveedor
            {
                produccion = new Produccion
                {
                    CamposProduccion = new List<CamposProduccion>()
                {
                    new CamposProduccion { CampoId= 1,granos= new List<Granos>(){ new Granos { granoId = 1, campañaId = 1 }, new Granos { granoId = 1, campañaId = 2 } } ,eliminarproduccion = new List<Granos>(){ new Granos { granoId = 1, campañaId = 1 } } },
                    new CamposProduccion { CampoId= 0, granos= new List<Granos>(){ new Granos { granoId= 1, campañaId= 1 } } }
                },
                    habilitaoSojaSust = "null",
                    objetivos = new List<Objetivos>() {
                        new Objetivos {campañaId=1,granoId=2,toneladasObjetivo="10" },
                        new Objetivos {campañaId=2,granoId=2,toneladasObjetivo="10" } },
                    eliminarobjetivos = new List<Objetivos>() {
                        new Objetivos {campañaId=1,granoId=1,toneladasObjetivo="10" }}
                },
                almacenamiento = new Almacenamiento
                {
                    CamposAlmacenamiento = new List<CamposAlmacenamiento>{
                        new CamposAlmacenamiento{ CampoId=1 },
                        new CamposAlmacenamiento{ CampoId=2,
                            eliminargranoalmacenamientograno = new List<GranosAlmacenamientoGrano>(){ new GranosAlmacenamientoGrano { granoId=1,campañaId=1} },
                            eliminargranoalmacenamiento = new List<GranosAlmacenamiento>(){ new GranosAlmacenamiento { campañaId= 1 } },
                            granosAlmacenamientoGrano = new  List<GranosAlmacenamientoGrano>(){
                                new GranosAlmacenamientoGrano { campañaId=2, granoId=2 },
                                new GranosAlmacenamientoGrano { campañaId=1, granoId=1 } },
                            granosAlmacenamiento = new List<GranosAlmacenamiento>(){ new GranosAlmacenamiento { campañaId= 2 }, new GranosAlmacenamiento { campañaId = 1 } }
                        },
                        new CamposAlmacenamiento {
                            CampoId = 0,
                            granosAlmacenamientoGrano = new  List<GranosAlmacenamientoGrano>(){ new GranosAlmacenamientoGrano { campañaId=1, granoId=1 } },
                            granosAlmacenamiento = new List<GranosAlmacenamiento>(){ new GranosAlmacenamiento { campañaId= 1 } } } }
                },
                basicos = new Basico() { cuit = "1", RazonSocial = "1", segmentacion = 1, calificacion = 1 },
                contacto = new Contacto() { canalesOperacion = new List<int>() { 2 }, entregaA = new List<int>() { 2 }, condPreferentes = new List<int>() { 2 } },
                contactocomercial = new List<ContactosComercial>() {
                    new ContactosComercial {
                        contactoComercialId= 2,
                        intereses =new List<int> { 2 },
                        telefonos = new List<Telefono> { new Telefono(), new Telefono(), new Telefono() },
                        emails = new List<string>(){"","","" },
                        CompraNet=false ,Cupo=false},
                    new ContactosComercial {
                        contactoComercialId=0,
                        intereses =new List<int>(){ 2,3 },
                        telefonos = new List<Telefono> { new Telefono(), new Telefono(), new Telefono() },
                        emails = new List<string>(){"","","" },
                        CompraNet =false ,Cupo=false}
                        },
                ProveedorCorredorId = 0,
                ProveedorId = 1,
                establecimiento = new List<CampoDetalleDto> {
                    new CampoDetalleDto { archivo = "", archivoFileResult = "", CampoId = 0, comercialId = 1, hcultivables = 1, htotales = 1, ImportId = 1, rinde = 1, latitud = "", longitud = "", nombre = "", localidadId = 1, materialId = 1 },
                    new CampoDetalleDto { archivo = "", archivoFileResult = "", CampoId = 1, comercialId = 1, hcultivables = 1, htotales = 1, ImportId = 1, rinde = 1, latitud = "", longitud = "", nombre = "", localidadId = 1, materialId = 1 },
                }

            };
            //UpdateDatosBasicosProveedor
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", RazonSocial = "a", SegmentacionId = 1, Calificacion = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Proveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
         .Returns(new List<Proveedor>() { new Proveedor { CUIT = "a" } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(new Proveedor { CUIT = "1", RazonSocial = "a", SegmentacionId = 1, Calificacion = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(new Comercial { IdActiveDirectory = "a", ComercialId = 1, PerfilId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>()))
               .Returns("111111");
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, string>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<string>() { "a" });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Estado>() { new Estado { Descripcion = "a", EstadoId = 1 } });
            repositorioMock.Setup(y => y.ListarConsulta(It.IsAny<ActualizarComercialHome>()))
                .Returns(new List<Datos>() { new Datos { CUIT = "1", UsuarioDirectory = "a" } });
            datosProveedorMock.Setup(y => y.ObtenerDatosDeProveedor(It.IsAny<List<Datos>>()))
                .Returns(new List<DatosProveedorAgentDto>() { new DatosProveedorAgentDto { USUARIO = "a", CLIENTE_MOA = "X", CUIT = "a", STATUS = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorEstado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorEstado>() { new ProveedorEstado { ComercialId = 1, EstadoId = 1, ProveedorId = 1, ProveedorEstadoId = 1 } });
            //UpdateDatosContacto - UpdateCanalOperacion
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCanalOperacion>() { new ProveedorCanalOperacion { ProveedorId = 1, CanalOperacionId = 1, ContactoCanalOperacionId = 1, NroItem = "1" } });
            //UpdateDatosContacto - UpdateDestinatario
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorDestinatario>() { new ProveedorDestinatario { DestinatarioId = 1, NroItem = 1, ProveedorId = 1, ContactoDestinatarioId = 1 } });
            //UpdateDatosContacto - UpdateCondicion
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCondicion>() { new ProveedorCondicion { ProveedorId = 1, NroItem = 1, CondicionId = 1, ContactoCondicionId = 1 } });
            //UpdateDatosContacto - UpdateObjetivos
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Objetivo, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Objetivo>() {
                    new Objetivo { ObjetivoId= 1,MaterialId=1,NroItem=1,CampañaId=1,ProveedorId=1,ToneladasObjetivos=100},
                    new Objetivo { ObjetivoId= 3,MaterialId=2,NroItem=1,CampañaId=2,ProveedorId=1,ToneladasObjetivos=100}});
            //UpdateContactoComerciales
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ContactoComercial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ContactoComercial>() {
                    new ContactoComercial {ContactoComercialId=1,ProveedorId = 1 },
                    new ContactoComercial {ContactoComercialId=2,ProveedorId = 1 }});
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ContactoComercialInteres, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ContactoComercialInteres>() { new ContactoComercialInteres { InteresId = 1, ContactoComercial = new ContactoComercial { ContactoComercialId = 1 } } });
            //UpdateProduccion
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Campo, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Campo>() { new Campo { CampoId = 1 }, new Campo { CampoId = 3 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampoMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CampoMaterial>() { new CampoMaterial { CampoId = 1, CampañaId = 1, MaterialId = 1 } });
            //UpdateAlmacenamiento
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Acopio, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Acopio>() { new Acopio { AcopioId = 1 }, new Acopio { AcopioId = 2 }, new Acopio { AcopioId = 3 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioMaterial>() { new AcopioMaterial { AcopioId = 1, CampañaId = 1, MaterialId = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioCampaña, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioCampaña>() { new AcopioCampaña { AcopioId = 1, CampañaId = 1, AcopioCampañaId = 1 } });
            //UpdateEstablecimiento
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampoDetalle, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CampoDetalle>() { new CampoDetalle { Id = 1 }, new CampoDetalle { Id = 3 } });
            comercialManagerMock.Setup(x => x.ListarEquipo(It.IsAny<string>())).Returns(new EquipoDto { Equipo = new List<int> { 1, 2, 3 } });

            var comercial = new Comercial { ComercialId = 1, Apellido = "a", Nombres = "a", PerfilId = 7, IdActiveDirectory = "a" };
            var campana = new Campaña { CampañaId = 1, Descripcion = "a" };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(comercial);
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()))
                .Returns(new List<BasicoProveedor>() { new BasicoProveedor() { NoOperable = true, Operando = true, EstadoCuit = 2, Facacop = 0 } });
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()))
                .Returns(new List<ActividadTraer>() { new ActividadTraer() });
            repositorioMock.Setup(y => y.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ContactosComerciales>() { new ContactosComerciales() });
            repositorioMock.Setup(y => y.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ObjetivosTraer>() { new ObjetivosTraer() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioMaterialPorProveedor>() { new AcopioMaterialPorProveedor { AcopioId = 1,
                    AcopioMaterialId = 1,
                    CampañaId = 1,
                    Campaña = "a",
                    MaterialId = 1,
                    NroItem = 1,
                    Toneladas = 1,
                    Material = "a",
                    LocalidadId = 1,
                    Localidad = "a",
                    ProvinciaId = 1,
                    Provincia = "a" } });
            repositorioMock.Setup(y => y.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampoProduccionAcopio>() { new CampoProduccionAcopio() { EsCampoProduccion = true, ProveedorId = 1 }, new CampoProduccionAcopio() { EsCampoProduccion = false, ProveedorId = 1, } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()))
                .Returns(new DatosContacto { });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Material>() { new Material { CampañaId = 1, Campaña = campana, Descripcion = "a", MaterialId = 1, Codigo = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                 .Returns(new List<CampañaMaterial>() { new CampañaMaterial { CampañaId = 1, Campaña = campana } });
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCorredorDto>() { new ProveedorCorredorDto() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
            .Returns(new List<CampanaMaterialDetallePorMes>() { new CampanaMaterialDetallePorMes() {
                 CampanaId = 1,
                     MaterialId = 2,
                     ProveedorId = 1,
                     Material = new Material
                     {
                         Descripcion = "Soja",
                         MaterialId = 1,
                         CampañaId = 1
                     },
                     Campana = new Campaña
                     {
                         Descripcion = "11",
                         CampañaId = 1
                     },
                 ClaseDoc = "as",
                 Clasificacion = "PRODUCTOR",
                 ComercialId = 1,
                 CorredorCuit = "2321123",
                 PendienteAFijar = 1,
                 PendienteAplicar = 1,
                 ToneladaAmpliada = 0,
                 ToneladaAnulada = 1,
                 ToneladaAplicada = 3,
                 ToneladaContrato = 2,
                 ToneladaFijada = 2,
                 Fecha = DateTime.Now,
                 CampanaMaterialDetalleId = 1,
                 Contrato = "11233"
             } });
            var result = target.UpdateProveedor(proveedor, "a", new List<int>() { 1, 2, 3 }, 1);

            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Exactly(3));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Exactly(3));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            //repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<ActualizarComercialHome>()), Times.Once);
            datosProveedorMock.Verify(x => x.ObtenerDatosDeProveedor(It.IsAny<List<Datos>>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorEstado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<ProveedorCanalOperacion>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<ProveedorCanalOperacion>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<ProveedorDestinatario>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<ProveedorDestinatario>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<ProveedorCondicion>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<ProveedorCondicion>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Objetivo, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<Objetivo>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Objetivo>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ContactoComercial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<ContactoComercial>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<ContactoComercialInteres>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Campo, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CampoMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Exactly(2));
            repositorioMock.Verify(x => x.Remover(It.IsAny<Campo>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<CampoMaterial>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Campo>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<CampoMaterial>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.RemoverTodos(It.IsAny<List<AcopioMaterial>>()), Times.Once);
            repositorioMock.Verify(x => x.RemoverTodos(It.IsAny<List<AcopioCampaña>>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<Acopio>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<AcopioMaterial>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Remover(It.IsAny<AcopioCampaña>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Acopio>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<AcopioCampaña>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Agregar(It.IsAny<AcopioMaterial>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Acopio, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
            Assert.NotNull(result);
            Assert.AreEqual(1, result.ProveedorId);
            Assert.IsFalse(result.HayErrores);
        }
        [Test]
        public void UpdateProveedorSinSapOkTest()
        {
            ConfigurationManager.AppSettings["SinConexionSap"] = "1";
            ConfigurationManager.AppSettings["NoCliente"] = "1";
            ConfigurationManager.AppSettings["PotencialCliente"] = "2";
            var fecha = new DateTime(2019, 1, 19);
            var proveedor = new NuevoProveedor
            {
                produccion = new Produccion
                {
                    CamposProduccion = new List<CamposProduccion>()
                {
                    new CamposProduccion { CampoId= 1,granos= new List<Granos>(){ new Granos { granoId = 1, campañaId = 1 }, new Granos { granoId = 1, campañaId = 2 } } ,eliminarproduccion = new List<Granos>(){ new Granos { granoId = 1, campañaId = 1 } } },
                    new CamposProduccion { CampoId= 0, granos= new List<Granos>(){ new Granos { granoId= 1, campañaId= 1 } } }
                },
                    habilitaoSojaSust = "null",
                    objetivos = new List<Objetivos>() {
                        new Objetivos {campañaId=1,granoId=2,toneladasObjetivo="10" },
                        new Objetivos {campañaId=2,granoId=2,toneladasObjetivo="10" } },
                    eliminarobjetivos = new List<Objetivos>() {
                        new Objetivos {campañaId=1,granoId=1,toneladasObjetivo="10" }}
                },
                almacenamiento = new Almacenamiento
                {
                    CamposAlmacenamiento = new List<CamposAlmacenamiento>{
                        new CamposAlmacenamiento{ CampoId=1 },
                        new CamposAlmacenamiento{ CampoId=2,
                            eliminargranoalmacenamientograno = new List<GranosAlmacenamientoGrano>(){ new GranosAlmacenamientoGrano { granoId=1,campañaId=1} },
                            eliminargranoalmacenamiento = new List<GranosAlmacenamiento>(){ new GranosAlmacenamiento { campañaId= 1 } },
                            granosAlmacenamientoGrano = new  List<GranosAlmacenamientoGrano>(){
                                new GranosAlmacenamientoGrano { campañaId=2, granoId=2 },
                                new GranosAlmacenamientoGrano { campañaId=1, granoId=1 } },
                            granosAlmacenamiento = new List<GranosAlmacenamiento>(){ new GranosAlmacenamiento { campañaId= 2 }, new GranosAlmacenamiento { campañaId = 1 } }
                        },
                        new CamposAlmacenamiento {
                            CampoId = 0,
                            granosAlmacenamientoGrano = new  List<GranosAlmacenamientoGrano>(){ new GranosAlmacenamientoGrano { campañaId=1, granoId=1 } },
                            granosAlmacenamiento = new List<GranosAlmacenamiento>(){ new GranosAlmacenamiento { campañaId= 1 } } } }
                },
                basicos = new Basico() { cuit = "1", RazonSocial = "1", segmentacion = 1, calificacion = 1 },
                contacto = new Contacto() { canalesOperacion = new List<int>() { 2 }, entregaA = new List<int>() { 2 }, condPreferentes = new List<int>() { 2 } },
                contactocomercial = new List<ContactosComercial>() {
                    new ContactosComercial {
                        contactoComercialId= 2,
                        intereses =new List<int> { 2 },
                        telefonos = new List<Telefono> { new Telefono(), new Telefono(), new Telefono() },
                        emails = new List<string>(){"","","" },
                        CompraNet=false ,Cupo=false},
                    new ContactosComercial {
                        contactoComercialId=0,
                        intereses =new List<int>(){ 2,3 },
                        telefonos = new List<Telefono> { new Telefono(), new Telefono(), new Telefono() },
                        emails = new List<string>(){"","","" },
                        CompraNet =false ,Cupo=false}
                        },
                ProveedorCorredorId = 0,
                ProveedorId = 1
            };
            //UpdateDatosBasicosProveedor
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", RazonSocial = "a", SegmentacionId = 1, Calificacion = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Proveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
             .Returns(new List<Proveedor>() { new Proveedor { CUIT = "a" } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(new Proveedor { CUIT = "1", RazonSocial = "a", SegmentacionId = 1, Calificacion = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(new Comercial { IdActiveDirectory = "a", ComercialId = 1, PerfilId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>()))
               .Returns("111111");
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, string>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<string>() { "a" });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Estado>() { new Estado { Descripcion = "a", EstadoId = 1 } });
            repositorioMock.Setup(y => y.ListarConsulta(It.IsAny<ActualizarComercialHome>()))
                .Returns(new List<Datos>() { new Datos { CUIT = "1", UsuarioDirectory = "a" } });
            datosProveedorMock.Setup(y => y.ObtenerDatosDeProveedor(It.IsAny<List<Datos>>()))
                .Returns(new List<DatosProveedorAgentDto>() { new DatosProveedorAgentDto { USUARIO = "a", CLIENTE_MOA = "X", CUIT = "a", STATUS = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorEstado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorEstado>() { new ProveedorEstado { ComercialId = 1, EstadoId = 1, ProveedorId = 1, ProveedorEstadoId = 1 } });
            //UpdateDatosContacto - UpdateCanalOperacion
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCanalOperacion>() { new ProveedorCanalOperacion { ProveedorId = 1, CanalOperacionId = 1, ContactoCanalOperacionId = 1, NroItem = "1" } });
            //UpdateDatosContacto - UpdateDestinatario
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorDestinatario>() { new ProveedorDestinatario { DestinatarioId = 1, NroItem = 1, ProveedorId = 1, ContactoDestinatarioId = 1 } });
            //UpdateDatosContacto - UpdateCondicion
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCondicion>() { new ProveedorCondicion { ProveedorId = 1, NroItem = 1, CondicionId = 1, ContactoCondicionId = 1 } });
            //UpdateDatosContacto - UpdateObjetivos
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Objetivo, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Objetivo>() {
                    new Objetivo { ObjetivoId= 1,MaterialId=1,NroItem=1,CampañaId=1,ProveedorId=1,ToneladasObjetivos=100},
                    new Objetivo { ObjetivoId= 3,MaterialId=2,NroItem=1,CampañaId=2,ProveedorId=1,ToneladasObjetivos=100}});
            //UpdateContactoComerciales
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ContactoComercial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ContactoComercial>() {
                    new ContactoComercial {ContactoComercialId=1,ProveedorId = 1 },
                    new ContactoComercial {ContactoComercialId=2,ProveedorId = 1 }});
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ContactoComercialInteres, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ContactoComercialInteres>() { new ContactoComercialInteres { InteresId = 1, ContactoComercial = new ContactoComercial { ContactoComercialId = 1 } } });
            //UpdateProduccion
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Campo, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Campo>() { new Campo { CampoId = 1 }, new Campo { CampoId = 3 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampoMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CampoMaterial>() { new CampoMaterial { CampoId = 1, CampañaId = 1, MaterialId = 1 } });
            //UpdateAlmacenamiento
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Acopio, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Acopio>() { new Acopio { AcopioId = 1 }, new Acopio { AcopioId = 2 }, new Acopio { AcopioId = 3 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioMaterial>() { new AcopioMaterial { AcopioId = 1, CampañaId = 1, MaterialId = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioCampaña, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioCampaña>() { new AcopioCampaña { AcopioId = 1, CampañaId = 1, AcopioCampañaId = 1 } });
            //UpdateEstablecimiento
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampoDetalle, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CampoDetalle>() { new CampoDetalle { Id = 1 }, new CampoDetalle { Id = 3 } });
            comercialManagerMock.Setup(x => x.ListarEquipo(It.IsAny<string>())).Returns(new EquipoDto { Equipo = new List<int> { 1, 2, 3 } });

            var comercial = new Comercial { ComercialId = 1, Apellido = "a", Nombres = "a", PerfilId = 7, IdActiveDirectory = "a" };
            var campana = new Campaña { CampañaId = 1, Descripcion = "a" };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(comercial);
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()))
                .Returns(new List<BasicoProveedor>() { new BasicoProveedor() { NoOperable = true, Operando = true, EstadoCuit = 2, Facacop = 0 } });
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()))
                .Returns(new List<ActividadTraer>() { new ActividadTraer() });
            repositorioMock.Setup(y => y.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ContactosComerciales>() { new ContactosComerciales() });
            repositorioMock.Setup(y => y.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ObjetivosTraer>() { new ObjetivosTraer() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioMaterialPorProveedor>() { new AcopioMaterialPorProveedor { AcopioId = 1,
                    AcopioMaterialId = 1,
                    CampañaId = 1,
                    Campaña = "a",
                    MaterialId = 1,
                    NroItem = 1,
                    Toneladas = 1,
                    Material = "a",
                    LocalidadId = 1,
                    Localidad = "a",
                    ProvinciaId = 1,
                    Provincia = "a" } });
            repositorioMock.Setup(y => y.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampoProduccionAcopio>() { new CampoProduccionAcopio() { EsCampoProduccion = true, ProveedorId = 1 }, new CampoProduccionAcopio() { EsCampoProduccion = false, ProveedorId = 1, } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()))
                .Returns(new DatosContacto { });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Material>() { new Material { CampañaId = 1, Campaña = campana, Descripcion = "a", MaterialId = 1, Codigo = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                 .Returns(new List<CampañaMaterial>() { new CampañaMaterial { CampañaId = 1, Campaña = campana } });
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCorredorDto>() { new ProveedorCorredorDto() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
            .Returns(new List<CampanaMaterialDetallePorMes>() { new CampanaMaterialDetallePorMes() {
                 CampanaId = 1,
                     MaterialId = 2,
                     ProveedorId = 1,
                     Material = new Material
                     {
                         Descripcion = "Soja",
                         MaterialId = 1,
                         CampañaId = 1
                     },
                     Campana = new Campaña
                     {
                         Descripcion = "11",
                         CampañaId = 1
                     },
                 ClaseDoc = "as",
                 Clasificacion = "PRODUCTOR",
                 ComercialId = 1,
                 CorredorCuit = "2321123",
                 PendienteAFijar = 1,
                 PendienteAplicar = 1,
                 ToneladaAmpliada = 0,
                 ToneladaAnulada = 1,
                 ToneladaAplicada = 3,
                 ToneladaContrato = 2,
                 ToneladaFijada = 2,
                 Fecha = DateTime.Now,
                 CampanaMaterialDetalleId = 1,
                 Contrato = "11233"
             } });
            var result = target.UpdateProveedor(proveedor, "a", new List<int>() { 1, 2, 3 }, 1);

            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Exactly(3));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            //repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<ActualizarComercialHome>()), Times.Once);
            datosProveedorMock.Verify(x => x.ObtenerDatosDeProveedor(It.IsAny<List<Datos>>()), Times.Never);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorEstado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<ProveedorCanalOperacion>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<ProveedorCanalOperacion>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<ProveedorDestinatario>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<ProveedorDestinatario>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<ProveedorCondicion>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<ProveedorCondicion>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Objetivo, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<Objetivo>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Objetivo>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ContactoComercial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<ContactoComercial>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<ContactoComercialInteres>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Campo, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CampoMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Exactly(2));
            repositorioMock.Verify(x => x.Remover(It.IsAny<Campo>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<CampoMaterial>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Campo>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<CampoMaterial>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.RemoverTodos(It.IsAny<List<AcopioMaterial>>()), Times.Once);
            repositorioMock.Verify(x => x.RemoverTodos(It.IsAny<List<AcopioCampaña>>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<Acopio>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<AcopioMaterial>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Remover(It.IsAny<AcopioCampaña>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Acopio>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<AcopioCampaña>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Agregar(It.IsAny<AcopioMaterial>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Acopio, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
            Assert.NotNull(result);
            Assert.AreEqual(1, result.ProveedorId);
            Assert.IsFalse(result.HayErrores);
        }
        [Test]
        public void UpdateProveedorSinDatosProveedorTest()
        {
            ConfigurationManager.AppSettings["SinConexionSap"] = "0";
            ConfigurationManager.AppSettings["NoCliente"] = "1";
            ConfigurationManager.AppSettings["PotencialCliente"] = "2";
            var fecha = new DateTime(2019, 1, 19);
            var proveedor = new NuevoProveedor
            {
                produccion = new Produccion
                {
                    CamposProduccion = new List<CamposProduccion>()
                {

                    new CamposProduccion { CampoId= 1}
                },
                    habilitaoSojaSust = "null"
                },
                basicos = new Basico() { cuit = "1", RazonSocial = "1", segmentacion = 1, calificacion = 1 },
                contacto = new Contacto() { canalesOperacion = new List<int>(), entregaA = new List<int>(), condPreferentes = new List<int>(), },
                contactocomercial = new List<ContactosComercial>(),
                ProveedorCorredorId = 0,
                ProveedorId = 1
            };


            //para pasar el logDataA
            comercialManagerMock.Setup(x => x.ListarEquipo(It.IsAny<string>())).Returns(new EquipoDto { Equipo = new List<int> { 1, 2, 3 } });

            var comercial = new Comercial { ComercialId = 1, Apellido = "a", Nombres = "a", PerfilId = 7, IdActiveDirectory = "a" };
            var campana = new Campaña { CampañaId = 1, Descripcion = "a" };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(comercial);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Proveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
         .Returns(new List<Proveedor>() { new Proveedor { CUIT = "a" } });
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()))
                .Returns(new List<BasicoProveedor>() { new BasicoProveedor() { NoOperable = true, Operando = true, EstadoCuit = 2, Facacop = 0 } });
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()))
                .Returns(new List<ActividadTraer>() { new ActividadTraer() });
            repositorioMock.Setup(y => y.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ContactosComerciales>() { new ContactosComerciales() });
            repositorioMock.Setup(y => y.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ObjetivosTraer>() { new ObjetivosTraer() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioMaterialPorProveedor>() { new AcopioMaterialPorProveedor { AcopioId = 1,
                    AcopioMaterialId = 1,
                    CampañaId = 1,
                    Campaña = "a",
                    MaterialId = 1,
                    NroItem = 1,
                    Toneladas = 1,
                    Material = "a",
                    LocalidadId = 1,
                    Localidad = "a",
                    ProvinciaId = 1,
                    Provincia = "a" } });
            repositorioMock.Setup(y => y.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampoProduccionAcopio>() { new CampoProduccionAcopio() { EsCampoProduccion = true, ProveedorId = 1 }, new CampoProduccionAcopio() { EsCampoProduccion = false, ProveedorId = 1, } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()))
                .Returns(new DatosContacto { });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Material>() { new Material { CampañaId = 1, Campaña = campana, Descripcion = "a", MaterialId = 1, Codigo = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                 .Returns(new List<CampañaMaterial>() { new CampañaMaterial { CampañaId = 1, Campaña = campana } });
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
            .Returns(new List<CampanaMaterialDetallePorMes>() { new CampanaMaterialDetallePorMes() {
                 CampanaId = 1,
                     MaterialId = 2,
                     ProveedorId = 1,
                     Material = new Material
                     {
                         Descripcion = "Soja",
                         MaterialId = 1,
                         CampañaId = 1
                     },
                     Campana = new Campaña
                     {
                         Descripcion = "11",
                         CampañaId = 1
                     },
                 ClaseDoc = "as",
                 Clasificacion = "PRODUCTOR",
                 ComercialId = 1,
                 CorredorCuit = "2321123",
                 PendienteAFijar = 1,
                 PendienteAplicar = 1,
                 ToneladaAmpliada = 0,
                 ToneladaAnulada = 1,
                 ToneladaAplicada = 3,
                 ToneladaContrato = 2,
                 ToneladaFijada = 2,
                 Fecha = DateTime.Now,
                 CampanaMaterialDetalleId = 1,
                 Contrato = "11233"
             } });
            //para pasar el logDataA



            //UpdateDatosBasicosProveedor
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", RazonSocial = "a", SegmentacionId = 1, Calificacion = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(new Proveedor { CUIT = "1", RazonSocial = "a", SegmentacionId = 1, Calificacion = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(new Comercial { IdActiveDirectory = "a", ComercialId = 1, PerfilId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>()))
               .Returns("111111");
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, string>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<string>() { "a" });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Estado>() { new Estado { Descripcion = "a", EstadoId = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorEstado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorEstado>());
            repositorioMock.Setup(y => y.ListarConsulta(It.IsAny<ActualizarComercialHome>()))
                .Returns(new List<Datos>() { new Datos { CUIT = "1", UsuarioDirectory = "a" } });
            datosProveedorMock.Setup(y => y.ObtenerDatosDeProveedor(It.IsAny<List<Datos>>()))
                .Returns(new List<DatosProveedorAgentDto>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorEstado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorEstado>() { });
            //UpdateDatosContacto - UpdateCanalOperacion
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCanalOperacion>());
            //UpdateDatosContacto - UpdateDestinatario
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorDestinatario>());
            //UpdateDatosContacto - UpdateCondicion
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCondicion>());
            //UpdateDatosContacto - UpdateObjetivos
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Objetivo, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Objetivo>());
            //UpdateDatosContacto
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ContactoComercial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ContactoComercial>());
            //UpdateProduccion
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Campo, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Campo>());
            //UpdateAlmacenamiento
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Acopio, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Acopio>());
            //UpdateEstablecimiento
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampoDetalle, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CampoDetalle>());
            comercialManagerMock.Setup(x => x.ListarEquipo(It.IsAny<string>())).Returns(new EquipoDto { Equipo = new List<int> { 1, 2, 3 } });



            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(comercial);
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()))
                .Returns(new List<BasicoProveedor>() { new BasicoProveedor() { NoOperable = true, Operando = true, EstadoCuit = 2, Facacop = 0 } });
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()))
                .Returns(new List<ActividadTraer>() { new ActividadTraer() });
            repositorioMock.Setup(y => y.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ContactosComerciales>() { new ContactosComerciales() });
            repositorioMock.Setup(y => y.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ObjetivosTraer>() { new ObjetivosTraer() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioMaterialPorProveedor>() { new AcopioMaterialPorProveedor { AcopioId = 1,
                    AcopioMaterialId = 1,
                    CampañaId = 1,
                    Campaña = "a",
                    MaterialId = 1,
                    NroItem = 1,
                    Toneladas = 1,
                    Material = "a",
                    LocalidadId = 1,
                    Localidad = "a",
                    ProvinciaId = 1,
                    Provincia = "a" } });
            repositorioMock.Setup(y => y.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampoProduccionAcopio>() { new CampoProduccionAcopio() { EsCampoProduccion = true, ProveedorId = 1 }, new CampoProduccionAcopio() { EsCampoProduccion = false, ProveedorId = 1, } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()))
                .Returns(new DatosContacto { });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Material>() { new Material { CampañaId = 1, Campaña = campana, Descripcion = "a", MaterialId = 1, Codigo = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                 .Returns(new List<CampañaMaterial>() { new CampañaMaterial { CampañaId = 1, Campaña = campana } });
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCorredorDto>() { new ProveedorCorredorDto() });

            var result = target.UpdateProveedor(proveedor, "a", new List<int>() { 1, 2, 3 }, 1);

            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Exactly(3));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<ProveedorEstado>()), Times.Once);
            datosProveedorMock.Verify(x => x.ObtenerDatosDeProveedor(It.IsAny<List<Datos>>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorEstado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Objetivo, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ContactoComercial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Campo, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CampoDetalle, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Acopio, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
            Assert.NotNull(result);
            Assert.AreEqual(1, result.ProveedorId);
            Assert.IsFalse(result.HayErrores);
        }
        [Test]
        public void UpdateProveedorSinDatosProveedorConEstadoTest()
        {
            ConfigurationManager.AppSettings["SinConexionSap"] = "0";
            ConfigurationManager.AppSettings["NoCliente"] = "1";
            ConfigurationManager.AppSettings["PotencialCliente"] = "2";
            var fecha = new DateTime(2019, 1, 19);
            var proveedor = new NuevoProveedor
            {
                produccion = new Produccion
                {
                    CamposProduccion = new List<CamposProduccion>()
                {

                    new CamposProduccion { CampoId= 1}
                },
                    habilitaoSojaSust = "null"
                },
                basicos = new Basico() { cuit = "1", RazonSocial = "1", segmentacion = 1, calificacion = 1 },
                contacto = new Contacto() { canalesOperacion = new List<int>(), entregaA = new List<int>(), condPreferentes = new List<int>(), },
                contactocomercial = new List<ContactosComercial>(),
                ProveedorCorredorId = 0,
                ProveedorId = 1
            };
            //UpdateDatosBasicosProveedor
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", RazonSocial = "a", SegmentacionId = 1, Calificacion = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Proveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
         .Returns(new List<Proveedor>() { new Proveedor { CUIT = "a" } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(new Proveedor { CUIT = "1", RazonSocial = "a", SegmentacionId = 1, Calificacion = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(new Comercial { IdActiveDirectory = "a", ComercialId = 1, PerfilId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>()))
               .Returns("111111");
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, string>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<string>() { "a" });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Estado>() { new Estado { Descripcion = "a", EstadoId = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorEstado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorEstado>() { new ProveedorEstado { ComercialId = 1, ProveedorId = 1, ProveedorEstadoId = 1 } });
            repositorioMock.Setup(y => y.ListarConsulta(It.IsAny<ActualizarComercialHome>()))
                .Returns(new List<Datos>() { new Datos { CUIT = "1", UsuarioDirectory = "a" } });
            datosProveedorMock.Setup(y => y.ObtenerDatosDeProveedor(It.IsAny<List<Datos>>()))
                .Returns(new List<DatosProveedorAgentDto>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorEstado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorEstado>() { });
            //UpdateDatosContacto - UpdateCanalOperacion
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCanalOperacion>());
            //UpdateDatosContacto - UpdateDestinatario
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorDestinatario>());
            //UpdateDatosContacto - UpdateCondicion
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCondicion>());
            //UpdateDatosContacto - UpdateObjetivos
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Objetivo, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Objetivo>());
            //UpdateDatosContacto
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ContactoComercial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ContactoComercial>());
            //UpdateProduccion
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Campo, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Campo>());
            //UpdateAlmacenamiento
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Acopio, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Acopio>());
            //UpdateEstablecimiento
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampoDetalle, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CampoDetalle>());
            comercialManagerMock.Setup(x => x.ListarEquipo(It.IsAny<string>())).Returns(new EquipoDto { Equipo = new List<int> { 1, 2, 3 } });

            var comercial = new Comercial { ComercialId = 1, Apellido = "a", Nombres = "a", PerfilId = 7, IdActiveDirectory = "a" };
            var campana = new Campaña { CampañaId = 1, Descripcion = "a" };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(comercial);
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()))
                .Returns(new List<BasicoProveedor>() { new BasicoProveedor() { NoOperable = true, Operando = true, EstadoCuit = 2, Facacop = 0 } });
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()))
                .Returns(new List<ActividadTraer>() { new ActividadTraer() });
            repositorioMock.Setup(y => y.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ContactosComerciales>() { new ContactosComerciales() });
            repositorioMock.Setup(y => y.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ObjetivosTraer>() { new ObjetivosTraer() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioMaterialPorProveedor>() { new AcopioMaterialPorProveedor { AcopioId = 1,
                    AcopioMaterialId = 1,
                    CampañaId = 1,
                    Campaña = "a",
                    MaterialId = 1,
                    NroItem = 1,
                    Toneladas = 1,
                    Material = "a",
                    LocalidadId = 1,
                    Localidad = "a",
                    ProvinciaId = 1,
                    Provincia = "a" } });
            repositorioMock.Setup(y => y.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampoProduccionAcopio>() { new CampoProduccionAcopio() { EsCampoProduccion = true, ProveedorId = 1 }, new CampoProduccionAcopio() { EsCampoProduccion = false, ProveedorId = 1, } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()))
                .Returns(new DatosContacto { });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Material>() { new Material { CampañaId = 1, Campaña = campana, Descripcion = "a", MaterialId = 1, Codigo = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                 .Returns(new List<CampañaMaterial>() { new CampañaMaterial { CampañaId = 1, Campaña = campana } });
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCorredorDto>() { new ProveedorCorredorDto() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
            .Returns(new List<CampanaMaterialDetallePorMes>() { new CampanaMaterialDetallePorMes() {
                 CampanaId = 1,
                     MaterialId = 2,
                     ProveedorId = 1,
                     Material = new Material
                     {
                         Descripcion = "Soja",
                         MaterialId = 1,
                         CampañaId = 1
                     },
                     Campana = new Campaña
                     {
                         Descripcion = "11",
                         CampañaId = 1
                     },
                 ClaseDoc = "as",
                 Clasificacion = "PRODUCTOR",
                 ComercialId = 1,
                 CorredorCuit = "2321123",
                 PendienteAFijar = 1,
                 PendienteAplicar = 1,
                 ToneladaAmpliada = 0,
                 ToneladaAnulada = 1,
                 ToneladaAplicada = 3,
                 ToneladaContrato = 2,
                 ToneladaFijada = 2,
                 Fecha = DateTime.Now,
                 CampanaMaterialDetalleId = 1,
                 Contrato = "11233"
             } });
            var result = target.UpdateProveedor(proveedor, "a", new List<int>() { 1, 2, 3 }, 1);

            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Exactly(3));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<ProveedorEstado>()), Times.Once);
            datosProveedorMock.Verify(x => x.ObtenerDatosDeProveedor(It.IsAny<List<Datos>>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorEstado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Objetivo, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ContactoComercial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Campo, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Acopio, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
            Assert.NotNull(result);
            Assert.AreEqual(1, result.ProveedorId);
            Assert.IsFalse(result.HayErrores);
        }

        [Test]
        public void TraerProveedorPorIdOkTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, ProveedorDto>>>())).Returns(new ProveedorDto { ProveedorId = 1 });
            var result = target.TraerProveedor(1);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, ProveedorDto>>>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.ProveedorId);
        }
        [Test]
        public void TraerProveedorSinIdOkTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, ProveedorDto>>>())).Returns(new ProveedorDto { ProveedorId = 1 });
            var result = target.TraerProveedor(null);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, ProveedorDto>>>()), Times.Never);
            Assert.IsNull(result);
        }

        [Test]
        public void ObtenerReporteProveedorOkTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, int>>>()))
                .Returns(1);
            repositorioMock.Setup(x => x.SelStore<ReporteProveedor>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<ReporteProveedor>());
            var result = target.ObtenerReporteProveedor("aa", "a");

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, int>>>()), Times.Once);
            repositorioMock.Verify(x => x.SelStore<ReporteProveedor>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(0, result.Count);
        }
        [Test]
        public void DevolverProveedoresConCorredorOkTest()
        {
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<DevolverProveedoresConCorredor>()))
                .Returns(new List<BusquedaHome>());

            var result = target.DevolverProveedoresConCorredor("aa", "a");

            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<DevolverProveedoresConCorredor>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(0, result.Count);
        }
        [Test]
        public void DevolverProveedoresOkTest()
        {
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<DevolverProveedores>()))
                .Returns(new List<BusquedaHome>());

            var result = target.DevolverProveedores("aa", It.IsAny<int>(), new List<int>() { 1, 2, 3 });

            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<DevolverProveedores>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(0, result.Count);
        }
        [Test]
        public void ListarProveedorOkTest()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Proveedor, ProveedorDto>>>(), It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorDto>());

            var result = target.ListarProveedor("aa");

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Proveedor, ProveedorDto>>>(), It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(0, result.Count);
        }
        [Test]
        public void ListarCorredorOkTest()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Proveedor, ProveedorDto>>>(), It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorDto>());

            var result = target.ListarCorredor("aa");

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Proveedor, ProveedorDto>>>(), It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(0, result.Count);
        }
        [Test]
        public void ListarProveedorTodosOkTest()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Proveedor, ProveedorDto>>>(), It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorDto>());

            var result = target.ListarProveedorTodos();

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Proveedor, ProveedorDto>>>(), It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(0, result.Count);
        }
        [Test]
        public void ListarProveedorCorredorOkTest()
        {
            ConfigurationManager.AppSettings["RiesgoComercialAltoSap"] = "1";
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCorredorDto> { new ProveedorCorredorDto { RiesgoComercialSap = "1", Facacop = true } });

            var result = target.ListarProveedorCorredor(1);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
        }
        [Test]
        public void ListarProveedorCorredorEstado3OkTest()
        {
            ConfigurationManager.AppSettings["RiesgoComercialAltoSap"] = "1";
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCorredorDto> { new ProveedorCorredorDto { RiesgoComercialSap = "1", Facacop = true, EstadoCuit = 3 } });
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<SISA, bool>>>(), It.IsAny<Expression<Func<SISA, int>>>()))
               .Returns(3);
            repositorioMock.Setup(x => x.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()))
                .Returns(true);

            var result = target.ListarProveedorCorredor(1);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
        }
        [Test]
        public void TraerProveedorParaCorredorOkTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, ProveedorDto>>>()))
                .Returns(new ProveedorDto() { ProveedorId = 1 });

            var result = target.TraerProveedorParaCorredor("a");

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, ProveedorDto>>>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Proveedor.ProveedorId);
            Assert.IsFalse(result.HayErrores);
        }
        [Test]
        public void GrabarNuevoCorredorYNuevoProveedorOkTest()
        {
            ConfigurationManager.AppSettings["SinConexionSap"] = "0";
            var fecha = new DateTime(2019, 1, 19);
            var corredor = new NuevoCorredor
            {
                basicos = new Basico() { cuit = "1", RazonSocial = "1", segmentacion = 1, calificacion = 1 },
                contacto = new Contacto(),
                contactocomercial = new List<ContactosComercial>(),
                proveedorCorredor = new List<NuevoProveedor>() { new NuevoProveedor
                {
                    basicos = new Basico() { cuit = "1", RazonSocial = "1", segmentacion = 1, calificacion = 1 },
                    contacto = new Contacto() { canalesOperacion = new List<int>(), entregaA = new List<int>(), condPreferentes = new List<int>(), },
                    contactocomercial = new List<ContactosComercial>()
                } },
                CorredorId = 0
            };


            //para pasar el logDataA
            comercialManagerMock.Setup(x => x.ListarEquipo(It.IsAny<string>())).Returns(new EquipoDto { Equipo = new List<int> { 1, 2, 3 } });

            var comercial = new Comercial { ComercialId = 1, Apellido = "a", Nombres = "a", PerfilId = 7, IdActiveDirectory = "a" };
            var campana = new Campaña { CampañaId = 1, Descripcion = "a" };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(comercial);
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()))
                .Returns(new List<BasicoProveedor>() { new BasicoProveedor() { NoOperable = true, Operando = true, EstadoCuit = 2, Facacop = 0 } });
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()))
                .Returns(new List<ActividadTraer>() { new ActividadTraer() });
            repositorioMock.Setup(y => y.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ContactosComerciales>() { new ContactosComerciales() });
            repositorioMock.Setup(y => y.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ObjetivosTraer>() { new ObjetivosTraer() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioMaterialPorProveedor>() { new AcopioMaterialPorProveedor { AcopioId = 1,
                    AcopioMaterialId = 1,
                    CampañaId = 1,
                    Campaña = "a",
                    MaterialId = 1,
                    NroItem = 1,
                    Toneladas = 1,
                    Material = "a",
                    LocalidadId = 1,
                    Localidad = "a",
                    ProvinciaId = 1,
                    Provincia = "a" } });
            repositorioMock.Setup(y => y.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampoProduccionAcopio>() { new CampoProduccionAcopio() { EsCampoProduccion = true, ProveedorId = 1 }, new CampoProduccionAcopio() { EsCampoProduccion = false, ProveedorId = 1, } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()))
                .Returns(new DatosContacto { });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Material>() { new Material { CampañaId = 1, Campaña = campana, Descripcion = "a", MaterialId = 1, Codigo = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                 .Returns(new List<CampañaMaterial>() { new CampañaMaterial { CampañaId = 1, Campaña = campana } });
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
              .Returns(new List<ProveedorCorredorDto>() { new ProveedorCorredorDto() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
            .Returns(new List<CampanaMaterialDetallePorMes>() { new CampanaMaterialDetallePorMes() {
                 CampanaId = 1,
                     MaterialId = 2,
                     ProveedorId = 1,
                     Material = new Material
                     {
                         Descripcion = "Soja",
                         MaterialId = 1,
                         CampañaId = 1
                     },
                     Campana = new Campaña
                     {
                         Descripcion = "11",
                         CampañaId = 1
                     },
                 ClaseDoc = "as",
                 Clasificacion = "PRODUCTOR",
                 ComercialId = 1,
                 CorredorCuit = "2321123",
                 PendienteAFijar = 1,
                 PendienteAplicar = 1,
                 ToneladaAmpliada = 0,
                 ToneladaAnulada = 1,
                 ToneladaAplicada = 3,
                 ToneladaContrato = 2,
                 ToneladaFijada = 2,
                 Fecha = DateTime.Now,
                 CampanaMaterialDetalleId = 1,
                 Contrato = "11233"
             } });
            //para pasar el logDataA



            //validar proveedor
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(false);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<SISA, bool>>>()))
                .Returns(true);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()))
                .Returns(false);
            repositorioMock.Setup(y => y.Agregar(It.IsAny<Proveedor>()))
                .Returns(new Proveedor { ProveedorId = 1 });

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(new Comercial { IdActiveDirectory = "a", ComercialId = 1, PerfilId = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Estado>() { new Estado { Descripcion = "a", EstadoId = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
               .Returns(new List<CorredorProveedor>());

            datosProveedorMock.Setup(y => y.ObtenerDatosDeProveedor(It.IsAny<List<Datos>>()))
                .Returns(new List<DatosProveedorAgentDto>() { new DatosProveedorAgentDto { USUARIO = "a", CLIENTE_MOA = "X", CUIT = "a", STATUS = "1" } });
            //

            var result = target.GrabarNuevoCorredor(corredor, "1");

            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<SISA, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()), Times.Exactly(4));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Exactly(4));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Proveedor>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(3));
            Assert.NotNull(result);
            Assert.IsFalse(result.HayErrores);
        }
        [Test]
        public void UpdateCorredorYUpdateProveedorOkTest()
        {
            ConfigurationManager.AppSettings["SinConexionSap"] = "0";
            var fecha = new DateTime(2019, 1, 19);
            var corredor = new NuevoCorredor
            {
                basicos = new Basico() { cuit = "1", RazonSocial = "1", segmentacion = 1, calificacion = 1 },
                contacto = new Contacto() { canalesOperacion = new List<int>(), entregaA = new List<int>(), condPreferentes = new List<int>() },
                contactocomercial = new List<ContactosComercial>(),
                proveedorCorredor = new List<NuevoProveedor>() {
                new NuevoProveedor{ produccion = new Produccion{
                    CamposProduccion = new List<CamposProduccion>()
                {
                    new CamposProduccion { CampoId= 1}
                },
                    habilitaoSojaSust ="null" },
                    basicos = new Basico() { cuit = "1", RazonSocial = "1", segmentacion = 1, calificacion = 1 },
                    contacto = new Contacto() { canalesOperacion =new List<int>(), entregaA=new List<int>(),condPreferentes= new List<int>() },
                    contactocomercial = new List<ContactosComercial>(),
                    ProveedorCorredorId = 1,
                    ProveedorId = 1} },
                CorredorId = 1
            };
            //UpdateDatosBasicosProveedor
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", RazonSocial = "a", SegmentacionId = 1, Calificacion = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Proveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
         .Returns(new List<Proveedor>() { new Proveedor { CUIT = "a" } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(new Proveedor { CUIT = "1", RazonSocial = "a", SegmentacionId = 1, Calificacion = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>()))
                .Returns("111111");
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, string>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<string>() { "a" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(new Comercial { IdActiveDirectory = "a", ComercialId = 1, PerfilId = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Estado>() { new Estado { Descripcion = "a", EstadoId = 1 } });
            repositorioMock.Setup(y => y.ListarConsulta(It.IsAny<ActualizarComercialHome>()))
                .Returns(new List<Datos>() { new Datos { CUIT = "1", UsuarioDirectory = "a" } });
            datosProveedorMock.Setup(y => y.ObtenerDatosDeProveedor(It.IsAny<List<Datos>>()))
                .Returns(new List<DatosProveedorAgentDto>() { new DatosProveedorAgentDto { USUARIO = "a", CLIENTE_MOA = "X", CUIT = "a", STATUS = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorEstado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorEstado>() { new ProveedorEstado { ComercialId = 1, EstadoId = 1, ProveedorId = 1, ProveedorEstadoId = 1 } });
            //UpdateDatosContacto - UpdateCanalOperacion
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCanalOperacion>());
            //UpdateDatosContacto - UpdateDestinatario
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorDestinatario>());
            //UpdateDatosContacto - UpdateCondicion
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCondicion>());
            //UpdateDatosContacto - UpdateObjetivos
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Objetivo, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Objetivo>());
            //UpdateContactoComerciales
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ContactoComercial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ContactoComercial>());
            //listar Corredor
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<SISA, bool>>>()))
                .Returns(true);
            repositorioMock.Setup(y => y.Agregar(It.IsAny<Proveedor>()))
                .Returns(new Proveedor { ProveedorId = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CorredorProveedor>() { new CorredorProveedor { CorredorId = 1, ProveedorId = 2 } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CorredorProveedor, bool>>>()))
                            .Returns(new CorredorProveedor { CorredorId = 1, ProveedorId = 2 });
            comercialManagerMock.Setup(x => x.ListarEquipo(It.IsAny<string>())).Returns(new EquipoDto { Equipo = new List<int> { 1, 2, 3 } });

            var comercial = new Comercial { ComercialId = 1, Apellido = "a", Nombres = "a", PerfilId = 7, IdActiveDirectory = "a" };
            var campana = new Campaña { CampañaId = 1, Descripcion = "a" };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(comercial);
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()))
                .Returns(new List<BasicoProveedor>() { new BasicoProveedor() { NoOperable = true, Operando = true, EstadoCuit = 2, Facacop = 0 } });
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()))
                .Returns(new List<ActividadTraer>() { new ActividadTraer() });
            repositorioMock.Setup(y => y.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ContactosComerciales>() { new ContactosComerciales() });
            repositorioMock.Setup(y => y.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ObjetivosTraer>() { new ObjetivosTraer() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioMaterialPorProveedor>() { new AcopioMaterialPorProveedor { AcopioId = 1,
                    AcopioMaterialId = 1,
                    CampañaId = 1,
                    Campaña = "a",
                    MaterialId = 1,
                    NroItem = 1,
                    Toneladas = 1,
                    Material = "a",
                    LocalidadId = 1,
                    Localidad = "a",
                    ProvinciaId = 1,
                    Provincia = "a" } });
            repositorioMock.Setup(y => y.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampoProduccionAcopio>() { new CampoProduccionAcopio() { EsCampoProduccion = true, ProveedorId = 1 }, new CampoProduccionAcopio() { EsCampoProduccion = false, ProveedorId = 1, } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()))
                .Returns(new DatosContacto { });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Material>() { new Material { CampañaId = 1, Campaña = campana, Descripcion = "a", MaterialId = 1, Codigo = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                 .Returns(new List<CampañaMaterial>() { new CampañaMaterial { CampañaId = 1, Campaña = campana } });
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCorredorDto>() { new ProveedorCorredorDto() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
            .Returns(new List<CampanaMaterialDetallePorMes>() { new CampanaMaterialDetallePorMes() {
                 CampanaId = 1,
                     MaterialId = 2,
                     ProveedorId = 1,
                     Material = new Material
                     {
                         Descripcion = "Soja",
                         MaterialId = 1,
                         CampañaId = 1
                     },
                     Campana = new Campaña
                     {
                         Descripcion = "11",
                         CampañaId = 1
                     },
                 ClaseDoc = "as",
                 Clasificacion = "PRODUCTOR",
                 ComercialId = 1,
                 CorredorCuit = "2321123",
                 PendienteAFijar = 1,
                 PendienteAplicar = 1,
                 ToneladaAmpliada = 0,
                 ToneladaAnulada = 1,
                 ToneladaAplicada = 3,
                 ToneladaContrato = 2,
                 ToneladaFijada = 2,
                 Fecha = DateTime.Now,
                 CampanaMaterialDetalleId = 1,
                 Contrato = "11233"
             } });

            var result = target.UpdateCorredor(corredor, "a", new List<int>() { 1, 2, 3 }, 1);

            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Exactly(3));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Exactly(3));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            //repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<ActualizarComercialHome>()), Times.Once);
            datosProveedorMock.Verify(x => x.ObtenerDatosDeProveedor(It.IsAny<List<Datos>>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorEstado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Objetivo, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ContactoComercial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<CorredorProveedor, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<CorredorProveedor>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<CorredorProveedor>()), Times.Never);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.ProveedorId);
            Assert.IsFalse(result.HayErrores);
        }
        [Test]
        public void TraerBoletoBolsaOkTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosCompraNetDto>>>()))
                .Returns(new DatosCompraNetDto() { ProveedorId = 1 });

            var result = target.TraerBoletoBolsa(1);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosCompraNetDto>>>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.ProveedorId);
        }
        [Test]
        public void ValidarProveedorEsCorredorOkTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(new Proveedor() { ProveedorId = 1, Segmentacion = new Segmentacion { Grupo = "Corredores" } });

            var result = target.ValidarProveedorEsCorredor(1);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Once);
            Assert.NotNull(result);
            Assert.IsTrue(result);
        }
        [Test]
        public void TraerCuitOkTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>()))
                .Returns("a");

            var result = target.TraerCuit(1);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual("a", result);
        }
        [Test]
        public void GrabarRolOkTest()
        {


            //para pasar el logDataA
            comercialManagerMock.Setup(x => x.ListarEquipo(It.IsAny<string>())).Returns(new EquipoDto { Equipo = new List<int> { 1, 2, 3 } });

            var comercial = new Comercial { ComercialId = 1, Apellido = "a", Nombres = "a", PerfilId = 7, IdActiveDirectory = "a" };
            var campana = new Campaña { CampañaId = 1, Descripcion = "a" };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(comercial);
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()))
                .Returns(new List<BasicoProveedor>() { new BasicoProveedor() { NoOperable = true, Operando = true, EstadoCuit = 2, Facacop = 0 } });
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()))
                .Returns(new List<ActividadTraer>() { new ActividadTraer() });
            repositorioMock.Setup(y => y.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ContactosComerciales>() { new ContactosComerciales() });
            repositorioMock.Setup(y => y.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ObjetivosTraer>() { new ObjetivosTraer() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioMaterialPorProveedor>() { new AcopioMaterialPorProveedor { AcopioId = 1,
                    AcopioMaterialId = 1,
                    CampañaId = 1,
                    Campaña = "a",
                    MaterialId = 1,
                    NroItem = 1,
                    Toneladas = 1,
                    Material = "a",
                    LocalidadId = 1,
                    Localidad = "a",
                    ProvinciaId = 1,
                    Provincia = "a" } });
            repositorioMock.Setup(y => y.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampoProduccionAcopio>() { new CampoProduccionAcopio() { EsCampoProduccion = true, ProveedorId = 1 }, new CampoProduccionAcopio() { EsCampoProduccion = false, ProveedorId = 1, } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()))
                .Returns(new DatosContacto { });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Material>() { new Material { CampañaId = 1, Campaña = campana, Descripcion = "a", MaterialId = 1, Codigo = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                 .Returns(new List<CampañaMaterial>() { new CampañaMaterial { CampañaId = 1, Campaña = campana } });
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCorredorDto>() { new ProveedorCorredorDto() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
            .Returns(new List<CampanaMaterialDetallePorMes>() { new CampanaMaterialDetallePorMes() {
                 CampanaId = 1,
                     MaterialId = 2,
                     ProveedorId = 1,
                     Material = new Material
                     {
                         Descripcion = "Soja",
                         MaterialId = 1,
                         CampañaId = 1
                     },
                     Campana = new Campaña
                     {
                         Descripcion = "11",
                         CampañaId = 1
                     },
                 ClaseDoc = "as",
                 Clasificacion = "PRODUCTOR",
                 ComercialId = 1,
                 CorredorCuit = "2321123",
                 PendienteAFijar = 1,
                 PendienteAplicar = 1,
                 ToneladaAmpliada = 0,
                 ToneladaAnulada = 1,
                 ToneladaAplicada = 3,
                 ToneladaContrato = 2,
                 ToneladaFijada = 2,
                 Fecha = DateTime.Now,
                 CampanaMaterialDetalleId = 1,
                 Contrato = "11233"
             } });
            //para pasar el logDataA



            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>()))
                .Returns("00000000");
            List<Rol> roles = new List<Rol> { new Rol() };
            List<ProveedorComercial> provsComercial = new List<ProveedorComercial> { new ProveedorComercial() };
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<Proveedor>() { new Proveedor { RolesAsociados = roles, ProveedorComercialAsociados = provsComercial } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Rol, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<Rol>() { new Rol() });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<Comercial>() { new Comercial() });
            var result = target.GrabarRol(1, new List<Rol>(), new List<Comercial>());

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Rol, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }
        [Test]
        public void TraerRolesProveedorOkTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, ICollection<Rol>>>>()))
                .Returns(new List<Rol>() { new Rol() });

            var result = target.TraerRolesProveedor(1);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, ICollection<Rol>>>>()), Times.Once);
            Assert.AreEqual(1, result.Count);
        }
        [Test]
        public void ValidarDirectoOkTest()
        {
            repositorioMock.Setup(x => x.Existe(It.IsAny<Expression<Func<CorredorProveedor, bool>>>()))
                .Returns(true);

            var result = target.ValidarDirecto("a");

            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<CorredorProveedor, bool>>>()), Times.Once);
            Assert.IsFalse(result);
        }

        [Test]
        public void GrabarRecordatorioConCitaOkTest()
        {
            var fecha = new DateTime(2019, 10, 10);
            var actividad = new ActividadInsetarIni { ProveedorId = 1, ComercialId = 1, fechaYHoraActividad = fecha, fechaYHoraRecordatorio = fecha, tipoactividad = 1, ActividadId = 1, fechaYHoraRecordatorioFin = fecha };
            ConfigurationManager.AppSettings["EmailAgenda"] = "1";
            ConfigurationManager.AppSettings["maildeUsuarios"] = "dataagro.baufest@gmail.com";
            ConfigurationManager.AppSettings["AgendaCita"] = "1";
            ConfigurationManager.AppSettings["AgendaTareas"] = "1";

            mailManagerMock.Setup(x => x.GetEmailUserActiveDirectory(It.IsAny<string>()))
                .Returns("dataagro.baufest@gmail.com");
            //para pasar el logDataA
            comercialManagerMock.Setup(x => x.ListarEquipo(It.IsAny<string>())).Returns(new EquipoDto { Equipo = new List<int> { 1, 2, 3 } });

            var comercial = new Comercial { ComercialId = 1, Apellido = "a", Nombres = "a", PerfilId = 7, IdActiveDirectory = "a" };
            var campana = new Campaña { CampañaId = 1, Descripcion = "a" };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(comercial);
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()))
                .Returns(new List<BasicoProveedor>() { new BasicoProveedor() { NoOperable = true, Operando = true, EstadoCuit = 2, Facacop = 0 } });
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()))
                .Returns(new List<ActividadTraer>() { new ActividadTraer() });
            repositorioMock.Setup(y => y.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ContactosComerciales>() { new ContactosComerciales() });
            repositorioMock.Setup(y => y.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ObjetivosTraer>() { new ObjetivosTraer() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<AcopioMaterialPorProveedor>() { new AcopioMaterialPorProveedor { AcopioId = 1,
                    AcopioMaterialId = 1,
                    CampañaId = 1,
                    Campaña = "a",
                    MaterialId = 1,
                    NroItem = 1,
                    Toneladas = 1,
                    Material = "a",
                    LocalidadId = 1,
                    Localidad = "a",
                    ProvinciaId = 1,
                    Provincia = "a" } });
            repositorioMock.Setup(y => y.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampoProduccionAcopio>() { new CampoProduccionAcopio() { EsCampoProduccion = true, ProveedorId = 1 }, new CampoProduccionAcopio() { EsCampoProduccion = false, ProveedorId = 1, } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()))
                .Returns(new DatosContacto { });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Material>() { new Material { CampañaId = 1, Campaña = campana, Descripcion = "a", MaterialId = 1, Codigo = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                 .Returns(new List<CampañaMaterial>() { new CampañaMaterial { CampañaId = 1, Campaña = campana } });
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCorredorDto>() { new ProveedorCorredorDto() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
            .Returns(new List<CampanaMaterialDetallePorMes>() { new CampanaMaterialDetallePorMes() {
                 CampanaId = 1,
                     MaterialId = 2,
                     ProveedorId = 1,
                     Material = new Material
                     {
                         Descripcion = "Soja",
                         MaterialId = 1,
                         CampañaId = 1
                     },
                     Campana = new Campaña
                     {
                         Descripcion = "11",
                         CampañaId = 1
                     },
                 ClaseDoc = "as",
                 Clasificacion = "PRODUCTOR",
                 ComercialId = 1,
                 CorredorCuit = "2321123",
                 PendienteAFijar = 1,
                 PendienteAplicar = 1,
                 ToneladaAmpliada = 0,
                 ToneladaAnulada = 1,
                 ToneladaAplicada = 3,
                 ToneladaContrato = 2,
                 ToneladaFijada = 2,
                 Fecha = DateTime.Now,
                 CampanaMaterialDetalleId = 1,
                 Contrato = "11233"
             } });
            //para pasar el logDataA


            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Actividad, bool>>>()))
                .Returns(new Actividad());
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Material>() { new Material { CampañaId = 1, Campaña = campana, Descripcion = "a", MaterialId = 1, Codigo = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                 .Returns(new List<CampañaMaterial>() { new CampañaMaterial { CampañaId = 1, Campaña = campana } });
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });
            repositorioMock.Setup(y => y.Obtener<ContactoComercial>(It.IsAny<Expression<Func<ContactoComercial, bool>>>()))
                .Returns(new ContactoComercial { Nombres = "1", Telefono1 = "", Email1 = "" });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(new Proveedor { CUIT = "1", RazonSocial = "1" });
            var result = target.GrabarRecordatorio(actividad);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Actividad, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Exactly(3));
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(result);
            Assert.IsFalse(result.HayErrores);
        }

        [Test]
        public void EnviarEmailEliminarOkTest()
        {
            var PrecioPactado = new List<PrecioPactado> { new PrecioPactado { MonedaPactado = new Moneda { Descripcion = "" }, MonedaImportePactado = new Moneda { Descripcion = "" }, Precio = 1, ImportePactado = 1, Porcentaje = 1, FechaDesde = DateTime.Now, FechaHasta = DateTime.Now } };
            var oContrato = new Contrato()
            {
                Comercial = new Comercial { IdActiveDirectory = "q", ComercialId = 1 },
                ComercialCreador = new Comercial { IdActiveDirectory = "q", ComercialId = 1 },
                ProveedorId = 1,
                TrigoEspecial = true,
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
                Moneda = new Moneda { Descripcion = "" },
                Localidad = new Localidad { Nombre = "", Provincia = new Provincia { Nombre = "" } },
                Provincia = new Provincia { Nombre = "" },
                Campana = new Campaña { Descripcion = "" },
                ContratoMadre = "1",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Sustentable = false,
                PorcentajeDePago = 95,
                Calidad = new List<Calidad>(),
                Material = new Material { Descripcion = "" },
                ContratoSAP = "0001111",
                Destino = new Centro { Descripcion = "" },
                Proveedor = new Proveedor { CUIT = "", RazonSocial = "" },
                Corredor = new Proveedor { CUIT = "", RazonSocial = "" },
                Clasificacion = new ClasificacionCompraNet { Descripcion = "" },
                Consignatario = true,
                CantidadCamiones = 1,
                Boleto = new BoletoCompraNet { Descripcion = "" },
                Dolarizado = true,
                ImporteSustentable = 1,
                MonedaSustentable = new Moneda { Descripcion = "" },
                FechaDolarizado = DateTime.Now,
                DiasPesificado = 1,
                CD = true,
                Warrant = true,
                PagoDirectoVendedor = true,
                MercsDeposito = true,
                PrecioPactado = PrecioPactado,
                PlanCanje = true,
                ContratoVendedor = "",
                ContratoCorredor = "",
                SelCargoMOA = true,
                SelCargoVendedor = true,
                Compensacion = true,
                NivelTarifa = new NivelTarifa { Descripcion = "" },
                TarifaFlete = 2,
                Observacion = "",
                FechaCierta = DateTime.Now,
                DolarizadoExpress = true,
                PrecioNeto = 1,

            };
            List<DescuentoBonificacion> objDescuento = new List<DescuentoBonificacion> { new DescuentoBonificacion { FechaDesde = DateTime.Now, FechaHasta = DateTime.Now, ContratoId = 1, Id = 1, Importe = 1, MonedaId = "ARS", Porcentaje = 1, TipoDBId = 1, TipoPeriodoDBId = 1, Moneda = new Moneda { Descripcion = "" }, TipoDB = new TipoDB { Descripcion = "" } } };
            List<Calidad> objCalidad = new List<Calidad> { new Calidad { CalidadEspecialId = 1, Id = 1, NegocioId = 1, PorcentajeDesde = 1, PorcentajeHasta = 1, StandardDeCalidadId = 1, Valor = 1, CalidadEspecial = new CalidadEspecial { Descripcion = "" } } };
            string idActiveDirectory = "emartin1";
            bool? eliminar = true;
            ConfigurationManager.AppSettings["AmbientePruebas"] = "1";
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ContactoComercial, bool>>>()))
                .Returns(new ContactoComercial { Email1 = "dataagro.baufest@gmail.com" });
            repositorioMock.Setup(y => y.Listar<ContactoComercial>(It.IsAny<Expression<Func<ContactoComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ContactoComercial> { new ContactoComercial { Email1 = "dataagro.baufest@gmail.com" } });
            mailManagerMock.Setup(x => x.GetEmailUserActiveDirectory(It.IsAny<string>()))
               .Returns("dataagro.baufest@gmail.com");
            comercialManagerMock.Setup(x => x.ListarComercialesCorredor()).Returns(new List<Comercial>());

            var result = target.EnviarEmail(oContrato, objDescuento, objCalidad, idActiveDirectory, eliminar);
            Assert.AreEqual(0, result.ListaErrores.Count);


        }

        [Test]
        public void EnviarEmailOkTest()
        {
            var PrecioPactado = new List<PrecioPactado> { new PrecioPactado { MonedaPactado = new Moneda { Descripcion = "" }, MonedaImportePactado = new Moneda { Descripcion = "" }, Precio = 1, ImportePactado = 1, Porcentaje = 1, FechaDesde = DateTime.Now, FechaHasta = DateTime.Now } };
            var oContrato = new Contrato()
            {
                Comercial = new Comercial { IdActiveDirectory = "q", ComercialId = 1 },
                ComercialCreador = new Comercial { IdActiveDirectory = "q", ComercialId = 1 },
                ProveedorId = 1,
                HastaFijacion = DateTime.Now,
                CondicionFijacion = new CondicionFijacion { Descripcion = "" },
                TrigoEspecial = true,
                ClasificacionId = 1,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 1,
                DestinoId = 1,
                LocalidadId = 1,
                ProvinciaId = 1,
                FechaEntrega = DateTime.Now,
                FechaOperacion = DateTime.Now.Date,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                Moneda = new Moneda { Descripcion = "" },
                Localidad = new Localidad { Nombre = "", Provincia = new Provincia { Nombre = "" } },
                Provincia = new Provincia { Nombre = "" },
                Campana = new Campaña { Descripcion = "" },
                ContratoMadre = "1",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 1,
                Sustentable = false,
                PorcentajeDePago = 95,
                Calidad = new List<Calidad>(),
                Material = new Material { Descripcion = "" },
                ContratoSAP = "0001111",
                Destino = new Centro { Descripcion = "" },
                Proveedor = new Proveedor { CUIT = "", RazonSocial = "" },
                Corredor = new Proveedor { CUIT = "", RazonSocial = "" },
                Clasificacion = new ClasificacionCompraNet { Descripcion = "" },
                Consignatario = true,
                CantidadCamiones = 1,
                Boleto = new BoletoCompraNet { Descripcion = "" },
                Dolarizado = true,
                ImporteSustentable = 1,
                MonedaSustentable = new Moneda { Descripcion = "" },
                FechaDolarizado = DateTime.Now,
                DiasPesificado = 1,
                CD = true,
                Warrant = true,
                PagoDirectoVendedor = true,
                MercsDeposito = true,
                PrecioPactado = PrecioPactado,
                PlanCanje = true,
                ContratoVendedor = "",
                ContratoCorredor = "",
                SelCargoMOA = true,
                SelCargoVendedor = true,
                Compensacion = true,
                NivelTarifa = new NivelTarifa { Descripcion = "" },
                TarifaFlete = 2,
                Observacion = "",
                FechaCierta = DateTime.Now,
                DolarizadoExpress = true,
                PrecioNeto = 1,

            };
            List<DescuentoBonificacion> objDescuento = new List<DescuentoBonificacion> { new DescuentoBonificacion { FechaDesde = DateTime.Now, FechaHasta = DateTime.Now, ContratoId = 1, Id = 1, Importe = 1, MonedaId = "ARS", Porcentaje = 1, TipoDBId = 1, TipoPeriodoDBId = 1, Moneda = new Moneda { Descripcion = "" }, TipoDB = new TipoDB { Descripcion = "" } } };
            List<Calidad> objCalidad = new List<Calidad> { new Calidad { CalidadEspecialId = 1, Id = 1, NegocioId = 1, PorcentajeDesde = 1, PorcentajeHasta = 1, StandardDeCalidadId = 1, Valor = 1, CalidadEspecial = new CalidadEspecial { Descripcion = "" } } };
            string idActiveDirectory = "emartin1";
            bool? eliminar = false;
            ConfigurationManager.AppSettings["AmbientePruebas"] = "1";
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ContactoComercial, bool>>>()))
                .Returns(new ContactoComercial { Email1 = "dataagro.baufest@gmail.com" });
            repositorioMock.Setup(y => y.Listar<ContactoComercial>(It.IsAny<Expression<Func<ContactoComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ContactoComercial> { new ContactoComercial { Email1 = "dataagro.baufest@gmail.com" } });
            mailManagerMock.Setup(x => x.GetEmailUserActiveDirectory(It.IsAny<string>()))
               .Returns("dataagro.baufest@gmail.com");
            comercialManagerMock.Setup(x => x.ListarComercialesSinRecibirMail()).Returns(new List<Comercial> { new Comercial { IdActiveDirectory = "asd" } });

            comercialManagerMock.Setup(x => x.ListarComercialesCorredor()).Returns(new List<Comercial> { new Comercial { IdActiveDirectory = "asd" } });
            Thread.CurrentPrincipal = new TestPrincipal(new Claim[] {
            new Claim(ClaimTypes.Role, PermisosDataAgro.VerCorredorComercial.ToString())
            });

            var result = target.EnviarEmail(oContrato, objDescuento, objCalidad, idActiveDirectory, eliminar);
            Assert.AreEqual(0, result.ListaErrores.Count);


        }

        [Test]
        public void EnviarEmailFijacionOkTest()
        {
            var PrecioPactado = new List<PrecioPactado> { new PrecioPactado { MonedaPactado = new Moneda { Descripcion = "" }, MonedaImportePactado = new Moneda { Descripcion = "" }, Precio = 1, ImportePactado = 1, Porcentaje = 1 } };
            var oContrato = new Contrato()
            {
                Comercial = new Comercial { IdActiveDirectory = "q", ComercialId = 1 },
                ComercialCreador = new Comercial { IdActiveDirectory = "q", ComercialId = 1 },
                ProveedorId = 1,

                TrigoEspecial = true,
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
                Moneda = new Moneda { Descripcion = "" },
                Localidad = new Localidad { Nombre = "", Provincia = new Provincia { Nombre = "" } },
                Provincia = new Provincia { Nombre = "" },
                Campana = new Campaña { Descripcion = "" },
                ContratoMadre = "1",
                CampanaId = 1,
                ComercialId = 70,
                EstablecimientoPropio = true,
                BoletoId = 3,
                StandardDeCalidadId = 2,
                StandardDeCalidad = new StandardDeCalidad { Descripcion = "Grado 2" },
                Sustentable = true,
                PorcentajeDePago = 95,
                Calidad = new List<Calidad>(),
                Material = new Material { Descripcion = "" },
                ContratoSAP = "0001111",
                Destino = new Centro { Descripcion = "" },
                Proveedor = new Proveedor { CUIT = "", RazonSocial = "" },
                Corredor = new Proveedor { CUIT = "", RazonSocial = "" },
                Clasificacion = new ClasificacionCompraNet { Descripcion = "" },
                Consignatario = true,
                CantidadCamiones = 1,
                Boleto = new BoletoCompraNet { Descripcion = "" },
                Dolarizado = true,
                ImporteSustentable = 1,
                MonedaSustentable = new Moneda { Descripcion = "" },
                FechaDolarizado = DateTime.Now,
                DiasPesificado = 1,
                CD = true,
                Warrant = true,
                PagoDirectoVendedor = true,
                MercsDeposito = true,
                PrecioPactado = PrecioPactado,
                PlanCanje = true,
                ContratoVendedor = "",
                ContratoCorredor = "",
                SelCargoMOA = true,
                SelCargoVendedor = true,
                Compensacion = true,
                NivelTarifa = new NivelTarifa { Descripcion = "" },
                TarifaFlete = 2,
                Observacion = "",
                FechaCierta = DateTime.Now,
                DolarizadoExpress = true,
                PrecioNeto = 1,

            };

            var fijacion = new FijacionDePrecioContrato()
            {
                PagoDiferido = true,
                FijacionSAP = "00011111",
                Comercial = new Comercial { IdActiveDirectory = "q", ComercialId = 1 },
                ComercialCreador = new Comercial { IdActiveDirectory = "q", ComercialId = 1 },
                ProveedorId = 1,
                TrigoEspecial = true,
                CorredorId = null,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1000,
                TipoNegocioId = 1,
                DestinoId = 1,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                MonedaId = "ARS ",
                Moneda = new Moneda { Descripcion = "" },
                Campana = new Campaña { Descripcion = "" },
                CampanaId = 1,
                ComercialId = 70,
                Material = new Material { Descripcion = "" },
                ContratoSAP = "0001111",
                Destino = new Centro { Descripcion = "" },
                Proveedor = new Proveedor { CUIT = "", RazonSocial = "" },
                Corredor = new Proveedor { CUIT = "", RazonSocial = "" },
                Dolarizado = true,
                FechaDolarizado = DateTime.Now,
                DiasPesificado = 1,
                CD = true,
                Warrant = true,
                Observacion = "",
                FechaCierta = DateTime.Now,
                DolarizadoExpress = true,
                PrecioNeto = 1,

            };
            string idActiveDirectory = "emartin1";
            ConfigurationManager.AppSettings["AmbientePruebas"] = "1";
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ContactoComercial, bool>>>()))
                .Returns(new ContactoComercial { Email1 = "dataagro.baufest@gmail.com" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>()))
                .Returns(oContrato);
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Calidad, bool>>>()))
               .Returns(true);
            comercialManagerMock.Setup(x => x.ListarComercialesSinRecibirMail()).Returns(new List<Comercial> { new Comercial { IdActiveDirectory = "asd" } });

            repositorioMock.Setup(y => y.Listar<ContactoComercial>(It.IsAny<Expression<Func<ContactoComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ContactoComercial> { new ContactoComercial { Email1 = "dataagro.baufest@gmail.com" } });
            mailManagerMock.Setup(x => x.GetEmailUserActiveDirectory(It.IsAny<string>()))
               .Returns("dataagro.baufest@gmail.com");
            comercialManagerMock.Setup(x => x.ListarComercialesCorredor()).Returns(new List<Comercial> { new Comercial { IdActiveDirectory = "asd" } });
            Thread.CurrentPrincipal = new TestPrincipal(new Claim[] {
            new Claim(ClaimTypes.Role, PermisosDataAgro.VerCorredorComercial.ToString())
            });

            var result = target.EnviarEmailFijacion(fijacion, idActiveDirectory);
            Assert.AreEqual(0, result.ListaErrores.Count);


        }

        [Test]
        public void DevolverProveedoresCorredoresTest()
        {
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<DevolverProveedoresCorredores>()))
                       .Returns(new List<BusquedaHome>());

            var result = target.DevolverProveedoresCorredores("filtro");

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void BuscarDatosProveedorTest()
        {
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<BusquedaDatosProveedores>()))
                       .Returns(new Kendo.DynamicLinq.DataSourceResult());

            var result = target.BuscarDatosProveedor(new Kendo.DynamicLinq.DataSourceRequest(), new List<int>());

            Assert.IsNotNull(result);
        }

        [Test]
        public void BuscarDatosContactoTest()
        {
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<BusquedaContactosComercial>()))
                       .Returns(new Kendo.DynamicLinq.DataSourceResult());

            var result = target.BuscarDatosContacto(new Kendo.DynamicLinq.DataSourceRequest(), new List<int>());

            Assert.IsNotNull(result);
        }

        [Test]
        public void BuscarDatosProduccionTest()
        {
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<BusquedaDatosProduccion>()))
                       .Returns(new Kendo.DynamicLinq.DataSourceResult());

            var result = target.BuscarDatosProduccion(new Kendo.DynamicLinq.DataSourceRequest(), new List<int>());

            Assert.IsNotNull(result);
        }

        [Test]
        public void BuscarDatosAlmacenamientoTest()
        {
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<BusquedaDatosAlmacenamiento>()))
                       .Returns(new Kendo.DynamicLinq.DataSourceResult());

            var result = target.BuscarDatosAlmacenamiento(new Kendo.DynamicLinq.DataSourceRequest(), new List<int>());

            Assert.IsNotNull(result);
        }

        [Test]
        public void ImportarEstablecimientosTest()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<CampoDetalle, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<CampoDetalle> { new CampoDetalle { ImportId = 1 }, new CampoDetalle { ImportId = 2 } });

            target.ImportarEstablecimientos(new List<CampoDetalleDto> { new CampoDetalleDto { ImportId = 1 }, new CampoDetalleDto { ImportId = 4 } }, new Resultado());

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CampoDetalle, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Exactly(1));
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void ObtenerIdProveedorPorCuitTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, int>>>()))
                       .Returns(1);

            var result = target.ObtenerIdProveedorPorCuit("");

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result);
        }

        [Test]
        public void ObtenerEmailProveedorPorCuitTest()
        {
            repositorioMock.Setup(x => x.Obtener<Proveedor>(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                       .Returns(new Proveedor());

            var result = target.ObtenerEmailProveedorPorCuit("");

            Assert.IsNotNull(result);
        }

        [Test]
        public void BuscarDatosTablaComprasTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, CampanaMaterialDetallePorMesDto>>>(), It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
               .Returns(new List<CampanaMaterialDetallePorMesDto>() { new CampanaMaterialDetallePorMesDto() });

            var result = target.BuscarDatosTablaCompras();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ActualizarRazonSocialTest()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<SISA, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
               .Returns(new List<SISA> { new SISA { CUIT = "1", RazonSocial = "a", FechaVigenciaEstado = new DateTime(2020, 1, 1) }, new SISA { CUIT = "1", RazonSocial = "b", FechaVigenciaEstado = new DateTime(2000, 1, 1) } });

            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
              .Returns(new List<Proveedor> { new Proveedor { CUIT = "1", RazonSocial = "aaaa" }, new Proveedor { CUIT = "1", RazonSocial = "bbbb" }, new Proveedor { CUIT = "2", RazonSocial = "b" } });

            target.ActualizarRazonSocial();

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<SISA, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
        }

        [Test]
        public void TraerProveedoresPorCuitTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Proveedor, ProveedorDto>>>(), It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
              .Returns(new List<ProveedorDto>() { new ProveedorDto { ProveedorId = 1 } });

            var result = target.TraerProveedoresPorCuit("");

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Proveedor, ProveedorDto>>>(), It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
        }

        [Test]
        public void ListarProveedorTodosFiltroOkTest()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Proveedor, ProveedorDto>>>(), It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorDto> { new ProveedorDto {Alias="aa",CUIT="1",RazonSocial="1" }  });

            var result = target.ListarProveedorTodos("aa");

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Proveedor, ProveedorDto>>>(), It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
        }

    }
}
