using Autofac.Extras.NLog;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ComprasManagerTest
    {
        private ComprasManager target;
        private Mock<ILogger> logger;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IComprasAgent> comprasAgentMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            logger = new Mock<ILogger>();
            comprasAgentMock = new Mock<IComprasAgent>();
            target = new ComprasManager(logger.Object, repositorioMock.Object, comprasAgentMock.Object);
        }

        [Test]
        public void ActualizarComprasAyerOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Proveedor, string>>>(), It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<string>() { "a" });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<Comercial>() { new Comercial { IdActiveDirectory = "A" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes { ComercialId = 1, Año = 2020, CampañaMaterialId = 1, CampañaMaterialPorMesId = 1, Mes = 1, NroItem = 1, Toneladas = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<CampañaMaterial>() { new CampañaMaterial { NroItem = 1, CampañaMaterialId = 1, CampañaId = 1, MaterialId = 1, ProveedorId = 1, ToneladasCompradas = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Proveedor, ProveedorBasicoDto>>>(), It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ProveedorBasicoDto>() { new ProveedorBasicoDto { ProveedorId = 1, CUIT = "A" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Campaña, CampañaDto>>>(), It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<CampañaDto>() { new CampañaDto { Descripcion = "19-20", CampañaId = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, MaterialBasicoDto>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<MaterialBasicoDto>() { new MaterialBasicoDto { Codigo = "A", MaterialId = 1 } });
            comprasAgentMock.Setup(y => y.ComprarIniciales(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(new List<CompraAgentDto> { new CompraAgentDto { ANIO = "2020", COSECHA = "19-20", MATERIAL = "A", MES = "1", TN_COMPRADAS = 100, VENDEDOR = "A" } });


            target.ActualizarComprasAyer();

            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<Proveedor, string>>>(), It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<Material, MaterialBasicoDto>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            comprasAgentMock.Verify(y => y.ComprarIniciales(It.IsAny<List<string>>(), It.IsAny<string>()), Times.Once);
            repositorioMock.Verify(y => y.AgregarTodos(It.IsAny<List<CampañaMaterialPorMes>>(), It.IsAny<List<KeyValuePair<string, string>>>()), Times.Once);
            repositorioMock.Verify(y => y.AgregarTodos(It.IsAny<List<CampañaMaterial>>(), It.IsAny<List<KeyValuePair<string, string>>>()), Times.Once);
        }
        [Test]
        public void ActualizarComprasOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Proveedor, string>>>(), It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<string>() { "a" });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<Comercial>() { new Comercial { IdActiveDirectory = "A" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<CampañaMaterialPorMes>() { new CampañaMaterialPorMes { ComercialId = 1, Año = 2020, CampañaMaterialId = 1, CampañaMaterialPorMesId = 1, Mes = 1, NroItem = 1, Toneladas = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<CampañaMaterial>() { new CampañaMaterial { NroItem = 1, CampañaMaterialId = 1, CampañaId = 1, MaterialId = 1, ProveedorId = 1, ToneladasCompradas = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Proveedor, ProveedorBasicoDto>>>(), It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ProveedorBasicoDto>() { new ProveedorBasicoDto { ProveedorId = 1, CUIT = "A" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Campaña, CampañaDto>>>(), It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<CampañaDto>() { new CampañaDto { Descripcion = "19-20", CampañaId = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, MaterialBasicoDto>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<MaterialBasicoDto>() { new MaterialBasicoDto { Codigo = "A", MaterialId = 1 } });
            comprasAgentMock.Setup(y => y.ComprarIniciales(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(new List<CompraAgentDto> { new CompraAgentDto { ANIO = "2020", COSECHA = "19-20", MATERIAL = "A", MES = "1", TN_COMPRADAS = 100, VENDEDOR = "A" } });


            target.ActualizarCompras();

            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<Proveedor, string>>>(), It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterialPorMes, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Exactly(2));
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<CampañaMaterial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Exactly(2));
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<Material, MaterialBasicoDto>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Exactly(2));
            comprasAgentMock.Verify(y => y.ComprarIniciales(It.IsAny<List<string>>(), It.IsAny<string>()), Times.Exactly(2));
            repositorioMock.Verify(y => y.AgregarTodos(It.IsAny<List<CampañaMaterialPorMes>>(), It.IsAny<List<KeyValuePair<string, string>>>()), Times.Exactly(2));
            repositorioMock.Verify(y => y.AgregarTodos(It.IsAny<List<CampañaMaterial>>(), It.IsAny<List<KeyValuePair<string, string>>>()), Times.Exactly(2));
        }
    }
}
