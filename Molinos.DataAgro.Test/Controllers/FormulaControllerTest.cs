using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces.Managers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    class FormulaControllerTest
    {
        private FormulaController target;
        private Mock<IFormulaManager> formulaManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            formulaManagerMock = new Mock<IFormulaManager>();
            target = new FormulaController(formulaManagerMock.Object);
        }
        [Test]
        public void InicializarTest()
        {
            formulaManagerMock.Setup(x => x.todosLosCriterios()).Returns(new List<CriterioIni>()
            {
                new CriterioIni{ Id=0,Descripcion="CriterioRaiz",DisplayName="Criterios" },
                new CriterioIni{ Id=0,Descripcion="CriterioEsFason",DisplayName="Fason" }
            });
            formulaManagerMock.Setup(x => x.ultimaFormula()).Returns(new ResultIniFormula()
            {
                Formula = new FormulaIni
                {
                    Id = 1,
                    Inicio = 1,
                    CantDias = 6,
                    CriterioId = 1
                }
            });
            var result = target.Inicializar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":{\"ultimaFormulaTraida\":{\"Formula\":{\"Id\":1,\"Criterio\":null,\"Inicio\":1,\"CantDias\":6,\"CriterioId\":1,\"Fecha\":\"\\/Date(-62135586000000)\\/\"}},\"Criterios\":{\"Criterios\":[{\"Id\":0,\"Padre\":null,\"PadreId\":null,\"Hijos\":null,\"Prioridad\":0,\"Descripcion\":\"CriterioRaiz\",\"Concreta\":false,\"DisplayName\":\"Criterios\"},{\"Id\":0,\"Padre\":null,\"PadreId\":null,\"Hijos\":null,\"Prioridad\":0,\"Descripcion\":\"CriterioEsFason\",\"Concreta\":false,\"DisplayName\":\"Fason\"}]}},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void BuscarTest()
        {
            formulaManagerMock.Setup(x => x.TraerCriteriosGuardados()).Returns(new ResultIniCriterio
            {

                Criterios = new List<CriterioIni> { new CriterioIni { Id = 1, Descripcion = "CriterioRaizTest" } }
            });
            var result = target.Buscar();

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

            formulaManagerMock.Setup(x => x.eliminarCriterio(It.IsAny<CriterioIni>())).Returns(new Resultado { Errores = new List<ErrorMessage>() });
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
                Inicio = 1,
                CantDias = 5
            };
            formulaManagerMock.Setup(x => x.actualizarDias(It.IsAny<FormulaIni>())).Returns(new Resultado { Errores = new List<ErrorMessage>() });

            var result = target.ActualizarDiasFormula(DiasNuevos);


            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Formula\":{\"Id\":0,\"Criterio\":null,\"Inicio\":0,\"CantDias\":0},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
