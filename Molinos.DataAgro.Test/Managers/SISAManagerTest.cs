using Autofac.Extras.NLog;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using WebDataAgro.Models;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class SISAManagerTest
    {
        private SISAManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();

            target = new SISAManager(logger.Object,repositorioMock.Object);
        }

        [Test]
        public void InsertarSISATest()
        {
            var fecha = new DateTime(2018, 10, 26);
            var datos = new List<SISA>() { 
                new SISA { 
                    Id = 1, 
                    Categoria = "A", 
                    CBU = "1", 
                    CodCategoria = (int)EnumEstadoSisa.PRODUCTOR, CUIT = "1", EstadoCuit = 1, FechaActCBU = fecha, FechaGeneracion = fecha, 
                    FechaNotifDFECategoria = fecha, FechaNotifDFEEstado = fecha, FechaVigenciaCategoria = fecha, FechaVigenciaEstado = fecha,
                    Observaciones = "", RazonSocial = "A", SituacionCategoria = "A" } };

            repositorioMock.Setup(x => x.Listar<SISA>(null, 0, null, Entities.Helpers.DirOrden.Asc)).Returns(datos);
            var result = target.InsertarSISA(new List<SISA>(), new List<SISA>() { new SISA {
                CUIT = "1",
                CodCategoria = (int)EnumEstadoSisa.PRODUCTOR,
                FechaVigenciaCategoria = DateTime.Now.AddDays(1),
                SituacionCategoria="A",
                FechaVigenciaEstado= DateTime.Now.AddDays(1),
                EstadoCuit=1
            }});

            repositorioMock.Verify(x => x.Listar<SISA>(null, 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            repositorioMock.Verify(x => x.RemoverTodos<SISA>(It.IsAny<Expression<Func<SISA,bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.AgregarTodos(It.IsAny<List<SISA>>(), It.IsAny<List<KeyValuePair<string,string>>>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result);
        }
    }
}
