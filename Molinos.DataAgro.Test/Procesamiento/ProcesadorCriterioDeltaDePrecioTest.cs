using Autofac.Extras.NLog;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Molinos.DataAgro.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorCriterioDeltaDePrecioTest
    {
        private ProcesadorCriterioDeltaDePrecio target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ITipoDeCambioAgent> tipoDeCambioMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            tipoDeCambioMock = new Mock<ITipoDeCambioAgent>();
            target = new ProcesadorCriterioDeltaDePrecio(repositorioMock.Object, new NullLogger(), tipoDeCambioMock.Object);
        }

        [Test]
        public void CalcularTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PrecioPizarra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
           .Returns(new List<PrecioPizarra>() { new PrecioPizarra { Id = 1, FechaDesde = DateTime.Now.Date, FechaHasta = DateTime.Now.Date.AddDays(1), MaterialId = 1, MonedaId = "ARP  ", PizarraId = 1, Precio = 600, UnidadMedida = "Tons" } });
            tipoDeCambioMock.Setup(y => y.TraerTipoDeCambio(null)).Returns(1);
            var criterio = new CriterioDeltaDePrecio { Dto = new SugerenciaCupoDto { Precio = 600, FechaHasta = DateTime.Now.Date, MaterialId = 1, MonedaId = "ARP  " } };
            var resultado = target.Calcular(criterio);

            Assert.That(resultado, Is.Not.Null);
            Assert.AreEqual(resultado, 0);

            criterio = new CriterioDeltaDePrecio { Dto = new SugerenciaCupoDto { Precio = 0, FechaHasta = DateTime.Now.Date, MaterialId = 1, MonedaId = "ARP  " } };
            resultado = target.Calcular(criterio);
            Assert.That(resultado, Is.Not.Null);
            Assert.AreEqual(resultado, 0);

        }


    }
}
