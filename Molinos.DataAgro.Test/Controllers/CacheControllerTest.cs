using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using static WebDataAgro.MvcApplication;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class CacheControllerTest
    {
        private CacheController target;
        private Mock<ILogger> loggerMock;
        private Mock<ICache> cacheMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            serializer = new JavaScriptSerializer();
            loggerMock = new Mock<ILogger>();
            cacheMock = new Mock<ICache>();

            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new CacheController(loggerMock.Object, cacheMock.Object);
        }

        [Test]
        public void IndexTest()
        {
            cacheMock.Setup(x => x.ListAllCacheItems()).Returns(new List<KeyValuePair<string, string>>());

            var result = target.Index() as ViewResult;
            Assert.NotNull(result);
            cacheMock.Verify(x => x.ListAllCacheItems(), Times.Once);

        }
    }
}
