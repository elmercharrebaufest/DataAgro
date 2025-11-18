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
using System.Linq.Expressions;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class CupoNoPropioManagerTest
    {
        private CupoNoPropioManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<IConfiguracionCupoManager> configuracionCupoMangerMock;
        private Mock<ICentroManager> centroMangerMock;

        [SetUp]
        public void Setup()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            configuracionCupoMangerMock = new Mock<IConfiguracionCupoManager>();
            centroMangerMock = new Mock<ICentroManager>();

            target = new CupoNoPropioManager(repositorioMock.Object, logger.Object, configuracionCupoMangerMock.Object, centroMangerMock.Object);
            repositorioMock.Setup(x => x.Obtener<Configuracion>(1)).Returns(new Configuracion { ConexionABMStop = true });
        }

        [Test]
        public void GrabarDisponibilidadCupoNoPropioOk()
        {
            repositorioMock.Setup(x => x.Obtener<CupoNoPropio>(It.IsAny<int>()))
                .Returns(new CupoNoPropio { Id = 1 });
            var result = target.GrabarDisponibilidadCupoNoPropio(1, true);
            repositorioMock.Verify(x => x.Obtener<CupoNoPropio>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }

        [Test]
        public void GrabarCupoNoPropio()
        {
            var cupoDto = new CupoDto
            {
                MaterialId = 1,
                Centro = "1029",
                FechaIngreso = DateTime.Now,
                Codigo = "MOA/2022"
            };
            var configuracion = new ConfiguracionCupo { Id = 1, LimiteCupo = 10, LimiteAlgoritmo = 0, Fecha = DateTime.Now, CentroId = 1, MaterialId = 1 };
            var dia = new List<DiaCupo>() { new DiaCupo { Cantidad = 1, Fecha = DateTime.Now } };
            centroMangerMock.Setup(x => x.ObtenerCentroPorCodigoSap(It.IsAny<string>())).Returns(new CentroDto { Id = 1 });
            repositorioMock.Setup(x => x.ListarEntidadMasiva<CupoNoPropio>(It.IsAny<string>(), It.IsAny<List<string>>())).Returns(new List<CupoNoPropio>());
            repositorioMock.Setup(x => x.AgregarTodos(It.IsAny<List<CupoNoPropio>>(), null)).Verifiable();
            repositorioMock.Setup(x => x.Obtener<ConfiguracionCupo>(It.IsAny<int>()))
            .Returns(configuracion);
            configuracionCupoMangerMock.Setup(x => x.GrabarConfiguracionCupo(configuracion, dia)).Returns(new Resultado());
            var result = target.GrabarCupoNoPropio(cupoDto);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }

        [Test]
        public void ModificacionMasivaDisponibleOk()
        {
            var ids = new List<int>() { 1 };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CupoNoPropio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(),
                It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<CupoNoPropio>() { new CupoNoPropio { Disponible = true } });
            var result = target.ModificacionMasivaDisponible(ids, true);
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<CupoNoPropio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(),
                It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
    }
}
