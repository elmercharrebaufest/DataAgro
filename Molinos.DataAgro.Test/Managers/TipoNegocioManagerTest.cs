using NLog;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Linq.Expressions;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class TipoNegocioManagerTest
    {
        private TipoNegocioManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();

            target = new TipoNegocioManager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void TraerTipoNegociod()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<TipoNegocio, bool>>>(),
                It.IsAny<Expression<Func<TipoNegocio, TipoNegocioDto>>>()))
            .Returns( new TipoNegocioDto());
            var result = target.TraerTipoNegociod(1);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<TipoNegocio, bool>>>(), It.IsAny<Expression<Func<TipoNegocio, TipoNegocioDto>>>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(0, result.TipoNegocioId);
            Assert.IsNull(result.Descripcion);
        }
        [Test]
        public void TraerTipoNegociodConResultado()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<TipoNegocio, bool>>>(),
                It.IsAny<Expression<Func<TipoNegocio, TipoNegocioDto>>>()))
            .Returns(new TipoNegocioDto {Descripcion="a",TipoNegocioId=1 });
            var result = target.TraerTipoNegociod(1);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<TipoNegocio, bool>>>(), It.IsAny<Expression<Func<TipoNegocio, TipoNegocioDto>>>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.TipoNegocioId);
            Assert.AreEqual("a",result.Descripcion);
        }
    }
}
