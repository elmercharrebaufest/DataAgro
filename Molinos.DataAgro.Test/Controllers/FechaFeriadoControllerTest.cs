using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WebDataAgro.Controllers;
using WebDataAgro.Models;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class FechaFeriadoControllerTest
    {
        private FechaFeriadoController target;
        private Mock<IFechaFeriadoManager> fechaFeriadoManagerMock;

        [SetUp]
        public void SetUp()
        {
            fechaFeriadoManagerMock = new Mock<IFechaFeriadoManager>();
            target = new FechaFeriadoController(fechaFeriadoManagerMock.Object);
        }

        // ------------------------------------------------------------------ //
        //  Buscar
        // ------------------------------------------------------------------ //

        [Test]
        public void BuscarFechaFeriadoTest_RetornaJsonConListaDeFeriados()
        {
            var feriado = new FechaFeriadoDto { Id = 1, Feriado = new DateTime(2020, 1, 1) };
            fechaFeriadoManagerMock.Setup(x => x.TraerTodo())
                .Returns(new List<FechaFeriadoDto> { feriado });

            var result = target.Buscar() as JsonResult;

            Assert.NotNull(result);
            var data = result.Data as ResultIniFechaFeriadoModel;
            Assert.NotNull(data);
            Assert.AreEqual(1, data.Datos.Count);
            Assert.AreEqual(1, data.Datos[0].Id);
            Assert.AreEqual(new DateTime(2020, 1, 1), data.Datos[0].Feriado);
            Assert.IsFalse(data.HayError);
            fechaFeriadoManagerMock.Verify(x => x.TraerTodo(), Times.Once);
        }

        [Test]
        public void BuscarFechaFeriadoTest_ListaVaciaRetornaSinErrores()
        {
            fechaFeriadoManagerMock.Setup(x => x.TraerTodo())
                .Returns(new List<FechaFeriadoDto>());

            var result = target.Buscar() as JsonResult;

            Assert.NotNull(result);
            var data = result.Data as ResultIniFechaFeriadoModel;
            Assert.NotNull(data);
            Assert.IsEmpty(data.Datos);
            Assert.IsFalse(data.HayError);
        }

        [Test]
        public void BuscarFechaFeriadoTest_ManagerRetornaNull_RetornaListaVacia()
        {
            fechaFeriadoManagerMock.Setup(x => x.TraerTodo())
                .Returns((List<FechaFeriadoDto>)null);

            var result = target.Buscar() as JsonResult;

            Assert.NotNull(result);
            var data = result.Data as ResultIniFechaFeriadoModel;
            Assert.NotNull(data);
            Assert.IsEmpty(data.Datos);
        }

        // ------------------------------------------------------------------ //
        //  Aplicar
        // ------------------------------------------------------------------ //

        [Test]
        public void AplicarFechaFeriadoTest_RetornaFechaFeriadoCorrectamente()
        {
            var dto = new FechaFeriadoDto { Id = 1, Feriado = new DateTime(2020, 1, 1) };
            fechaFeriadoManagerMock.Setup(x => x.Traer(1)).Returns(dto);

            var result = target.Aplicar(new AbmFechaFeriadoParam { Id = 1 }) as JsonResult;

            Assert.NotNull(result);
            var data = result.Data as AbmFechaFeriadoResult;
            Assert.NotNull(data);
            Assert.AreEqual(1, data.FechaFeriado.Id);
            Assert.AreEqual(new DateTime(2020, 1, 1), data.FechaFeriado.Feriado);
            Assert.IsFalse(data.HayError);
            fechaFeriadoManagerMock.Verify(x => x.Traer(1), Times.Once);
        }

        [Test]
        public void AplicarFechaFeriadoTest_IdInexistente_RetornaFeriadoNull()
        {
            fechaFeriadoManagerMock.Setup(x => x.Traer(It.IsAny<int>()))
                .Returns((FechaFeriadoDto)null);

            var result = target.Aplicar(new AbmFechaFeriadoParam { Id = 99 }) as JsonResult;

            Assert.NotNull(result);
            var data = result.Data as AbmFechaFeriadoResult;
            Assert.NotNull(data);
            Assert.IsNull(data.FechaFeriado);
        }

        // ------------------------------------------------------------------ //
        //  Grabar
        // ------------------------------------------------------------------ //

        [Test]
        public void GrabarFechaFeriadoTest_SinErrores_NoRetornaFechaFeriadoEnBody()
        {
            var entidad = new FechaFeriado { Id = 1, Feriado = new DateTime(2020, 1, 1) };
            fechaFeriadoManagerMock.Setup(x => x.Grabar(entidad))
                .Returns(new Resultado { Errores = new List<ErrorMessage>() });

            var result = target.Grabar(entidad) as JsonResult;

            Assert.NotNull(result);
            var data = result.Data as AbmFechaFeriadoResult;
            Assert.NotNull(data);
            Assert.IsFalse(data.HayError);
            Assert.IsEmpty(data.Errores);
            // Sin errores el controller NO asigna FechaFeriado, queda en default (Id = 0)
            Assert.AreEqual(0, data.FechaFeriado.Id);
            fechaFeriadoManagerMock.Verify(x => x.Grabar(entidad), Times.Once);
        }

        [Test]
        public void GrabarFechaFeriadoTest_ConErrores_RetornaFechaFeriadoYErrores()
        {
            var entidad = new FechaFeriado { Id = 5, Feriado = new DateTime(2020, 6, 15) };
            fechaFeriadoManagerMock.Setup(x => x.Grabar(entidad))
                .Returns(new Resultado
                {
                    Errores = new List<ErrorMessage> { new ErrorMessage(400, "Fecha duplicada") }
                });

            var result = target.Grabar(entidad) as JsonResult;

            Assert.NotNull(result);
            var data = result.Data as AbmFechaFeriadoResult;
            Assert.NotNull(data);
            Assert.IsTrue(data.HayError);
            Assert.AreEqual(1, data.Errores.Count);
            Assert.AreEqual(entidad.Feriado, data.FechaFeriado.Feriado);
        }

        // ------------------------------------------------------------------ //
        //  Eliminar
        // ------------------------------------------------------------------ //

        [Test]
        public void EliminarFechaFeriadoTest_EliminaCorrectamente()
        {
            fechaFeriadoManagerMock.Setup(x => x.Eliminar(1))
                .Returns(new Resultado { Errores = new List<ErrorMessage>() });

            var result = target.Eliminar(new AbmFechaFeriadoParam { Id = 1 }) as JsonResult;

            Assert.NotNull(result);
            var data = result.Data as Resultado;
            Assert.NotNull(data);
            Assert.IsFalse(data.HayError);
            fechaFeriadoManagerMock.Verify(x => x.Eliminar(1), Times.Once);
        }

        [Test]
        public void EliminarFechaFeriadoTest_ConErrores_RetornaErrores()
        {
            fechaFeriadoManagerMock.Setup(x => x.Eliminar(It.IsAny<int>()))
                .Returns(new Resultado
                {
                    Errores = new List<ErrorMessage> { new ErrorMessage(404, "No encontrado") }
                });

            var result = target.Eliminar(new AbmFechaFeriadoParam { Id = 99 }) as JsonResult;

            Assert.NotNull(result);
            var data = result.Data as Resultado;
            Assert.IsTrue(data.HayError);
            Assert.AreEqual(1, data.Errores.Count);
        }

        // ------------------------------------------------------------------ //
        //  Cancelar
        // ------------------------------------------------------------------ //

        [Test]
        public void CancelarFechaFeriadoTest_RetornaModeloVacioSinErrores()
        {
            var result = target.Cancelar() as JsonResult;

            Assert.NotNull(result);
            var data = result.Data as AbmFechaFeriadoResult;
            Assert.NotNull(data);
            Assert.IsFalse(data.HayError);
            Assert.IsEmpty(data.Errores);
            // FechaFeriado inicializado por defecto (Id = 0)
            Assert.AreEqual(0, data.FechaFeriado.Id);
            // El manager nunca debe ser invocado al cancelar
            fechaFeriadoManagerMock.Verify(x => x.Eliminar(It.IsAny<int>()), Times.Never);
            fechaFeriadoManagerMock.Verify(x => x.Grabar(It.IsAny<FechaFeriado>()), Times.Never);
        }

        // ------------------------------------------------------------------ //
        //  Index
        // ------------------------------------------------------------------ //

        [Test]
        public void Index_RetornaView()
        {
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
    }
}
