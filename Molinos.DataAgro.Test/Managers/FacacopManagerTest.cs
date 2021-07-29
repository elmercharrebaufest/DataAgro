using Autofac.Extras.NLog;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq.Expressions;
using System.Web;
using System.Web.Script.Serialization;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class FacacopManagerTest
    {
        private FacacopManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            target = new FacacopManager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void InsetarFacacopTest()
        {
            var result = target.InsetarFacacop(new List<FACACOP>());
            repositorioMock.Verify(x => x.RemoverTodos(It.IsAny<Expression<Func<FACACOP, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.AgregarTodos(It.IsAny<List<FACACOP>>(),null), Times.Once);
            Assert.NotNull(result);
            Assert.IsTrue(result);
        }
    }
}
