using Autofac.Extras.NLog;
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
    public class ComercialManagerTest
    {
        private ComercialManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<IDatoDelComercialAgent> datoDelComercialAgentMock;

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            datoDelComercialAgentMock = new Mock<IDatoDelComercialAgent>();

            target = new ComercialManager(logger.Object, repositorioMock.Object, datoDelComercialAgentMock.Object);
        }

        [Test]
        public void TraerDatosInicialesTest()
        {
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial,bool>>>(), It.IsAny<Expression<Func<Comercial, int?>>>()))
                            .Returns(1);
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Perfil, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>())).Returns(new List<Perfil>() { new Perfil { PerfilId = 1, Descripcion = "a" } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<GrupoDeCompras, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>())).Returns(new List<GrupoDeCompras>() { new GrupoDeCompras { Id = 1, Descripcion = "a" } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Comercial, ComercialCombo>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ComercialCombo>() { new ComercialCombo { Apellido = "a", ComercialId = 1 } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Comercial, ComercialQry>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ComercialQry>() { new ComercialQry { Apellido = "a", ComercialId = 1,EmpleadorACargo=2 } });
            repositorioMock.Setup(x=>x.Listar(It.IsAny<Expression<Func<Rol, RolCombo>>>(), It.IsAny<Expression<Func<Rol, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<RolCombo>() { new RolCombo { Id = 1, Descripcion = "A" } });

            var result = target.TraerDatosIniciales();
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial,bool>>>(), It.IsAny<Expression<Func<Comercial, int?>>>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Comercial, ComercialCombo>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Comercial, ComercialQry>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
        }
        [Test]
        public void TraerTodoComercialTest()
        {
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<TraerComerciales>()))
                .Returns(new List<ComercialIni>() { new ComercialIni { ComercialId = 1, Apellido = "a", Nombres = "a", PerDescripcion = "2" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, ComercialCombo>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ComercialCombo>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, ComercialQry>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ComercialQry>());
            var resultado = target.TraerTodoComercial();

            repositorioMock.Verify(x => x.ObtenerConsultaEscalar(It.IsAny<TraerComerciales>()), Times.Once);
            Assert.NotNull(resultado);
            Assert.AreEqual(1,resultado.Comercial.Count);
        }
        [Test]
        public void TraerComercialTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, ComercialDto>>>()))
                .Returns( new ComercialDto { ComercialId = 1, Apellido = "a", Nombres = "a", PerfilId = 1 });
            var resultado = target.TraerComercial(1);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, ComercialDto>>>()), Times.Once);
            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.ComercialId);
        }
        [Test]
        public void EliminarComercialTest()
        {
            repositorioMock.Setup(x => x.Obtener<Comercial>(It.IsAny<int>()))
                  .Returns(new Comercial { ComercialId = 1, Apellido = "a", Nombres = "a", PerfilId = 1 });
            var resultado = target.EliminarComercial(1);

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayError);
        }
        [Test]
        public void ComercialExisteTest()
        {
            repositorioMock.Setup(x => x.Existe(It.IsAny<Expression<Func<Comercial, bool>>>())).Returns(true);
            var resultado = target.ComercialExiste("a");

            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Once);

            Assert.IsTrue(resultado);
        }
        [Test]
        public void ComercialPerteneceProveedorTest()
        {
            repositorioMock.Setup(x => x.Existe(It.IsAny<Expression<Func<ProveedorComercial, bool>>>())).Returns(true);
            var resultado = target.ComercialPerteneceProveedor(new List<int>() { 1 }, 1, new List<int>() { 1, 2 });

            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<ProveedorComercial, bool>>>()), Times.Once);

            Assert.IsTrue(resultado);
        }
        [Test]
        public void ListarComercialTest()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Comercial, ComercialDto>>>(),It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(),It.IsAny<string>(),It.IsAny<DirOrden>()))
                .Returns(new List<ComercialDto>() { new ComercialDto { ComercialId= 1, IdActiveDirectory="a"} });
            var resultado = target.ListarComercial("a", new List<int>() { 1, 2 });

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Comercial, ComercialDto>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1,resultado.Count);
        }
        [Test]
        public void ObtenerPerfilDeUsuarioTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, int>>>()))
                .Returns(1);
            var resultado = target.ObtenerPerfilDeUsuario("a");

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, int>>>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(EnumPerfil.Comercial, resultado);
        }
        [Test]
        public void EsAdministradorTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(true);
            var resultado = target.EsAdministrador("a");

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Once);

            Assert.IsTrue(resultado);
        }
        [Test]
        public void EsCuperaTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(true);
            var resultado = target.EsCupera("a");

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Once);

            Assert.IsTrue(resultado);
        }
        [Test]
        public void ListarEquipoTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(),It.IsAny<Expression<Func<Comercial, int>>>()))
                .Returns(1);
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Comercial, ComercialQry>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(),It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ComercialQry>() { new ComercialQry {ComercialId=1,IdActiveDirectory="a" } });
            var resultado = target.ListarEquipo("a");

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, int>>>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Comercial, ComercialQry>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.AreEqual(1, resultado.Equipo.Count);
            Assert.AreEqual(1, resultado.EquipoReal.Count);
        }
        [Test]
        public void CadenaComercialesTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, bool>>>()))
                .Returns(false);
            repositorioMock.Setup(x => x.Obtener<Comercial>(It.IsAny<int>()))
                .Returns(new Comercial { ComercialId = 1, PerfilId = 1, RolesAsociados = new List<Rol>() { new Rol { PermisosAsociados = new List<RolPermiso>() { new RolPermiso { Permiso = PermisosDataAgro.NotificacionesMailJerarquia } } } } });
            var resultado = target.CadenaComerciales(1);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Comercial>(It.IsAny<int>()), Times.Once);

            Assert.AreEqual(1, resultado.Count);
        }
        [Test]
        public void ObtenerComercialIdTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, int>>>()))
                .Returns(1);
            var resultado = target.ObtenerComercialId("a");

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, int>>>()), Times.Once);

            Assert.AreEqual(1, resultado);
        }
        [Test]
        public void ListarCorredoresComercialTestNoCorredor()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, int>>>()))
                .Returns(1);
            var resultado = target.ListarCorredoresComercial();

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Comercial, int>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Never);

            Assert.AreEqual(0, resultado.Count);
        }
        [Test]
        public void ListarCorredoresComercialTestCorredor()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Comercial, int>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<int>() { 1, 2 });
            var resultado = target.ListarCorredoresComercial();

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Comercial, int>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Never);

            Assert.AreEqual(0, resultado.Count);
        }
        [Test]
        public void ListarComercialesPorPerfilTest()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<Comercial>() { new Comercial { ComercialId = 1 } });
            var resultado = target.ListarComercialesCorredor();

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.AreEqual(1, resultado.Count);
        }
        [Test]
        public void ListarGrupoDeComprasTestConFiltro()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<GrupoDeCompras, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<GrupoDeCompras>() { new GrupoDeCompras { Id = 1,Descripcion="1" } });
            var resultado = target.ListarGrupoDeCompras("a");

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<GrupoDeCompras, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.AreEqual(1, resultado.Count);
        }
        [Test]
        public void ListarGrupoDeComprasTestSinFiltro()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<GrupoDeCompras, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<GrupoDeCompras>() { new GrupoDeCompras { Id = 1, Descripcion = "1" } });
            var resultado = target.ListarGrupoDeCompras("");

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<GrupoDeCompras, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.AreEqual(1, resultado.Count);
        }

        [Test]
        public void TraerZonaDelComercialAsociadoTestOk()
        {
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ProveedorComercial, bool>>>(), It.IsAny<Expression<Func<ProveedorComercial, string>>>()))
                             .Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ZonaCupo, bool>>>(), It.IsAny<Expression<Func<ZonaCupo, int>>>()))
                           .Returns(1);
            var resultado = target.TraerZonaDelComercialAsociado();

            repositorioMock.Verify(y => y.Obtener(It.IsAny<Expression<Func<ProveedorComercial, bool>>>(), It.IsAny<Expression<Func<ProveedorComercial, string>>>()), Times.Once);
            repositorioMock.Verify(y => y.Obtener(It.IsAny<Expression<Func<ZonaCupo, bool>>>(), It.IsAny<Expression<Func<ZonaCupo, int>>>()), Times.Once);

            Assert.AreEqual(1, resultado);
        }
        
    }
}