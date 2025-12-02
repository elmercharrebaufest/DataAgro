using Molinos.DataAgro.Business;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Moq;
using NLog;
using NUnit.Framework;
using System;

namespace Molinos.DataAgro.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorCriterioFechaDeEntregaTest
    {
        private ProcesadorCriterioFechaDeEntrega target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            target = new ProcesadorCriterioFechaDeEntrega(repositorioMock.Object, logger.Object);
        }

        [Test]
        public void CalcularTest()
        {
            var criterio = new CriterioFechaDeEntrega { Dto = new SugerenciaCupoDto { FechaHasta = DateTime.Now.Date } };
            var resultado = target.Calcular(criterio);

            Assert.That(resultado, Is.Not.Null);
            Assert.AreEqual(resultado, 1);

            criterio = new CriterioFechaDeEntrega { Dto = new SugerenciaCupoDto { FechaHasta = DateTime.Now.Date.AddDays(2) } };
            resultado = target.Calcular(criterio);
            Assert.That(resultado, Is.Not.Null);
            Assert.AreEqual(resultado, 0.5);

        }


    }
}
