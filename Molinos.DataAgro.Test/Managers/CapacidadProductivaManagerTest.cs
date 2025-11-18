using NLog;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Molinos.DataAgro.Test.Managers
{
    [TestFixture]
    public class CapacidadProductivaManagerTest
    {
        private CapacidadProductivaManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<IVisualizarCapacidadProductivaAgent> capProductivaAgentMock;

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            capProductivaAgentMock = new Mock<IVisualizarCapacidadProductivaAgent>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            target = new CapacidadProductivaManager(logger.Object, repositorioMock.Object, capProductivaAgentMock.Object);
        }

        [Test]
        public void ObtenerCapacidadProductivaTest()
        {
            capProductivaAgentMock.Setup(x => x.VisualizarCapacidadProductiva(It.IsAny<int>(), null, null, null))
                .Returns(new List<CapacidadProductivaDto> { new CapacidadProductivaDto { ProveedorId = 1 } });
            var resultado = target.ObtenerCapacidadProductiva(1);
            capProductivaAgentMock.Verify(x => x.VisualizarCapacidadProductiva(It.IsAny<int>(), null, null, null), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.FirstOrDefault().ProveedorId);
        }

        [Test]
        public void ActualizarCapacidadProductivaTest()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Proveedor, ProveedorDto>>>(), It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<ProveedorDto>());
            repositorioMock.Setup(x => x.AgregarTodos(It.IsAny<List<CapacidadProductiva>>(), null)).Verifiable();

            target.ActualizarCapacidadProductiva();

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Proveedor, ProveedorDto>>>(), It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.AgregarTodos(It.IsAny<List<CapacidadProductiva>>(), null), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
    }
}
