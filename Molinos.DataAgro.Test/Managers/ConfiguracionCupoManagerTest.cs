using Autofac.Extras.NLog;
using KendoGridBinder;
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
    public class ConfiguracionCupoManagerTest
    {
        private ConfiguracionCupoManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<IAdministracionCuperaAgent> administracionCuperaAgent;
        private Mock<ICupoManager> cupoManager;

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            administracionCuperaAgent = new Mock<IAdministracionCuperaAgent>();
            cupoManager = new Mock<ICupoManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new ConfiguracionCupoManager(logger.Object, repositorioMock.Object, administracionCuperaAgent.Object, cupoManager.Object);
        }
        //[Test]
        //public void GrabarNuevaConfiguracionCupoTest()
        //{
        //    var cupo = new ConfiguracionCupo
        //    {
        //        Id = 0,
        //        CentroId = 1,
        //        MaterialId = 1,
        //        Fecha = new DateTime(2019, 8, 1),
        //        LimiteCupo = 10
        //    };
        //    var diaCupo = new List<DiaCupo> { new DiaCupo { Cantidad = 10, Fecha = DateTime.Now } };
        //    repositorioMock.Setup(x => x.Existe(It.IsAny<Expression<Func<ConfiguracionCupo, bool>>>())).Returns(false);
        //    repositorioMock.Setup(x => x.Agregar(It.IsAny<ConfiguracionCupo>()));
        //    repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ZonaCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
        //      Returns(new List<ZonaCupo> { new ZonaCupo { Id = 1, CodigoSap = "CBA", Descripcion = "Bs" } });
        //    repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
        //        Returns(new List<Material> { new Material { MaterialId = 1, Descripcion = "Bs" } });
        //    repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
        //        Returns(new List<Centro> { new Centro { Id = 1, CodigoSap = "CBA", Descripcion = "Bs" } });
        //    repositorioMock.Setup(x => x.Obtener<ConfiguracionCupo>(It.IsAny<int>())).Returns(cupo);
        //    administracionCuperaAgent.Setup(x => x.AdministrarCupera(It.IsAny<ConfiguracionCupoDto>())).Returns("OK");
        //    repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Negocio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
        //     Returns(new List<Negocio> { new Negocio { Id = 1, Cantidad = 1000 } });
        //    repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
        //     Returns(new List<Contrato> { new Contrato { Id = 1, Cantidad = 100 } });

        //    var resultado = target.GrabarConfiguracionCupo(cupo, diaCupo);
        //    Assert.That(!resultado.HayError);
        //    repositorioMock.Verify(x => x.Agregar(It.IsAny<ConfiguracionCupo>()), Times.Once);
        //    repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        //}
        [Test]
        public void UpdateConfiguracionCupoTest()
        {
            var cupo = new ConfiguracionCupo
            {
                Id = 1,
                CentroId = 1,
                MaterialId = 1,
                Fecha = new DateTime(2019, 8, 1),
                LimiteCupo = 100,
                CantidadCupo = new List<LimiteCupo>
                {
                    new LimiteCupo
                    {
                        CantidadCupo = 100,
                        ConfiguracionCupoId = 1,
                        Id = 1,
                        LimiteAnterior = null,
                        ZonaCupoId = 1,
                    }
                },
                Centro = new Centro {
                    Id = 1,
                    NoPropio = false
                }
            };
            var diaCupo = new List<DiaCupo> { new DiaCupo { Cantidad = 10, Fecha = DateTime.Now } };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ZonaCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
                Returns(new List<ZonaCupo> { new ZonaCupo { Id = 1, CodigoSap = "CBA", Descripcion = "Bs" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
                Returns(new List<Material> { new Material { MaterialId = 1, Descripcion = "Bs" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
                Returns(new List<Centro> { new Centro { Id = 1, CodigoSap = "CBA", Descripcion = "Bs" } });


            repositorioMock.Setup(x => x.Obtener<ConfiguracionCupo>(It.IsAny<int>())).Returns(cupo);
            administracionCuperaAgent.Setup(x => x.AdministrarCupera(It.IsAny<ConfiguracionCupoDto>())).Returns("OK");
            var resultado = target.GrabarConfiguracionCupo(cupo, diaCupo);

            Assert.That(!resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
        [Test]
        public void TraerLimitesTest()
        {
            var cupo = new ConfiguracionCupo
            {
                Id = 1,
                CentroId = 1,
                MaterialId = 1,
                Fecha = new DateTime(2019, 8, 1),
                LimiteCupo = 100,
                CantidadCupo = new List<LimiteCupo>
                {
                    new LimiteCupo
                    {
                        CantidadCupo = 100,
                        ConfiguracionCupoId = 1,
                        Id = 1,
                        LimiteAnterior = null,
                        ZonaCupoId = 1,
                    }
                },
                Centro = new Centro { CodigoSap = "ASF"},
                Material = new Material { Codigo = "aa"}
            };
            var centro = new Centro { 
                Id = 1,                
                NoPropio = false 
            };
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<LimiteCupo, LimiteCupoDto>>>(), It.IsAny<Expression<Func<LimiteCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<LimiteCupoDto>() { new LimiteCupoDto { Id = 1, CantidadCupo = 1, ZonaCupoId = 1, ZonaCupo = "aaa" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<Cupo>() { new Cupo { Id = 1, ZonaCupoId = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ZonaCupo, string>>>(), It.IsAny<Expression<Func<ZonaCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
           Returns(new List<string>() { "CBA" });
            cupoManager.Setup(x => x.TraerCupoDisponibilidad(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>())).Returns(new List<DisponibilidadCuposDto>());
            repositorioMock.Setup(x => x.Obtener<ConfiguracionCupo>(It.IsAny<int>())).Returns(cupo);
            repositorioMock.Setup(x => x.Obtener<Centro>(It.IsAny<int>())).Returns(centro);
            var resultado = target.TraerLimites(1);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<LimiteCupo, LimiteCupoDto>>>(), It.IsAny<Expression<Func<LimiteCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.AreEqual(1, resultado.Count);
        }
        [Test]
        public void GrabarLimitesTest()
        {
            var limites = new List<LimiteCupo>()
            {
                new LimiteCupo{ Id = 0, ZonaCupoId = 1 , ConfiguracionCupoId = 1, CantidadCupo = 10 },
                new LimiteCupo{ Id = 1, ZonaCupoId = 2 , ConfiguracionCupoId = 1, CantidadCupo = 10, LimiteAnterior = 9 }
            };
            var cupo = new ConfiguracionCupo
            {
                CantidadCupo = limites,
                CentroId = 1,
                Id = 1,
                CierreCupera = false,
                MaterialId = 1,
                LimiteCupo = 20,
                Fecha = DateTime.Now,
                Material = new Material { Descripcion = "Soja", Codigo = "000" },
                Centro = new Centro { CodigoSap = "002" }
            };
            repositorioMock.Setup(x => x.Obtener<ConfiguracionCupo>(It.IsAny<int>()))
                .Returns(cupo);
            repositorioMock.Setup(x => x.Agregar(It.IsAny<LimiteCupo>()));
            repositorioMock.Setup(x => x.Obtener<LimiteCupo>(It.IsAny<int>()))
                .Returns(new LimiteCupo { Id = 1, ZonaCupoId = 2, ConfiguracionCupoId = 1, CantidadCupo = 10 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ZonaCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
             Returns(new List<ZonaCupo> { new ZonaCupo { Id = 2, CodigoSap = "CBA", Descripcion = "Bs" }, new ZonaCupo { Id = 1, CodigoSap = "CBA", Descripcion = "Bs" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
                Returns(new List<Material> { new Material { MaterialId = 1, Descripcion = "Bs" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
                Returns(new List<Centro> { new Centro { Id = 1, CodigoSap = "CBA", Descripcion = "Bs" } });
            repositorioMock.Setup(x => x.Obtener<LimiteCupo>(It.IsAny<int>())).Returns(new LimiteCupo());
            administracionCuperaAgent.Setup(x => x.AdministrarCupera(It.IsAny<ConfiguracionCupoDto>())).Returns("OK");
            repositorioMock.Setup(x => x.GuardarCambios());
            var resultado = target.GrabarLimites(limites);

            Assert.That(!resultado.HayError);
        }
        [Test]
        public void TraerConfiguracionCupoTest()
        {

            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<ConfiguracionCupo, bool>>>(), It.IsAny<Expression<Func<ConfiguracionCupo, ConfiguracionCupoDto>>>()))
                .Returns(new ConfiguracionCupoDto { Id = 1, LimiteCupo = 100 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ZonaCupo, string>>>(), It.IsAny<Expression<Func<ZonaCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
            Returns(new List<string>() { "CBA" });
            cupoManager.Setup(x => x.TraerCupoDisponibilidad(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>())).Returns(new List<DisponibilidadCuposDto>());

            var resultado = target.TraerConfiguracionCupo(1);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<ConfiguracionCupo, bool>>>(), It.IsAny<Expression<Func<ConfiguracionCupo, ConfiguracionCupoDto>>>()), Times.Once);
            Assert.AreEqual(1, resultado.Id);
        }

        [Test]
        public void CambioMasivoTest()
        {
            var ids = new List<int> { 1 };
            var cupo = new ConfiguracionCupo
            {
                CentroId = 1,
                Id = 1,
                CierreCupera = false,
                MaterialId = 1,
                LimiteCupo = 100,
                Fecha = DateTime.Now,
                CantidadCupo = new List<LimiteCupo>
                {
                    new LimiteCupo
                    {
                        CantidadCupo = 100,
                        ConfiguracionCupoId = 1,
                        Id = 1,
                        LimiteAnterior = null,
                        ZonaCupoId = 1,
                    }
                }
            };
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<ConfiguracionCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
              .Returns(new List<ConfiguracionCupo>() { cupo });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ZonaCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
             Returns(new List<ZonaCupo> { new ZonaCupo { Id = 1, CodigoSap = "CBA", Descripcion = "Bs" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
                Returns(new List<Material> { new Material { MaterialId = 1, Descripcion = "Bs" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
                Returns(new List<Centro> { new Centro { Id = 1, CodigoSap = "CBA", Descripcion = "Bs" } });
            repositorioMock.Setup(x => x.Obtener<ConfiguracionCupo>(It.IsAny<int>())).Returns(cupo);
            administracionCuperaAgent.Setup(x => x.AdministrarCupera(It.IsAny<ConfiguracionCupoDto>())).Returns("OK");
            var resultado = target.CambioMasivo(true);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void GrabarLimitesMasivoTest()
        {
            var limites = new List<LimiteCupo>()
            {
                new LimiteCupo{ Id = 0, ZonaCupoId = 1 , ConfiguracionCupoId = 1, CantidadCupo = 10,ZonaCupo=new ZonaCupo{Id=1,CodigoSap="",Descripcion="" } },
                new LimiteCupo{ Id = 1, ZonaCupoId = 2 , ConfiguracionCupoId = 1, CantidadCupo = 10, LimiteAnterior = 9,ZonaCupo=new ZonaCupo{Id=1,CodigoSap="",Descripcion="" } }
            };
            var ids = new List<int> { 1 };
            var cupo = new ConfiguracionCupo
            {
                CantidadCupo = limites,
                CentroId = 1,
                Id = 1,
                CierreCupera = false,
                MaterialId = 1,
                LimiteCupo = 20,
                Fecha = DateTime.Now,
                Material = new Material { Descripcion = "Soja", Codigo = "000" },
                Centro = new Centro { CodigoSap = "002" }
            };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ConfiguracionCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
             Returns(new List<ConfiguracionCupo> { cupo });
            repositorioMock.Setup(x => x.Obtener<ConfiguracionCupo>(It.IsAny<int>()))
              .Returns(cupo);
            repositorioMock.Setup(x => x.Agregar(It.IsAny<LimiteCupo>()));
            repositorioMock.Setup(x => x.Obtener<LimiteCupo>(It.IsAny<int>()))
                .Returns(new LimiteCupo { Id = 1, ZonaCupoId = 2, ConfiguracionCupoId = 1, CantidadCupo = 10 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ZonaCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
             Returns(new List<ZonaCupo> { new ZonaCupo { Id = 2, CodigoSap = "CBA", Descripcion = "Bs" }, new ZonaCupo { Id = 1, CodigoSap = "CBA", Descripcion = "Bs" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
                Returns(new List<Material> { new Material { MaterialId = 1, Descripcion = "Bs" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
                Returns(new List<Centro> { new Centro { Id = 1, CodigoSap = "CBA", Descripcion = "Bs" } });
            repositorioMock.Setup(x => x.Obtener<LimiteCupo>(It.IsAny<int>())).Returns(new LimiteCupo());
            administracionCuperaAgent.Setup(x => x.AdministrarCupera(It.IsAny<ConfiguracionCupoDto>())).Returns("OK");
            repositorioMock.Setup(x => x.GuardarCambios());
            var resultado = target.GrabarLimitesMasivo(limites, ids, 1, 1);

            Assert.That(!resultado.HayError);
        }



    }
}