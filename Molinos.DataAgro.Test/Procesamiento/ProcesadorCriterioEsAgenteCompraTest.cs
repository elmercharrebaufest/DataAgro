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
    public class ProcesadorCriterioEsAgenteCompraTest
    {
        private ProcesadorCriterioEsAgenteCompra target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;


        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            target = new ProcesadorCriterioEsAgenteCompra(repositorioMock.Object, logger.Object);
        }

        [Test]
        public void CalcularTest()
        {
            var criterio = new CriterioEsAgenteCompra { Dto = new SugerenciaCupoDto { TipoAgenteCompraId = 1 } };
            var resultado = target.Calcular(criterio);

            Assert.That(resultado, Is.Not.Null);
            Assert.AreEqual(resultado, 1);

            criterio = new CriterioEsAgenteCompra { Dto = new SugerenciaCupoDto { TipoAgenteCompraId = null } };
            resultado = target.Calcular(criterio);

            Assert.That(resultado, Is.Not.Null);
            Assert.AreEqual(resultado, 0);
        }
    }
}