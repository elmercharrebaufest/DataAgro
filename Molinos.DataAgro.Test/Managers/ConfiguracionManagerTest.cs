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
    public class ConfiguracionManagerTest
    {
        private ConfiguracionManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<IComercialManager> comercialManagerMock;

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();

            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            target = new ConfiguracionManager(repositorioMock.Object, logger.Object);
        }
        [Test]
        public void GrabarNuevaConfiguracionCupoTest()
        {
            var config = new Configuracion
            {
                Id = 1,
                CantidadDias = 1,
                ClaveStop = "a",
                CodigoLocalidadStop = 1,
                ConexionABMStop = true,
                ConexionConsultaStop = true,
                CuitDestinoStop = "a",
                TerminalStopId = 1
            };
            repositorioMock.Setup(x => x.Obtener<Configuracion>(It.IsAny<int>())).Returns(config);
            var resultado = target.GrabarFechaPesificacionDolarizado(config);

            Assert.That(!resultado.HayError);
            repositorioMock.Verify(x => x.Obtener<Configuracion>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

    }
}