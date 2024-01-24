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
    public class ResearchReporteControllerTests
    {

        private Mock<IResearchManager> researchManagerMock;
        private Mock<IMaterialManager> materialManagerMock;
        private Mock<ICampañaManager> campañaManagerMock;
        private Mock<IComercialManager> comercialManagerMock;
        private Mock<IProvinciaManager> provinciaManagerMock;
        private Mock<ILocalidadManager> localidadManagerMock;
        private ResearchReporteController controller;

        [SetUp]
        public void Setup()
        {
            researchManagerMock = new Mock<IResearchManager>();
            materialManagerMock = new Mock<IMaterialManager>();
            campañaManagerMock = new Mock<ICampañaManager>();
            comercialManagerMock = new Mock<IComercialManager>();
            provinciaManagerMock = new Mock<IProvinciaManager>();
            localidadManagerMock = new Mock<ILocalidadManager>();

            controller = new ResearchReporteController(
                researchManagerMock.Object,
                materialManagerMock.Object,
                campañaManagerMock.Object,
                comercialManagerMock.Object,
                provinciaManagerMock.Object,
                localidadManagerMock.Object);
        }

        [Test]
        public void Index_ReturnsViewResult()
        {
            //Arrange 
            materialManagerMock.Setup(m => m.TraerTodoMaterial()).Returns(new ResultIniMaterial { Material = new List<MaterialIni>() });

            campañaManagerMock.Setup(c => c.TraerTodoCampania()).Returns(new List<CampañaDto>());          
            comercialManagerMock.Setup(c => c.TraerTodoComercial()).Returns(new ResultIniComercial { Comercial = new List<ComercialIni>() });
            var condicionList = new List<ResearchCondicion>();
            researchManagerMock.Setup(r => r.TraerResearchCondicion()).Returns(condicionList);
            var estadioList = new List<ResearchEstadio>();
            researchManagerMock.Setup(r => r.TraerResearchEstadio()).Returns(estadioList);
            var tipoCargaList = new List<ResearchTipoCarga>();
            researchManagerMock.Setup(r => r.TraerResearchTipoCarga()).Returns(tipoCargaList);
            var tipoMuestraList = new List<ResearchTipoMuestra>();
            researchManagerMock.Setup(r => r.TraerResearchTipoMuestra()).Returns(tipoMuestraList);
            var humedadSueloList = new List<ResearchHumedadSuelo>();
            researchManagerMock.Setup(r => r.TraerResearchHumedadSuelo()).Returns(humedadSueloList);
            var provinciaList = new List<ProvinciaDto>();
            provinciaManagerMock.Setup(p => p.ListarProvincia("")).Returns(provinciaList);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ViewBag.Condicion);
            Assert.IsNotNull(result.ViewBag.Estadio);
            Assert.IsNotNull(result.ViewBag.TipoCarga);
            Assert.IsNotNull(result.ViewBag.TipoMuestra);
            Assert.IsNotNull(result.ViewBag.HumedadSuelo);
            Assert.IsNotNull(result.ViewBag.Material);
            Assert.IsNotNull(result.ViewBag.Campania);
            Assert.IsNotNull(result.ViewBag.Comercial);
            Assert.IsNotNull(result.ViewBag.Provincia);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void BuscaDatosTabla_ReturnsJsonResult()
        {
            // Arrange
            var filtro = new DataSourceRequest();
            researchManagerMock.Setup(r => r.BuscaDatosTabla(filtro)).Returns(new DataSourceResult());

            // Act
            var result = controller.BuscaDatosTabla(filtro) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsInstanceOf<DataSourceResult>(result.Data);
        }

        [Test]
        public void Export_ReturnsJsonResult()
        {
            // Arrange
            var filtro = new DataSourceRequest();
            // Mockear la llamada a researchManager.TraerContratosFiltrados según sea necesario

            // Act
            var result = controller.Export(filtro) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            // Ajusta según la estructura y tipo de datos esperados
        }

        [Test]
        public void BorrarRegistro_ReturnsJsonResult()
        {
            // Arrange
            int id = 1;
            researchManagerMock.Setup(r => r.BorrarResearch(id)).Returns(new Resultado());

            // Act
            var result = controller.BorrarRegistro(id) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsInstanceOf<Resultado>(result.Data);
        }
    }
}
