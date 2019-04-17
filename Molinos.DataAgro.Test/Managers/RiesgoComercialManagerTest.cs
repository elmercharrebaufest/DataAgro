using Autofac.Extras.NLog;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
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
    public class RiesgoComercialManagerTest
    {
        private RiesgoComercialManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();

            target = new RiesgoComercialManager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void ActualizacionDeRiesgoComercialConProveedor()
        {
            var riesgo = new RiesgoComercial { CUIT = "1", RiesgoComercialDesc = "A" };
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
            .Returns( new Proveedor { ProveedorId = 1,CUIT="1",RiesgoComercialSap="A"});
            var result = target.ActualizacionDeRiesgoComercial(riesgo);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }
        [Test]
        public void ActualizacionDeRiesgoComercialSinProveedor()
        {
            var riesgo = new RiesgoComercial { CUIT = "1", RiesgoComercialDesc = "A" };
            var result = target.ActualizacionDeRiesgoComercial(riesgo);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
        }
    }
}
