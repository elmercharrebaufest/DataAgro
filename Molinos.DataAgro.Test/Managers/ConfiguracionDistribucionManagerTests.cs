using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Dto.Distribucion;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Moq;
using NLog;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Molinos.DataAgro.Test.Managers
{
    [TestFixture]
    public class ConfiguracionDistribucionManagerTests
    {
        private ConfiguracionDistribucionManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> loggerMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            loggerMock = new Mock<ILogger>();
            target = new ConfiguracionDistribucionManager(repositorioMock.Object, loggerMock.Object);
        }

        [Test]
        public void ObtenerConfiguracion_SinRegistro_RetornaDefaults()
        {
            repositorioMock.Setup(x => x.ObtenerPrimero(It.IsAny<Expression<Func<ConfiguracionesDistribucionPlanta, bool>>>())).Returns((ConfiguracionesDistribucionPlanta)null);

            var resultado = target.ObtenerConfiguracion();

            Assert.That(resultado.CupoKg, Is.EqualTo(30000));
            Assert.That(resultado.CuitMaxPct, Is.EqualTo(0.30m));
            Assert.That(resultado.CosechasValidas, Is.EquivalentTo(new[] { "23-24", "24-25", "25-26" }));
        }

        [Test]
        public void GuardarConfiguracion_CuitMaxPctInvalido_LanzaArgumentException()
        {
            var dto = new ConfiguracionDistribucionDto { CuitMaxPct = 1.5m };
            var ex = Assert.Throws<ArgumentException>(() => target.GuardarConfiguracion(dto));
            Assert.That(ex.Message, Does.Contain("cuitMaxPct"));
        }

        [Test]
        public void GuardarConfiguracion_CuotasOperadorSuperanCien_LanzaArgumentException()
        {
            var dto = new ConfiguracionDistribucionDto
            {
                CuitMaxPct = 0.30m,
                CuotasPorOperador = new Dictionary<string, int>
                {
                    { "acopiador", 60 },
                    { "productor", 50 }
                }
            };

            var ex = Assert.Throws<ArgumentException>(() => target.GuardarConfiguracion(dto));
            Assert.That(ex.Message, Does.Contain("operador"));
        }

        [Test]
        public void GuardarConfiguracion_ActualizaUltimaActualizacionYPersiste()
        {
            var entity = new ConfiguracionesDistribucionPlanta { Id = 1, PlantaCodigo = "SL" };
            repositorioMock.Setup(x => x.ObtenerPrimero(It.IsAny<Expression<Func<ConfiguracionesDistribucionPlanta, bool>>>())).Returns(entity);
            var dto = new ConfiguracionDistribucionDto
            {
                PlantaCodigo = "SL",
                PlantaNombre = "Planta San Lorenzo",
                CupoKg = 30000,
                CuitMaxPct = 0.30m,
                CosechasValidas = new List<string> { "23-24", "24-25", "25-26" },
                CuotasPorOperador = new Dictionary<string, int> { { "acopiador", 40 } },
                CuotasPorClase = new Dictionary<string, int> { { "Fijo", 50 } }
            };

            target.GuardarConfiguracion(dto);

            Assert.That(entity.UltimaActualizacion, Is.Not.Null);
            Assert.That(entity.CosechasValidas, Does.Contain("23-24"));
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
    }
}
