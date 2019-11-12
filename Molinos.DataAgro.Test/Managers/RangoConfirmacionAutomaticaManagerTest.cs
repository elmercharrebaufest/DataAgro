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

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class RangoConfirmacionAutomaticaManagerTest
    {
        private RangoConfirmacionAutomaticaManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            target = new RangoConfirmacionAutomaticaManager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void TraerDatosInicialesTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, MaterialCombo>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<MaterialCombo>() { new MaterialCombo { Descripcion = "1", MaterialId = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Moneda, MonedaQry>>>(), It.IsAny<Expression<Func<Moneda, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<MonedaQry>() { new MonedaQry { Descripcion = "1", MonedaId="1" } });
            var result = target.TraerDatosIniciales();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Material, MaterialCombo>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Moneda, MonedaQry>>>(), It.IsAny<Expression<Func<Moneda, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Material.Count);
            Assert.AreEqual(1, result.Moneda.Count);
        }

        [Test]
        public void TraerTodoRangoOk()
        {
            
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoConfirmacionAutomatica, RangoConfirmacionAutomaticaIni>>>(),It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny <int>(),It.IsAny<string>(),It.IsAny<DirOrden>()))
                .Returns(new List<RangoConfirmacionAutomaticaIni>() { new RangoConfirmacionAutomaticaIni { Id=1 } });
            
            var resultado = target.TraerTodoRango();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<RangoConfirmacionAutomatica, RangoConfirmacionAutomaticaIni>>>(), It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Rango.Count);
        }
        [Test]
        public void TraerRangoOk()
        {

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny<Expression<Func<RangoConfirmacionAutomatica, RangoConfirmacionAutomaticaDto>>>()))
                .Returns( new RangoConfirmacionAutomaticaDto { Id = 1 });

            var resultado = target.TraerRango(1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny<Expression<Func<RangoConfirmacionAutomatica, RangoConfirmacionAutomaticaDto>>>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Id);
        }
        [Test]
        public void GrabarRangoOk()
        {
            var rango = new RangoConfirmacionAutomatica { Id = 1, MaterialId = 1, MonedaId = "a", PrecioMaximo = 10, PrecioMinimo = 1,
            Cantidad=1,
            HastaAnio=1,
            DesdeAnio=1,
            HastaMes=1,
            DesdeMes=1,
            ZonaId=1,
            FechaHasta=new DateTime(2019,11,11),
            FechaDesde= new DateTime(2019, 11, 11)};
            repositorioMock.Setup(y => y.Obtener<RangoConfirmacionAutomatica>(It.IsAny<int>()))
                .Returns(new RangoConfirmacionAutomatica { Id = 1 });
            repositorioMock.Setup(y => y.Listar( It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<RangoConfirmacionAutomatica>());
            var resultado = target.GrabarRangoConfirmacionAutomatica(rango);
            repositorioMock.Verify(x => x.Obtener<RangoConfirmacionAutomatica>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            logger.Verify(x => x.Debug(It.IsAny<string>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void GrabarRangoNuevoOk()
        {
            var rango = new RangoConfirmacionAutomatica
            {
                Id = 0,
                MaterialId = 1,
                MonedaId = "a",
                PrecioMaximo = 10,
                PrecioMinimo = 1,
                Cantidad = 1,
                HastaAnio = 1,
                DesdeAnio = 1,
                HastaMes = 1,
                DesdeMes = 1,
                ZonaId = 1,
                FechaHasta = new DateTime(2019, 11, 11),
                FechaDesde = new DateTime(2019, 11, 11)
            };
            repositorioMock.Setup(y => y.Obtener<RangoConfirmacionAutomatica>(It.IsAny<int>()))
                .Returns(new RangoConfirmacionAutomatica { Id = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<RangoConfirmacionAutomatica>());
            var resultado = target.GrabarRangoConfirmacionAutomatica(rango);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<RangoConfirmacionAutomatica>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<RangoConfirmacionAutomatica>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            logger.Verify(x => x.Debug(It.IsAny<string>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void GrabarRangoErrorPrecioMaximo()
        {
            var rango = new RangoConfirmacionAutomatica { Id = 1, MaterialId = 1, MonedaId = "a", PrecioMaximo = 0, PrecioMinimo = 1 };
            repositorioMock.Setup(y => y.Obtener<RangoConfirmacionAutomatica>(It.IsAny<int>()))
                .Returns(new RangoConfirmacionAutomatica { Id = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<RangoConfirmacionAutomatica>());
            var resultado = target.GrabarRangoConfirmacionAutomatica(rango);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<RangoConfirmacionAutomatica>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<RangoConfirmacionAutomatica>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            logger.Verify(x => x.Debug(It.IsAny<string>()), Times.Never);

            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual("El valor máximo no puede ser cero", resultado.ListaErrores[0].Message);
        }
        [Test]
        public void GrabarRangoErrorMoneda()
        {
            var rango = new RangoConfirmacionAutomatica { Id = 1, MaterialId = 1,  PrecioMaximo = 10, PrecioMinimo = 1 };
            repositorioMock.Setup(y => y.Obtener<RangoConfirmacionAutomatica>(It.IsAny<int>()))
                .Returns(new RangoConfirmacionAutomatica { Id = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<RangoConfirmacionAutomatica>());
            var resultado = target.GrabarRangoConfirmacionAutomatica(rango);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<RangoConfirmacionAutomatica>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<RangoConfirmacionAutomatica>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            logger.Verify(x => x.Debug(It.IsAny<string>()), Times.Never);

            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual("El campo Moneda no puede estar vacío", resultado.ListaErrores[0].Message);
        }
        [Test]
        public void GrabarRangoErrorPrecioMinimoMaximo()
        {
            var rango = new RangoConfirmacionAutomatica { Id = 1, MaterialId = 1, MonedaId = "a", PrecioMaximo = 10, PrecioMinimo = 100 };
            repositorioMock.Setup(y => y.Obtener<RangoConfirmacionAutomatica>(It.IsAny<int>()))
                .Returns(new RangoConfirmacionAutomatica { Id = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<RangoConfirmacionAutomatica>());
            var resultado = target.GrabarRangoConfirmacionAutomatica(rango);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<RangoConfirmacionAutomatica>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<RangoConfirmacionAutomatica>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            logger.Verify(x => x.Debug(It.IsAny<string>()), Times.Never);

            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual("El valor minimo no puede ser mayor que el máximo", resultado.ListaErrores[0].Message);
        }
        [Test]
        public void GrabarRangoErrorMaterial()
        {
            var rango = new RangoConfirmacionAutomatica {};
            repositorioMock.Setup(y => y.Obtener<RangoConfirmacionAutomatica>(It.IsAny<int>()))
                .Returns(new RangoConfirmacionAutomatica { Id = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<RangoConfirmacionAutomatica>());
            var resultado = target.GrabarRangoConfirmacionAutomatica(rango);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<RangoConfirmacionAutomatica>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<RangoConfirmacionAutomatica>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            logger.Verify(x => x.Debug(It.IsAny<string>()), Times.Never);

            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual(8, resultado.Errores.Count);
        }
        [Test]
        public void GrabarRangoErrorRangoexistente()
        {
            var fecha = new DateTime(2019, 04, 25);
            var rango = new RangoConfirmacionAutomatica
            {
                Id = 0,
                MaterialId = 1,
                MonedaId = "a",
                PrecioMaximo = 10,
                PrecioMinimo = 1,
                Cantidad = 1,
                HastaAnio = 1,
                DesdeAnio = 1,
                HastaMes = 1,
                DesdeMes = 1,
                ZonaId = 1,
                FechaHasta = new DateTime(2019, 04, 25),
                FechaDesde = new DateTime(2019, 04, 25)
            };
            repositorioMock.Setup(y => y.Obtener<RangoConfirmacionAutomatica>(It.IsAny<int>()))
                .Returns(new RangoConfirmacionAutomatica { Id = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<RangoConfirmacionAutomatica>() { new RangoConfirmacionAutomatica {
                    Id=1,
                    MaterialId = 1,
                MonedaId = "a",
                PrecioMaximo = 10,
                PrecioMinimo = 1,
                Cantidad = 1,
                HastaAnio = 1,
                DesdeAnio = 1,
                HastaMes = 1,
                DesdeMes = 1,
                ZonaId = 1,
                FechaHasta = new DateTime(2019, 04, 25),
                FechaDesde = new DateTime(2019, 04, 25) } });
            var resultado = target.GrabarRangoConfirmacionAutomatica(rango);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<RangoConfirmacionAutomatica>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<RangoConfirmacionAutomatica>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            logger.Verify(x=>x.Debug(It.IsAny<string>()),Times.Never);
            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual("Ya existe un rango para los valores seleccionados", resultado.ListaErrores[0].Message);
        }
        [Test]
        public void EliminarRangoOk()
        {
            var resultado = target.EliminarRangoConfirmacionAutomatica(1);
            repositorioMock.Verify(x => x.Remover<RangoConfirmacionAutomatica>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            logger.Verify(x => x.Debug(It.IsAny<string>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
    }
}
