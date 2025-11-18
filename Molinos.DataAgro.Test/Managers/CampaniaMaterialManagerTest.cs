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
    public class CampaniaMaterialManagerTest
    {
        private CampaniaMaterialManager target;
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

            target = new CampaniaMaterialManager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void TraerCampañasPorGranoTestOk()
        {
            var fecha = new DateTime(2019, 10, 01);
            var campania = new List<CampaniaMaterialSAPDTO>() {
                new CampaniaMaterialSAPDTO{ CUIT="1", Campania="1-2", Material="a",Comercial="a", Mes="enero",Anio=2019,Toneladas=100},
            new CampaniaMaterialSAPDTO{ CUIT="1", Campania="1-2", Material="a",Comercial="a", Mes="febrero",Anio=2019,Toneladas=100},
            new CampaniaMaterialSAPDTO{ CUIT="1", Campania="1-2", Material="a",Comercial="a", Mes="marzo",Anio=2019,Toneladas=100},
            new CampaniaMaterialSAPDTO{ CUIT="1", Campania="1-2", Material="a",Comercial="a", Mes="abril",Anio=2019,Toneladas=100},
            new CampaniaMaterialSAPDTO{ CUIT="1", Campania="1-2", Material="a",Comercial="a", Mes="mayo",Anio=2019,Toneladas=100},
            new CampaniaMaterialSAPDTO{ CUIT="1", Campania="1-2", Material="a",Comercial="a", Mes="junio",Anio=2019,Toneladas=100},
            new CampaniaMaterialSAPDTO{ CUIT="1", Campania="1-2", Material="a",Comercial="a", Mes="julio",Anio=2019,Toneladas=100},
            new CampaniaMaterialSAPDTO{ CUIT="1", Campania="1-2", Material="a",Comercial="a", Mes="agosto",Anio=2019,Toneladas=100},
            new CampaniaMaterialSAPDTO{ CUIT="1", Campania="1-2", Material="a",Comercial="a", Mes="septiembre",Anio=2019,Toneladas=100},
            new CampaniaMaterialSAPDTO{ CUIT="1", Campania="1-2", Material="a",Comercial="a", Mes="octubre",Anio=2019,Toneladas=100},
            new CampaniaMaterialSAPDTO{ CUIT="1", Campania="1-2", Material="a",Comercial="a", Mes="noviembre",Anio=2019,Toneladas=100},
            new CampaniaMaterialSAPDTO{ CUIT="1", Campania="1-2", Material="a",Comercial="a", Mes="diciembre",Anio=2019,Toneladas=100},};

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                            .Returns(new Proveedor { ProveedorId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>()))
                            .Returns(new Campaña { CampañaId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>()))
                            .Returns(new Material { MaterialId = 1, CampañaId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                            .Returns(new Comercial { ComercialId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CampañaMaterial, bool>>>()))
                            .Returns(new CampañaMaterial { CampañaMaterialId = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterialPorMes, double>>>(), It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>(),It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<double>() { 10,10,80});
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>()))
                            .Returns(new CampañaMaterialPorMes { ComercialId = 1 });

            var result = target.TraerCampañasPorGrano(campania);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Exactly(12));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>()), Times.Exactly(12));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Material, bool>>>()), Times.Exactly(12));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Exactly(12));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<CampañaMaterial, bool>>>()), Times.Exactly(12));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>()), Times.Exactly(12));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CampañaMaterialPorMes, double>>>(), It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Exactly(12));
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(12));

            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }
        [Test]
        public void TraerCampañasPorGranoTestSinCampaniaMaterialOk()
        {
            var fecha = new DateTime(2019, 10, 01);
            var campania = new List<CampaniaMaterialSAPDTO>() {
                new CampaniaMaterialSAPDTO{ CUIT="1", Campania="1-2", Material="a",Comercial="a", Mes="enero",Anio=2019,Toneladas=100}};

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                            .Returns(new Proveedor { ProveedorId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>()))
                            .Returns(new Campaña { CampañaId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>()))
                            .Returns(new Material { MaterialId = 1, CampañaId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                            .Returns(new Comercial { ComercialId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CampañaMaterial, bool>>>()));
            repositorioMock.Setup(y => y.Agregar(It.IsAny<CampañaMaterial>()))
                           .Returns(new CampañaMaterial { CampañaMaterialId = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterialPorMes, double>>>(), It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()));
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>()));

            var result = target.TraerCampañasPorGrano(campania);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Material, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<CampañaMaterial, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CampañaMaterialPorMes, double>>>(), It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<CampañaMaterial>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<CampañaMaterialPorMes>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }
        [Test]
        public void TraerCampañasPorGranoTestErrorCuit()
        {
            var fecha = new DateTime(2019, 10, 01);
            var campania = new List<CampaniaMaterialSAPDTO>() { new CampaniaMaterialSAPDTO{ CUIT="1"} };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()));
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>()))
                            .Returns(new Campaña { CampañaId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>()))
                            .Returns(new Material { MaterialId = 1, CampañaId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                            .Returns(new Comercial { ComercialId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CampañaMaterial, bool>>>()))
                            .Returns(new CampañaMaterial { CampañaMaterialId = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterialPorMes, double>>>(), It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<double>() { 10, 10, 80 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>()))
                            .Returns(new CampañaMaterialPorMes { ComercialId = 1 });

            var result = target.TraerCampañasPorGrano(campania);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Material, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<CampañaMaterial, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CampañaMaterialPorMes, double>>>(), It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual("No existe el CUIT",result.ListaErrores[0].Message);
        }
        [Test]
        public void TraerCampañasPorGranoTestErrorCampania()
        {
            var fecha = new DateTime(2019, 10, 01);
            var campania = new List<CampaniaMaterialSAPDTO>() { new CampaniaMaterialSAPDTO {CUIT = "a",Campania="1-2" } };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                            .Returns(new Proveedor { ProveedorId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>()));
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>()))
                            .Returns(new Material { MaterialId = 1, CampañaId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                            .Returns(new Comercial { ComercialId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CampañaMaterial, bool>>>()))
                            .Returns(new CampañaMaterial { CampañaMaterialId = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterialPorMes, double>>>(), It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<double>() { 10, 10, 80 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>()))
                            .Returns(new CampañaMaterialPorMes { ComercialId = 1 });

            var result = target.TraerCampañasPorGrano(campania);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Material, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<CampañaMaterial, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CampañaMaterialPorMes, double>>>(), It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual("No existe la Campaña.", result.ListaErrores[0].Message);
        }
        [Test]
        public void TraerCampañasPorGranoTestErrorMaterial()
        {
            var fecha = new DateTime(2019, 10, 01);
            var campania = new List<CampaniaMaterialSAPDTO>() { new CampaniaMaterialSAPDTO { CUIT = "a",Campania="1-2",Material="a" } };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                            .Returns(new Proveedor { ProveedorId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>()))
                            .Returns(new Campaña { CampañaId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>()));
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()))
                            .Returns(new Comercial { ComercialId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CampañaMaterial, bool>>>()))
                            .Returns(new CampañaMaterial { CampañaMaterialId = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterialPorMes, double>>>(), It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<double>() { 10, 10, 80 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>()))
                            .Returns(new CampañaMaterialPorMes { ComercialId = 1 });

            var result = target.TraerCampañasPorGrano(campania);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Material, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<CampañaMaterial, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CampañaMaterialPorMes, double>>>(), It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual("No existe el Material.", result.ListaErrores[0].Message);
        }
        [Test]
        public void TraerCampañasPorGranoTestErrorComercial()
        {
            var fecha = new DateTime(2019, 10, 01);
            var campania = new List<CampaniaMaterialSAPDTO>() { new CampaniaMaterialSAPDTO { CUIT = "a", Campania = "1-2", Material = "a",Comercial="a" } };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                            .Returns(new Proveedor { ProveedorId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>()))
                            .Returns(new Campaña { CampañaId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>()))
                            .Returns(new Material { MaterialId = 1, CampañaId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()));
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CampañaMaterial, bool>>>()))
                            .Returns(new CampañaMaterial { CampañaMaterialId = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterialPorMes, double>>>(), It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<double>() { 10, 10, 80 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>()))
                            .Returns(new CampañaMaterialPorMes { ComercialId = 1 });

            var result = target.TraerCampañasPorGrano(campania);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Material, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<CampañaMaterial, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CampañaMaterialPorMes, double>>>(), It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual("No existe el comercial.", result.ListaErrores[0].Message);
        }
    }
}
