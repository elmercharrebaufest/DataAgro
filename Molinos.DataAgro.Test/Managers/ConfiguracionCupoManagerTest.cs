using Autofac.Extras.NLog;
using KendoGridBinder;
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
    public class ConfiguracionCupoManagerTest
    {
        private ConfiguracionCupoManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<IComercialManager> comercialManagerMock;

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();

            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            target = new ConfiguracionCupoManager(logger.Object, repositorioMock.Object);
        }
        [Test]
        public void GrabarNuevaConfiguracionCupoTest()
        {
            var cupo = new ConfiguracionCupo
            {
                Id = 0,
                CentroId = 1,
                MaterialId = 1,
                Fecha = new DateTime(2019, 8, 1),
                LimiteCupo = 10
            };
            repositorioMock.Setup(x => x.Existe(It.IsAny<Expression<Func<ConfiguracionCupo, bool>>>())).Returns(false);
            repositorioMock.Setup(x => x.Agregar(It.IsAny<ConfiguracionCupo>()));
            var resultado = target.GrabarConfiguracionCupo(cupo);

            Assert.That(!resultado.HayError);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<ConfiguracionCupo>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
        [Test]
        public void UpdateConfiguracionCupoTest()
        {
            var cupo = new ConfiguracionCupo
            {
                Id = 1,
                CentroId = 1,
                MaterialId = 1,
                Fecha = new DateTime(2019, 8, 1),
                LimiteCupo = 100
            };
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<LimiteCupo, LimiteCupoDto>>>(), It.IsAny<Expression<Func<LimiteCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<LimiteCupoDto>() { new LimiteCupoDto { Id = 1, CantidadCupo = 50 } });
            repositorioMock.Setup(x => x.Obtener<ConfiguracionCupo>(It.IsAny<int>())).Returns(cupo);
            var resultado = target.GrabarConfiguracionCupo(cupo);

            Assert.That(!resultado.HayError);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<LimiteCupo, LimiteCupoDto>>>(), It.IsAny<Expression<Func<LimiteCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
        [Test]
        public void TraerLimitesTest()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<LimiteCupo, LimiteCupoDto>>>(), It.IsAny<Expression<Func<LimiteCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<LimiteCupoDto>() { new LimiteCupoDto { Id = 1, CantidadCupo = 1, ZonaCupoId = 1 } });
            
            var resultado = target.TraerLimites(1);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<LimiteCupo, LimiteCupoDto>>>(), It.IsAny<Expression<Func<LimiteCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.AreEqual(1, resultado.Count);
        }
        [Test]
        public void GrabarLimitesTest()
        {
            var limites = new List<LimiteCupo>()
            {
                new LimiteCupo{ Id = 0, ZonaCupoId = 1 , ConfiguracionCupoId = 1, CantidadCupo = 10 },
                new LimiteCupo{ Id = 1, ZonaCupoId = 2 , ConfiguracionCupoId = 1, CantidadCupo = 10 }
            };
            repositorioMock.Setup(x => x.Obtener<ConfiguracionCupo>(It.IsAny<int>()))
                .Returns(new ConfiguracionCupo { Id = 1, LimiteCupo = 100 } );
            repositorioMock.Setup(x => x.Agregar(It.IsAny<LimiteCupo>()));
            repositorioMock.Setup(x => x.Obtener<LimiteCupo>(It.IsAny<int>()))
                .Returns(new LimiteCupo { Id = 1, ZonaCupoId = 2, ConfiguracionCupoId = 1, CantidadCupo = 10 });
            repositorioMock.Setup(x => x.GuardarCambios());
            var resultado = target.GrabarLimites(limites);

            repositorioMock.Verify(x => x.Obtener<ConfiguracionCupo>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<LimiteCupo>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<LimiteCupo>(It.IsAny<int>()), Times.Once);
            Assert.That(!resultado.HayError);
        }
        [Test]
        public void TraerConfiguracionCupoTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<ConfiguracionCupo, bool>>>(), It.IsAny<Expression<Func<ConfiguracionCupo, ConfiguracionCupoDto>>>()))
                .Returns(new ConfiguracionCupoDto { Id = 1, LimiteCupo = 100 });
            var resultado = target.TraerConfiguracionCupo(1);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<ConfiguracionCupo, bool>>>(), It.IsAny<Expression<Func<ConfiguracionCupo, ConfiguracionCupoDto>>>()), Times.Once);
            Assert.AreEqual(1, resultado.Id);
        }
    }
}