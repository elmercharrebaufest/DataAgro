using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class HedgeControllerTest
    {
        private HedgeController target;
        private Mock<IHedgeManager> hedgeManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            hedgeManagerMock = new Mock<IHedgeManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            HttpContext.Current.Session["perfil"] = 7;
            HttpContext.Current.Session["comercialId"] = 1;
            target = new HedgeController(hedgeManagerMock.Object);
        }

        [Test]
        public void IndexOk()
        {
            hedgeManagerMock.Setup(x => x.Dia()).Returns(false);
            var result = target.Index() as ViewResult;

            hedgeManagerMock.Verify(x => x.Dia(), Times.Once);
            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
        [Test]
        public void HedgeMaterialPartialTest()
        {
            hedgeManagerMock.Setup(x => x.TraerTodosHedgeMaterial()).Returns(new List<HedgeMaterialDto>() {
                new HedgeMaterialDto { Id= 1, Cantidad=1, MaterialId= 1,TipoHedgeMaterialId = 1}});
            var result = target.HedgeMaterialPartial(new Resultado()) as PartialViewResult;

            Assert.NotNull(result);

            hedgeManagerMock.Verify(x => x.TraerTodosHedgeMaterial(), Times.Once);

            Assert.AreEqual("_HedgeMaterial", result.ViewName);
            Assert.IsInstanceOf<HedgeModel>(result.Model);
        }

        [Test]
        public void HedgeObjetivoPartialTest()
        {
            hedgeManagerMock.Setup(x => x.TraerTodosHedgeObjetivo()).Returns(new List<HedgeObjetivoDto>() {
                new HedgeObjetivoDto { Id= 1, Cantidad=1, MaterialId= 1,TipoObjetivoId = 1}});
            var result = target.HedgeObjetivoPartial(new Resultado()) as PartialViewResult;

            Assert.NotNull(result);

            hedgeManagerMock.Verify(x => x.TraerTodosHedgeObjetivo(), Times.Once);

            Assert.AreEqual("_HedgeObjetivo", result.ViewName);
            Assert.IsInstanceOf<HedgeModel>(result.Model);
        }

        [Test]
        public void HedgeTCPartialTest()
        {
            hedgeManagerMock.Setup(x => x.TraerTodosHedgeTC()).Returns(new List<HedgeTCDto>());
            var result = target.HedgeTCPartial(new Resultado()) as PartialViewResult;

            Assert.NotNull(result);

            hedgeManagerMock.Verify(x => x.TraerTodosHedgeTC(), Times.Once);

            Assert.AreEqual("_HedgeTC", result.ViewName);
            Assert.IsInstanceOf<HedgeModel>(result.Model);
        }

        [Test]
        public void GrabarHedgeMaterialTest()
        {
            var hedge = new HedgeModel { HedgeMaterial = 
                new List<HedgeMaterialModel>() {
                    new HedgeMaterialModel { MaterialId = 1,Disponible= 1,Forward=1 ,NewCrop= 1} } };
            hedgeManagerMock.Setup(x => x.GrabarHedgeMaterial(new List<HedgeMaterial>(), 1)).Returns(new Resultado {Errores = new List<ErrorMessage>() });
            hedgeManagerMock.Setup(x => x.TraerTodosHedgeMaterial()).Returns(new List<HedgeMaterialDto>() {
                new HedgeMaterialDto { Id= 1, Cantidad=1, MaterialId= 1,TipoHedgeMaterialId = 1}});
            var result = target.GrabarHedgeMaterial(hedge) as PartialViewResult;

            Assert.NotNull(result);

            hedgeManagerMock.Verify(x => x.GrabarHedgeMaterial(It.IsAny<List<HedgeMaterial>>(),It.IsAny<int>()), Times.Once);

            Assert.AreEqual("_HedgeMaterial", result.ViewName);
        }
        [Test]
        public void GrabarHedgeObjetivoTest()
        {
            var hedge = new HedgeModel
            {
                HedgeObjetivo =
                new List<HedgeObjetivoModel>() {
                    new HedgeObjetivoModel { MaterialId = 1,Pricing=1 ,ARemitir =1 } }
            };
            hedgeManagerMock.Setup(x => x.GrabarHedgeObjetivo(new List<HedgeObjetivo>(), 1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            hedgeManagerMock.Setup(x => x.TraerTodosHedgeObjetivo()).Returns(new List<HedgeObjetivoDto>() {
                new HedgeObjetivoDto { Id= 1, Cantidad=1, MaterialId= 1,TipoObjetivoId = 1}});
            var result = target.GrabarHedgeObjetivo(hedge) as PartialViewResult;

            Assert.NotNull(result);

            hedgeManagerMock.Verify(x => x.GrabarHedgeObjetivo(It.IsAny<List<HedgeObjetivo>>(), It.IsAny<int>()), Times.Once);

            Assert.AreEqual("_HedgeObjetivo", result.ViewName);
        }
        [Test]
        public void GrabarHedgeTCTest()
        {
            var hedge = new HedgeModel
            {
                TCModel =new TcModel {HedgePesos=1,TC=1 }
            };
            hedgeManagerMock.Setup(x => x.GrabarHedgeTC(new HedgeTC(), 1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            hedgeManagerMock.Setup(x => x.TraerTodosHedgeTC()).Returns(new List<HedgeTCDto>());

            var result = target.GrabarHedgeTC(hedge) as PartialViewResult;

            Assert.NotNull(result);

            hedgeManagerMock.Verify(x => x.GrabarHedgeTC(It.IsAny<HedgeTC>(), It.IsAny<int>()), Times.Once);

            Assert.AreEqual("_HedgeTC", result.ViewName);
        }
        [Test]
        public void EliminarHedgeTCTest()
        {
            
            hedgeManagerMock.Setup(x => x.EliminarHedgeTC(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            hedgeManagerMock.Setup(x => x.TraerTodosHedgeTC()).Returns(new List<HedgeTCDto>());

            var result = target.EliminarHedgeTC(1) as PartialViewResult;

            Assert.NotNull(result);

            hedgeManagerMock.Verify(x => x.EliminarHedgeTC(It.IsAny<int>()), Times.Once);

            Assert.AreEqual("_HedgeTC", result.ViewName);
        }
        [Test]
        public void CerrarDiaTest()
        {

            hedgeManagerMock.Setup(x => x.CerrarDia(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            
            var result = target.CerrarDia() as RedirectToRouteResult;

            Assert.NotNull(result);

            hedgeManagerMock.Verify(x => x.CerrarDia(It.IsAny<int>()), Times.Once);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}
