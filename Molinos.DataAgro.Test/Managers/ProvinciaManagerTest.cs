using Autofac.Extras.NLog;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ProvinciaManagerTest
    {
        private ProvinciaManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            target = new ProvinciaManager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void TraerTodoProvinciaTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Provincia, ProvinciaIni>>>(), It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<ProvinciaIni>() { new ProvinciaIni { ProvinciaId = 1, Nombre = "1" } });
            var result = target.TraerTodoProvincia();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Provincia, ProvinciaIni>>>(), It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Provincia.Count);
        }

        [Test]
        public void ObtenerProvinciaOk()
        {

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<Expression<Func<Provincia, ProvinciaDto>>>()))
                .Returns(new ProvinciaDto { ProvinciaId = 1 });

            var resultado = target.ObtenerProvincia(1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<Expression<Func<Provincia, ProvinciaDto>>>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.ProvinciaId);
        }
        [Test]
        public void GrabarProvinciaOk()
        {
            var prov = new Provincia { ProvinciaId = 1, Nombre = "a" };
            repositorioMock.Setup(y => y.Obtener<Provincia>(It.IsAny<int>()))
                .Returns(new Provincia { ProvinciaId = 1 });

            var resultado = target.GrabarProvincia(prov);
            repositorioMock.Verify(x => x.Obtener<Provincia>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Provincia>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void GrabarProvinciaNuevaOk()
        {
            var prov = new Provincia { ProvinciaId = 0, Nombre = "a" };
            repositorioMock.Setup(y => y.Obtener<Provincia>(It.IsAny<int>()))
                .Returns(new Provincia { ProvinciaId = 1 });

            var resultado = target.GrabarProvincia(prov);
            repositorioMock.Verify(x => x.Obtener<Provincia>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Provincia>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void EliminarProvinciaOk()
        {
            var resultado = target.EliminarProvincia(1);
            repositorioMock.Verify(x => x.Remover<Provincia>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void ListarProvinciaOk()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Provincia, ProvinciaDto>>>(), It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ProvinciaDto>() { new ProvinciaDto { Nombre = "a", ProvinciaId = 1 } });
            var resultado = target.ListarProvincia("a");
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Provincia, ProvinciaDto>>>(), It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Count);
        }
    }
}
