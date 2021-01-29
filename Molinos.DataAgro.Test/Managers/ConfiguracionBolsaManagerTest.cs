using Autofac.Extras.NLog;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
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
    public class ConfiguracionBolsaManagerTest
    {
        private ConfiguracionBolsaManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new ConfiguracionBolsaManager(logger.Object, repositorioMock.Object);
        }
        [Test]
        public void GrabarNuevaConfiguracionBolsaTest()
        {
            var bolsa = new ConfiguracionBolsa
            {
                Id = 0,
                DestinoId = 1,
                BolsaId = 1,
                ProvinciaId = 1
            };
            var resultado = target.GrabarConfiguracionBolsa(bolsa);
            Assert.That(!resultado.HayError);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<ConfiguracionBolsa>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
        [Test]
        public void ActualizarConfiguracionBolsaTest()
        {
            var bolsa = new ConfiguracionBolsa
            {
                Id = 1,
                DestinoId = 1,
                BolsaId = 1,
                ProvinciaId = 1
            };
            repositorioMock.Setup(x => x.Obtener<ConfiguracionBolsa>(It.IsAny<int>())).Returns(bolsa);
            var resultado = target.GrabarConfiguracionBolsa(bolsa);
            Assert.That(!resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void TraerConfiguracionCupoTest()
        {

            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<ConfiguracionBolsa, bool>>>(), It.IsAny<Expression<Func<ConfiguracionBolsa, ConfiguracionBolsaDto>>>()))
                .Returns(new ConfiguracionBolsaDto
                {
                    Id = 1,
                    DestinoId = 1,
                    BolsaId = 1,
                    ProvinciaId = 1
                });
            var resultado = target.TraerConfiguracionBolsa(1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<ConfiguracionBolsa, bool>>>(), It.IsAny<Expression<Func<ConfiguracionBolsa, ConfiguracionBolsaDto>>>()), Times.Once);
            Assert.AreEqual(1, resultado.Id);
        }

        [Test]
        public void TraerConfiguracionBolsaConDestinoYProcedenciaTest()
        {

            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<ConfiguracionBolsa, bool>>>()))
                .Returns(new ConfiguracionBolsa
                {
                    Id = 1,
                    DestinoId = 1,
                    BolsaId = 1,
                    ProvinciaId = 1
                });
            var resultado = target.TraerConfiguracionBolsaConDestinoYProcedencia(1, 1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<ConfiguracionBolsa, bool>>>()), Times.Once);
            Assert.AreEqual(1, resultado.Id);
        }

        [Test]
        public void TraerTodaConfiguracionBolsaTest()
        {
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<TraerConfiguracionesBolsa>()))
             .Returns(new KendoGrid<ConfiguracionBolsaDto>(new List<ConfiguracionBolsaDto> { new ConfiguracionBolsaDto { Id = 1, ProvinciaId = 1, BolsaId = 1, DestinoId = 1 } }, 1));
            var resultado = target.TraerTodaConfiguracionBolsa(It.IsAny<KendoGridMvcRequest>());
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

    }
}