using Autofac.Extras.NLog;
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
    public class LogManagerTest
    {
        private LogManager target;
        private Mock<IRepositorio> repositorioMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            target = new LogManager(repositorioMock.Object);
        }

        [Test]
        public void TraerTodoLogOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Log, LogDto>>>(), It.IsAny<Expression<Func<Log, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<LogDto>() { new LogDto { Fecha= DateTime.Now, Id= 1, Xml="a"} });

            var resultado = target.TraerTodoLog(DateTime.Now);

            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<Log, LogDto>>>(), It.IsAny<Expression<Func<Log, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()));

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Count);
        }
    }    
}
