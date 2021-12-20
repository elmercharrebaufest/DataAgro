using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using WebDataAgro.Models;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    class FormulaControllerTest
    {
        private FormulaController target;
        private Mock<IFormulaManager> formulaManagerMock;
        private Mock<IMaterialManager> materialManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            formulaManagerMock = new Mock<IFormulaManager>();
            materialManagerMock = new Mock<IMaterialManager>();
            target = new FormulaController(formulaManagerMock.Object, materialManagerMock.Object);
        }
        [Test]
        public void InicializarTest()
        {
            materialManagerMock.Setup(x => x.TraerTodoMaterial()).Returns(new ResultIniMaterial { Material = new List<MaterialIni>() });

            formulaManagerMock.Setup(x => x.TodosLosCriterios()).Returns(new List<CriterioIni>()
            {
                new CriterioIni{ Id=0,Descripcion="CriterioRaiz",DisplayName="Criterios" },
                new CriterioIni{ Id=0,Descripcion="CriterioEsFason",DisplayName="Fason" }
            });
            formulaManagerMock.Setup(x => x.UltimaFormula(1)).Returns(new ResultIniFormula()
            {
                Formula = new FormulaIni
                {
                    Id = 1,
                    CuposDesde = new DateTime(2021, 3, 15),
                    CuposHasta = new DateTime(2021, 3, 15),
                    NegociosDesde = new DateTime(2021, 3, 15),
                    NegociosHasta = new DateTime(2021, 3, 15),
                    CriterioId = 1,
                    MaterialId = 1
                }
            });
            var result = target.Inicializar(1);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            var data = (ResultIniFormulaModel)((JsonResult)result).Data;
            Assert.AreEqual(data.Datos.ultimaFormulaTraida.Formula.Id, 1);
            Assert.AreEqual(data.Datos.Criterios.Criterios.Count, 2);
        }
        [Test]
        public void BuscarTest()
        {
            formulaManagerMock.Setup(x => x.TraerCriteriosGuardados(1)).Returns(new ResultIniCriterio
            {

                Criterios = new List<CriterioIni> { new CriterioIni { Id = 1, Descripcion = "CriterioRaizTest" } }
            });
            var result = target.Buscar(1);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Id\":1,\"Padre\":null,\"PadreId\":null,\"Hijos\":null,\"Prioridad\":0,\"Descripcion\":\"CriterioRaizTest\",\"Concreta\":false,\"DisplayName\":null}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void UpdateNuevoCriterioTest()
        {
            var criterioNuevo = new CriterioIni
            {
                Id = 0,
                Descripcion = "CriterioNuevoTest"
            };
            formulaManagerMock.Setup(x => x.GrabarCriterio(It.IsAny<CriterioIni>())).Returns(new Resultado { Errores = new List<ErrorMessage>() });

            var result = target.Update(criterioNuevo);


            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Criterio\":{\"Id\":0,\"Prioridad\":0,\"PadreId\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void EliminarCriterioTest()
        {
            CriterioIni criterioAEliminar = new CriterioIni { Id = 1, Descripcion = "CriterioEliminarTest" };

            formulaManagerMock.Setup(x => x.EliminarCriterio(It.IsAny<CriterioIni>())).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Eliminar(criterioAEliminar);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Criterio\":{\"Id\":0,\"Prioridad\":0,\"PadreId\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ActualizarDiasFormulaTest()
        {
            var DiasNuevos = new FormulaIni
            {
                CuposDesde = DateTime.Now.Date,
                CuposHasta = DateTime.Now.Date,
                NegociosDesde = DateTime.Now.Date,
                NegociosHasta = DateTime.Now.Date
            };
            formulaManagerMock.Setup(x => x.ActualizarDias(It.IsAny<FormulaIni>())).Returns(new Resultado { Errores = new List<ErrorMessage>() });

            var result = target.ActualizarDiasFormula(DiasNuevos);


            Assert.NotNull(result);

            var a = serializer.Serialize(result);

            var data = (AbmFormulaResult)((JsonResult)result).Data;
            Assert.IsNotNull(data);
        }
        [Test]
        public void ActualizarCierreTest()
        {
            var DiasNuevos = new FormulaIni
            {
                CuposDesde = DateTime.Now.Date,
                CuposHasta = DateTime.Now.Date,
                NegociosDesde = DateTime.Now.Date,
                NegociosHasta = DateTime.Now.Date,
                Cierre = true
            };
            formulaManagerMock.Setup(x => x.ActualizarCierre(It.IsAny<FormulaIni>())).Returns(new Resultado { Errores = new List<ErrorMessage>() });

            var result = target.ActualizarCierre(DiasNuevos);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);

            var data = (AbmFormulaResult)((JsonResult)result).Data;
            Assert.IsNotNull(data);
        }
    }
}
