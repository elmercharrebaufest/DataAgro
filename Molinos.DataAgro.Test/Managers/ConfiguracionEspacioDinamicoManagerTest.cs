using Autofac.Extras.NLog;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Script.Serialization;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ConfiguracionEspacioDinamicoManagerTest
    {
        private ConfiguracionEspacioDinamicoManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();

            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            target = new ConfiguracionEspacioDinamicoManager(logger.Object, repositorioMock.Object);
        }
        [Test]
        public void GrabarNuevaConfiguracionEspacioDinamicoTest()
        {
            var listaCupos = new List<DiaCupo>();

            var diaCupo = new DiaCupo()
            {
                Cantidad = 5,
                Fecha = new DateTime(2021, 07, 07),

            };
            listaCupos.Add(diaCupo);
            var espacioDinamico = new ConfiguracionEspacioDinamico
            {
                Id = 0,
                CentroId = 1,
                MaterialId = 1,
                Fecha = new DateTime(2021, 06, 07),
                CantidadDeCupo = 10,
                Calidad = "",
                ComercialId = 1,
                ProveedorId = 1
            };
            repositorioMock.Setup(x => x.Existe(It.IsAny<Expression<Func<ConfiguracionEspacioDinamico, bool>>>())).Returns(false);
            repositorioMock.Setup(x => x.Agregar(It.IsAny<ConfiguracionEspacioDinamico>()));
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<ObtenerUltimaFormula>()))
               .Returns(new Formula
               {
                   Id = 1,
                   CuposDesde = DateTime.Now.Date,
                   CuposHasta = DateTime.Now.Date,
                   NegociosDesde = new DateTime(2021, 06, 05),
                   NegociosHasta = new DateTime(2021, 07, 07),
                   Fecha = DateTime.Now.Date,
                   CentroId = 1,
                   CriterioId = 1,
                   Criterio =
               new CriterioRaiz
               {
                   Id = 1,
                   Prioridad = 100,
                   Hijos = new List<Criterio> {
                       new CriterioEsContratoAFijar { Id = 3, PadreId = 2, Prioridad = 60 },
                        new CriterioEsContratoAPrecio { Id = 4, PadreId = 2, Prioridad = 40 },
                    }
               }
               });
            repositorioMock.Setup(y => y.ObtenerPrimero<TipoNegocio>(It.IsAny<Expression<Func<TipoNegocio, bool>>>()))
             .Returns(new TipoNegocio { TipoNegocioId = 7, Descripcion = "ESPACIO DINAMICO" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, string>>>()))
               .Returns("CORREDOR BS AS");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ZonaCupo, bool>>>()))
               .Returns(new ZonaCupo { Id = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ConfiguracionEspacioDinamico, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ConfiguracionEspacioDinamico>() { espacioDinamico });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ConfiguracionCupo, bool>>>()))
             .Returns(new ConfiguracionCupo { Id = 1, LimiteAlgoritmo = 100, LimiteCupo = 1000, MaterialId = 1, CentroId = 1, Fecha = new DateTime(2021, 06, 07) });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SugerenciaPorComercial, bool>>>()))
                 .Returns(new SugerenciaPorComercial { Id = 1, MaterialId = 1, CentroId = 1, Fecha = new DateTime(2021, 06, 07), Total = 1000 });
            
            var resultado = target.GrabarConfiguracionEspacioDinamico(espacioDinamico, listaCupos);

            //Assert.That(!resultado.HayError);
            //repositorioMock.Verify(x => x.Agregar(It.IsAny<ConfiguracionEspacioDinamico>()), Times.Once);
            ////repositorioMock.Verify(x => x.Agregar(It.IsAny<SugerenciaCupo>()), Times.Once);
            //repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(1));
        }

        [Test]
        public void TraerConfiguracionEspacioDinamicoTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<ConfiguracionEspacioDinamico, bool>>>(), It.IsAny<Expression<Func<ConfiguracionEspacioDinamico, ConfiguracionEspacioDinamicoDto>>>()))
                .Returns(new ConfiguracionEspacioDinamicoDto { Id = 1, CantidadDeCupo = 100 });
            var resultado = target.TraerConfiguracionEspacioDinamico(1);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<ConfiguracionEspacioDinamico, bool>>>(), It.IsAny<Expression<Func<ConfiguracionEspacioDinamico, ConfiguracionEspacioDinamicoDto>>>()), Times.Once);
            Assert.AreEqual(1, resultado.Id);
        }


        [Test]
        public void EliminarConfiguracionEspacioDinamicoTestOk()
        {
            repositorioMock.Setup(y => y.Obtener<ConfiguracionEspacioDinamico>(It.IsAny<int>())).Returns(new ConfiguracionEspacioDinamico() { Id = 1 });
            var resultado = target.EliminarConfiguracionEspacioDinamico(1);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void EliminarConfiguracionEspacioDinamicoErrorEspacioUsado()
        {
            repositorioMock.Setup(y => y.Obtener<ConfiguracionEspacioDinamico>(It.IsAny<int>())).Returns(new ConfiguracionEspacioDinamico() { Id = 1 });
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<Cupo, bool>>>())).Returns(true);
            var resultado = target.EliminarConfiguracionEspacioDinamico(1);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }
    }
}