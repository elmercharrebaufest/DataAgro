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
    public class FijacionDePrecioContratoManagerTest
    {
        private Mock<ILogDataAgroManager> logDataAgroManagerMock;
        private FijacionDePrecioContratoManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<IComercialManager> comercialManagerMock;
        private Mock<IPushNotificationManager> pushNotificacionManagerMock;
        private Mock<IProveedorManager> proveedorManagerMock;
        private Mock<IFinalizarFijacionAgent> finalizarFijacionAgentMock;
        private Mock<IRelacionCorredorProveedorAgent> relacionCorredorProveedorAgentMock;
        private Mock<IContratosParaFijacionAgent> contratosParaFijacionMock;
        private Mock<IMailManager> mailManagerMock;

        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            comercialManagerMock = new Mock<IComercialManager>();
            pushNotificacionManagerMock = new Mock<IPushNotificationManager>();
            proveedorManagerMock = new Mock<IProveedorManager>();
            finalizarFijacionAgentMock = new Mock<IFinalizarFijacionAgent>();
            relacionCorredorProveedorAgentMock = new Mock<IRelacionCorredorProveedorAgent>();
            contratosParaFijacionMock = new Mock<IContratosParaFijacionAgent>();
            mailManagerMock = new Mock<IMailManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            logDataAgroManagerMock = new Mock<ILogDataAgroManager>();

            target = new FijacionDePrecioContratoManager(logger.Object, repositorioMock.Object,
                proveedorManagerMock.Object, comercialManagerMock.Object,
                pushNotificacionManagerMock.Object, finalizarFijacionAgentMock.Object,
                contratosParaFijacionMock.Object, relacionCorredorProveedorAgentMock.Object,
                mailManagerMock.Object, logDataAgroManagerMock.Object);
        }

        [Test]
        public void TraerDatosInicialesOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, MaterialQry>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<MaterialQry>() { new MaterialQry { MaterialId = 1, Descripcion = "TRIGO" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Moneda, MonedaQry>>>(), It.IsAny<Expression<Func<Moneda, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<MonedaQry>() { new MonedaQry { MonedaId = "AUS ", Descripcion = "AUSTRAL" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, ComercialQry>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<ComercialQry>() { new ComercialQry { ComercialId = 1, Comercial = "MARCO ANTONIO" } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Proveedor, ProveedorQry>>>(), It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<ProveedorQry>() { new ProveedorQry { ProveedorId = 1, Descripcion = "OP SD" } });

            var result = target.TraerDatosIniciales();

            Assert.NotNull(result);
        }



        //[Test]
        //public void BasicoFijacionPrecioContratoTraerPorFiltroOk()
        //{
        //    repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, FijacionDePrecioContratoIni>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
        //        .Returns(new List<FijacionDePrecioContratoIni>() {
        //                    new FijacionDePrecioContratoIni {
        //                                     FijacionDePrecioContratoId = 1,
        //                                    ContratoId = 1,
        //                                    Proveedor = "HOLA",
        //                                    Fecha = "Fecha",
        //                                    Comercial = "Marc",
        //                                    Material = "SOja",
        //                                    Cantidad = 1,
        //                                    MonedaId = "ARS ",
        //                                    Ampliaciones = 2,
        //                                    Estado = "PENDIENTE",
        //                                    Observacion = "soy una observacion chiquita"
        //                                                    } });


        //    var result = target.BasicoFijacionPrecioContratoTraerPorFiltro(It.IsAny<int>());

        //    Assert.NotNull(result);
        //}

        [Test]
        public void GrabarAmpliacionFijacionOk()
        {
            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(
                        new FijacionDePrecioContrato
                        {
                            Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Confirmado, Descripcion = "Confirmado" },
                            Ampliaciones = 2
                        });

            repositorioMock.Setup(y => y.Obtener<EstadoContrato>(It.IsAny<int>())).Returns(
                       new EstadoContrato
                       {
                           EstadoContratoId = (int)EnumEstadoContrato.Pendiente,
                           Descripcion = "Pendiente"

                       });

            var result = target.GrabarAmpliacionFijacion(new FijacionDePrecioContrato { Id = It.IsAny<int>(), Ampliaciones = 2 });

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.That(!result.HayError);
        }

        [Test]
        public void GrabarAmpliacionFijacionErrorEstado()
        {
            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(
                        new FijacionDePrecioContrato
                        {
                            Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Finalizado, Descripcion = "Finalizado" },
                            Ampliaciones = 2
                        });
            var result = target.GrabarAmpliacionFijacion(new FijacionDePrecioContrato { Id = It.IsAny<int>(), Ampliaciones = 2 });

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.That(result.HayError);
        }


        [Test]
        public void GrabarFijacionDePrecioOk()
        {

            var fijacion = new FijacionDePrecioContrato
            {
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1,
                MonedaId = "ARP  ",
                ComercialId = 1,
                ContratoSAP = "1234",
                CampanaId = 1
            };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
              .Returns(new List<RangoPrecio>() { new RangoPrecio { MonedaId = "AUS ", MaterialId = 2, PrecioMaximo = 20000, PrecioMinimo = 0 } });

            contratosParaFijacionMock.Setup(y => y.ObtenerContratos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<DatosFijacionDeContratoDto>() { new DatosFijacionDeContratoDto { ContratoId = "1234", KilosPendiente = "10", KilosContrato = "100", Calidad = true } });
            repositorioMock.Setup(y => y.Obtener<Contrato, int>(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, int>>>())).Returns(1);

            var result = target.GrabarFijacionDePrecio(fijacion);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<FijacionDePrecioContrato>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.That(!result.HayError);
        }
        [Test]
        public void GrabarFijacionDePrecioOkDos()
        {

            var fijacion = new FijacionDePrecioContrato
            {
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1,
                MonedaId = "ARP  ",
                ComercialId = 1,
                ContratoSAP = "1234",
                CampanaId = 1
            };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
              .Returns(new List<RangoPrecio>() { new RangoPrecio { MonedaId = "AUS ", MaterialId = 2, PrecioMaximo = 20000, PrecioMinimo = 0 } });
            contratosParaFijacionMock.Setup(y => y.ObtenerContratos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
               .Returns(new List<DatosFijacionDeContratoDto>() { new DatosFijacionDeContratoDto { ContratoId = "1234", KilosPendiente = "10", KilosContrato = "100", Calidad = true } });

            repositorioMock.Setup(y => y.Obtener<Contrato, int>(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, int>>>())).Returns(0);

            var result = target.GrabarFijacionDePrecio(fijacion);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<FijacionDePrecioContrato>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.That(!result.HayError);
        }

        [Test]
        public void UpdateFijacionDePrecioOk()
        {

            var fijacion = new FijacionDePrecioContrato
            {
                Id = 1,
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1,
                MonedaId = "ARP  ",
                ComercialId = 1,
                ContratoSAP = "1234",
                CampanaId = 1,
                Fecha = DateTime.Now,
                Posicion = "",
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                AperturaPrecio = new List<AperturaPrecio> { }
            };
            var fijacionSave = new FijacionDePrecioContrato
            {
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 2,
                MonedaId = "ARP  ",
                ComercialId = 1,
                ContratoSAP = "1234",
                CampanaId = 1,
                EstadoId = (int)EnumEstadoContrato.Confirmado
            };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
              .Returns(new List<RangoPrecio>() { new RangoPrecio { MonedaId = "AUS ", MaterialId = 2, PrecioMaximo = 20000, PrecioMinimo = 0 } });

            repositorioMock.Setup(y => y.Obtener<Contrato, int>(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, int>>>())).Returns(0);
            contratosParaFijacionMock.Setup(y => y.ObtenerContratos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
               .Returns(new List<DatosFijacionDeContratoDto>() { new DatosFijacionDeContratoDto { ContratoId = "1234", KilosPendiente = "10", KilosContrato = "100", Calidad = true } });

            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(fijacionSave);

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
             .Returns(new List<AperturaPrecio>());

            var result = target.GrabarFijacionDePrecio(fijacion);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<FijacionDePrecioContrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.That(!result.HayError);
        }


        [Test]
        public void UpdateFijacionDePrecioerrorEstado()
        {

            var fijacion = new FijacionDePrecioContrato
            {
                Id = 1,
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 1,
                MonedaId = "ARP  ",
                ComercialId = 1,
                ContratoSAP = "1234",
                CampanaId = 1,
                Fecha = DateTime.Now,
                Posicion = "",
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                AperturaPrecio = new List<AperturaPrecio> { }
            };
            var fijacionSave = new FijacionDePrecioContrato
            {
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 2,
                MonedaId = "ARP  ",
                ComercialId = 1,
                ContratoSAP = "1234",
                CampanaId = 1,
                EstadoId = (int)EnumEstadoContrato.Finalizado
            };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
              .Returns(new List<RangoPrecio>() { new RangoPrecio { MonedaId = "AUS ", MaterialId = 2, PrecioMaximo = 20000, PrecioMinimo = 0 } });

            repositorioMock.Setup(y => y.Obtener<Contrato, int>(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, int>>>())).Returns(0);

            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(fijacionSave);
            contratosParaFijacionMock.Setup(y => y.ObtenerContratos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
               .Returns(new List<DatosFijacionDeContratoDto>() { new DatosFijacionDeContratoDto { ContratoId = "1234", KilosPendiente = "10", KilosContrato = "100", Calidad = true } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
             .Returns(new List<AperturaPrecio>());

            var result = target.GrabarFijacionDePrecio(fijacion);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<FijacionDePrecioContrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.That(result.HayError);
        }


        [Test]
        public void GrabarFijacionDePrecioError()
        {

            var fijacion = new FijacionDePrecioContrato
            {
                ProveedorId = 0,
                MaterialId = 0,
                Cantidad = 0,
                Precio = 0,
                MonedaId = null,
                ComercialId = 0,
                ContratoSAP = "1",
                CampanaId = 0,
            };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
              .Returns(new List<RangoPrecio>() { new RangoPrecio { MonedaId = null, MaterialId = 0, PrecioMaximo = 20000, PrecioMinimo = 10 } });
            contratosParaFijacionMock.Setup(y => y.ObtenerContratos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<DatosFijacionDeContratoDto>() { new DatosFijacionDeContratoDto { ContratoId = "1", KilosPendiente = "10", KilosContrato = "100", Calidad = true } });
            repositorioMock.Setup(y => y.Obtener<Contrato, int>(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, int>>>())).Returns(1);

            var result = target.GrabarFijacionDePrecio(fijacion);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<FijacionDePrecioContrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.That(result.HayError);
        }

        [Test]
        public void ConfirmarFijacionOk()
        {

            var fijacionSave = new FijacionDePrecioContrato
            {
                Ampliaciones = 1,
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 2,
                MonedaId = "ARP  ",
                ComercialId = 1,
                ContratoSAP = "1234",
                CampanaId = 1,
                EstadoId = (int)EnumEstadoContrato.Confirmado,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Pendiente },
                Comercial = new Comercial { ComercialId = 1 }
            };

            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(fijacionSave);
            repositorioMock.Setup(y => y.Obtener<EstadoContrato>(It.IsAny<int>())).Returns(new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Confirmado });
            comercialManagerMock.Setup(y => y.CadenaComerciales(It.IsAny<int>())).Returns(new List<int> { 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SuscripcionComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<SuscripcionComercial>() { new SuscripcionComercial { ComercialId = 1, Id = 2, Key = "HOLA" } });

            var result = target.ConfirmarFijacion(It.IsAny<int>());

            repositorioMock.Verify(x => x.Agregar(It.IsAny<FijacionDePrecioContrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.That(!result.HayError);
        }

        [Test]
        public void ConfirmarFijacionErrorEstado()
        {

            var fijacionSave = new FijacionDePrecioContrato
            {
                Ampliaciones = 1,
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 2,
                MonedaId = "ARP  ",
                ComercialId = 1,
                ContratoSAP = "1234",
                CampanaId = 1,
                EstadoId = (int)EnumEstadoContrato.Confirmado,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Confirmado },
                Comercial = new Comercial { ComercialId = 1 }
            };
            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(fijacionSave);

            repositorioMock.Setup(y => y.Obtener<EstadoContrato>(It.IsAny<int>())).Returns(new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Confirmado });

            var result = target.ConfirmarFijacion(It.IsAny<int>());

            repositorioMock.Verify(x => x.Agregar(It.IsAny<FijacionDePrecioContrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.That(result.HayError);
        }

        [Test]
        public void FinalizarFijacionOk()
        {

            var fijacionSave = new FijacionDePrecioContrato
            {
                Proveedor = new Proveedor { CUIT = "11" },
                Corredor = new Proveedor { CUIT = "22" },
                CorredorId = 1,
                Ampliaciones = 1,
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 2,
                MonedaId = "ARP  ",
                ComercialId = 1,
                ContratoSAP = "1234",
                CampanaId = 1,
                EstadoId = (int)EnumEstadoContrato.Finalizado,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Confirmado },
                Comercial = new Comercial { ComercialId = 1 }
            };

            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(fijacionSave);
            repositorioMock.Setup(y => y.Obtener<EstadoContrato>(It.IsAny<int>())).Returns(new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Finalizado });
            comercialManagerMock.Setup(y => y.CadenaComerciales(It.IsAny<int>())).Returns(new List<int> { 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SuscripcionComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<SuscripcionComercial>() { new SuscripcionComercial { ComercialId = 1, Id = 2, Key = "HOLA" } });

            relacionCorredorProveedorAgentMock.Setup(y => y.ObtenerRelacionCorredorProveedor(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ConceptoAperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<ConceptoAperturaPrecio>() { new ConceptoAperturaPrecio { CodigoSap = "RE", Descripcion = "Redespacho", Id = 2 } });

            finalizarFijacionAgentMock.Setup(y => y.Finalizar(It.IsAny<FijacionDePrecioContrato>())).Returns("OK");

            var result = target.FinalizarFijacion(It.IsAny<int>(), It.IsAny<string>());

            repositorioMock.Verify(x => x.Agregar(It.IsAny<FijacionDePrecioContrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.That(!result.HayError);
        }

        [Test]
        public void FinalizarFijacionErrorCorredorProveedor()
        {

            var fijacionSave = new FijacionDePrecioContrato
            {
                Proveedor = new Proveedor { CUIT = "11" },
                Corredor = new Proveedor { CUIT = "22" },
                CorredorId = 1,
                Ampliaciones = 1,
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 2,
                MonedaId = "ARP  ",
                ComercialId = 1,
                ContratoSAP = "1234",
                CampanaId = 1,
                EstadoId = (int)EnumEstadoContrato.Finalizado,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Confirmado },
                Comercial = new Comercial { ComercialId = 1 }
            };

            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(fijacionSave);
            relacionCorredorProveedorAgentMock.Setup(y => y.ObtenerRelacionCorredorProveedor(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var result = target.FinalizarFijacion(It.IsAny<int>(), It.IsAny<string>());

            repositorioMock.Verify(x => x.Agregar(It.IsAny<FijacionDePrecioContrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.That(result.HayError);
        }

        [Test]
        public void FinalizarFijacionOkConAperturaDePrecio()
        {

            var fijacionSave = new FijacionDePrecioContrato
            {
                Proveedor = new Proveedor { CUIT = "11" },
                Ampliaciones = 1,
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 2,
                MonedaId = "ARP  ",
                ComercialId = 1,
                ContratoSAP = "1234",
                CampanaId = 1,
                EstadoId = (int)EnumEstadoContrato.Finalizado,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Confirmado },
                Comercial = new Comercial { ComercialId = 1 }
            };

            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(fijacionSave);
            repositorioMock.Setup(y => y.Obtener<EstadoContrato>(It.IsAny<int>())).Returns(new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Finalizado });
            comercialManagerMock.Setup(y => y.CadenaComerciales(It.IsAny<int>())).Returns(new List<int> { 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SuscripcionComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<SuscripcionComercial>() { new SuscripcionComercial { ComercialId = 1, Id = 2, Key = "HOLA" } });

            relacionCorredorProveedorAgentMock.Setup(y => y.ObtenerRelacionCorredorProveedor(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<AperturaPrecio>());



            finalizarFijacionAgentMock.Setup(y => y.Finalizar(It.IsAny<FijacionDePrecioContrato>())).Returns("");

            var result = target.FinalizarFijacion(It.IsAny<int>(), It.IsAny<string>());

            repositorioMock.Verify(x => x.Agregar(It.IsAny<FijacionDePrecioContrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.That(!result.HayError);
        }

        [Test]
        public void FinalizarFijacionErrorFinalizarSap()
        {

            var fijacionSave = new FijacionDePrecioContrato
            {
                Proveedor = new Proveedor { CUIT = "11" },
                Ampliaciones = 1,
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 2,
                MonedaId = "ARP  ",
                ComercialId = 1,
                ContratoSAP = "1234",
                CampanaId = 1,
                EstadoId = (int)EnumEstadoContrato.Finalizado,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Confirmado },
                Comercial = new Comercial { ComercialId = 1 }
            };

            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(fijacionSave);
            repositorioMock.Setup(y => y.Obtener<EstadoContrato>(It.IsAny<int>())).Returns(new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Finalizado });
            comercialManagerMock.Setup(y => y.CadenaComerciales(It.IsAny<int>())).Returns(new List<int> { 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SuscripcionComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<SuscripcionComercial>() { new SuscripcionComercial { ComercialId = 1, Id = 2, Key = "HOLA" } });

            relacionCorredorProveedorAgentMock.Setup(y => y.ObtenerRelacionCorredorProveedor(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<AperturaPrecio>());



            finalizarFijacionAgentMock.Setup(y => y.Finalizar(It.IsAny<FijacionDePrecioContrato>())).Throws(new Exception("error finalizar sap"));

            var result = target.FinalizarFijacion(It.IsAny<int>(), It.IsAny<string>());

            repositorioMock.Verify(x => x.Agregar(It.IsAny<FijacionDePrecioContrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.That(result.HayError);
        }


        [Test]
        public void FinalizarFijacionErrorEstadoFinalizado()
        {

            var fijacionSave = new FijacionDePrecioContrato
            {
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Finalizado },
            };

            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(fijacionSave);
            var result = target.FinalizarFijacion(It.IsAny<int>(), It.IsAny<string>());

            repositorioMock.Verify(x => x.Agregar(It.IsAny<FijacionDePrecioContrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.That(result.HayError);
        }

        [Test]
        public void FinalizarFijacionErrorEstadoRechazado()
        {

            var fijacionSave = new FijacionDePrecioContrato
            {
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Rechazado },
            };

            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(fijacionSave);
            var result = target.FinalizarFijacion(It.IsAny<int>(), It.IsAny<string>());

            repositorioMock.Verify(x => x.Agregar(It.IsAny<FijacionDePrecioContrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.That(result.HayError);
        }

        [Test]
        public void FinalizarFijacionErrorEstadoPendiente()
        {

            var fijacionSave = new FijacionDePrecioContrato
            {
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Pendiente },
            };

            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(fijacionSave);
            var result = target.FinalizarFijacion(It.IsAny<int>(), It.IsAny<string>());

            repositorioMock.Verify(x => x.Agregar(It.IsAny<FijacionDePrecioContrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.That(result.HayError);
        }

        [Test]
        public void BorrarFijacionOk()
        {

            var fijacionSave = new FijacionDePrecioContrato
            {
                Id = 1,
                EstadoId = (int)EnumEstadoContrato.Rechazado,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Pendiente },
                Comercial = new Comercial { ComercialId = 1 },
                MotivoRechazo = "test"
            };

            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(fijacionSave);
            repositorioMock.Setup(y => y.Obtener<EstadoContrato>(It.IsAny<int>())).Returns(new EstadoContrato { Descripcion = "Rechazado", EstadoContratoId = (int)EnumEstadoContrato.Rechazado });

            comercialManagerMock.Setup(y => y.CadenaComerciales(It.IsAny<int>())).Returns(new List<int> { 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SuscripcionComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<SuscripcionComercial>() { new SuscripcionComercial { ComercialId = 1, Id = 2, Key = "HOLA" } });

            var result = target.BorrarFijacion(fijacionSave);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<FijacionDePrecioContrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.That(!result.HayError);
        }
        [Test]
        public void TraerFijacionTestOk()
        {

            var fijacionSave = new FijacionDePrecioContrato
            {
                Id = 1,
                EstadoId = (int)EnumEstadoContrato.Rechazado,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Pendiente },
                Comercial = new Comercial { ComercialId = 1 }
            };

            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(fijacionSave);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, double>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<double>() { 10000 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, BasicoContrato>>>()))
                .Returns(new BasicoContrato()
                {
                    ContratoId = 1,
                    DatosFijacion = new DatosFijacionDeContratoDto
                    {
                        ContratoId = "00011111",
                        FechaDesde = "2019/10/30",
                        FechaHasta = "2019/11/30",
                        KilosAplicados = "1111",
                        KilosPendiente = "1111"
                    }
                });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, AperturaPrecioDto>>>(), It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<AperturaPrecioDto>());


            var result = target.TraerFijacion(1);

            repositorioMock.Verify(x => x.Obtener<FijacionDePrecioContrato>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, double>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, BasicoContrato>>>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<AperturaPrecio, AperturaPrecioDto>>>(), It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.IsNotNull(result);
        }

        [Test]
        public void TraerAperturaDePrecioPorFijacionOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, AperturaPrecioDto>>>(), It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<AperturaPrecioDto>() { new AperturaPrecioDto() });

            var result = target.TraerAperturaDePrecioPorFijacion(1);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<AperturaPrecio, AperturaPrecioDto>>>(), It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }
        [Test]
        public void FinalizacionAutomaticaOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<FijacionDePrecioContrato>() { new FijacionDePrecioContrato { Id = 1 } });

            var fijacionSave = new FijacionDePrecioContrato
            {
                Proveedor = new Proveedor { CUIT = "11" },
                Corredor = new Proveedor { CUIT = "22" },
                CorredorId = 1,
                Ampliaciones = 1,
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 2,
                MonedaId = "ARP  ",
                ComercialId = 1,
                ContratoSAP = "1234",
                CampanaId = 1,
                EstadoId = (int)EnumEstadoContrato.Finalizado,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Confirmado },
                Comercial = new Comercial { ComercialId = 1 }
            };

            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(fijacionSave);
            repositorioMock.Setup(y => y.Obtener<EstadoContrato>(It.IsAny<int>())).Returns(new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Finalizado });
            comercialManagerMock.Setup(y => y.CadenaComerciales(It.IsAny<int>())).Returns(new List<int> { 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SuscripcionComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<SuscripcionComercial>() { new SuscripcionComercial { ComercialId = 1, Id = 2, Key = "HOLA" } });

            relacionCorredorProveedorAgentMock.Setup(y => y.ObtenerRelacionCorredorProveedor(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ConceptoAperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<ConceptoAperturaPrecio>() { new ConceptoAperturaPrecio { CodigoSap = "RE", Descripcion = "Redespacho", Id = 2 } });

            finalizarFijacionAgentMock.Setup(y => y.Finalizar(It.IsAny<FijacionDePrecioContrato>())).Returns("OK");

            target.FinalizacionAutomatica("a");

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            finalizarFijacionAgentMock.Verify(y => y.Finalizar(It.IsAny<FijacionDePrecioContrato>()), Times.Once);
        }
    }
}
