using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using WebDataAgro.Models;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ResearchControllerTest
    {
        private ResearchController target;
        private Mock<IResearchManager> researchManagerMock;
        private Mock<IMaterialManager> materialManagerMock;
        private Mock<IEstadioManager> estadioManagerMock;
        private Mock<ICampañaManager> campañaManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            materialManagerMock = new Mock<IMaterialManager>();
            researchManagerMock = new Mock<IResearchManager>();
            estadioManagerMock = new Mock<IEstadioManager>();
            campañaManagerMock = new Mock<ICampañaManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new ResearchController(researchManagerMock.Object, materialManagerMock.Object, estadioManagerMock.Object, campañaManagerMock.Object);
            HttpContext.Current.Session["comercialId"] = 1;
        }
        [Test]
        public void IndexOk()
        {
            var result = target.Index() as ViewResult;
            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
        [Test]
        public void AvanceSiembraPartialOk()
        {
            materialManagerMock.Setup(x => x.TraerTodoMaterial()).Returns(new ResultIniMaterial
            {
                Material = new List<MaterialIni>
                 {
                     new MaterialIni { Codigo = "a", Descripcion = "a", MaterialId = 1, CampaniaIdActual = 1}
                 }
            });

            researchManagerMock.Setup(x => x.TraerTodoResearchAvanceSiembra()).Returns(new List<ResearchAvanceSiembraDto> {
                new ResearchAvanceSiembraDto {Id = 1, Avance = 1, CambioAA = 1, Comercial = "", FechaHora = new DateTime(2018, 10, 26), IntencionSiembra = 1, LocalidadId = 1, MaterialId = 1, Observaciones = "" }
            });

            var result = target.AvanceSiembraPartial() as PartialViewResult;

            Assert.IsNotNull(result);

            researchManagerMock.Verify(x => x.TraerTodoResearchAvanceSiembra(), Times.Once);
            materialManagerMock.Verify(x => x.TraerTodoMaterial(), Times.Once);

            Assert.AreEqual("_AvanceSiembra", result.ViewName);
            Assert.IsInstanceOf<ResearchAvanceSiembraModel>(result.Model);
        }
        [Test]
        public void AvanceCosechaPartialOk()
        {
            materialManagerMock.Setup(x => x.TraerTodoMaterial()).Returns(new ResultIniMaterial
            {
                Material = new List<MaterialIni>
                 {
                     new MaterialIni { Codigo = " ", Descripcion = " ", MaterialId = 1, CampaniaIdActual = 1}
                 }
            });


            researchManagerMock.Setup(x => x.TraerTodoResearchAvanceCosecha()).Returns(new List<ResearchAvanceCosechaDto> {
                new ResearchAvanceCosechaDto {Id = 1, Avance = 1, Comercial = "", FechaHora = new DateTime(2018, 10, 26), LocalidadId = 1, MaterialId = 1, Observaciones = "", RangoDesde = 1, RangoHasta = 2, Rendimiento = 67}
            });

            var result = target.AvanceCosechaPartial() as PartialViewResult;

            Assert.IsNotNull(result);

            researchManagerMock.Verify(x => x.TraerTodoResearchAvanceCosecha(), Times.Once);
            materialManagerMock.Verify(x => x.TraerTodoMaterial(), Times.Once);

            Assert.AreEqual("_AvanceCosecha", result.ViewName);
            Assert.IsInstanceOf<ResearchAvanceCosechaModel>(result.Model);
        }

        [Test]
         public void SituacionCultivoPartialOk()
        {
            materialManagerMock.Setup(x => x.TraerTodoMaterial()).Returns(new ResultIniMaterial
            {
                Material = new List<MaterialIni>
                 {
                     new MaterialIni { Codigo = " ", Descripcion = " ", MaterialId = 1, CampaniaIdActual = 1}
                 }
            });


            researchManagerMock.Setup(x => x.TraerTodoResearchSituacionCultivo()).Returns(new List<ResearchSituacionCultivoDto> {
                new ResearchSituacionCultivoDto {Id = 1, Comercial = "", FechaHora = new DateTime(2018, 10, 26), LocalidadId = 1, MaterialId = 1, Observaciones = "", EstadioId = 1, Situacion = ""}
            });

            var result = target.SituacionCultivoPartial() as PartialViewResult;

            Assert.IsNotNull(result);

            researchManagerMock.Verify(x => x.TraerTodoResearchSituacionCultivo(), Times.Once);
            materialManagerMock.Verify(x => x.TraerTodoMaterial(), Times.Once);

            Assert.AreEqual("_SituacionCultivo", result.ViewName);
            Assert.IsInstanceOf<ResearchSituacionCultivoModel>(result.Model);
        }

        [Test]
        public void VentaStockPartial() {

            materialManagerMock.Setup(x => x.TraerTodoMaterial()).Returns(new ResultIniMaterial
            {
                Material = new List<MaterialIni>
                 {
                     new MaterialIni { Codigo = " ", Descripcion = " ", MaterialId = 1, CampaniaIdActual = 1}
                 }
            });

            researchManagerMock.Setup(x => x.TraerTodoResearchVentaStock()).Returns(new List<ResearchVentaStockDto> {
                new ResearchVentaStockDto {Id = 1, Comercial = "", FechaHora = new DateTime(2018, 10, 26), LocalidadId = 1, MaterialId = 1, Observaciones = ""}
            });

            var result = target.VentaStockPartial() as PartialViewResult;

            Assert.NotNull(result);

            researchManagerMock.Verify(x => x.TraerTodoResearchVentaStock(), Times.Once);
            materialManagerMock.Verify(x => x.TraerTodoMaterial(), Times.Once);

            Assert.AreEqual("_VentaStock", result.ViewName);
            Assert.IsInstanceOf<ResearchVentaStockModel>(result.Model);

        }

        [Test]
        public void GrabarAvanceSiembraOk()
        {
            var researchModel = new ResearchAvanceSiembraModel();            

            researchManagerMock.Setup(x => x.GrabarResearchAvanceSiembra(It.IsAny<ResearchAvanceSiembra>(), 1)).Returns(new Resultado());
            researchManagerMock.Setup(x => x.TraerTodoResearchAvanceSiembra()).Returns(new List<ResearchAvanceSiembraDto> {
                new ResearchAvanceSiembraDto {Id = 1, Avance = 1, Comercial = "", FechaHora = new DateTime(2018, 10, 26), LocalidadId = 1, MaterialId = 1, Observaciones = ""}
            });

            var result = target.GrabarAvanceSiembra(researchModel) as PartialViewResult;
            Assert.NotNull(result);

            researchManagerMock.Verify(x => x.GrabarResearchAvanceSiembra(It.IsAny<ResearchAvanceSiembra>(), 1), Times.Once);
            researchManagerMock.Verify(x => x.TraerTodoResearchAvanceSiembra(), Times.Once);

            Assert.AreEqual("_ListaAvanceSiembra", result.ViewName);
            Assert.IsInstanceOf<ResearchAvanceSiembraModel>(result.Model);

        }

        [Test]
        public void GrabarAvanceCosechaOk()
        {
            var researchCosecha = new ResearchAvanceCosechaModel();
            researchManagerMock.Setup(x => x.GrabarResearchAvanceCosecha(It.IsAny<ResearchAvanceCosecha>(), 1)).Returns(new Resultado());
            researchManagerMock.Setup(x => x.TraerTodoResearchAvanceCosecha()).Returns(new List<ResearchAvanceCosechaDto> {
                new ResearchAvanceCosechaDto {Id = 1, Avance = 1, Comercial = "", FechaHora = new DateTime(2018, 10, 26), LocalidadId = 1, MaterialId = 1, Observaciones = "", RangoDesde = 1, RangoHasta = 2, Rendimiento = 67}
            });

            var result = target.GrabarAvanceCosecha(researchCosecha) as PartialViewResult;

            Assert.NotNull(result);

            researchManagerMock.Verify(x => x.GrabarResearchAvanceCosecha(It.IsAny<ResearchAvanceCosecha>(), 1), Times.Once);
            researchManagerMock.Verify(x => x.TraerTodoResearchAvanceCosecha(), Times.Once);
            Assert.AreEqual("_ListaAvanceCosecha", result.ViewName);
            Assert.IsInstanceOf<ResearchAvanceCosechaModel>(result.Model);
        }

        [Test]
        public void GrabarSituacionCultivoOk()
        {
            var researchSituacionCultivo = new ResearchSituacionCultivoModel();
            researchManagerMock.Setup(x => x.GrabarResearchSituacionCultivo(It.IsAny<ResearchSituacionCultivo>(), 1)).Returns(new Resultado());
            researchManagerMock.Setup(x => x.TraerTodoResearchSituacionCultivo()).Returns(new List<ResearchSituacionCultivoDto> {
                new ResearchSituacionCultivoDto {Id = 1, Comercial = "", FechaHora = new DateTime(2018, 10, 26), LocalidadId = 1, MaterialId = 1, Observaciones = ""}
            });

            var result = target.GrabarSituacionCultivo(researchSituacionCultivo) as PartialViewResult;

            Assert.NotNull(result);

            researchManagerMock.Verify(x => x.GrabarResearchSituacionCultivo(It.IsAny<ResearchSituacionCultivo>(), 1), Times.Once);
            researchManagerMock.Verify(x => x.TraerTodoResearchSituacionCultivo(), Times.Once);

            Assert.AreEqual("_ListaSituacionCultivo", result.ViewName);
            Assert.IsInstanceOf<ResearchSituacionCultivoModel>(result.Model);
        }

        [Test]
        public void GrabarVentaStockOk()
        {
            var ventaStock = new ResearchVentaStockModel();

            researchManagerMock.Setup(x => x.GrabarResearchVentaStock(It.IsAny<ResearchVentaStock>(), 1)).Returns(new Resultado());
            researchManagerMock.Setup(x => x.TraerTodoResearchVentaStock()).Returns(new List<ResearchVentaStockDto> {
                new ResearchVentaStockDto {Id = 1, Comercial = "", FechaHora = new DateTime(2018, 10, 26), LocalidadId = 1, MaterialId = 1, Observaciones = ""}
            });

            var result = target.GrabarVentaStock(ventaStock) as PartialViewResult;

            Assert.NotNull(result);

            researchManagerMock.Verify(x => x.GrabarResearchVentaStock(It.IsAny<ResearchVentaStock>(), 1), Times.Once);
            researchManagerMock.Verify(x => x.TraerTodoResearchVentaStock(), Times.Once);

            Assert.AreEqual("_ListaVentaStock", result.ViewName);
            Assert.IsInstanceOf<ResearchVentaStockModel>(result.Model);
        }

        [Test]
        public void EliminarAvanceSiembraOk()
        {
            researchManagerMock.Setup(x => x.TraerTodoResearchAvanceSiembra()).Returns(new List<ResearchAvanceSiembraDto> {
                new ResearchAvanceSiembraDto {Id = 1, Avance = 1, CambioAA = 1, Comercial = "", FechaHora = new DateTime(2018, 10, 26), IntencionSiembra = 1, LocalidadId = 1, MaterialId = 1, Observaciones = "" }
            });

            researchManagerMock.Setup(x => x.EliminarResearchAvanceCosecha(1)).Returns(new Resultado());

            var result = target.EliminarAvanceSiembra(1) as PartialViewResult;
            Assert.NotNull(result);

            researchManagerMock.Verify(x => x.TraerTodoResearchAvanceSiembra(), Times.Once);
            researchManagerMock.Verify(x => x.EliminarResearchAvanceSiembra(1), Times.Once);

            Assert.AreEqual("_ListaAvanceSiembra", result.ViewName);
            Assert.IsInstanceOf<ResearchAvanceSiembraModel>(result.Model);
        }

        [Test]
        public void EliminarAvanceCosechaOk()
        {
            
            researchManagerMock.Setup(x => x.EliminarResearchAvanceCosecha(1)).Returns(new Resultado());
            researchManagerMock.Setup(x => x.TraerTodoResearchAvanceCosecha()).Returns(new List<ResearchAvanceCosechaDto> {
                new ResearchAvanceCosechaDto {Id = 1, Avance = 1, Comercial = "", FechaHora = new DateTime(2018, 10, 26), LocalidadId = 1, MaterialId = 1, Observaciones = "", RangoDesde = 1, RangoHasta = 2, Rendimiento = 67}
            });

            var result = target.EliminarAvanceCosecha(1) as PartialViewResult;

            Assert.NotNull(result);

            researchManagerMock.Verify(x => x.TraerTodoResearchAvanceCosecha(), Times.Once);
            researchManagerMock.Verify(x => x.EliminarResearchAvanceCosecha(1), Times.Once);

            Assert.AreEqual("_ListaAvanceCosecha", result.ViewName);
            Assert.IsInstanceOf<ResearchAvanceCosechaModel>(result.Model);
        }
        [Test]
        public void EliminarSituacionCultivoOk()
        {
            
            researchManagerMock.Setup(x => x.EliminarResearchSituacionCultivo(1)).Returns(new Resultado());
            researchManagerMock.Setup(x => x.TraerTodoResearchSituacionCultivo()).Returns(new List<ResearchSituacionCultivoDto> {
                new ResearchSituacionCultivoDto {Id = 1, Comercial = "", FechaHora = new DateTime(2018, 10, 26), LocalidadId = 1, MaterialId = 1, Observaciones = ""}
            });

            var result = target.EliminarSituacionCultivo(1) as PartialViewResult;

            Assert.NotNull(result);

            researchManagerMock.Verify(x => x.EliminarResearchSituacionCultivo(1), Times.Once);
            researchManagerMock.Verify(x => x.TraerTodoResearchSituacionCultivo(), Times.Once);

            Assert.AreEqual("_ListaSituacionCultivo", result.ViewName);
            Assert.IsInstanceOf<ResearchSituacionCultivoModel>(result.Model);
        }
        [Test]
        public void EliminarVentaStock()
        {
            researchManagerMock.Setup(x => x.EliminarResearchVentaStock(1)).Returns(new Resultado());
            researchManagerMock.Setup(x => x.TraerTodoResearchVentaStock()).Returns(new List<ResearchVentaStockDto> {
                new ResearchVentaStockDto {Id = 1, Comercial = "", FechaHora = new DateTime(2018, 10, 26), LocalidadId = 1, MaterialId = 1, Observaciones = ""}
            });

            var result = target.EliminarVentaStock(1) as PartialViewResult;

            Assert.NotNull(result);

            researchManagerMock.Verify(x => x.EliminarResearchVentaStock(1), Times.Once);
            researchManagerMock.Verify(x => x.TraerTodoResearchVentaStock(), Times.Once);

            Assert.AreEqual("_ListaVentaStock", result.ViewName);
            Assert.IsInstanceOf<ResearchVentaStockModel>(result.Model);
        }
        [Test]
        public void TraerEstadio()
        {
            estadioManagerMock.Setup(x => x.TraerTodoEstadioPorMaterial(1)).Returns(new List<EstadioDto>() { new EstadioDto { Id=1,Descripcion="a",MaterialId=1} });
            var result = target.TraerEstadio(1) as JsonResult;

            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            estadioManagerMock.Verify(x => x.TraerTodoEstadioPorMaterial(1), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Disabled\":false,\"Group\":null,\"Selected\":false,\"Text\":\"a\",\"Value\":\"1\"}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerCampañaPorMaterialTest()
        {
            campañaManagerMock.Setup(x => x.TraerCampañaPorMaterial(1)).Returns(new List<CampañaDto>() { new CampañaDto { CampañaId = 1, Descripcion = "a" } });
            var result = target.TraerCampañaPorMaterial(1) as JsonResult;

            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            campañaManagerMock.Verify(x => x.TraerCampañaPorMaterial(1), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Disabled\":false,\"Group\":null,\"Selected\":false,\"Text\":\"a\",\"Value\":\"1\"}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void BuscaAvanceSiembraTest()
        {
            researchManagerMock.Setup(x => x.TraerAvanceSiembra(It.IsAny<KendoGridMvcRequest>()))
                .Returns(new KendoGrid<ResearchAvanceSiembraDto>(new List<ResearchAvanceSiembraDto>(), 1));

            var result = target.BuscaAvanceSiembra(It.IsAny<KendoGridMvcRequest>()) as JsonResult;

            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            researchManagerMock.Verify(x => x.TraerAvanceSiembra(It.IsAny<KendoGridMvcRequest>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Groups\":null,\"Data\":[],\"Aggregates\":null,\"Total\":1},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
