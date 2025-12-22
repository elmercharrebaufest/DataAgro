using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
    public class ProveedorControllerTest
    {
        private ProveedorController target;
        private Mock<IProveedorManager> proveedorManagerMock;
        private Mock<IHomeManager> homeManagerMock;
        private Mock<ILocalidadManager> localidadManagerMock;
        private Mock<IProvinciaManager> provinciaManagerMock;
        private Mock<ICampañaManager> campanaManagerMock;
        private Mock<IComercialManager> comercialManagerMock;
        private Mock<IReportesManager> reportesManagerMock;
        private Mock<IInformeComercialManager> informeComercialManagerMock;
        private Mock<IRepositorio> repositorioMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            proveedorManagerMock = new Mock<IProveedorManager>();
            homeManagerMock = new Mock<IHomeManager>();
            localidadManagerMock = new Mock<ILocalidadManager>();
            provinciaManagerMock = new Mock<IProvinciaManager>();
            campanaManagerMock = new Mock<ICampañaManager>();
            comercialManagerMock = new Mock<IComercialManager>();
            reportesManagerMock = new Mock<IReportesManager>();
            informeComercialManagerMock = new Mock<IInformeComercialManager>();
            repositorioMock = new Mock<IRepositorio>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            HttpContext.Current.Session["perfil"] = 1;
            target = new ProveedorController(proveedorManagerMock.Object,
                homeManagerMock.Object, campanaManagerMock.Object,
                comercialManagerMock.Object, reportesManagerMock.Object,
                localidadManagerMock.Object, provinciaManagerMock.Object, informeComercialManagerMock.Object, repositorioMock.Object);
        }

        [Test]
        public void IndexOk()
        {
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void ReporteProveedorOkTest()
        {

            var result = target.ReporteProveedor("") as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
        [Test]
        public void AgregarOkTest()
        {

            var result = target.Agregar(1) as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
        [Test]
        public void DetailsOkTest()
        {

            var result = target.Details(1) as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
        [Test]
        public void CreateOkTest()
        {

            var result = target.Create() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
        [Test]
        public void DeleteOkTest()
        {

            var result = target.Delete(1) as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void DetalleTest()
        {

            comercialManagerMock.Setup(x => x.ComercialExiste(GlobalVariables.IdActiveDirectory)).Returns(true);
            comercialManagerMock.Setup(x => x.ComercialPerteneceProveedor(GlobalVariables.Equipo, 1, GlobalVariables.CorredoresComercial)).Returns(true);
            var result = target.Detalle(1, null);
            Assert.NotNull(result);
        }

        [Test]
        public void TraerProveedorTest()
        {
            proveedorManagerMock.Setup(x => x.TraerProveedor(1, GlobalVariables.IdActiveDirectory, GlobalVariables.Equipo)).Returns(new StoredPorProveedorResult());
            var result = target.TraerProveedor(1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            proveedorManagerMock.Verify(x => x.TraerProveedor(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<int>>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ActividadTraerPorProveedores\":null,\"BasicoProveedorTraerPorProveedores\":null,\"ContactosComercialesTraerPorProveedores\":null,\"CampoProduccionAcopioPorProveedores\":null,\"Acopio\":null,\"AcopioMaterialPorProveedores\":null,\"ActividadHistoriaTraerPorProveedores\":null,\"DatosContacto\":null,\"Historial\":null,\"CanalesDeOperacion\":null,\"ProveedorDestinatario\":null,\"ProveedorCondicion\":null,\"ProveedorCampoDetalle\":null,\"ObjetivosTraerPorProveedorId\":null,\"ProveedorCorredor\":null,\"CompraDetalle\":null,\"Material\":null,\"Campanias\":null,\"CapacidadProductiva\":null},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void IniciliazarTest()
        {
            proveedorManagerMock.Setup(x => x.TraerDatosCombo(1, false)).Returns(new DatosIniProveedor());

            var result = target.Iniciliazar(1);

            proveedorManagerMock.Verify(x => x.TraerDatosCombo(It.IsAny<int>(), false), Times.Once);

            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Segmentacion\":[],\"TipoTelefono\":[],\"prov\":[],\"Localidad\":[],\"CanalOperacion\":[],\"Material\":[],\"dest\":[],\"cond\":[],\"inte\":[],\"TipoActividad\":[],\"concom\":[],\"ClasComNet\":[],\"BoleComNet\":[],\"BolsComNet\":[],\"comercial\":[],\"TipoApoderado\":[]},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void TraerLocalidadTest()
        {
            proveedorManagerMock.Setup(x => x.TraerLocalidad(1)).Returns(new List<LocalidadDto>() { new LocalidadDto {
                LocalidadId = 1,
                CodLocalidad = "A",
                Nombre = "A",
                ProvinciaId = 2,
                Provincia_Nombre = "A",
                CodigoPostal = "A",
                SubCodigoPostal = "B",
                CodigoConfirma = "AB",
            } });
            var result = target.TraerLocalidad(1);

            proveedorManagerMock.Verify(x => x.TraerLocalidad(It.IsAny<int>()), Times.Once);
            Assert.NotNull(result);
            var serializedResult = serializer.Serialize(result);
            Console.WriteLine("Serialized Result: " + serializedResult);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"LocalidadId\":1,\"Nombre\":\"A\",\"CodLocalidad\":\"A\",\"CodigoPostal\":\"A\",\"SubCodigoPostal\":\"B\",\"ProvinciaId\":2,\"Provincia_Nombre\":\"A\",\"Partido_Nombre\":null,\"PartidoId\":null,\"CodigoConfirma\":\"AB\"}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                serializedResult);
        }

        [Test]
        public void CrearActividadTest()
        {
            var param = new ActividadInsertarIni { UserName = "dominio\\nombre", ComercialId = 1 };
            homeManagerMock.Setup(x => x.TraerIdComercial(GlobalVariables.IdActiveDirectory)).Returns(1);
            proveedorManagerMock.Setup(x => x.GrabarRecordatorio(param)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.CrearActividad(new ActividadInsertarIni());

            homeManagerMock.Verify(x => x.TraerIdComercial(It.IsAny<string>()), Times.Once);
            proveedorManagerMock.Verify(x => x.GrabarRecordatorio(It.IsAny<ActividadInsertarIni>()), Times.Once);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ActividadId\":0,\"TipoActividadId\":0,\"Detalle\":null,\"ProveedorId\":0,\"FechaHoraActividad\":\"\\/Date(-62135586000000)\\/\",\"FechaHoraRecordatorio\":null,\"ComercialId\":null,\"ContactoComercialId\":null,\"asunto\":null,\"FechaHoraRecordatorioFin\":null,\"TipoActividad\":null,\"Proveedor\":null,\"Comercial\":null,\"ContactoComercial\":null},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void EliminarRecordatorioTest()
        {
            proveedorManagerMock.Setup(x => x.EliminarRecordatorio(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.EliminarRecordatorio(1);

            proveedorManagerMock.Verify(x => x.EliminarRecordatorio(It.IsAny<int>()), Times.Once);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ActividadId\":0,\"TipoActividadId\":0,\"Detalle\":null,\"ProveedorId\":0,\"FechaHoraActividad\":\"\\/Date(-62135586000000)\\/\",\"FechaHoraRecordatorio\":null,\"ComercialId\":null,\"ContactoComercialId\":null,\"asunto\":null,\"FechaHoraRecordatorioFin\":null,\"TipoActividad\":null,\"Proveedor\":null,\"Comercial\":null,\"ContactoComercial\":null},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerRazonSocialTest()
        {
            proveedorManagerMock.Setup(x => x.TraerRazonSocial("1")).Returns(new ProveedorNuevo());
            var result = target.TraerRazonSocial("1");

            proveedorManagerMock.Verify(x => x.TraerRazonSocial(It.IsAny<string>()), Times.Once);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"razonSocial\":\"\",\"Operable\":0,\"CUIT\":\"\",\"Condicion\":\"\",\"EstadoCuit\":0,\"FechaVigenciaEstado\":\"\\/Date(-62135586000000)\\/\",\"RiesgoComercial\":\"\",\"Existe\":0},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void GrabarProveedorNuevoTest()
        {
            var prove = new NuevoProveedor { ProveedorId = 0, basicos = new Basico { cuit = "111", RazonSocial = "A", } };
            var modificados = new CampaniaDto { CampaniaId = new List<int> { 9 }, CampaniaDesc = new List<string> { "20-21" }, ComercialId = 57 };
            proveedorManagerMock.Setup(x => x.GrabarNuevoProveedor(prove, GlobalVariables.IdActiveDirectory)).Returns(new GrabarProveedorResult { ProveedorId = 2, Errores = new List<ErrorMessage>() });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<InformeComercial, int>>>(), It.IsAny<Expression<Func<InformeComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<int>() { 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Campo, int>>>(), It.IsAny<Expression<Func<Campo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<int>() { 2 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Acopio, int>>>(), It.IsAny<Expression<Func<Acopio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<int>() { 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampoMaterial, CampoMaterialDto>>>(), It.IsAny<Expression<Func<CampoMaterial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<CampoMaterialDto>() { new CampoMaterialDto { CampoId = 5, MaterialId = 3 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                    .Returns(new List<AcopioMaterial>());

            informeComercialManagerMock.Setup(x => x.EliminarInformes(It.IsAny<int>())).Returns(new Resultado());
            informeComercialManagerMock.Setup(x => x.GrabarInformeComercial(It.IsAny<ParamInformeComercial>(), It.IsAny<int>(), It.IsAny<List<NuevoProduccion>>(), It.IsAny<List<NuevoAcopio>>(), It.IsAny<ContactoComercial>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>())).Returns(new InformeResult { InformeId = 1 });
            informeComercialManagerMock.Setup(x => x.GenerarInformeComercial(It.IsAny<ParamInformeComercial>(), It.IsAny<int>())).Returns(new RptInformeComercialInfo { CUIT = "333333333", RazonSocial = "PARISI" });
            informeComercialManagerMock.Setup(x => x.EnviarMailInformeComercial("downloadKey"));

            var result = target.GrabarProveedor(prove, modificados) as JsonResult;

            proveedorManagerMock.Verify(x => x.UpdateProveedor(It.IsAny<NuevoProveedor>(), It.IsAny<string>(), It.IsAny<List<int>>(), It.IsAny<int>()), Times.Never);
            proveedorManagerMock.Verify(x => x.GrabarNuevoProveedor(It.IsAny<NuevoProveedor>(), It.IsAny<string>()), Times.Once);
            informeComercialManagerMock.Verify(x => x.EliminarInformes(It.IsAny<int>()), Times.Once);
            informeComercialManagerMock.Verify(x => x.GrabarInformeComercial(It.IsAny<ParamInformeComercial>(), It.IsAny<int>(), It.IsAny<List<NuevoProduccion>>(), It.IsAny<List<NuevoAcopio>>(), It.IsAny<ContactoComercial>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>()), Times.Once);
            informeComercialManagerMock.Verify(x => x.GenerarInformeComercial(It.IsAny<ParamInformeComercial>(), It.IsAny<int>()), Times.Once);
            informeComercialManagerMock.Verify(x => x.EnviarMailInformeComercial(It.IsAny<string>()), Times.Once);
            Assert.NotNull(result);

            var model = serializer.Deserialize<GrabarProveedorResult>(serializer.Serialize(result.Data));
            Assert.AreEqual(false, model.HayErrores);
            Assert.AreEqual(1, model.DownloadKey.Count);
        }
        [Test]
        public void GrabarProveedorUpdateTest()
        {
            HttpContext.Current.Session["comercialId"] = 1;
            var prove = new NuevoProveedor { ProveedorId = 1, basicos = new Basico { cuit = "111", RazonSocial = "A", } };
            var modificados = new CampaniaDto { CampaniaId = new List<int> { 9 }, CampaniaDesc = new List<string> { "20-21" }, ComercialId = 57 };
            proveedorManagerMock.Setup(x => x.UpdateProveedor(prove, GlobalVariables.IdActiveDirectory, GlobalVariables.Equipo, GlobalVariables.ComercialId)).Returns(new GrabarProveedorResult { ProveedorId = 1, Errores = new List<ErrorMessage>() });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<InformeComercial, int>>>(), It.IsAny<Expression<Func<InformeComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<int>() { 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Campo, int>>>(), It.IsAny<Expression<Func<Campo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<int>() { 2 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Acopio, int>>>(), It.IsAny<Expression<Func<Acopio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<int>() { 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampoMaterial, CampoMaterialDto>>>(), It.IsAny<Expression<Func<CampoMaterial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<CampoMaterialDto>() { new CampoMaterialDto { CampoId = 5, MaterialId = 3 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AcopioMaterial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<AcopioMaterial>());

            informeComercialManagerMock.Setup(x => x.EliminarInformes(It.IsAny<int>())).Returns(new Resultado());
            informeComercialManagerMock.Setup(x => x.GrabarInformeComercial(It.IsAny<ParamInformeComercial>(), It.IsAny<int>(), It.IsAny<List<NuevoProduccion>>(), It.IsAny<List<NuevoAcopio>>(), It.IsAny<ContactoComercial>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>())).Returns(new InformeResult { InformeId = 1 });
            informeComercialManagerMock.Setup(x => x.GenerarInformeComercial(It.IsAny<ParamInformeComercial>(), It.IsAny<int>())).Returns(new RptInformeComercialInfo { CUIT = "333333333", RazonSocial = "PARISI" });
            informeComercialManagerMock.Setup(x => x.EnviarMailInformeComercial("downloadKey"));

            var result = target.GrabarProveedor(prove, modificados) as JsonResult;

            proveedorManagerMock.Verify(x => x.UpdateProveedor(It.IsAny<NuevoProveedor>(), It.IsAny<string>(), It.IsAny<List<int>>(), It.IsAny<int>()), Times.Once);
            proveedorManagerMock.Verify(x => x.GrabarNuevoProveedor(It.IsAny<NuevoProveedor>(), It.IsAny<string>()), Times.Never);
            informeComercialManagerMock.Verify(x => x.EliminarInformes(It.IsAny<int>()), Times.Once);
            informeComercialManagerMock.Verify(x => x.GrabarInformeComercial(It.IsAny<ParamInformeComercial>(), It.IsAny<int>(), It.IsAny<List<NuevoProduccion>>(), It.IsAny<List<NuevoAcopio>>(), It.IsAny<ContactoComercial>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>()), Times.Once);
            informeComercialManagerMock.Verify(x => x.GenerarInformeComercial(It.IsAny<ParamInformeComercial>(), It.IsAny<int>()), Times.Once);
            informeComercialManagerMock.Verify(x => x.EnviarMailInformeComercial(It.IsAny<string>()), Times.Once);
            Assert.NotNull(result);

            var model = serializer.Deserialize<GrabarProveedorResult>(serializer.Serialize(result.Data));
            Assert.AreEqual(false, model.HayErrores);
            Assert.AreEqual(1, model.DownloadKey.Count);
        }

        [Test]
        public void TraerCampañasActivasTest()
        {
            campanaManagerMock.Setup(x => x.TraerCampañasActivas()).Returns(new List<CampañaDto>() { new CampañaDto { CampañaId = 1, Descripcion = "18-19" } });
            var result = target.TraerCampañasActivas();

            campanaManagerMock.Verify(x => x.TraerCampañasActivas(), Times.Once);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"CampañaId\":1,\"Descripcion\":\"18-19\",\"CodigoSIO\":null,\"Sugerido\":false}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerMaterialPorCampañaTest()
        {
            campanaManagerMock.Setup(x => x.TraerMaterialPorCampaña(1)).Returns(new List<MaterialDto>() { new MaterialDto { CampañaId = 1, Descripcion = "A", MaterialId = 1 } });
            var result = target.TraerMaterialPorCampaña(1);

            campanaManagerMock.Verify(x => x.TraerMaterialPorCampaña(It.IsAny<int>()), Times.Once);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"MaterialId\":1,\"Codigo\":null,\"Descripcion\":\"A\",\"CampañaId\":1,\"Campana\":null,\"CampaniaTableroId\":null,\"CampaniaTablero\":null,\"IVA\":null,\"DestinoId\":0}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerCampañaPorMaterialTest()
        {
            campanaManagerMock.Setup(x => x.TraerCampañaPorMaterial(1)).Returns(new List<CampañaDto>() { new CampañaDto { CampañaId = 1, Descripcion = "18-19" } });
            var result = target.TraerCampañaPorMaterial(1);

            campanaManagerMock.Verify(x => x.TraerCampañaPorMaterial(It.IsAny<int>()), Times.Once);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"CampañaId\":1,\"Descripcion\":\"18-19\",\"CodigoSIO\":null,\"Sugerido\":false}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerFiltrosTest()
        {
            var histo = new HistorialActividad { ActividadId = 1, ProveedorId = 1, TipoActividad = "A" };
            proveedorManagerMock.Setup(x => x.TraerHistorialActividad(histo, 1, "A")).Returns(new StoredHistorialResult());
            var result = target.TraerFiltros("A", 1, histo);

            proveedorManagerMock.Verify(x => x.TraerHistorialActividad(It.IsAny<HistorialActividad>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ActividadHistoriaTraerPorProveedores\":null},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ImprimirReporteProveedorTest()
        {
            var prov = new StoredPorProveedorResult
            {
                BasicoProveedorTraerPorProveedores = new List<BasicoProveedor>() { new BasicoProveedor { CUIT = "a", ProveedorId = 1, RazonSocial = "A", Estado = "b", Segmentacion = "c" } },
                CanalesDeOperacion = new List<CanalOperacion>(),
                DatosContacto = new DatosContacto(),
                ProveedorDestinatario = new List<Destinatario>(),
                ProveedorCondicion = new List<Condicion>(),
                ContactosComercialesTraerPorProveedores = new List<ContactosComerciales>(),
                CampoProduccionAcopioPorProveedores = new List<CampoProduccionAcopio>(),
                Acopio = new List<CampoProduccionAcopio>(),
                ObjetivosTraerPorProveedorId = new List<ObjetivosTraer>()
            };
            var histo = new HistorialActividad { ActividadId = 1, ProveedorId = 1, TipoActividad = "A" };
            proveedorManagerMock.Setup(x => x.TraerProveedor(1, GlobalVariables.IdActiveDirectory, GlobalVariables.Equipo)).Returns(prov);
            var result = target.ImprimirReporteProveedor(1);

            proveedorManagerMock.Verify(x => x.TraerProveedor(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<int>>()), Times.Once);

            var model = result.Result as JsonResult;
            Assert.NotNull(result);
            Assert.IsTrue(((ReportesModel)model.Data).DownloadKey != "");
        }

        [Test]
        public void TraerCampañasPorGranoTest()
        {
            campanaManagerMock.Setup(x => x.TraerCampañasPorGrano(1)).Returns(new List<CampañaDto>() { new CampañaDto { CampañaId = 1, Descripcion = "18-19" } });
            var result = target.TraerCampañasPorGrano(1);

            campanaManagerMock.Verify(x => x.TraerCampañasPorGrano(It.IsAny<int>()), Times.Once);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"CampañaId\":1,\"Descripcion\":\"18-19\",\"CodigoSIO\":null,\"Sugerido\":false}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ObtenerReporteProveedorTest()
        {
            proveedorManagerMock.Setup(x => x.ObtenerReporteProveedor("a", GlobalVariables.IdActiveDirectory)).Returns(new List<ReporteProveedor>() { new ReporteProveedor { CUIT = "111", ProveedorId = 1, RazonSocial = "A", Comerciales = "B" } });
            var result = target.ObtenerReporteProveedor("a");

            proveedorManagerMock.Verify(x => x.ObtenerReporteProveedor(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"ProveedorId\":1,\"CUIT\":\"111\",\"RazonSocial\":\"A\",\"EstadoId\":null,\"Comerciales\":\"B\",\"Estado\":null,\"Alias\":null}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void BuscarCorredoresTest()
        {
            proveedorManagerMock.Setup(x => x.DevolverProveedores("agr", 1, GlobalVariables.Equipo, It.IsAny<int>())).Returns(new List<BusquedaHome> { new BusquedaHome { Id = 1, RazonSocial = "Agro A.", Cuit = "202", Filtro = "agr|AGRO A." } });
            var result = target.BuscarCorredores("agr", 1, 1);

            proveedorManagerMock.Verify(x => x.DevolverProveedores(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<List<int>>(), It.IsAny<int>()), Times.Once);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Id\":1,\"RazonSocial\":\"Agro A.\",\"Cuit\":\"202\",\"Corredor\":null,\"Filtro\":\"agr|AGRO A.\",\"Alias\":null,\"ClasificacionId\":null,\"RiesgoComercialSap\":null,\"Estado\":null,\"Deshabilitado\":null,\"Consignatario\":null,\"PlanCanje\":null,\"Deshabilitar\":false,\"Color\":null,\"ComisionistaId\":null,\"CuposConRiesgo\":null,\"OperaConMATBA\":null,\"EstaAsignado\":false,\"Segmentacion\":null,\"Grupo\":null,\"SegmentacionId\":0}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void BuscarProveedoresConCorredorOkSinCorredorTest()
        {
            proveedorManagerMock.Setup(x => x.DevolverProveedores("agr", 0, GlobalVariables.Equipo, It.IsAny<int>())).Returns(new List<BusquedaHome> { new BusquedaHome { Id = 1, RazonSocial = "Agro A.", Cuit = "202", Filtro = "agr|AGRO A." } });
            var result = target.BuscarProveedoresConCorredor("agr", "", 1);

            proveedorManagerMock.Verify(x => x.DevolverProveedores(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<List<int>>(), It.IsAny<int>()), Times.Once);
            proveedorManagerMock.Verify(x => x.DevolverProveedoresConCorredor(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Id\":1,\"RazonSocial\":\"Agro A.\",\"Cuit\":\"202\",\"Corredor\":null,\"Filtro\":\"agr|AGRO A.\",\"Alias\":null,\"ClasificacionId\":null,\"RiesgoComercialSap\":null,\"Estado\":null,\"Deshabilitado\":null,\"Consignatario\":null,\"PlanCanje\":null,\"Deshabilitar\":false,\"Color\":null,\"ComisionistaId\":null,\"CuposConRiesgo\":null,\"OperaConMATBA\":null,\"EstaAsignado\":false,\"Segmentacion\":null,\"Grupo\":null,\"SegmentacionId\":0}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void BuscarProveedoresConCorredorOkConCorredorTest()
        {
            proveedorManagerMock.Setup(x => x.DevolverProveedoresConCorredor("agr", "2011")).Returns(new List<BusquedaHome> { new BusquedaHome { Id = 1, RazonSocial = "Agro A.", Cuit = "202", Filtro = "agr|AGRO A." } });
            var result = target.BuscarProveedoresConCorredor("agr", "2011", 1);

            proveedorManagerMock.Verify(x => x.DevolverProveedores(It.IsAny<string>(), 1, It.IsAny<List<int>>(), It.IsAny<int>()), Times.Never);
            proveedorManagerMock.Verify(x => x.DevolverProveedoresConCorredor(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Id\":1,\"RazonSocial\":\"Agro A.\",\"Cuit\":\"202\",\"Corredor\":null,\"Filtro\":\"agr|AGRO A.\",\"Alias\":null,\"ClasificacionId\":null,\"RiesgoComercialSap\":null,\"Estado\":null,\"Deshabilitado\":null,\"Consignatario\":null,\"PlanCanje\":null,\"Deshabilitar\":false,\"Color\":null,\"ComisionistaId\":null,\"CuposConRiesgo\":null,\"OperaConMATBA\":null,\"EstaAsignado\":false,\"Segmentacion\":null,\"Grupo\":null,\"SegmentacionId\":0}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }

        //[Test]
        //public void BuscarProveedoresEnSugerenciaTest() {
        //    homeManagerMock.Setup(x => x.BusquedaHome("agr", 1, GlobalVariables.Equipo, new List<int> {1,2,3})).Returns(new List<BusquedaHome> { new BusquedaHome { Id = 1, RazonSocial = "Agro A.", Cuit = "202", Filtro = "agr|AGRO A." } });
        //    var result = target.BuscarProveedoresEnSugerencia("2011", "agr", 1); //da error por el GlobalVariables.ComercialId, que se pasa a BusquedaHome

        //    homeManagerMock.Verify(x => x.BusquedaHome(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<List<int>>(), It.IsAny<List<int>>()), Times.Once);
        //    Assert.NotNull(result);
        //    var a = serializer.Serialize(result);
        //    Assert.AreEqual(
        //        "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Id\":1,\"RazonSocial\":\"Agro A.\",\"Cuit\":\"202\",\"Corredor\":null,\"Filtro\":\"agr|AGRO A.\",\"Alias\":null,\"ClasificacionId\":null,\"RiesgoComercialSap\":null,\"Estado\":null,\"Deshabilitado\":null,\"Consignatario\":null,\"PlanCanje\":null,\"Deshabilitar\":false,\"Color\":null,\"ComisionistaId\":null,\"CuposConRiesgo\":null,\"OperaConMATBA\":null,\"EstaAsignado\":false}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
        //        a);
        //}

        [Test]
        public void BuscarLocalidadesTest()
        {
            localidadManagerMock.Setup(x => x.DevolverLocalidades("a")).Returns(new List<BusquedaLocalidad> { new BusquedaLocalidad { Id = 1, Localidad = "A", Provincia = "B", ProvinciaId = 2, Filtro = "a|AG" } });
            var result = target.BuscarLocalidades("a");

            localidadManagerMock.Verify(x => x.DevolverLocalidades(It.IsAny<string>()), Times.Once);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Id\":1,\"Localidad\":\"A\",\"CodLocalidad\":null,\"Provincia\":\"B\",\"Partido\":null,\"Descripcion\":null,\"PartidoId\":0,\"ProvinciaId\":2,\"Filtro\":\"a|AG\"}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void TraerProveedoresCorredorTest()
        {
            proveedorManagerMock.Setup(x => x.ListarProveedorCorredor(1)).Returns(new List<ProveedorCorredorDto> { new ProveedorCorredorDto { CUIT = "A", ProveedorCorredorId = 1, CodigoPostal = "1", ProvinciaId = 2, ClasificacionCompraNetId = 1, ProveedorId = 1 } });
            var result = target.TraerProveedoresCorredor(1);

            proveedorManagerMock.Verify(x => x.ListarProveedorCorredor(It.IsAny<int>()), Times.Once);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"ProveedorId\":1,\"CUIT\":\"A\",\"RazonSocial\":null,\"Localidad\":null,\"Provincia\":null,\"LocalidadId\":null,\"ProvinciaId\":2,\"LocalidadCompraNet\":null,\"ProvinciaCompraNet\":null,\"LocalidadCompraNetId\":null,\"ProvinciaCompraNetId\":null,\"Direccion\":null,\"CodigoPostal\":\"1\",\"ClasificacionCompraNetId\":1,\"ClasificacionDescripcion\":null,\"ProveedorCorredorId\":1,\"Consignatario\":null,\"NoOperable\":null,\"Operando\":null,\"TooltipNoOperable\":null,\"EstadoCuit\":null,\"RiesgoComercialSap\":null,\"Facacop\":false,\"Alias\":null}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void TraerProveedorParaCorredorTest()
        {
            proveedorManagerMock.Setup(x => x.TraerProveedorParaCorredor("201")).Returns(new TraerProveedorResult { Proveedor = new ProveedorDto { ProveedorId = 1, RazonSocial = "A", CUIT = "201" }, Errores = new List<ErrorMessage>() });
            var result = target.TraerProveedorParaCorredor("201");

            proveedorManagerMock.Verify(x => x.TraerProveedorParaCorredor(It.IsAny<string>()), Times.Once);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Proveedor\":{\"ProveedorId\":1,\"CUIT\":\"201\",\"RazonSocial\":\"A\",\"Localidad\":null,\"Provincia\":null,\"LocalidadId\":null,\"ProvinciaId\":null,\"EstadoId\":null,\"Direccion\":null,\"CodigoPostal\":null,\"LocalidadCompraNetId\":null,\"ProvinciaCompraNetId\":null,\"LocalidadCompraNet\":null,\"ProvinciaCompraNet\":null,\"ClasificacionCompraNetId\":null,\"ClasificacionDescripcion\":null,\"ComisionPorcentaje\":null,\"Consignatario\":null,\"SegmentacionId\":0,\"Deshabilitado\":null,\"Alias\":null,\"ComisionistaId\":null,\"CuposConRiesgo\":null,\"Comisionista\":false,\"EstadoHomeId\":null,\"EstadoHomeMensaje\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GrabarCorredorUpdateTest()
        {
            HttpContext.Current.Session["comercialId"] = 1;
            var corredor = new NuevoCorredor { CorredorId = 1, basicos = new Basico { RazonSocial = "A", cuit = "201", segmentacion = 5 }, contacto = new Contacto { provincia = 1, localidad = 2 } };
            proveedorManagerMock.Setup(x => x.UpdateCorredor(corredor, GlobalVariables.IdActiveDirectory, GlobalVariables.Equipo, GlobalVariables.ComercialId)).Returns(new GrabarProveedorResult { ProveedorId = 1, Errores = new List<ErrorMessage>() });
            var result = target.GrabarCorredor(corredor) as JsonResult;

            proveedorManagerMock.Verify(x => x.UpdateCorredor(It.IsAny<NuevoCorredor>(), It.IsAny<string>(), It.IsAny<List<int>>(), It.IsAny<int>()), Times.Once);
            proveedorManagerMock.Verify(x => x.GrabarNuevoCorredor(It.IsAny<NuevoCorredor>(), It.IsAny<string>()), Times.Never);
            Assert.NotNull(result);
            var model = serializer.Deserialize<GrabarProveedorResult>(serializer.Serialize(result.Data));
            Assert.AreEqual(false, model.HayErrores);
            Assert.AreEqual(0, model.DownloadKey.Count);
        }

        [Test]
        public void GrabarCorredorNuevoTest()
        {
            var corredor = new NuevoCorredor { CorredorId = 0, basicos = new Basico { RazonSocial = "A", cuit = "201", segmentacion = 5 }, contacto = new Contacto { provincia = 1, localidad = 2 } };
            proveedorManagerMock.Setup(x => x.GrabarNuevoCorredor(corredor, GlobalVariables.IdActiveDirectory)).Returns(new GrabarProveedorResult { ProveedorId = 1, Errores = new List<ErrorMessage>() });
            var result = target.GrabarCorredor(corredor) as JsonResult;

            proveedorManagerMock.Verify(x => x.UpdateCorredor(It.IsAny<NuevoCorredor>(), It.IsAny<string>(), It.IsAny<List<int>>(), It.IsAny<int>()), Times.Never);
            proveedorManagerMock.Verify(x => x.GrabarNuevoCorredor(It.IsAny<NuevoCorredor>(), It.IsAny<string>()), Times.Once);
            Assert.NotNull(result);
            var model = serializer.Deserialize<GrabarProveedorResult>(serializer.Serialize(result.Data));
            Assert.AreEqual(false, model.HayErrores);
            Assert.AreEqual(0, model.DownloadKey.Count);
        }
    }
}
