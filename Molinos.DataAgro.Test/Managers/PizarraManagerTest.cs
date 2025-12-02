using NLog;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class PizarraManagerTest
    {
        private PizarraManager target;
        private Mock<IRepositorio> repositorioMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            target = new PizarraManager(repositorioMock.Object);
        }

        [Test]
        public void TraerTodoPizarraOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Pizarra, PizarraDto>>>(), It.IsAny<Expression<Func<Pizarra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<PizarraDto>() { new PizarraDto { Id = 1,Codigo="a",Descripcion="a" } });

            var resultado = target.TraerTodoPizarra();

            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<Pizarra, PizarraDto>>>(), It.IsAny<Expression<Func<Pizarra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()));

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Count);
        }
    }    
}
