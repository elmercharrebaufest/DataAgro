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
    public class RangoManagerTest
    {
        private RangoManager target;
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

            target = new RangoManager(logger.Object, repositorioMock.Object);
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
            
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, RangoIni>>>(),It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny <int>(),It.IsAny<string>(),It.IsAny<DirOrden>()))
                .Returns(new List<RangoIni>() { new RangoIni { Id=1 } });
            
            var resultado = target.TraerTodoRango();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<RangoPrecio, RangoIni>>>(), It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Rango.Count);
        }
        [Test]
        public void TraerRangoOk()
        {

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<Expression<Func<RangoPrecio, RangoPrecioDto>>>()))
                .Returns( new RangoPrecioDto { Id = 1 });

            var resultado = target.TraerRango(1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<Expression<Func<RangoPrecio, RangoPrecioDto>>>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Id);
        }
        [Test]
        public void GrabarRangoOk()
        {
            var rango = new RangoPrecio { Id = 1, MaterialId = 1, MonedaId = "a", PrecioMaximo = 10, PrecioMinimo = 1 };
            repositorioMock.Setup(y => y.Obtener<RangoPrecio>(It.IsAny<int>()))
                .Returns(new RangoPrecio { Id = 1 });
            repositorioMock.Setup(y => y.Listar( It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<RangoPrecio>());
            var resultado = target.GrabarRango(rango);
            repositorioMock.Verify(x => x.Obtener<RangoPrecio>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            logger.Verify(x => x.Debug(It.IsAny<string>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void GrabarRangoNuevoOk()
        {
            var rango = new RangoPrecio { Id = 0, MaterialId = 1, MonedaId = "a", PrecioMaximo = 10, PrecioMinimo = 1 };
            repositorioMock.Setup(y => y.Obtener<RangoPrecio>(It.IsAny<int>()))
                .Returns(new RangoPrecio { Id = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<RangoPrecio>());
            var resultado = target.GrabarRango(rango);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<RangoPrecio>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<RangoPrecio>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            logger.Verify(x => x.Debug(It.IsAny<string>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void GrabarRangoErrorPrecioMaximo()
        {
            var rango = new RangoPrecio { Id = 1, MaterialId = 1, MonedaId = "a", PrecioMaximo = 0, PrecioMinimo = 1 };
            repositorioMock.Setup(y => y.Obtener<RangoPrecio>(It.IsAny<int>()))
                .Returns(new RangoPrecio { Id = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<RangoPrecio>());
            var resultado = target.GrabarRango(rango);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<RangoPrecio>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<RangoPrecio>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            logger.Verify(x => x.Debug(It.IsAny<string>()), Times.Never);

            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual("El valor máximo no puede ser cero", resultado.ListaErrores[0].Message);
        }
        [Test]
        public void GrabarRangoErrorMoneda()
        {
            var rango = new RangoPrecio { Id = 1, MaterialId = 1,  PrecioMaximo = 10, PrecioMinimo = 1 };
            repositorioMock.Setup(y => y.Obtener<RangoPrecio>(It.IsAny<int>()))
                .Returns(new RangoPrecio { Id = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<RangoPrecio>());
            var resultado = target.GrabarRango(rango);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<RangoPrecio>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<RangoPrecio>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            logger.Verify(x => x.Debug(It.IsAny<string>()), Times.Never);

            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual("El campo Moneda no puede estar vacío", resultado.ListaErrores[0].Message);
        }
        [Test]
        public void GrabarRangoErrorPrecioMinimoMaximo()
        {
            var rango = new RangoPrecio { Id = 1, MaterialId = 1, MonedaId = "a", PrecioMaximo = 10, PrecioMinimo = 100 };
            repositorioMock.Setup(y => y.Obtener<RangoPrecio>(It.IsAny<int>()))
                .Returns(new RangoPrecio { Id = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<RangoPrecio>());
            var resultado = target.GrabarRango(rango);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<RangoPrecio>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<RangoPrecio>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            logger.Verify(x => x.Debug(It.IsAny<string>()), Times.Never);

            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual("El valor mínimo no puede ser mayor que el máximo", resultado.ListaErrores[0].Message);
        }
        [Test]
        public void GrabarRangoErrorMaterial()
        {
            var rango = new RangoPrecio { Id = 1, MonedaId = "a", PrecioMaximo = 10, PrecioMinimo = 1 };
            repositorioMock.Setup(y => y.Obtener<RangoPrecio>(It.IsAny<int>()))
                .Returns(new RangoPrecio { Id = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<RangoPrecio>());
            var resultado = target.GrabarRango(rango);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<RangoPrecio>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<RangoPrecio>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            logger.Verify(x => x.Debug(It.IsAny<string>()), Times.Never);

            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual("El campo Material no puede estar vacío", resultado.ListaErrores[0].Message);
        }
        [Test]
        public void GrabarRangoErrorRangoexistente()
        {
            var rango = new RangoPrecio { Id = 0, MaterialId = 1, MonedaId = "a", PrecioMaximo = 10, PrecioMinimo = 1 };
            repositorioMock.Setup(y => y.Obtener<RangoPrecio>(It.IsAny<int>()))
                .Returns(new RangoPrecio { Id = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<RangoPrecio>() { new RangoPrecio { MaterialId=1,MonedaId="a", Id =2 } });
            var resultado = target.GrabarRango(rango);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<RangoPrecio>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<RangoPrecio>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            logger.Verify(x=>x.Debug(It.IsAny<string>()),Times.Never);
            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual("Ya existe un rango para el grano y moneda elegidos", resultado.ListaErrores[0].Message);
        }
        [Test]
        public void EliminarRangoOk()
        {
            var resultado = target.EliminarRango(1);
            repositorioMock.Verify(x => x.Remover<RangoPrecio>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            logger.Verify(x => x.Debug(It.IsAny<string>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
    }
}
