using Autofac.Extras.NLog;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class RG2300ManagerTest
    {
        private RG2300Manager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();

            target = new RG2300Manager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void InsetarRG2300Test()
        {
            var fecha = new DateTime(2018, 10, 26);
            var rg2300 = new List<RG2300>() { new RG2300 { Id = 1, Categoria = "A", CBU = "1", Situacion = "A", CUIT = "1", FechaActCBU = fecha, FechaGeneracion = fecha, Observaciones = "", RazonSocial = "A",FechaActRegistro=fecha, FechaLevSuspension = fecha, FechaNotExclusion = fecha, FechaPubInclusion = fecha, FechaPubSuspension = fecha } };
            
            var result = target.InsetarRG2300(rg2300);

            repositorioMock.Verify(x => x.RemoverTodos(It.IsAny<Expression<Func<RG2300, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.AgregarTodos(It.IsAny<List<RG2300>>(),null), Times.Once);
            Assert.NotNull(result);
            Assert.IsFalse(result);
        }
    }
}
