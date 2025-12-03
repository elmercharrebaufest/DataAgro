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
    public class MaterialManagerTest
    {
        private MaterialManager target;
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

            target = new MaterialManager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void TraerFiltroMaterialTestOk()
        {
            var filtro = new ParamAbmMaterial { Codigo = "1", Descripcion = "a" };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, MaterialIni>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List< MaterialIni>() { new MaterialIni { MaterialId = 1, Descripcion="1" } });
            var result = target.TraerFiltroMaterial(filtro);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Material, MaterialIni>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Material.Count);
        }

        [Test]
        public void TraerMaterialOk()
        {
            
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func< Material,  bool >>>(),It.IsAny<Expression<Func<Material, MaterialDto>>>()))
                .Returns( new MaterialDto { MaterialId=1,CampañaId=1,Descripcion="a" } );
            
            var resultado = target.TraerMaterial(1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func< Material, bool >>>(), It.IsAny<Expression<Func<Material, MaterialDto>>>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.MaterialId);
        }
        [Test]
        public void GrabarMaterialOk()
        {
            var prov = new Material { MaterialId = 1, Descripcion = "a",CampañaId=1,Codigo="1" };
            repositorioMock.Setup(y => y.Obtener<Material>(It.IsAny<int>()))
                .Returns( new Material { MaterialId = 1 });

            var resultado = target.GrabarMaterial(prov);
            repositorioMock.Verify(x => x.Obtener<Material>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Material>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void GrabarMaterialNuevaOk()
        {
            var prov = new Material { MaterialId = 0, Descripcion = "a", CampañaId = 1, Codigo = "1" };
            repositorioMock.Setup(y => y.Obtener<Material>(It.IsAny<int>()))
                .Returns(new Material { MaterialId = 1 });

            var resultado = target.GrabarMaterial(prov);
            repositorioMock.Verify(x => x.Obtener<Material>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Material>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void EliminarOperadorOk()
        {
            var resultado = target.EliminarMaterial(1);
            repositorioMock.Verify(x => x.Remover<Material>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void TraerDatosInicialesTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, MaterialCombo>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<MaterialCombo>() { new MaterialCombo { MaterialId = 1, Descripcion = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Campaña, CampaniaCombo>>>(), It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                           .Returns(new List<CampaniaCombo>() { new CampaniaCombo { Descripcion = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Campaña, CampaniaTableroCombo>>>(), It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                          .Returns(new List<CampaniaTableroCombo>() { new CampaniaTableroCombo { Descripcion = "1" } });
            var res = target.TraerDatosIniciales();
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<Material, MaterialCombo>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<Campaña, CampaniaCombo>>>(), It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<Campaña, CampaniaTableroCombo>>>(), It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
        }

        [Test]
        public void TraerTodoMaterialTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, MaterialIni>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                           .Returns(new List<MaterialIni>() { new MaterialIni { MaterialId = 1, Descripcion = "1" } });
            target.TraerTodoMaterial();
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<Material, MaterialIni>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

        }
    }
}
