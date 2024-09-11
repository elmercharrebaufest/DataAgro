using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Moq;
using NUnit.Framework;
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
    public class HabilitacionBoletoControllerTest
    {
        private HabilitacionBoletoController target;
        private Mock<IHabilitacionBoletoManager> habilitacionBoletoManagerMock;
        private Mock<ITipoNegocioManager> tipoNegocioManagerMock;
        private Mock<IProvinciaManager> provinciaManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            habilitacionBoletoManagerMock = new Mock<IHabilitacionBoletoManager>();
            tipoNegocioManagerMock = new Mock<ITipoNegocioManager>();
            provinciaManagerMock = new Mock<IProvinciaManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new HabilitacionBoletoController(habilitacionBoletoManagerMock.Object, tipoNegocioManagerMock.Object, provinciaManagerMock.Object);
            HttpContext.Current.Session["comercialId"] = 1;
        }

        private void CompletarViewModelParaTest()
        {
            tipoNegocioManagerMock.Setup(x => x.TraerTodoTipoNegocio()).Returns(new List<TipoNegocioDto>
            {
                new TipoNegocioDto { Descripcion = "AFIJAR", TipoNegocioId = 1 }
            });

            habilitacionBoletoManagerMock.Setup(x => x.ListarTipoNegociosDetalle()).Returns(new List<TipoNegocioDetalleDto> {
                new TipoNegocioDetalleDto {Id = 1, TipoNegocioId = 1, Descripcion = "A FIJAR", BoletoFisico = true, CartaOferta = true, Confirma = true, TipoNegocioDescripcion = "A FIJAR"}
            });

            habilitacionBoletoManagerMock.Setup(x => x.ListarBoletoCompraNetProvincia()).Returns(new List<BoletoCompraNetProvinciaDto> {
                new BoletoCompraNetProvinciaDto {Id = 1, ProvinciaId = 1}
            });

            habilitacionBoletoManagerMock.Setup(x => x.ListarBoletoCompraNet()).Returns(new List<BoletoCompraNetDto> {
                new BoletoCompraNetDto {Id = 1, Descripcion = "CONFIRMA"}
            });

            provinciaManagerMock.Setup(x => x.ListarProvincia("")).Returns(new List<ProvinciaDto> {
                new ProvinciaDto {ProvinciaId = 1, Nombre = "BUENOS AIRES"}
            });
        }

        [Test]
        public void IndexOk()
        {
            CompletarViewModelParaTest();

            var result = target.Index() as ViewResult;
            tipoNegocioManagerMock.Verify(x => x.TraerTodoTipoNegocio(), Times.Once);
            habilitacionBoletoManagerMock.Verify(x => x.ListarTipoNegociosDetalle(), Times.Once);
            habilitacionBoletoManagerMock.Verify(x => x.ListarBoletoCompraNetProvincia(), Times.Once);
            habilitacionBoletoManagerMock.Verify(x => x.ListarBoletoCompraNet(), Times.Once);
            provinciaManagerMock.Verify(x => x.ListarProvincia(""), Times.Once);
            Assert.NotNull(result);
        }


        [Test]
        public void ActualizarTipoNegocioDetalleOk()
        {
            CompletarViewModelParaTest();

            var result = target.ActualizarTipoNegocioDetalle(new HabilitacionBoletoModel
            {
                Descripcion = "A FIJAR",
                TipoNegocioId = 1,
                TipoNegocioDetalles = new List<TipoNegocioDetalleDto>() { new TipoNegocioDetalleDto { BoletoFisico = true, Id = 1, CartaOferta = true, Confirma = false } }
            }) as ViewResult;

            tipoNegocioManagerMock.Verify(x => x.TraerTodoTipoNegocio(), Times.Once);
            habilitacionBoletoManagerMock.Verify(x => x.ListarTipoNegociosDetalle(), Times.Once);
        }

    }
}
