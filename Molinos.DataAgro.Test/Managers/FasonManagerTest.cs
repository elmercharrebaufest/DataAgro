using Autofac.Extras.NLog;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using WebDataAgro.Models;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class FasonManagerTest
    {
        private FasonManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<IProveedorManager> proveedorManagerMock;
        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            proveedorManagerMock = new Mock<IProveedorManager>();

            target = new FasonManager(logger.Object, repositorioMock.Object, proveedorManagerMock.Object);
        }

        [Test]
        public void GrabarFasonOk()
        {
            var oFason = new Fason
            {
                Id = 0,
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1000,
                Precio = 1000,
                MonedaId = "ARP  ",
                TipoFasonId = 1,
                CampanaId = 1,
                Posicion = "01/2019",
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now
            };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<RangoPrecio>() { });

            var result = target.GrabarFason(oFason);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<Fason>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void GrabarFasonError()
        {
            var oFason = new Fason
            {
                Id = 0,
                ProveedorId = 0,
                MaterialId = 0,
                Cantidad = 0,
                Precio = 0,
                TipoFasonId = 0,
                CampanaId = 0,
                Posicion = ""
            };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<RangoPrecio>() { new RangoPrecio { MaterialId = 0, MonedaId = null, PrecioMinimo = 100 } });

            var result = target.GrabarFason(oFason);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<Fason>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.That(result.HayError);
        }

        [Test]
        public void UpdateFasonOk()
        {
            var oFason = new Fason
            {
                Id = 1,
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1000,
                Precio = 1000,
                MonedaId = "ARP  ",
                TipoFasonId = 1,
                CampanaId = 1,
                Posicion = "01/2019",
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                ComercialCreadorId = 1,
                ComercialId = 2,
                TrigoEspecial = true,
                Estado = new EstadoContrato { EstadoContratoId = 1 }
            };

            repositorioMock.Setup(y => y.Obtener<Fason>(It.IsAny<int>())).Returns(oFason);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<RangoPrecio>() { });

            var result = target.GrabarFason(oFason);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<Fason>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
        [Test]
        public void UpdateFasonError()
        {
            var oFason = new Fason
            {
                Id = 1,
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1000,
                Precio = 1000,
                MonedaId = "ARP  ",
                TipoFasonId = 1,
                CampanaId = 1,
                Posicion = "01/2019",
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                ComercialCreadorId = 1,
                ComercialId = 2,
                TrigoEspecial = true,
                Estado = new EstadoContrato { EstadoContratoId = 5 }
            };

            repositorioMock.Setup(y => y.Obtener<Fason>(It.IsAny<int>())).Returns(oFason);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<RangoPrecio>() { });

            var result = target.GrabarFason(oFason);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<Fason>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.That(result.HayError);
        }


        [Test]
        public void FinalizarFasonOk()
        {
            var oFason = new Fason
            {
                Ampliaciones = 2,
                Id = 0,
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1000,
                Precio = 1000,
                MonedaId = "ARP  ",
                TipoFasonId = 1,
                CampanaId = 1,
                Posicion = "01/2019",
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                EstadoId = (int)EnumEstadoContrato.Confirmado,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Confirmado }
            };

            repositorioMock.Setup(y => y.Obtener<Fason>(It.IsAny<int>())).Returns(oFason);

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<RangoPrecio>() { });
            repositorioMock.Setup(y => y.Obtener<EstadoContrato>(It.IsAny<int>())).Returns(new EstadoContrato { });

            var result = target.FinalizarFason(It.IsAny<int>());

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.That(!result.HayError);
        }

        [Test]
        public void FinalizarFasonConErrorOk()
        {
            var oFason = new Fason
            {
                Ampliaciones = 2,
                Id = 0,
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1000,
                Precio = 1000,
                MonedaId = "ARP  ",
                TipoFasonId = 1,
                CampanaId = 1,
                Posicion = "01/2019",
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                EstadoId = (int)EnumEstadoContrato.Confirmado,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Confirmado }
            };

            repositorioMock.Setup(y => y.Obtener<Fason>(It.IsAny<int>())).Returns(oFason);

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<RangoPrecio>() { });
            repositorioMock.Setup(y => y.Obtener<EstadoContrato>(5)).Throws(new Exception("Error obtener estado"));
            repositorioMock.Setup(y => y.Obtener<EstadoContrato>(4)).Returns(new EstadoContrato { });

            var result = target.FinalizarFason(It.IsAny<int>());

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.That(result.HayError);
        }
        [Test]
        public void FinalizarFasonErrorEstadoFinalizado()
        {
            var oFason = new Fason
            {
                Ampliaciones = 2,
                Id = 0,
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1000,
                Precio = 1000,
                MonedaId = "ARP  ",
                TipoFasonId = 1,
                CampanaId = 1,
                Posicion = "01/2019",
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                EstadoId = (int)EnumEstadoContrato.Finalizado,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Finalizado }
            };

            repositorioMock.Setup(y => y.Obtener<Fason>(It.IsAny<int>())).Returns(oFason);

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<RangoPrecio>() { });

            var result = target.FinalizarFason(It.IsAny<int>());

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.That(result.HayError);
        }
        [Test]
        public void FinalizarFasonErrorEstadoRechazado()
        {
            var oFason = new Fason
            {
                Ampliaciones = 2,
                Id = 0,
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1000,
                Precio = 1000,
                MonedaId = "ARP  ",
                TipoFasonId = 1,
                CampanaId = 1,
                Posicion = "01/2019",
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                EstadoId = (int)EnumEstadoContrato.Rechazado,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Rechazado }
            };

            repositorioMock.Setup(y => y.Obtener<Fason>(It.IsAny<int>())).Returns(oFason);

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<RangoPrecio>() { });

            var result = target.FinalizarFason(It.IsAny<int>());

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.That(result.HayError);
        }

        [Test]
        public void BorrarFasonOk()
        {
            var oFason = new Fason
            {
                Ampliaciones = 2,
                Id = 0,
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1000,
                Precio = 1000,
                MonedaId = "ARP  ",
                TipoFasonId = 1,
                CampanaId = 1,
                Posicion = "01/2019",
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                EstadoId = (int)EnumEstadoContrato.Confirmado,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Confirmado },
                MotivoRechazo = "test"
            };

            repositorioMock.Setup(y => y.Obtener<Fason>(It.IsAny<int>())).Returns(oFason);

            repositorioMock.Setup(y => y.Obtener<EstadoContrato>(It.IsAny<int>())).Returns(new EstadoContrato { });

            var result = target.BorrarFason(oFason);

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.That(!result.HayError);
        }
        [Test]
        public void BorrarFasonError()
        {
            var oFason = new Fason
            {
                Ampliaciones = 2,
                Id = 0,
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1000,
                Precio = 1000,
                MonedaId = "ARP  ",
                TipoFasonId = 1,
                CampanaId = 1,
                Posicion = "01/2019",
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                EstadoId = (int)EnumEstadoContrato.Confirmado,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Confirmado }
            };

            repositorioMock.Setup(y => y.Obtener<Fason>(It.IsAny<int>())).Returns(oFason);
            repositorioMock.Setup(y => y.Obtener<EstadoContrato>(It.IsAny<int>())).Returns(new EstadoContrato { });
            repositorioMock.Setup(y => y.GuardarCambios()).Throws(new Exception("Error guardar cambios"));

            var result = target.BorrarFason(oFason);
            Assert.That(result.HayError);
        }
        [Test]
        public void BorrarFasonErrorEstado()
        {
            var oFason = new Fason
            {
                Ampliaciones = 2,
                Id = 0,
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1000,
                Precio = 1000,
                MonedaId = "ARP  ",
                TipoFasonId = 1,
                CampanaId = 1,
                Posicion = "01/2019",
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                EstadoId = (int)EnumEstadoContrato.Rechazado,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Rechazado }
            };

            repositorioMock.Setup(y => y.Obtener<Fason>(It.IsAny<int>())).Returns(oFason);

            var result = target.BorrarFason(oFason);
            Assert.That(result.HayError);
        }

        [Test]
        public void TraerFasonOk()
        {
            repositorioMock.Setup(y => y.Obtener<Fason, BasicoContrato>(It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<Expression<Func<Fason, BasicoContrato>>>()))
                .Returns(new BasicoContrato
                {
                    Id = 2
                });

            var result = target.TraerFason(It.IsAny<int>());
            Assert.That(result.Id == 2);
        }

        [Test]
        public void GrabarAmpliacionFasonOk()
        {
            var oFason = new Fason
            {
                Id = 0,
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1000,
                Precio = 1000,
                MonedaId = "ARP  ",
                TipoFasonId = 1,
                CampanaId = 1,
                Posicion = "01/2019",
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                Ampliaciones = 2,
                Estado = new EstadoContrato { EstadoContratoId = 1 }
            };

            repositorioMock.Setup(y => y.Obtener<Fason>(It.IsAny<int>())).Returns(oFason);

            var result = target.GrabarAmpliacionFason(oFason);
            Assert.That(!result.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void GrabarAmpliacionFasonErrorEstado()
        {
            var oFason = new Fason
            {
                Id = 0,
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1000,
                Precio = 1000,
                MonedaId = "ARP  ",
                TipoFasonId = 1,
                CampanaId = 1,
                Posicion = "01/2019",
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                Ampliaciones = 2,
                Estado = new EstadoContrato { EstadoContratoId = 6}
            };

            repositorioMock.Setup(y => y.Obtener<Fason>(It.IsAny<int>())).Returns(oFason);

            var result = target.GrabarAmpliacionFason(oFason);
            Assert.That(result.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }
    }
}
