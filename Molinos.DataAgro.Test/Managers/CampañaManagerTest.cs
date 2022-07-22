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
    public class CampañaManagerTest
    {
        private CampañaManager target;
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

            target = new CampañaManager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void TraerCampaniaTestOk()
        {
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<Expression<Func<Campaña, CampañaDto>>>()))
                            .Returns(new CampañaDto { CampañaId = 1, Descripcion = "a" });
            var result = target.TraerCampania(1);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<Expression<Func<Campaña, CampañaDto>>>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.CampañaId);
        }
        [Test]
        public void TraerCampañasActivasTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, CampañaDto>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<CampañaDto>() { new CampañaDto { CampañaId = 1, Descripcion = "a" } });
            var result = target.TraerCampañasActivas();

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Material, CampañaDto>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
        }
        [Test]
        public void TraerMaterialPorCampañaTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, MaterialDto>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<MaterialDto>() { new MaterialDto { CampañaId = 1, Descripcion = "a" } });
            var result = target.TraerMaterialPorCampaña(1);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Material, MaterialDto>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
        }
        [Test]
        public void TraerCampañaHomeTestOkMenorA5()
        {
            repositorioMock.Setup(y => y.ListarConsulta(It.IsAny<TraerComprasHome>()))
                            .Returns(new List<MaterialCampaña>() { new MaterialCampaña{  Campaña="1-2", Nombre="a",Toneladas=10} });
            var result = target.TraerCampañaHome(1, new List<int>() { 1, 2 });

            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<TraerComprasHome>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Materiales.Count);
        }
        [Test]
        public void TraerCampañaHomeTestOkMayorA5()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<Material>() { 
                                new Material { MaterialId = 1, Descripcion = "a", Campaña = new Campaña { Descripcion = "1-2" } },
                                new Material { MaterialId = 1, Descripcion = "b", Campaña = new Campaña { Descripcion = "1-2" } },
                                new Material { MaterialId = 1, Descripcion = "c", Campaña = new Campaña { Descripcion = "1-2" } },
                                new Material { MaterialId = 1, Descripcion = "d", Campaña = new Campaña { Descripcion = "1-2" } }
                            });
            repositorioMock.Setup(y => y.ListarConsulta(It.IsAny<TraerComprasHome>()))
                            .Returns(new List<MaterialCampaña>() {
                            new MaterialCampaña { Campaña = "1-2", Nombre = "a", Toneladas = 10 },
                            new MaterialCampaña { Campaña = "1-2", Nombre = "b", Toneladas = 10 },
                            new MaterialCampaña { Campaña = "1-2", Nombre = "c", Toneladas = 10 },
                            new MaterialCampaña { Campaña = "1-2", Nombre = "d", Toneladas = 10 },
                            new MaterialCampaña { Campaña = "1-2", Nombre = "e", Toneladas = 10 },
                            new MaterialCampaña { Campaña = "1-2", Nombre = "f", Toneladas = 10 }});
            var result = target.TraerCampañaHome(1, new List<int>() { 1, 2 });

            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<TraerComprasHome>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(5, result.Materiales.Count);
        }
        [Test]
        public void TraerCampañasPorGranoTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterialHistorico, CampañaDto>>>(), It.IsAny<Expression<Func<CampañaMaterialHistorico, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<CampañaDto>() { new CampañaDto { CampañaId = 1, Descripcion = "a" } });
            var result = target.TraerCampañasPorGrano(1);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CampañaMaterialHistorico, CampañaDto>>>(), It.IsAny<Expression<Func<CampañaMaterialHistorico, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
        }
        [Test]
        public void TraerCampañaPorMaterialTestOk()
        {
            ConfigurationManager.AppSettings["CampanaDesde"] = "1";
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, int>>>()))
                            .Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func< Campaña, bool >>>(), It.IsAny<Expression<Func<Campaña, int>>>()))
                            .Returns(1);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, CampañaDto>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<CampañaDto>() { new CampañaDto { CampañaId = 1, Descripcion = "a" } });
            var result = target.TraerCampañaPorMaterial(1);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, int>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<Expression<Func<Campaña, int>>>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CampañaMaterial, CampañaDto>>>(), It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
        }
        [Test]
        public void TraerCalidadPorMaterialOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CalidadEspecial, CalidadEspecialDto>>>(), It.IsAny<Expression<Func<CalidadEspecial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<CalidadEspecialDto>() { new CalidadEspecialDto { Id = 1, Descripcion = "a" } });
            var result = target.TraerCalidadPorMaterial(3);
            
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CalidadEspecial, CalidadEspecialDto>>>(), It.IsAny<Expression<Func<CalidadEspecial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(3, result.Count);
        }
    }
}