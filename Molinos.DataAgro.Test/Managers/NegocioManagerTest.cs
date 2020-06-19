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
    public class NegocioManagerTest
    {
        private NegocioManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<IMailManager> mailManagerMock;

        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            mailManagerMock = new Mock<IMailManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
           
            target = new NegocioManager(logger.Object, repositorioMock.Object, mailManagerMock.Object);
        }

        [Test]
        public void OcultarEnTableroOk()
        {
          
            var negocio = new Negocio()
            {
                Id = 1,
               OcultarEnTablero=true
            };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Negocio, bool>>>())).Returns(negocio);
            
            var resultado = target.OcultarEnTablero(negocio);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

        }

    }

}
