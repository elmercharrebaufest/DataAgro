using Molinos.DataAgro.Business.Procesamiento;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Moq;
using NLog;
using NUnit.Framework;

namespace Molinos.DataAgro.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorCriterioEsFijacionDePrecioContratoTest
    {
        private ProcesadorCriterioEsFijacionDePrecioContrato target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            target = new ProcesadorCriterioEsFijacionDePrecioContrato(repositorioMock.Object, logger.Object);
        }

        [Test]
        public void CalcularTest()
        {
            var criterio = new CriterioEsFijacionDePrecioContrato { Dto = new SugerenciaCupoDto { TipoNegocioId = 3 } };
            var resultado = target.Calcular(criterio);

            Assert.That(resultado, Is.Not.Null);
            Assert.AreEqual(resultado, 1);

            criterio = new CriterioEsFijacionDePrecioContrato { Dto = new SugerenciaCupoDto { TipoNegocioId = 99 } };
            resultado = target.Calcular(criterio);
            Assert.That(resultado, Is.Not.Null);
            Assert.AreEqual(resultado, 0);

        }


    }
}
