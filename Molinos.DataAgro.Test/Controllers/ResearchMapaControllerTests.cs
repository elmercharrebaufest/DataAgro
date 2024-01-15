using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.DynamicLinq;
using Moq;
using WebDataAgro.Controllers;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ResearchMapaControllerTests
    {

        private Mock<ICampañaManager> campañaManagerMock;
        private Mock<IMaterialManager> materialManagerMock;
        private ResearchMapaController controller;

        [SetUp]
        public void Setup()
        {
            campañaManagerMock = new Mock<ICampañaManager>();
            materialManagerMock = new Mock<IMaterialManager>();

            controller = new ResearchMapaController(
                campañaManagerMock.Object,
                materialManagerMock.Object);
        }

        [Test]
        public void Index_ReturnsViewResult()
        {
            //Arrange 
            materialManagerMock.Setup(m => m.TraerTodoMaterial()).Returns(new ResultIniMaterial { Material = new List<MaterialIni>() });

            campañaManagerMock.Setup(c => c.TraerTodoCampania()).Returns(new List<CampañaDto>());

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ViewBag.Material);
            Assert.IsNotNull(result.ViewBag.Campania);
            Assert.IsInstanceOf<ViewResult>(result);
        }
    }
}
