using NLog;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class HabilitacionBoletoManagerTest
    {
        private HabilitacionBoletoManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            target = new HabilitacionBoletoManager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void ListarTipoNegociosDetalleTest()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<TipoNegocioDetalle, TipoNegocioDetalleDto>>>(), It.IsAny<Expression<Func<TipoNegocioDetalle, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
             .Returns(new List<TipoNegocioDetalleDto>() { new TipoNegocioDetalleDto {
                Id = 1,
                Descripcion = "A FIJAR",
                TipoNegocioId = 1,
                TipoNegocioDescripcion = "A FIJAR",
                CartaOferta = true,
                Confirma = false,
                BoletoFisico = false
             }
           });
            var result = target.ListarTipoNegociosDetalle();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<TipoNegocioDetalle, TipoNegocioDetalleDto>>>(), It.IsAny<Expression<Func<TipoNegocioDetalle, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
        }

        [Test]
        public void ActualizarTipoNegocioDetalleTest()
        {
            var tipos = new List<TipoNegocioDetalleDto>() { new TipoNegocioDetalleDto
            {
                Id = 1,
                Descripcion = "A FIJAR",
                TipoNegocioId = 1,
                TipoNegocioDescripcion = "A FIJAR",
                CartaOferta = true,
                Confirma = false,
                BoletoFisico = false
            }};
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<TipoNegocioDetalle, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
           .Returns(new List<TipoNegocioDetalle>() { new TipoNegocioDetalle {
                Id = 1,
                Descripcion = "A FIJAR",
                TipoNegocioId = 1,
                CartaOferta = true,
                Confirma = false,
                BoletoFisico = false
             }
         });
            target.ActualizarTipoNegocioDetalle(tipos);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<TipoNegocioDetalle, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once); 
        }

        [Test]
        public void AgregarTipoNegocioDetalleTest()
        {
            var tipo = new TipoNegocioDetalleDto {
                Id = 1,
                Descripcion = "A FIJAR",
                TipoNegocioId = 1,
                CartaOferta = true,
                Confirma = false,
                BoletoFisico = false
            };
         
            target.AgregarTipoNegocioDetalle(tipo);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<TipoNegocioDetalle>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void ListarBoletoCompraNetProvinciaTest()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<BoletoCompraNetProvincia, BoletoCompraNetProvinciaDto>>>(), It.IsAny<Expression<Func<BoletoCompraNetProvincia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
             .Returns(new List<BoletoCompraNetProvinciaDto>() { new BoletoCompraNetProvinciaDto {
                Id = 1,
                BoletoCompraNetId = 1,
                ProvinciaId = 1,
                BoletoDescripcion = "CONFIRMA",
                ProvinciaNombre = "BUENOS AIRES"
             }
           });
            var result = target.ListarBoletoCompraNetProvincia();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<BoletoCompraNetProvincia, BoletoCompraNetProvinciaDto>>>(), It.IsAny<Expression<Func<BoletoCompraNetProvincia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
        }
        
        [Test]
        public void ListarBoletoCompraNetTest()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<BoletoCompraNet, BoletoCompraNetDto>>>(), It.IsAny<Expression<Func<BoletoCompraNet, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
             .Returns(new List<BoletoCompraNetDto>() { new BoletoCompraNetDto {
                Id = 1,
                Descripcion = "CONFIRMA",
             }
           });
            var result = target.ListarBoletoCompraNet();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<BoletoCompraNet, BoletoCompraNetDto>>>(), It.IsAny<Expression<Func<BoletoCompraNet, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
        }

        [Test]
        public void AgregarBoletoCompraNetProvinciaTest()
        {
            var boleto = new BoletoCompraNetProvinciaDto
            {
                BoletoCompraNetId = 1,
                ProvinciaId = 1
            };

            var resultado = target.AgregarBoletoCompraNetProvincia(boleto);

            Assert.IsFalse(resultado.HayError);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<BoletoCompraNetProvincia>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void EliminarBoletoCompraNetProvinciaTest()
        {
            var boletoId = 1;
            var boleto = new BoletoCompraNetProvincia
            {
                BoletoCompraNetId = 1,
                ProvinciaId = 1
            };

            repositorioMock.Setup(x => x.Obtener<BoletoCompraNetProvincia>(boletoId)).Returns(boleto);

            var resultado = target.EliminarBoletoCompraNetProvincia(boletoId);

            Assert.IsFalse(resultado.HayError);
            repositorioMock.Verify(x => x.Obtener<BoletoCompraNetProvincia>(boletoId), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<BoletoCompraNetProvincia>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

    }
}