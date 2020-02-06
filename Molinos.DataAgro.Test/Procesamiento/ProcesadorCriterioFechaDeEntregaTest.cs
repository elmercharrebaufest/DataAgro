using Autofac.Extras.NLog;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Procesamiento;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorCriterioFechaDeEntregaTest
    {
        private ProcesadorCriterioFechaDeEntrega target;
        private Mock<IRepositorio> repositorioMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            target = new ProcesadorCriterioFechaDeEntrega(repositorioMock.Object, new NullLogger());
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
