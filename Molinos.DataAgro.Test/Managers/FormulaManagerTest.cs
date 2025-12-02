using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
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
using System.Configuration;
using System.IO;
using System.Linq.Expressions;
using System.Web;
using System.Web.Script.Serialization;
using Molinos.DataAgro.Entities;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    class FormulaManagerTest
    {
        private FormulaManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<ICupoManager> cupoManagerMock;

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            cupoManagerMock = new Mock<ICupoManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            target = new FormulaManager(logger.Object, repositorioMock.Object, cupoManagerMock.Object);
        }
        [Test]
        public void UltimaFormulaTestOk()
        {
            repositorioMock.Setup(y => y.Listar<Formula>(It.IsAny<Expression<Func<Formula, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<Formula>() { new Formula { Material = new Material { MaterialId = 1, Descripcion = "a" }, Id = 1, CuposDesde = DateTime.Now.Date, CuposHasta = DateTime.Now.Date, NegociosDesde = DateTime.Now.Date, NegociosHasta = DateTime.Now.Date, CriterioId = 2 }, new Formula { Id = 2, CuposDesde = DateTime.Now.Date, CuposHasta = DateTime.Now.Date, NegociosDesde = DateTime.Now.Date, NegociosHasta = DateTime.Now.Date, CriterioId = 2 } });
            cupoManagerMock.Setup(x => x.DevolverTodoCierreCupera()).Returns(new List<CierreCupera>());
            var result = target.UltimaFormula(1);

            FormulaIni ultimaFormula = new FormulaIni { CuposDesde = DateTime.Now.Date, CuposHasta = DateTime.Now.Date, NegociosDesde = DateTime.Now.Date, NegociosHasta = DateTime.Now.Date, CriterioId = 2 };

            Assert.NotNull(result);
            Assert.AreEqual(ultimaFormula.CuposHasta, result.Formula.CuposHasta);
        }
        [Test]
        public void TraerTodoCriterioOk()
        {
            repositorioMock.Setup(y => y.Listar<Formula>(It.IsAny<Expression<Func<Formula, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                           .Returns(new List<Formula>() {
                               new Formula { Id = 1, CuposDesde = DateTime.Now.Date, CuposHasta = DateTime.Now.Date, NegociosDesde = DateTime.Now.Date, NegociosHasta = DateTime.Now.Date, CriterioId = 1 },
                               new Formula { Id = 2, CuposDesde = DateTime.Now.Date, CuposHasta = DateTime.Now.Date, NegociosDesde = DateTime.Now.Date, NegociosHasta = DateTime.Now.Date, CriterioId = 2,
                                             Criterio = new CriterioRaiz{Id = 2, Concreta = false, Prioridad = 100,
                                                 Hijos = new List<Criterio>() { new CriterioDeltaDePrecio { Id = 3, Prioridad = 30, PadreId = 2 } }}
                               } });
            repositorioMock.Setup(y => y.ObtenerConsultaEscalar<Formula>(It.IsAny<ObtenerUltimaFormula>()))
                          .Returns(new Formula
                          {
                              Id = 2,
                              CuposDesde = DateTime.Now.Date,
                              CuposHasta = DateTime.Now.Date,
                              NegociosDesde = DateTime.Now.Date,
                              NegociosHasta = DateTime.Now.Date,
                              CriterioId = 2,
                              Criterio = new CriterioRaiz
                              {
                                  Id = 2,
                                  Concreta = false,
                                  Prioridad = 100,
                                  Hijos = new List<Criterio>() { new CriterioDeltaDePrecio { Id = 3, Prioridad = 30, PadreId = 2 } }
                              }
                          });
         
            var result = target.TraerCriteriosGuardados(1);

            Assert.NotNull(result);
            Assert.AreEqual(2, result.Criterios.Count);
        }
        [Test]
        public void GrabarCriterioHijoOk()
        {
            repositorioMock.Setup(y => y.Obtener<Criterio>(It.IsAny<int>())).Returns(new CriterioRaiz() { PadreId = null, Id = 1 });

            repositorioMock.Setup(y => y.ObtenerConsultaEscalar<Formula>(It.IsAny<ObtenerUltimaFormula>()))
                .Returns(new Formula { Id = 1, CuposDesde = DateTime.Now.Date, CuposHasta = DateTime.Now.Date, NegociosDesde = DateTime.Now.Date, NegociosHasta = DateTime.Now.Date, Criterio = new CriterioRaiz { Id = 1, Prioridad = 100, Hijos = new List<Criterio>() } });


            var criterioNuevo = new CriterioIni { Prioridad = 20, PadreId = 1, Descripcion = "CriterioDeltaDePrecio" };

            repositorioMock.Setup(y => y.Agregar<Formula>(It.IsAny<Formula>()))
                .Returns(new Formula { Id = 1, CuposDesde = DateTime.Now.Date, CuposHasta = DateTime.Now.Date, NegociosDesde = DateTime.Now.Date, NegociosHasta = DateTime.Now.Date });

            var resultado = target.GrabarCriterio(criterioNuevo);



            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void ModificarCriterioPrioridadExcedidaOk()
        {
            repositorioMock.Setup(y => y.Obtener<Criterio>(It.IsAny<int>())).Returns(new CriterioRaiz() { PadreId = null, Id = 1 });

            repositorioMock.Setup(y => y.ObtenerConsultaEscalar<Formula>(It.IsAny<ObtenerUltimaFormula>()))
                .Returns(new Formula { Id = 1, CuposDesde = DateTime.Now.Date, CuposHasta = DateTime.Now.Date, NegociosDesde = DateTime.Now.Date, NegociosHasta = DateTime.Now.Date, Criterio = new CriterioRaiz { Id = 1, Prioridad = 100, Hijos = new List<Criterio>() } });

            repositorioMock.Setup(y => y.Listar<Criterio>(It.IsAny<Expression<Func<Criterio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<Criterio>() { new CriterioRaiz { Id = 1, Prioridad = 100 }, new CriterioEsFason { Id = 2, Prioridad = 10, PadreId = 1 }, new CriterioDeltaDePrecio { Id = 3, Prioridad = 70, PadreId = 1 } });

            repositorioMock.Setup(y => y.Agregar<Formula>(It.IsAny<Formula>()))
                .Returns(new Formula { Id = 2 });


            var criterioModificado = new CriterioIni { Id = 2, Prioridad = 110, PadreId = 1, Descripcion = "CriterioPrioridadExcedida" };

            var resultado = target.GrabarCriterio(criterioModificado);



            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual("La Prioridad Debe ser menor a : 30", resultado.Errores[0].Message);
        }
        [Test]
        public void CriterioNuevoPrioridadCeroOk()
        {
            repositorioMock.Setup(y => y.Obtener<Criterio>(It.IsAny<int>())).Returns(new CriterioRaiz() { PadreId = null, Id = 1 });

            repositorioMock.Setup(y => y.ObtenerConsultaEscalar<Formula>(It.IsAny<ObtenerUltimaFormula>()))
                .Returns(new Formula { Id = 1, CuposDesde = DateTime.Now.Date, CuposHasta = DateTime.Now.Date, NegociosDesde = DateTime.Now.Date, NegociosHasta = DateTime.Now.Date, Criterio = new CriterioRaiz { Id = 1, Prioridad = 100, Hijos = new List<Criterio>() } });

            repositorioMock.Setup(y => y.Listar<Criterio>(It.IsAny<Expression<Func<Criterio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<Criterio>() { new CriterioRaiz { Id = 1, Prioridad = 100 }, new CriterioEsFason { Id = 2, Prioridad = 10, PadreId = 1 }, new CriterioDeltaDePrecio { Id = 3, Prioridad = 70, PadreId = 1 } });

            repositorioMock.Setup(y => y.Agregar<Formula>(It.IsAny<Formula>()))
                .Returns(new Formula { Id = 2 });


            var criterioModificado = new CriterioIni { Id = 0, Prioridad = 0, PadreId = 1, Descripcion = "CriterioPrioridadCero" };

            var resultado = target.GrabarCriterio(criterioModificado);



            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual("La Prioridad debe ser mayor a 0", resultado.Errores[0].Message);
        }
        [Test]
        public void ModificarCriterioDescripcionNulaOk()
        {
            repositorioMock.Setup(y => y.Obtener<Criterio>(It.IsAny<int>())).Returns(new CriterioRaiz() { PadreId = null, Id = 1 });

            repositorioMock.Setup(y => y.ObtenerConsultaEscalar<Formula>(It.IsAny<ObtenerUltimaFormula>()))
                .Returns(new Formula { Id = 1, CuposDesde = DateTime.Now.Date, CuposHasta = DateTime.Now.Date, NegociosDesde = DateTime.Now.Date, NegociosHasta = DateTime.Now.Date, Criterio = new CriterioRaiz { Id = 1, Prioridad = 100, Hijos = new List<Criterio>() } });

            repositorioMock.Setup(y => y.Listar<Criterio>(It.IsAny<Expression<Func<Criterio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                           .Returns(new List<Criterio>() { new CriterioRaiz { Id = 1, Prioridad = 100, Hijos = new List<Criterio>() }, new CriterioEsFason { Id = 2, Prioridad = 10, PadreId = 1 }, new CriterioDeltaDePrecio { Id = 3, Prioridad = 70, PadreId = 1 } });

            repositorioMock.Setup(y => y.Agregar<Formula>(It.IsAny<Formula>()))
               .Returns(new Formula { Id = 1, CuposDesde = DateTime.Now.Date, CuposHasta = DateTime.Now.Date, NegociosDesde = DateTime.Now.Date, NegociosHasta = DateTime.Now.Date });


            var criterioNuevo = new CriterioIni { Id = 2, Prioridad = 20, PadreId = 1 };
            var resultado = target.GrabarCriterio(criterioNuevo);



            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual("Debe seleccionar un Criterio", resultado.Errores[0].Message);
        }
        [Test]
        public void ModificarCriterioPrioridadCerooOk()
        {
            repositorioMock.Setup(y => y.Obtener<Criterio>(It.IsAny<int>())).Returns(new CriterioRaiz() { PadreId = null, Id = 1 });

            repositorioMock.Setup(y => y.ObtenerConsultaEscalar<Formula>(It.IsAny<ObtenerUltimaFormula>()))
                .Returns(new Formula { Id = 1, CuposDesde = DateTime.Now.Date, CuposHasta = DateTime.Now.Date, NegociosDesde = DateTime.Now.Date, NegociosHasta = DateTime.Now.Date, Criterio = new CriterioRaiz { Id = 1, Prioridad = 100, Hijos = new List<Criterio>() } });

            repositorioMock.Setup(y => y.Listar<Criterio>(It.IsAny<Expression<Func<Criterio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<Criterio>() { new CriterioRaiz { Id = 1, Prioridad = 100 }, new CriterioEsFason { Id = 2, Prioridad = 10, PadreId = 1 }, new CriterioDeltaDePrecio { Id = 3, Prioridad = 70, PadreId = 1 } });

            repositorioMock.Setup(y => y.Agregar<Formula>(It.IsAny<Formula>()))
                .Returns(new Formula { Id = 2 });


            var criterioModificado = new CriterioIni { Id = 2, Prioridad = 0, PadreId = 1 };

            var resultado = target.GrabarCriterio(criterioModificado);



            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual("La Prioridad debe ser mayor a 0", resultado.Errores[0].Message);
        }
        [Test]
        public void EliminarCriterioOk()
        {
            repositorioMock.Setup(y => y.Obtener<Criterio>(It.IsAny<int>())).Returns(new CriterioRaiz() { PadreId = null, Id = 1 });

            repositorioMock.Setup(y => y.ObtenerConsultaEscalar<Formula>(It.IsAny<ObtenerUltimaFormula>()))
                .Returns(new Formula { Id = 1, CuposDesde = DateTime.Now.Date, CuposHasta = DateTime.Now.Date, NegociosDesde = DateTime.Now.Date, NegociosHasta = DateTime.Now.Date, Criterio = new CriterioRaiz { Id = 1, Prioridad = 100, Hijos = new List<Criterio>() { new CriterioDeltaDePrecio { Id = 7, Prioridad = 55 } } } });

            repositorioMock.Setup(y => y.Agregar<Formula>(It.IsAny<Formula>()))
               .Returns(new Formula { Id = 1, CuposDesde = DateTime.Now.Date, CuposHasta = DateTime.Now.Date, NegociosDesde = DateTime.Now.Date, NegociosHasta = DateTime.Now.Date });


            CriterioIni criterioAEliminar = new CriterioIni { Id = 7, Prioridad = 55, Descripcion = "CriterioDeltaDePrecio" };
            var resultado = target.EliminarCriterio(criterioAEliminar);



            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void ActualizarDiasyCantDias()
        {

            repositorioMock.Setup(y => y.Listar<Formula>(It.IsAny<Expression<Func<Formula, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                    .Returns(new List<Formula>() { new Formula { Id = 5, CuposDesde = DateTime.Now.Date, CuposHasta = DateTime.Now.Date, NegociosDesde = DateTime.Now.Date, NegociosHasta = DateTime.Now.Date }, new Formula { Id = 1, CuposDesde = DateTime.Now.Date, CuposHasta = DateTime.Now.Date, NegociosDesde = DateTime.Now.Date, NegociosHasta = DateTime.Now.Date } });

            repositorioMock.Setup(y => y.ObtenerConsultaEscalar(It.IsAny<ObtenerUltimaFormula>()))
                  .Returns(new Formula { Id = 5, CuposDesde = DateTime.Now.Date, CuposHasta = DateTime.Now.Date, NegociosDesde = DateTime.Now.Date, NegociosHasta = DateTime.Now.Date });

            var nuevosDias = new FormulaIni { CuposDesde = DateTime.Now.Date, CuposHasta = DateTime.Now.Date, NegociosDesde = DateTime.Now.Date, NegociosHasta = DateTime.Now.Date };

            var resultado = target.ActualizarDias(nuevosDias);


            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
    }
}
