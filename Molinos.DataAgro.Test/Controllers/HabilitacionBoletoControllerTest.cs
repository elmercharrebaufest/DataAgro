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
    public class HabilitacionBoletoControllerTest
    {
        private HabilitacionBoletoController target;
        private Mock<IHabilitacionBoletoManager> habilitacionBoletoManagerMock;
        private Mock<ITipoNegocioManager> tipoNegocioManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            habilitacionBoletoManagerMock = new Mock<IHabilitacionBoletoManager>();
            tipoNegocioManagerMock = new Mock<ITipoNegocioManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new HabilitacionBoletoController(habilitacionBoletoManagerMock.Object, tipoNegocioManagerMock.Object);
            HttpContext.Current.Session["comercialId"] = 1;
        }     

        [Test]
        public void IndexOk()
        {
            tipoNegocioManagerMock.Setup(x => x.TraerTodoTipoNegocio()).Returns(new List<TipoNegocioDto>
            {
                new TipoNegocioDto
                {
                    Descripcion = "AFIJAR",
                    TipoNegocioId = 1
                }
            });

            habilitacionBoletoManagerMock.Setup(x => x.ListarTipoNegociosDetalle()).Returns(new List<TipoNegocioDetalleDto> {
                new TipoNegocioDetalleDto {Id = 1, TipoNegocioId = 1, Descripcion = "A FIJAR", BoletoFisico = true, CartaOferta = true, Confirma = true, TipoNegocioDescripcion = "A FIJAR"}
            });

            var result = target.Index() as ViewResult;      
            tipoNegocioManagerMock.Verify(x => x.TraerTodoTipoNegocio(), Times.Once);
            habilitacionBoletoManagerMock.Verify(x => x.ListarTipoNegociosDetalle(), Times.Once);
            Assert.NotNull(result);
        }


        [Test]
        public void ActualizarTipoNegocioDetalleOk()
        {
            tipoNegocioManagerMock.Setup(x => x.TraerTodoTipoNegocio()).Returns(new List<TipoNegocioDto>
            {
                new TipoNegocioDto
                {
                    Descripcion = "AFIJAR",
                    TipoNegocioId = 1
                }
            });

            habilitacionBoletoManagerMock.Setup(x => x.ListarTipoNegociosDetalle()).Returns(new List<TipoNegocioDetalleDto> {
                new TipoNegocioDetalleDto {Id = 1, TipoNegocioId = 1, Descripcion = "A FIJAR", BoletoFisico = true, CartaOferta = true, Confirma = true, TipoNegocioDescripcion = "A FIJAR"}
            });

            var result = target.ActualizarTipoNegocioDetalle(new HabilitacionBoletoModel { Descripcion = "A FIJAR", 
                TipoNegocioId = 1, TipoNegocioDetalles = new List<TipoNegocioDetalleDto>() { new TipoNegocioDetalleDto { BoletoFisico = true, Id = 1, CartaOferta = true, Confirma = false} }
            }) as ViewResult;

            tipoNegocioManagerMock.Verify(x => x.TraerTodoTipoNegocio(), Times.Once);
            habilitacionBoletoManagerMock.Verify(x => x.ListarTipoNegociosDetalle(), Times.Once);
        }
       
    }
}
