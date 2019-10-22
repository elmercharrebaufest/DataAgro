using Autofac.Extras.NLog;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
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
    public class CupoManagerTest
    {
        private CupoManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<ICrearCupoAgent> crearCupoAgentMock;
        private Mock<IEliminarCupoAgent> eliminarCupoAgentMock;
        private Mock<IClienteStopAgent> clienteStopMock;
        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            crearCupoAgentMock = new Mock<ICrearCupoAgent>();
            eliminarCupoAgentMock = new Mock<IEliminarCupoAgent>();
            clienteStopMock = new Mock<IClienteStopAgent>();
            target = new CupoManager(repositorioMock.Object, logger.Object, crearCupoAgentMock.Object, eliminarCupoAgentMock.Object, clienteStopMock.Object);
        }

        [Test]
        public void GrabarCupoTestOk()
        {
            var cupo = new Cupo
            {
                ProveedorId = 1,
                MaterialId = 1,
                CentroId = 1,
                ZonaCupoId = 1
            };
            
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { ProveedorId = 1 });
            repositorioMock.Setup(y => y.Obtener<Material>(It.IsAny<int>()))
                .Returns(new Material { MaterialId = 1 });
            repositorioMock.Setup(y => y.Obtener<Centro>(It.IsAny<int>()))
                .Returns(new Centro { Id = 1 });
            repositorioMock.Setup(y => y.Obtener<ZonaCupo>(It.IsAny<int>()))
                .Returns(new ZonaCupo { Id = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<LimiteCupo, bool>>>())).Returns(new LimiteCupo {ConfiguracionCupoId=1,ZonaCupoId=1,CantidadCupo=10 });
            repositorioMock.Setup(y => y.Contar(It.IsAny<Expression<Func<Cupo, bool>>>())).Returns(0);

            crearCupoAgentMock.Setup(x => x.Crear(It.IsAny<Cupo>(), It.IsAny<int>()))
                .Returns(new List<string>() { "a" });
            repositorioMock.Setup(x => x.Agregar(It.IsAny<Cupo>()));
            var result = target.GrabarCupo(cupo, 1);
           
            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Material>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Centro>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<ZonaCupo>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.AgregarTodos(It.IsAny<List<Cupo>>(),null), Times.Once);
            crearCupoAgentMock.Verify(x => x.Crear(It.IsAny<Cupo>(), It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }

        [Test]
        public void EliminarCupoTest()
        {
            repositorioMock.Setup(x => x.Obtener<Cupo>(It.IsAny<int>())).Returns(new Cupo { CupoSap = "a",CupoStop = 1, EstadoCupoId=1 });
            eliminarCupoAgentMock.Setup(x => x.Eliminar(It.IsAny<string>())).Returns("OK");
            clienteStopMock.Setup(x => x.EliminarCupo(It.IsAny<Cupo>())).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            repositorioMock.Setup(x => x.GuardarCambios());

            var result = target.EliminarCupo(1);
            repositorioMock.Verify(x => x.Obtener<Cupo>(It.IsAny<int>()), Times.Once);
            clienteStopMock.Verify(x => x.EliminarCupo(It.IsAny<Cupo>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            eliminarCupoAgentMock.Verify(x => x.Eliminar(It.IsAny<string>()), Times.Once);
            Assert.IsFalse(result.HayError);
        }
    }
}