using NLog;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class CompraNetManagerTest
    {
        private CompraNetManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();

            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            target = new CompraNetManager(logger.Object, repositorioMock.Object);
        }
        [Test]
        public void TraerDatosInicialesTest()
        {            
            repositorioMock.Setup(y=>y.Listar(It.IsAny<Expression<Func<ProveedorComercial, ProveedorCombo>>>(), It.IsAny<Expression<Func<ProveedorComercial, bool>>>(),It.IsAny<int>(),It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ProveedorCombo>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, ComercialCombo>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
               .Returns(new List<ComercialCombo>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, MaterialCombo>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
              .Returns(new List<MaterialCombo>());
            repositorioMock.Setup(y => y.Listar( It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
              .Returns(new List<Provincia>()); 
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Localidad, LocalidadCombo>>>(), It.IsAny<Expression<Func<Localidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
               .Returns(new List<LocalidadCombo>());
            var resultado = target.TraerDatosIniciales(new List<int>());

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ProveedorComercial, ProveedorCombo>>>(), It.IsAny<Expression<Func<ProveedorComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Comercial, ComercialCombo>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Material, MaterialCombo>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Localidad, LocalidadCombo>>>(), It.IsAny<Expression<Func<Localidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.IsNotNull(resultado);            
        }

        [Test]
        public void GrabarSuscripcionTestAgregar()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SuscripcionComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<SuscripcionComercial>());

            var resultado = target.GrabarSuscripcion("a",1);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<SuscripcionComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<SuscripcionComercial>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.HayError);
        }
        [Test]
        public void GrabarSuscripcionTestRemover()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SuscripcionComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<SuscripcionComercial>() { new SuscripcionComercial()});

            var resultado = target.GrabarSuscripcion("", 1);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<SuscripcionComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<SuscripcionComercial>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.HayError);
        }
        [Test]
        public void UsuarioSuscriptoTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SuscripcionComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<SuscripcionComercial>());

            var resultado = target.UsuarioSuscripto(1);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<SuscripcionComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);            
            Assert.IsNotNull(resultado);
        }
    }
}