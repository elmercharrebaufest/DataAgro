using NUnit.Framework;
using System.Collections.Generic;
using System.Web.Mvc;
using Moq;
using WebDataAgro.Controllers;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Dto;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ResearchMapaControllerTests
    {
        private Mock<ICampañaManager> campañaManagerMock;
        private Mock<IMaterialManager> materialManagerMock;
        private Mock<IResearchManager> researchManagerMock;
        private ResearchMapaController controller;

        [SetUp]
        public void Setup()
        {
            campañaManagerMock = new Mock<ICampañaManager>();
            materialManagerMock = new Mock<IMaterialManager>();
            researchManagerMock = new Mock<IResearchManager>();

            controller = new ResearchMapaController(campañaManagerMock.Object, materialManagerMock.Object, researchManagerMock.Object);
        }

        [Test]
        public void Index_ReturnsViewResult()
        {
            materialManagerMock.Setup(m => m.TraerTodoMaterial()).Returns(new ResultIniMaterial { Material = new List<MaterialIni>() });
            campañaManagerMock.Setup(c => c.TraerTodoCampania()).Returns(new List<CampañaDto>());
            researchManagerMock.Setup(c => c.TraerResearchId()).Returns(new List<int> { 1 });

            var result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ViewBag.Material);
            Assert.IsNotNull(result.ViewBag.Campania);
            Assert.IsNotNull(result.ViewBag.ResearchId);
            Assert.IsInstanceOf<ViewResult>(result);
        }
    }
}
