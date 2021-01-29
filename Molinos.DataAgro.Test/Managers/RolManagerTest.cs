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
    public class RolManagerTest
    {
        private RolManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            target = new RolManager(repositorioMock.Object, logger.Object);
        }

        [Test]
        public void EliminarRolOk()
        {            
            var resultado = target.EliminarRol(1);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void TraerRolOk()
        {
            repositorioMock.Setup(y => y.Obtener<Rol>(It.IsAny<int>())).Returns(new Rol { Id = 1 });
            var resultado = target.TraerRol(1);
            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Id);
        }




    }
}