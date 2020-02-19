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
    public class LocalidadManagerTest
    {
        private LocalidadManager target;
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

            target = new LocalidadManager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void TraerDatosInicialesTestOk()
        {
            repositorioMock.Setup(y => y.Listar( It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<Provincia>() { new Provincia { ProvinciaId = 1, Nombre="1" } });
            var result = target.TraerDatosIniciales();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Provincia.Count);
        }
        [Test]
        public void TraerFiltroLocalidadTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Localidad, LocalidadIni>>>(), It.IsAny<Expression<Func<Localidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<LocalidadIni>() { new LocalidadIni { LocalidadId = 1, Nombre = "1" } });
            var result = target.TraerFiltroLocalidad(new ParamAbmLocalidad());
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Localidad, LocalidadIni>>>(), It.IsAny<Expression<Func<Localidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Localidad.Count);
        }
        [Test]
        public void TraerLocalidadPorProvinciaTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Localidad, LocalidadIni>>>(), It.IsAny<Expression<Func<Localidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<LocalidadIni>() { new LocalidadIni { LocalidadId = 1, Nombre = "1" } });
            var result = target.TraerLocalidadPorProvincia(1);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Localidad, LocalidadIni>>>(), It.IsAny<Expression<Func<Localidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Localidad.Count);
        }
        [Test]
        public void TraerLocalidadOk()
        {
            
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Localidad, bool>>>(),It.IsAny<Expression<Func<Localidad, LocalidadDto>>>()))
                .Returns( new LocalidadDto { LocalidadId=1 } );
            
            var resultado = target.TraerLocalidad(1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Localidad, bool>>>(), It.IsAny<Expression<Func<Localidad, LocalidadDto>>>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.LocalidadId);
        }
        [Test]
        public void GrabarLocalidadOk()
        {
            var prov = new Provincia { ProvinciaId = 1, Nombre = "a", Orden = 1 };
            var loc = new Localidad { LocalidadId = 1, Nombre = "a", ProvinciaId = 1, CodLocalidad = "1", Provincia = prov };
            repositorioMock.Setup(y => y.Obtener<Localidad>(It.IsAny<int>()))
                .Returns( new Localidad { LocalidadId = 1 });

            var resultado = target.GrabarLocalidad(loc);
            repositorioMock.Verify(x => x.Obtener<Localidad>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Localidad>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void GrabarLocalidadNuevaOk()
        {
            var prov = new Provincia { ProvinciaId = 1, Nombre = "a", Orden = 1 };
            var loc = new Localidad { LocalidadId = 0, Nombre = "a", ProvinciaId = 1, CodLocalidad = "1", Provincia = prov };
            repositorioMock.Setup(y => y.Obtener<Localidad>(It.IsAny<int>()))
                .Returns(new Localidad { LocalidadId = 1 });

            var resultado = target.GrabarLocalidad(loc);
            repositorioMock.Verify(x => x.Obtener<Localidad>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Localidad>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void EliminarLocalidadOk()
        {
            var resultado = target.EliminarLocalidad(1);
            repositorioMock.Verify(x => x.Remover<Localidad>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void ListarLocalidadOk()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Localidad, LocalidadDto>>>(), It.IsAny<Expression<Func<Localidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<LocalidadDto>() { new LocalidadDto { Nombre = "a", LocalidadId = 1 } });
            var resultado = target.ListarLocalidad("a");
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Localidad, LocalidadDto>>>(), It.IsAny<Expression<Func<Localidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1,resultado.Count);
        }
        [Test]
        public void DevolverLocalidadesOk()
        {
            repositorioMock.Setup(x => x.SelStore<BusquedaLocalidad>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(new List<BusquedaLocalidad>() { new BusquedaLocalidad { ProvinciaId = 1, Localidad="a",Provincia="a",Filtro="a",Id=1 } });
            var resultado = target.DevolverLocalidades("a");
            repositorioMock.Verify(x => x.SelStore<BusquedaLocalidad>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Count);
        }
        [Test]
        public void TraerLocalidadProvinciaOk()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Localidad, bool>>>(), It.IsAny<Expression<Func<Localidad, LocalidadQry>>>()))
                .Returns( new LocalidadQry { LocalidadId=1 ,ProvinciaId=1});
            var resultado = target.TraerLocalidadProvincia("a","a");
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Localidad, bool>>>(), It.IsAny<Expression<Func<Localidad, LocalidadQry>>>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.LocalidadId);
        }
    }
}
