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
    public class DiferencialManagerTest
    {
        private DiferencialManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<IHedgeManager> hedgeManagerMock;
        private Mock<IMailManager> mailManagerManagerMock;
        private Mock<IReportesManager> reportesManagerManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            hedgeManagerMock = new Mock<IHedgeManager>();
            reportesManagerManagerMock = new Mock<IReportesManager>();
            mailManagerManagerMock = new Mock<IMailManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new DiferencialManager(logger.Object, repositorioMock.Object, mailManagerManagerMock.Object, reportesManagerManagerMock.Object, hedgeManagerMock.Object);
        }

        [Test]
        public void TraerDiferencialTestOk()
        {
            //repositorioMock.Setup(y => y.ObtenerMayor(It.IsAny<Expression<Func<Diferencial, int, DiferencialDto>>>(),It.IsAny<Expression<Func<Diferencial, bool>>>(), It.IsAny<Expression<Func<Diferencial, int>>>(), It.IsAny<Expression<Func<Diferencial, DiferencialDto>>>()))
            //    .Returns(new DiferencialDto());

        }
    }
}
