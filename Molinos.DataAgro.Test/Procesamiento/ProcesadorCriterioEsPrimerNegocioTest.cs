using NLog;
using Molinos.DataAgro.Business.Procesamiento;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Molinos.DataAgro.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorCriterioEsPrimerNegocioTest
    {
        private ProcesadorCriterioEsPrimerNegocio target;
        private Mock<IRepositorio> repositorioMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            target = new ProcesadorCriterioEsPrimerNegocio(repositorioMock.Object, new NullLogger());
        }

        [Test]
        public void CalcularTest()
        {
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, bool>>>())).Returns(true);

            var criterio = new CriterioEsPrimerNegocio { Dto = new SugerenciaCupoDto { ProveedorId = 1 } };
            var resultado = target.Calcular(criterio);

            Assert.That(resultado, Is.Not.Null);
            Assert.AreEqual(resultado, 0);
        }

        [Test]
        public void CalcularNoExisteTest()
        {
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<CampañaMaterial, bool>>>())).Returns(false);


            var criterio = new CriterioEsPrimerNegocio { Dto = new SugerenciaCupoDto { ProveedorId = 1 } };
            var resultado = target.Calcular(criterio);

            Assert.That(resultado, Is.Not.Null);
            Assert.AreEqual(resultado, 1);


        }

    }
}
