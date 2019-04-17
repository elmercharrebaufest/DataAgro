using Autofac.Extras.NLog;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
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
using System.Linq.Expressions;
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
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            comercialManagerMock = new Mock<IComercialManager>();
            riesgoComercialAgentMock = new Mock<IRiesgoComercialAgent>();
            datosProveedorMock = new Mock<IDatosProveedorAgent>();
            target = new ProveedorManager(logger.Object, repositorioMock.Object,comercialManagerMock.Object,riesgoComercialAgentMock.Object,datosProveedorMock.Object);
        }

        [Test]
        public void TraerHistorialActividadOkTest()
        {
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()))
                .Returns(new List<ActividadTraer>() { new ActividadTraer() });
            var result = target.TraerHistorialActividad(new HistorialActiviad(),1,"a");

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
                .Returns(new List<BasicoProveedor>() { new BasicoProveedor() {NoOperable=true,Operando=true,EstadoCuit=2,Facacop=0 } });
            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()))
                .Returns(new List<ActividadTraer>() { new ActividadTraer() });
            repositorioMock.Setup(y => y.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ContactosComerciales>() { new ContactosComerciales() });
            repositorioMock.Setup(y => y.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ObjetivosTraer>() { new ObjetivosTraer()});
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
                .Returns(new List<CampoProduccionAcopio>() { new CampoProduccionAcopio() { EsCampoProduccion=true,ProveedorId=1}, new CampoProduccionAcopio() { EsCampoProduccion = false, ProveedorId = 1, } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()))
                .Returns(new DatosContacto {  });
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor {CUIT="1", });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Material>() { new Material { CampañaId = 1, Campaña = campana, Descripcion = "a", MaterialId = 1, Codigo = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                 .Returns(new List<CampañaMaterial>() { new CampañaMaterial { CampañaId = 1, Campaña = campana } });
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });

            var result = target.TraerProveedor(1, "a", new List<int>() { 1, 2, 3 });

            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()), Times.Once);
            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
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
            Assert.IsNull (result.DatosContacto.CodigoPostal);
            Assert.IsNull (result.DatosContacto.Direccion);
            Assert.IsNull (result.DatosContacto.Intermediario);
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
                .Returns(new List<BasicoProveedor>() { new BasicoProveedor() { NoOperable = true, Operando = true, EstadoCuit = 2, Facacop = 0,RiesgoComercialSap="A" } });
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
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });

            var result = target.TraerProveedor(1, "a", new List<int>() { 1, 2, 3 });

            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()), Times.Once);
            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.BasicoProveedorTraerPorProveedores.Count);
            Assert.AreEqual("A", result.BasicoProveedorTraerPorProveedores[0].RiesgoComercialSap);
            Assert.AreEqual("Riesgo Comercial Alto", result.BasicoProveedorTraerPorProveedores[0].TooltipNoOperable);
            Assert.IsFalse(result.BasicoProveedorTraerPorProveedores[0].Operando);
            Assert.IsTrue( result.BasicoProveedorTraerPorProveedores[0].NoOperable);
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
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });

            var result = target.TraerProveedor(1, "a", new List<int>() { 1, 2, 3 });

            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()), Times.Once);
            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
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
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });

            var result = target.TraerProveedor(1, "a", new List<int>() { 1, 2, 3 });

            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()), Times.Once);
            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
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
            repositorioMock.Setup(y => y.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes() { Año = 1000, Mes = 12, Toneladas = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCanalOperacion, CanalOperacion>>>(), It.IsAny<Expression<Func<ProveedorCanalOperacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CanalOperacion>() { new CanalOperacion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorCondicion, Condicion>>>(), It.IsAny<Expression<Func<ProveedorCondicion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Condicion>() { new Condicion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ProveedorDestinatario, Destinatario>>>(), It.IsAny<Expression<Func<ProveedorDestinatario, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Destinatario>() { new Destinatario() });

            var result = target.TraerProveedor(1, "a", new List<int>() { 1, 2, 3 });

            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<TraerDatosBasicosProveedor>()), Times.Once);
            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<ConsultaActividadHistoriaTraerPorProveedorId>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.SelStore<ContactosComerciales>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.SelStore<ObjetivosTraer>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<AcopioMaterial, AcopioMaterialPorProveedor>>>(), It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.SelStore<CampoProduccionAcopio>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, DatosContacto>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CampañaMaterial, Material>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.SelStore<CampañaMaterialPorMes>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
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
            var proveedor = new Proveedor { ProveedorId = 1, CUIT = "1", RazonSocial = "a", LocalidadCompraNetId = 1, ProvinciaCompraNetId = 1,ProvinciaCompraNet= prov,LocalidadCompraNet= loc };
            var datos = new DatosLocalidadProvincia { ProveedorId = 1, CUIT = "a", RazonSocial = "a", Localidad = "a", Provincia = "b",LocalidadId=1, ProvinciaId = 2 };
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
            var proveedor = new Proveedor { ProveedorId = 1, CUIT = "1", RazonSocial = "a", ClasificacionCompraNetId =1, Consignatario = true,BoletoCompraNetId=1,BolsaCompraNetId=1, SegmentacionId=5};
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
            var actividad = new ActividadInsetarIni{ ProveedorId = 1, ComercialId= 1,fechaYHoraActividad= fecha,fechaYHoraRecordatorio= fecha,tipoactividad=1,ActividadId=1 };
            
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Actividad, bool>>>()))
                .Returns(new Actividad());
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                            .Returns(new Proveedor());

            var result = target.GrabarRecordatorio(actividad);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Actividad, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Once);
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

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Segmentacion, SegmentacionQry>>>(), It.IsAny<Expression<Func<Segmentacion, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
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
            Assert.AreEqual(1,result.tiptel.Count);
            Assert.AreEqual(1,result.prov.Count);
            Assert.AreEqual(0,result.loc.Count);
            Assert.AreEqual(1,result.cope.Count);
            Assert.AreEqual(1,result.gran.Count);
            Assert.AreEqual(1,result.dest.Count);
            Assert.AreEqual(1,result.cond.Count);
            Assert.AreEqual(1,result.inte.Count);
            Assert.AreEqual(1,result.tipoact.Count);
            Assert.AreEqual(1,result.concom.Count);
            Assert.AreEqual(1,result.ClasComNet.Count);
            Assert.AreEqual(1,result.BoleComNet.Count);
            Assert.AreEqual(1,result.BolsComNet.Count);

        }
        [Test]
        public void TraerLocalidadOkTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Localidad, LocalidadDto>>>(), It.IsAny<Expression<Func<Localidad, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<LocalidadDto>() { new LocalidadDto { CodLocalidad="a",LocalidadId=1,Nombre="a", ProvinciaId=1,Provincia_Nombre="b" } });

            var result = target.TraerLocalidad(1);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Localidad, LocalidadDto>>>(), It.IsAny<Expression<Func<Localidad, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
        }
        [Test]
        public void TraerProveedorPorCuitOkTest()
        {
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, ProveedorQry>>>()))
                .Returns(new ProveedorQry { ProveedorId = 1, Descripcion= "b" } );

            var result = target.TraerProveedorPorCuit("a",false);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, ProveedorQry>>>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.ProveedorId);
        }
        [Test]
        public void TraerProveedorPorCuitCorredorOkTest()
        {
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, ProveedorQry>>>()))
                .Returns(new ProveedorQry { ProveedorId = 1, Descripcion = "b" });

            var result = target.TraerProveedorPorCuit("a", true);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, ProveedorQry>>>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.ProveedorId);
        }
        [Test]
        public void TraerRazonSocialOkTest()
        {
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
        public void GrabarNuevoProveedorOkTest()
        {
            ConfigurationManager.AppSettings["SinConexionSap"] = "0";
            var fecha = new DateTime(2019, 1, 19);
            var proveedor = new NuevoProveedor
            {
                produccion = new Produccion(),
                almacenamiento = new Almacenamiento(),
                basicos = new Basico() { cuit = "1",RazonSocial="1",segmentacion=1,calificacion=1},
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
                .Returns(false);

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(new Comercial { IdActiveDirectory = "a", ComercialId = 1, PerfilId = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Estado>() { new Estado { Descripcion = "a", EstadoId = 1 } });

            datosProveedorMock.Setup(y => y.ObtenerDatosDeProveedor(It.IsAny<List<Datos>>()))
                .Returns(new List<DatosProveedorAgentDto>() { new DatosProveedorAgentDto{USUARIO="a",CLIENTE_MOA="X",CUIT="a",STATUS="1" } });


            var result = target.GrabarNuevoProveedor(proveedor, "1");

            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<SISA, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()), Times.Once);
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
                produccion = new Produccion{ CamposProduccion = new List<CamposProduccion>()
                {

                    new CamposProduccion { CampoId= 1}
                }, habilitaoSojaSust="null" },                
                basicos = new Basico() { cuit = "1", RazonSocial = "1", segmentacion = 1, calificacion = 1 },
                contacto = new Contacto() { canalesOperacion =new List<int>(), entregaA=new List<int>(),condPreferentes= new List<int>(), },
                contactocomercial = new List<ContactosComercial>(),
                ProveedorCorredorId = 0,
                ProveedorId = 1
            };
            //UpdateDatosBasicosProveedor
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { CUIT = "1", RazonSocial = "a", SegmentacionId = 1, Calificacion = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(new Proveedor { CUIT = "1", RazonSocial = "a", SegmentacionId = 1, Calificacion = 1 });
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
            //UpdateDatosContacto
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ContactoComercial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ContactoComercial>());
            //UpdateProduccion
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Campo, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Campo>());
            //UpdateAlmacenamiento
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Acopio, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<Acopio>());

            var result = target.UpdateProveedor(proveedor, "a", new List<int>() { 1, 2, 3 }, 1);

            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<ActualizarComercialHome>()), Times.Once);
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
            var result = target.ObtenerReporteProveedor("aa","a");

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

            var result = target.DevolverProveedores("aa", false, new List<int>() { 1, 2, 3 });

            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<DevolverProveedores>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(0, result.Count);
        }
        [Test]
        public void ListarProveedorOkTest()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Proveedor, ProveedorDto>>>(),It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
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
        public void ListarProveedorCorredorOkTest()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorCorredorDto>());

            var result = target.ListarProveedorCorredor(1);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CorredorProveedor, ProveedorCorredorDto>>>(), It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(0, result.Count);
        }
        [Test]
        public void TraerProveedorParaCorredorOkTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, ProveedorDto>>>()))
                .Returns(new ProveedorDto(){ProveedorId=1});

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
                    contactocomercial = new List<ContactosComercial>(),
                    ProveedorCorredorId = 0,
                    ProveedorId = 0
                } },
                CorredorId = 0
            };

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


            var result = target.GrabarNuevoCorredor(corredor, "1");

            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Exactly(4));
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<SISA, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<FACACOP, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Proveedor>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
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
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(new Proveedor { CUIT = "1", RazonSocial = "a", SegmentacionId = 1, Calificacion = 1 });
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
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<CorredorProveedor>() { new CorredorProveedor { CorredorId = 1, ProveedorId = 2 } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CorredorProveedor, bool>>>()))
                            .Returns( new CorredorProveedor { CorredorId = 1, ProveedorId = 2 } );

            var result = target.UpdateCorredor(corredor, "a", new List<int>() { 1, 2, 3 }, 1);

            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Estado, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<ActualizarComercialHome>()), Times.Once);
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
                .Returns(new Proveedor() { ProveedorId = 1,Segmentacion=new Segmentacion { Grupo= "Corredores" } });

            var result = target.ValidarProveedorEsCorredor(1);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Once);
            Assert.NotNull(result);
            Assert.IsTrue(result);
        }
    }
}
