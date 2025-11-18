using NLog;
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
    public class ProcesadorCriterioEsFijacionDePrecioContratoTest
    {
        private ProcesadorCriterioEsFijacionDePrecioContrato target;
        private Mock<IRepositorio> repositorioMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            target = new ProcesadorCriterioEsFijacionDePrecioContrato(repositorioMock.Object, new NullLogger());
        }

        [Test]
        public void CalcularTest()
        {
            var criterio = new CriterioEsFijacionDePrecioContrato { Dto = new SugerenciaCupoDto {TipoNegocioId=3 } };
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
