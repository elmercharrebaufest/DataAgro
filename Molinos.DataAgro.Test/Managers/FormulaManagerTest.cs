using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
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

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            target = new FormulaManager(logger.Object, repositorioMock.Object);
        }
        [Test]
        public void ultimaFormulaTestOk()
        {
            repositorioMock.Setup(y => y.Listar<Formula>(It.IsAny<Expression<Func<Formula, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<Formula>() { new Formula { Id = 1, CantDias = 88, Inicio = 4, CriterioId = 2 }, new Formula { Id = 2, CantDias = 3, Inicio = 1, CriterioId = 2 } });

            var result = target.ultimaFormula();

            FormulaIni ultimaFormula = new FormulaIni { CantDias = 3, Inicio = 1, CriterioId = 2 };

            Assert.NotNull(result);
            Assert.AreEqual(ultimaFormula.CantDias, result.Formula.CantDias);
        }
        [Test]
        public void TraerTodoCriterioOk()
        {
            repositorioMock.Setup(y => y.Listar<Criterio>(It.IsAny<Expression<Func<Criterio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<Criterio>() { new CriterioRaiz { Id = 1, Prioridad = 2 }, new CriterioContrato { Id = 2, Prioridad = 30, PadreId = 1 } });

            var result = target.TraerCriteriosGuardados();

            Assert.NotNull(result);
            Assert.AreEqual(2, result.Criterios.Count);
        }
        [Test]
        public void GrabarCriterioHijoOk()
        {

            repositorioMock.Setup(y => y.ObtenerConsultaEscalar<Formula>(It.IsAny<ObtenerUltimaFormula>()))
                .Returns(new Formula { Id = 1, CantDias = 88, Inicio = 4, Criterio = new CriterioRaiz { Id = 1, Prioridad = 100, Hijos = new List<Criterio>() } });


            var criterioNuevo = new CriterioIni { Prioridad = 20, PadreId = 1, Descripcion = "CriterioContrato" };

            repositorioMock.Setup(y => y.Agregar<Formula>(It.IsAny<Formula>()))
                .Returns(new Formula { Id = 1, CantDias = 88, Inicio = 4 });

            var resultado = target.GrabarCriterio(criterioNuevo);



            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void ModificarCriterioPrioridadExcedidaOk()
        {
            repositorioMock.Setup(y => y.ObtenerConsultaEscalar<Formula>(It.IsAny<ObtenerUltimaFormula>()))
                .Returns(new Formula { Id = 1, CantDias = 88, Inicio = 4, Criterio = new CriterioRaiz { Id = 1, Prioridad = 100, Hijos = new List<Criterio>() } });

            repositorioMock.Setup(y => y.Listar<Criterio>(It.IsAny<Expression<Func<Criterio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<Criterio>() { new CriterioRaiz { Id = 1, Prioridad = 100 }, new CriterioEsFason {Id=2, Prioridad = 10, PadreId = 1 }, new CriterioContrato { Id = 3, Prioridad = 70, PadreId = 1 } });

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
            repositorioMock.Setup(y => y.ObtenerConsultaEscalar<Formula>(It.IsAny<ObtenerUltimaFormula>()))
                .Returns(new Formula { Id = 1, CantDias = 88, Inicio = 4, Criterio = new CriterioRaiz { Id = 1, Prioridad = 100, Hijos = new List<Criterio>() } });

            repositorioMock.Setup(y => y.Listar<Criterio>(It.IsAny<Expression<Func<Criterio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<Criterio>() { new CriterioRaiz { Id = 1, Prioridad = 100 }, new CriterioEsFason { Id = 2, Prioridad = 10, PadreId = 1 }, new CriterioContrato { Id = 3, Prioridad = 70, PadreId = 1 } });

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

            repositorioMock.Setup(y => y.ObtenerConsultaEscalar<Formula>(It.IsAny<ObtenerUltimaFormula>()))
                .Returns(new Formula { Id = 1, CantDias = 88, Inicio = 4, Criterio = new CriterioRaiz { Id = 1, Prioridad = 100, Hijos = new List<Criterio>() } });

            repositorioMock.Setup(y => y.Listar<Criterio>(It.IsAny<Expression<Func<Criterio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                           .Returns(new List<Criterio>() { new CriterioRaiz { Id = 1, Prioridad = 100, Hijos = new List<Criterio>() }, new CriterioEsFason { Id = 2, Prioridad = 10, PadreId = 1 }, new CriterioContrato { Id = 3, Prioridad = 70, PadreId = 1 } });

            repositorioMock.Setup(y => y.Agregar<Formula>(It.IsAny<Formula>()))
               .Returns(new Formula { Id = 1, CantDias = 88, Inicio = 4 });


            var criterioNuevo = new CriterioIni { Id =2, Prioridad = 20, PadreId = 1 };
            var resultado = target.GrabarCriterio(criterioNuevo);



            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual("Debe seleccionar un Criterio", resultado.Errores[0].Message);
        }
        [Test]
        public void ModificarCriterioPrioridadCerooOk()
        {

            repositorioMock.Setup(y => y.ObtenerConsultaEscalar<Formula>(It.IsAny<ObtenerUltimaFormula>()))
                .Returns(new Formula { Id = 1, CantDias = 88, Inicio = 4, Criterio = new CriterioRaiz { Id = 1, Prioridad = 100, Hijos = new List<Criterio>() } });

            repositorioMock.Setup(y => y.Listar<Criterio>(It.IsAny<Expression<Func<Criterio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<Criterio>() { new CriterioRaiz { Id = 1, Prioridad = 100 }, new CriterioEsFason { Id = 2, Prioridad = 10, PadreId = 1 }, new CriterioContrato { Id = 3, Prioridad = 70, PadreId = 1 } });

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
            repositorioMock.Setup(y => y.ObtenerConsultaEscalar<Formula>(It.IsAny<ObtenerUltimaFormula>()))
                .Returns(new Formula { Id = 1, CantDias = 88, Inicio = 4, Criterio = new CriterioRaiz { Id = 1, Prioridad = 100, Hijos = new List<Criterio>() { new CriterioContrato { Id = 7, Prioridad = 55 } } } });

            repositorioMock.Setup(y => y.Agregar<Formula>(It.IsAny<Formula>()))
               .Returns(new Formula { Id = 1, CantDias = 88, Inicio = 4 });


            CriterioIni criterioAEliminar = new CriterioIni { Id = 7, Prioridad = 55, Descripcion = "CriterioContrato" };
            var resultado = target.eliminarCriterio(criterioAEliminar);



            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void ActualizarDiasyCantDias()
        {

            repositorioMock.Setup(y => y.Listar<Formula>(It.IsAny<Expression<Func<Formula, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                    .Returns(new List<Formula>() { new Formula { Id = 5, CantDias = 3, Inicio = 5 }, new Formula { Id = 1, CantDias = 10, Inicio = 11 } });


            var nuevosDias = new FormulaIni { CantDias = 7, Inicio = 7 };

            var resultado = target.actualizarDias(nuevosDias);


            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
    }
}
