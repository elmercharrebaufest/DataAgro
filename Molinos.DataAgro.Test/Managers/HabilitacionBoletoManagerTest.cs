using Autofac.Extras.NLog;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq.Expressions;
using System.Web;
using System.Web.Script.Serialization;

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
        public void AgregarHabilitacionBoletoTest()
        {
            var tipo = new TipoNegocioDetalleDto {
                Id = 1,
                Descripcion = "A FIJAR",
                TipoNegocioId = 1,
                CartaOferta = true,
                Confirma = false,
                BoletoFisico = false
            };
         
            target.AgregarHabilitacionBoleto(tipo);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<TipoNegocioDetalle>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }       
    }
}