using NLog;
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
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ZonaCupoManagerTest
    {
        private ZonaCupoManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            target = new ZonaCupoManager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void TraerDatosInicialesTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ZonaCupo, ZonaCupoCombo>>>(), It.IsAny<Expression<Func<ZonaCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<ZonaCupoCombo>() { new ZonaCupoCombo { Id = 1, Descripcion = "1" } });
            var result = target.TraerDatosIniciales();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ZonaCupo, ZonaCupoCombo>>>(), It.IsAny<Expression<Func<ZonaCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.ZonaCupo.Count);
        }
        [Test]
        public void TraerTodoZonaCupoTestOk()
        {
            Thread.CurrentPrincipal = new TestPrincipal(new Claim[] {
            new Claim(ClaimTypes.Role, PermisosDataAgro.AltaCorredoresBsAs.ToString()) ,
            new Claim(ClaimTypes.Role, PermisosDataAgro.AltaCorredoresRosario.ToString()),
            new Claim(ClaimTypes.Role, PermisosDataAgro.AltaOrigenCentro.ToString()),
            new Claim(ClaimTypes.Role, PermisosDataAgro.AltaOrigenNorte.ToString()),
            new Claim(ClaimTypes.Role, PermisosDataAgro.AltaOtrasZonas.ToString()),
            new Claim(ClaimTypes.Role, PermisosDataAgro.AltaOrigenSur.ToString()) });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ZonaCupo, ZonaCupoIni>>>(), It.IsAny<Expression<Func<ZonaCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<ZonaCupoIni>() { new ZonaCupoIni { Id = 1, Descripcion = "1",CodigoSap ="OIC" } });
            var result = target.TraerTodoZonaCupo();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ZonaCupo, ZonaCupoIni>>>(), It.IsAny<Expression<Func<ZonaCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.ZonaCupo.Count);
        }
        [Test]
        public void TraerZonaCupoOk()
        {

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ZonaCupo, bool>>>(), It.IsAny<Expression<Func<ZonaCupo, ZonaCupoDto>>>()))
                .Returns(new ZonaCupoDto { Id = 1 });

            var resultado = target.TraerZonaCupo(1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<ZonaCupo, bool>>>(), It.IsAny<Expression<Func<ZonaCupo, ZonaCupoDto>>>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Id);
        }
        [Test]
        public void GrabarZonaCupoOk()
        {
            var prov = new ZonaCupo { Id = 1, Descripcion = "a", CodigoSap = "a" };
            repositorioMock.Setup(y => y.Obtener<ZonaCupo>(It.IsAny<int>()))
                .Returns(new ZonaCupo { Id = 1 });

            var resultado = target.GrabarZonaCupo(prov);
            repositorioMock.Verify(x => x.Obtener<ZonaCupo>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<ZonaCupo>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void GrabarZonaCupoNuevaOk()
        {
            var prov = new ZonaCupo { Id = 0, Descripcion = "a", CodigoSap = "a" };
            repositorioMock.Setup(y => y.Obtener<ZonaCupo>(It.IsAny<int>()))
                .Returns(new ZonaCupo { Id = 1 });

            var resultado = target.GrabarZonaCupo(prov);
            repositorioMock.Verify(x => x.Obtener<ZonaCupo>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<ZonaCupo>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void EliminarZonaCupoOk()
        {
            var resultado = target.EliminarZonaCupo(1);
            repositorioMock.Verify(x => x.Remover<ZonaCupo>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
    }
}
