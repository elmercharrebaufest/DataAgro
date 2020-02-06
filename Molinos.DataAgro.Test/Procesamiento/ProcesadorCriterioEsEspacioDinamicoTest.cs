using Autofac.Extras.NLog;
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
    public class ProcesadorCriterioEsEspacioDinamicoTest
    {
        private ProcesadorCriterioEsEspacioDinamico target;
        private Mock<IRepositorio> repositorioMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            target = new ProcesadorCriterioEsEspacioDinamico(repositorioMock.Object, new NullLogger());
        }

        [Test]
        public void CalcularTest()
        {
            repositorioMock.Setup(y => y.ObtenerPrimero<TipoNegocio>(It.IsAny<Expression<Func<TipoNegocio, bool>>>()))
               .Returns(new TipoNegocio { TipoNegocioId = 1010, Descripcion = "ESPACIO DINAMICO" });
            var criterio = new CriterioEsEspacioDinamico { Dto = new SugerenciaCupoDto {TipoNegocioId=1010 } };
            var resultado = target.Calcular(criterio);
            
            Assert.That(resultado, Is.Not.Null);
            Assert.AreEqual(resultado, 1);

             criterio = new CriterioEsEspacioDinamico { Dto = new SugerenciaCupoDto { TipoNegocioId = 1 } };
             resultado = target.Calcular(criterio);
            Assert.That(resultado, Is.Not.Null);
            Assert.AreEqual(resultado, 0);

        }


    }
}
