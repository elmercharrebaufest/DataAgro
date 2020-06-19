using Kendo.DynamicLinq;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using WebDataAgro.Models;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ReporteRangoConfirmacionAutomaticaControllerTest
    {
        private ReporteRangoConfirmacionAutomaticaController target;
        private Mock<IRangoConfirmacionAutomaticaManager> rangoManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            rangoManagerMock = new Mock<IRangoConfirmacionAutomaticaManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            target = new ReporteRangoConfirmacionAutomaticaController(rangoManagerMock.Object);

           
        }

        [Test]
        public void IndexTest()
        {
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void BuscaDatosTablaTest()
        {
            rangoManagerMock.Setup(x => x.TraerTodoRango(It.IsAny<DataSourceRequest>())).Returns(new DataSourceResult());
            var result = target.BuscaDatosTabla(new DataSourceRequest());
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            rangoManagerMock.Verify(x => x.TraerTodoRango(It.IsAny<DataSourceRequest>()), Times.Once);

            Assert.AreEqual(
            "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Data\":null,\"Total\":0,\"Aggregates\":null},\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }


    }
}
