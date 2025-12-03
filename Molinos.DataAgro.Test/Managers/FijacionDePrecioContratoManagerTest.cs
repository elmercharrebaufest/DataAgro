using NLog;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Common.Enums;
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
        private Mock<IContratosParaFijacionAgent> contratosParaFijacionMock;
        private Mock<IMailManager> mailManagerMock;
        private Mock<IValidarDocProcPagoAgent> validarPagoAgente;
        private Mock<IDiasHabilesAgent> diasHabilesAgente;
        private Mock<IModificarFijacionAgent> modificarFijacionAgentMock;
        private Mock<IConfiguracionManager> configuracionManagerMock;
        private Mock<IValidarLiquidacionParaFijacionAgent> validarLiquidacionParaFijacionAgentMock;
        private Mock<ITipoDeCambioAgent> tipoDeCamcioAgentMock;
        private Mock<IContratosParaFijacionVirtualAgent> contratosFijacionVirtualMock;
        private Mock<IFinalizarFijacionVirtualAgent> finalizarFijacionMock;
        private Mock<IAnularFijacionVirtualAgent> anularFijacionVirtualMock;
        private JavaScriptSerializer serializer;
        private Mock<INegocioManager> negocioManagerMock;
        private Mock<IConfiguracionInternaManager> configuracionInternaManagerMock;
        private Mock<IAnularFijacionAgent> anularFijacionMock;
        private Mock<IValidarLiquidacionComisionesAgent> validarLiquidacionComisionMock;
        private Mock<IValidarLiquidacionFinalAgent> validarLiquidacionFinalMock;
        private Mock<IValidarLiquidacionParcialAgent> validarLiquidacionParcialMock;
        private Mock<IValidarPesificacionAgent> validarPesificacionMock;
        private Mock<IContratoManager> contratoManagerMock;

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
            contratosParaFijacionMock = new Mock<IContratosParaFijacionAgent>();
            mailManagerMock = new Mock<IMailManager>();
            configuracionManagerMock = new Mock<IConfiguracionManager>();
            validarLiquidacionParaFijacionAgentMock = new Mock<IValidarLiquidacionParaFijacionAgent>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            logDataAgroManagerMock = new Mock<ILogDataAgroManager>();
            validarPagoAgente = new Mock<IValidarDocProcPagoAgent>();
            diasHabilesAgente = new Mock<IDiasHabilesAgent>();
            modificarFijacionAgentMock = new Mock<IModificarFijacionAgent>();
            configuracionManagerMock = new Mock<IConfiguracionManager>();
            tipoDeCamcioAgentMock = new Mock<ITipoDeCambioAgent>();
            contratosFijacionVirtualMock = new Mock<IContratosParaFijacionVirtualAgent>();
            finalizarFijacionMock = new Mock<IFinalizarFijacionVirtualAgent>();
            anularFijacionVirtualMock = new Mock<IAnularFijacionVirtualAgent>();
            negocioManagerMock = new Mock<INegocioManager>();
            configuracionInternaManagerMock = new Mock<IConfiguracionInternaManager>();
            anularFijacionMock = new Mock<IAnularFijacionAgent>();
            validarLiquidacionComisionMock = new Mock<IValidarLiquidacionComisionesAgent>();
            validarLiquidacionFinalMock = new Mock<IValidarLiquidacionFinalAgent>();
            validarLiquidacionParcialMock = new Mock<IValidarLiquidacionParcialAgent>();
            validarPesificacionMock = new Mock<IValidarPesificacionAgent>();
            contratoManagerMock = new Mock<IContratoManager>();

            target = new FijacionDePrecioContratoManager(logger.Object, repositorioMock.Object,
                proveedorManagerMock.Object, comercialManagerMock.Object,
                pushNotificacionManagerMock.Object, finalizarFijacionAgentMock.Object,
                contratosParaFijacionMock.Object,
                mailManagerMock.Object, logDataAgroManagerMock.Object, validarPagoAgente.Object,
                modificarFijacionAgentMock.Object, diasHabilesAgente.Object, configuracionManagerMock.Object, validarLiquidacionParaFijacionAgentMock.Object,
                tipoDeCamcioAgentMock.Object, contratosFijacionVirtualMock.Object, finalizarFijacionMock.Object, anularFijacionVirtualMock.Object,
                negocioManagerMock.Object, configuracionInternaManagerMock.Object, anularFijacionMock.Object, validarLiquidacionComisionMock.Object, validarLiquidacionFinalMock.Object,
                validarLiquidacionParcialMock.Object, validarPesificacionMock.Object, contratoManagerMock.Object);
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
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>()));
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, double>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<double> { 1.0 });
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, BasicoContrato>>>()))
                .Returns(new BasicoContrato
                {
                    ContratoId = 1,
                    DatosFijacion = new DatosFijacionDeContratoDto
                    {
                        ContratoId = "00011111",
                        FechaDesde = "2019/10/30",
                        FechaHasta = "2019/11/30",
                        KilosAplicados = "1111",
                        KilosPendiente = "111111"
                    }
                });
            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(
                  new FijacionDePrecioContrato
                  {
                      Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Pendiente, Descripcion = "Pendiente" },
                      Ampliaciones = 2,
                      Proveedor = new Proveedor() { CUIT = "00027362" },
                      ContratoSAP = "0002343211",
                      MaterialId = 1,

                  });

            contratosParaFijacionMock.Setup(y => y.ObtenerContratos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
               .Returns(new List<DatosFijacionDeContratoDto>() { new DatosFijacionDeContratoDto { ContratoId = "1234", KilosPendiente = "10", KilosContrato = "100", Calidad = true } });
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
                            Ampliaciones = 2,
                            Proveedor = new Proveedor() { CUIT = "00027362" },
                            ContratoSAP = "0002343211",
                            MaterialId = 1,

                        });
            contratosParaFijacionMock.Setup(y => y.ObtenerContratos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
               .Returns(new List<DatosFijacionDeContratoDto>() { new DatosFijacionDeContratoDto { ContratoId = "1234", KilosPendiente = "10", KilosContrato = "100", Calidad = true } });
            repositorioMock.Setup(y => y.Obtener<Contrato, int>(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, int>>>())).Returns(1);

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
                CampanaId = 1,
                FechaOperacion = DateTime.Now,
                AperturaPrecio = new List<AperturaPrecio>()
            };
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668", Deshabilitado = false });

            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>()));
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, double>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<double> { 1.0 });
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, BasicoContrato>>>()))
                .Returns(new BasicoContrato
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
                CampanaId = 1,
                FechaOperacion = DateTime.Now,
                AperturaPrecio = new List<AperturaPrecio>()
            };

            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>()));
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, double>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<double> { 1.0 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668", Deshabilitado = false });

            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, BasicoContrato>>>()))
                .Returns(new BasicoContrato
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
                AperturaPrecio = new List<AperturaPrecio> { },
                FechaOperacion = DateTime.Now
            };
            var fijacionSave = new FijacionDePrecioContrato
            {
                Id = 1,
                ProveedorId = 1,
                MaterialId = 1,
                Cantidad = 1,
                Precio = 2,
                MonedaId = "ARP  ",
                ComercialId = 1,
                ContratoSAP = "1234",
                CampanaId = 1,
                EstadoId = (int)EnumEstadoContrato.Confirmado,
                FechaOperacion = DateTime.Now
            };

            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(new Contrato { Id = 1, ContratoSAP = "0002343223" });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, double>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<double> { 1.0 });
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, BasicoContrato>>>()))
                .Returns(new BasicoContrato
                {
                    ContratoId = 1,
                    DatosFijacion = new DatosFijacionDeContratoDto
                    {
                        ContratoId = "00011111",
                        FechaDesde = "2019/10/30",
                        FechaHasta = "2019/11/30",
                        KilosAplicados = "1111",
                        KilosPendiente = "1111",
                        Clasificacion = "PRODUCTOR"
                    }
                });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668", Deshabilitado = false });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
              .Returns(new List<RangoPrecio>() { new RangoPrecio { MonedaId = "AUS ", MaterialId = 2, PrecioMaximo = 20000, PrecioMinimo = 0 } });
            contratosParaFijacionMock.Setup(x => x.ObtenerContratos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>())).Returns(new List<DatosFijacionDeContratoDto>());
            repositorioMock.Setup(y => y.Obtener<Contrato, int>(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, int>>>())).Returns(0);
            contratosParaFijacionMock.Setup(y => y.ObtenerContratos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
               .Returns(new List<DatosFijacionDeContratoDto>() { new DatosFijacionDeContratoDto { ContratoId = "1234", KilosPendiente = "10", KilosContrato = "100", Calidad = true } });

            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(fijacionSave);

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
             .Returns(new List<AperturaPrecio>());

            repositorioMock.Setup(y => y.Obtener<Negocio>(It.IsAny<int>())).Returns(new Negocio { Fecha = DateTime.Now });
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
                AperturaPrecio = new List<AperturaPrecio> { },
                ChequeElectronico = true,
                PagoCBU = "1"
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
                EstadoId = (int)EnumEstadoContrato.Finalizado,
                ChequeElectronico = false,
                PagoCBU = "2"
            };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
              .Returns(new List<RangoPrecio>() { new RangoPrecio { MonedaId = "AUS ", MaterialId = 2, PrecioMaximo = 20000, PrecioMinimo = 0 } });

            repositorioMock.Setup(y => y.Obtener<Contrato, int>(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<Expression<Func<Contrato, int>>>())).Returns(0);

            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(fijacionSave);
            contratosParaFijacionMock.Setup(y => y.ObtenerContratos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
               .Returns(new List<DatosFijacionDeContratoDto>() { new DatosFijacionDeContratoDto { ContratoId = "1234", KilosPendiente = "10", KilosContrato = "100", Calidad = true } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
             .Returns(new List<AperturaPrecio>());

            repositorioMock.Setup(y => y.Obtener<Negocio>(It.IsAny<int>())).Returns(new Negocio { Fecha = DateTime.Now });
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

            var result = target.ConfirmarFijacion(It.IsAny<int>(), It.IsAny<int>());

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

            var result = target.ConfirmarFijacion(It.IsAny<int>(), It.IsAny<int>());

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

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ConceptoAperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<ConceptoAperturaPrecio>() { new ConceptoAperturaPrecio { CodigoSap = "RE", Descripcion = "Redespacho", Id = 2 } });

            finalizarFijacionAgentMock.Setup(y => y.Finalizar(It.IsAny<FijacionDePrecioContrato>())).Returns("OK");

            target.FinalizacionAutomatica("a");

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            finalizarFijacionAgentMock.Verify(y => y.Finalizar(It.IsAny<FijacionDePrecioContrato>()), Times.Once);
        }

        [Test]
        public void ModificarFijacionOk()
        {
            var contratoSave = new FijacionDePrecioContrato
            {
                ContratoSAP = "434343",
                ChequeElectronico = false
            };
            var contrato = new FijacionDePrecioContrato
            {
                ContratoSAP = "434343",
                ChequeElectronico = true,
                Id = 1
            };
            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(contrato);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>()))
                .Returns(new FijacionDePrecioContrato { ContratoSAP = "434343", Id = 1 });
            contratosParaFijacionMock.Setup(x => x.ObtenerContratos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>())).Returns(new List<DatosFijacionDeContratoDto>());
            validarPagoAgente.Setup(x => x.ValidarEstado(It.IsAny<String>(), It.IsAny<String>())).Returns("Ok");
            modificarFijacionAgentMock.Setup(x => x.Modificar(It.IsAny<FijacionDePrecioContrato>())).Returns("Se actualizaron los datos correctamente");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>()))
                .Returns(new Contrato { ContratoSAP = "434343", Id = 1 });
            var resultado = target.ActualizarFijacion(contrato);
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

        }

        [Test]
        public void ModificarFijacionError()
        {
            var contratoSave = new FijacionDePrecioContrato
            {
                ContratoSAP = "434343",
                ChequeElectronico = false
            };
            var contrato = new FijacionDePrecioContrato
            {
                ContratoSAP = "434343",
                ChequeElectronico = true,
                Id = 1
            };
            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(contrato);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>()))
                .Returns(new FijacionDePrecioContrato { ContratoSAP = "434343", Id = 1 });
            validarPagoAgente.Setup(x => x.ValidarEstado(It.IsAny<String>(), It.IsAny<String>())).Returns("Error");
            var resultado = target.ActualizarFijacion(contrato);
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            modificarFijacionAgentMock.Setup(x => x.Modificar(It.IsAny<FijacionDePrecioContrato>())).Returns("Ok Sap");

        }

        [Test]
        public void ModificarFijacionErrorSap()
        {
            var contratoSave = new FijacionDePrecioContrato
            {
                ContratoSAP = "434343",
                ChequeElectronico = false
            };
            var contrato = new FijacionDePrecioContrato
            {
                ContratoSAP = "434343",
                ChequeElectronico = false,
                Id = 1
            };
            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(contrato);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>()))
                .Returns(new FijacionDePrecioContrato { ContratoSAP = "434343", Id = 1 });
            modificarFijacionAgentMock.Setup(x => x.Modificar(It.IsAny<FijacionDePrecioContrato>())).Returns("Error");
            var resultado = target.ActualizarFijacion(contrato);
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

        }

        [Test]
        public void FechaFeriadosTest()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<FechaFeriado, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<FechaFeriado>() { new FechaFeriado { Feriado = DateTime.Now } });
            var res = target.FechaFeriados();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FechaFeriado, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()), Times.Once);
            Assert.IsNotNull(res);
        }

        [Test]
        public void UltimoDiaHabilTest()
        {
            diasHabilesAgente.Setup(x => x.UltimoDiaHabil(null)).Returns(DateTime.Now);
            var res = target.UltimoDiaHabil();
            diasHabilesAgente.Verify(x => x.UltimoDiaHabil(null), Times.Once);
            Assert.IsNotNull(res);
        }


        [Test]
        public void AltaFijacionSapTestOk()
        {
            var fijacion = new FijacionDePrecioContrato
            {
                Id = 1,
                FijacionSAP = "22304948",
                ChequeElectronico = null,
                PagoCBU = "",
                TrigoEspecial = null,
                ContratoSAP = "000345433",
                ContratoId = 1,
                Precio = 1000,
                Cantidad = 1000,
                DestinoId = 1,
                Pizarra = false,
                Posicion = "0034",
                PagoDiferido = false,
                FechaOperacion = DateTime.Now,
                FechaHasta = DateTime.Now,
                FechaDesde = DateTime.Now,
                ProveedorId = 1,
                ComercialId = 1,
                MaterialId = 1,
                CorredorId = 1,
                DiasPesificado = null,
                MonedaId = "ARP ",
                CampanaId = 1,
                PrecioNeto = 1000,
                EstadoId = 1,
                FechaConfirmacion = DateTime.Now,
                TipoNegocioId = 3,
                ComercialCreadorId = 1,
                Canje = true,
                Fecha = DateTime.Now,
                AperturaPrecio = new List<AperturaPrecio>()
            };
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { ProveedorId = 1, CUIT = "20358654668", Deshabilitado = false });

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(new Contrato { Id = 1 });
            repositorioMock.Setup(y => y.Obtener<Negocio>(It.IsAny<int>())).Returns(new Negocio { Id = 1, Fecha = DateTime.Now });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<RangoPrecio>() { new RangoPrecio { Id = 1, MaterialId = 1, PrecioMaximo = 10000, PrecioMinimo = 200, MonedaId = "ARP " } });
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
            var resultado = target.AltaFijacionSap(fijacion, null) as Resultado;
            repositorioMock.Verify(x => x.Agregar(It.IsAny<FijacionDePrecioContrato>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void AltaFijacionSapTestError()
        {
            var fijacion = new FijacionDePrecioContrato
            {
                Id = 1,
                FijacionSAP = "22304948",
                ChequeElectronico = null,
                PagoCBU = "",
                TrigoEspecial = null,
                ContratoSAP = "000345433",
                ContratoId = 1,
                Precio = 1000,
                Cantidad = 1000,
                DestinoId = 1,
                Pizarra = false,
                Posicion = "0034",
                PagoDiferido = false,
                FechaOperacion = DateTime.Now,
                FechaHasta = DateTime.Now,
                FechaDesde = DateTime.Now,
                ProveedorId = 1,
                ComercialId = 1,
                MaterialId = 1,
                CorredorId = 1,
                DiasPesificado = null,
                MonedaId = "ARP ",
                CampanaId = 1,
                PrecioNeto = 1000,
                EstadoId = 1,
                FechaConfirmacion = DateTime.Now,
                TipoNegocioId = 3,
                ComercialCreadorId = 1,
                Canje = true,
                Fecha = DateTime.Now,
                AperturaPrecio = null
            };
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(new Contrato { Id = 0 });
            repositorioMock.Setup(y => y.Obtener<Negocio>(It.IsAny<int>())).Returns(new Negocio { Id = 1, Fecha = DateTime.Now });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<RangoPrecio>() { new RangoPrecio { Id = 1, MaterialId = 1, PrecioMaximo = 10000, PrecioMinimo = 200, MonedaId = "ARP " } });

            var resultado = target.AltaFijacionSap(fijacion, null) as Resultado;
            repositorioMock.Verify(x => x.Agregar(It.IsAny<FijacionDePrecioContrato>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }


        [Test]
        public void BuscarComisionEnFijacion1()
        {
            var negocio = new BasicoContrato { ContratoId = 1, ComercialId = 1, TipoNegocioId = 3, PorcentajeComision = 0, ImporteComision = 0, Fecha = DateTime.Now };
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
                    },
                    PorcentajeSobrePrecioContrato = 1
                });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, AperturaPrecioDto>>>(), It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<AperturaPrecioDto>());

            target.BuscarComision(negocio);
            Assert.AreEqual(1, negocio.PorcentajeComision);
        }

        [Test]
        public void BuscarComisionEnFijacion2()
        {
            var negocio = new BasicoContrato { ContratoId = 1, ComercialId = 1, TipoNegocioId = 3, PorcentajeComision = 0, ImporteComision = 0, Precio = 100, MonedaId = "ARP  ", Fecha = DateTime.Now };
            var fijacionSave = new FijacionDePrecioContrato
            {
                Id = 1,
                EstadoId = (int)EnumEstadoContrato.Rechazado,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Pendiente },
                Comercial = new Comercial { ComercialId = 1 },
                Precio = 100,
                MonedaId = "ARP  ",
                MonedaSobrePrecioContrato = "ARP  ",
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
                    },
                    ImporteSobrePrecioContrato = 10,
                    Precio = 100,
                    MonedaId = "ARP  ",
                    MonedaSobrePrecioContrato = "ARP  ",
                });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, AperturaPrecioDto>>>(), It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<AperturaPrecioDto>());

            target.BuscarComision(negocio);
            Assert.AreEqual(10, negocio.ImporteComision);
        }

        [Test]
        public void BuscarComisionEnFijacion3()
        {
            var negocio = new BasicoContrato { FechaOperacion = DateTime.Now, ContratoId = 1, ComercialId = 1, TipoNegocioId = 3, PorcentajeComision = 0, ImporteComision = 0, Precio = 100, MonedaId = "ARP  ", Fecha = DateTime.Now };
            var fijacionSave = new FijacionDePrecioContrato
            {
                Id = 1,
                EstadoId = (int)EnumEstadoContrato.Rechazado,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Pendiente },
                Comercial = new Comercial { ComercialId = 1 },
                Precio = 100,
                MonedaId = "ARP  ",
                MonedaSobrePrecioContrato = "USD  ",
            };
            tipoDeCamcioAgentMock.Setup(y => y.TraerTipoDeCambio(It.IsAny<DateTime?>(), "M")).Returns(1);
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
                    },
                    ImporteSobrePrecioContrato = 10,
                    Precio = 100,
                    MonedaId = "ARP  ",
                    MonedaSobrePrecioContrato = "USD  ",
                });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, AperturaPrecioDto>>>(), It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<AperturaPrecioDto>());

            target.BuscarComision(negocio);
            Assert.AreEqual(0, negocio.ImporteComision);
        }



        [Test]
        public void BuscarComisionEnAfijar1()
        {
            var negocio = new BasicoContrato { ContratoId = 1, ComercialId = 1, TipoNegocioId = 3, PorcentajeComision = 0, ImporteComision = 0, Fecha = DateTime.Now };
            var fijacionSave = new FijacionDePrecioContrato
            {
                Id = 1,
                EstadoId = (int)EnumEstadoContrato.Rechazado,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Pendiente },
                Comercial = new Comercial { ComercialId = 1 }
            };
            Contrato afijar = new Contrato { Descuentos = new List<DescuentoBonificacion>() { new DescuentoBonificacion { TipoDBId = 1, TipoPeriodoDBId = 1, Porcentaje = 1 } } };
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(afijar);

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
                    },
                    PorcentajeSobrePrecioContrato = 0
                });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, AperturaPrecioDto>>>(), It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<AperturaPrecioDto>());

            target.BuscarComision(negocio);
            Assert.AreEqual(1, negocio.PorcentajeComision);
        }

        [Test]
        public void BuscarComisionEnAfijar2()
        {
            var negocio = new BasicoContrato { ContratoId = 1, ComercialId = 1, TipoNegocioId = 3, PorcentajeComision = 0, ImporteComision = 0, Precio = 100, MonedaId = "ARP  ", Fecha = DateTime.Now };
            var fijacionSave = new FijacionDePrecioContrato
            {
                Id = 1,
                EstadoId = (int)EnumEstadoContrato.Rechazado,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Pendiente },
                Comercial = new Comercial { ComercialId = 1 },
                Precio = 100,
                MonedaId = "ARP  ",
                MonedaSobrePrecioContrato = "ARP  ",
            };
            Contrato afijar = new Contrato { Descuentos = new List<DescuentoBonificacion>() { new DescuentoBonificacion { TipoDBId = 1, TipoPeriodoDBId = 1, Porcentaje = 0, Importe = 10, MonedaId = "ARP  " } } };
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(afijar);

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
                    },
                    Precio = 100,
                    MonedaId = "ARP  ",
                });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, AperturaPrecioDto>>>(), It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<AperturaPrecioDto>());

            target.BuscarComision(negocio);
            Assert.AreEqual(10, negocio.ImporteComision);
        }

        [Test]
        public void BuscarComisionEnAfijar3()
        {
            var negocio = new BasicoContrato { FechaOperacion = DateTime.Now, ContratoId = 1, ComercialId = 1, TipoNegocioId = 3, PorcentajeComision = 0, ImporteComision = 0, Precio = 100, MonedaId = "ARP  ", Fecha = DateTime.Now };
            var fijacionSave = new FijacionDePrecioContrato
            {
                Id = 1,
                EstadoId = (int)EnumEstadoContrato.Rechazado,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Pendiente },
                Comercial = new Comercial { ComercialId = 1 },
                Precio = 100,
                MonedaId = "ARP  ",
            };
            Contrato afijar = new Contrato { Descuentos = new List<DescuentoBonificacion>() { new DescuentoBonificacion { TipoDBId = 1, TipoPeriodoDBId = 1, Porcentaje = 0, Importe = 10, MonedaId = "USD  " } } };
            repositorioMock.Setup(y => y.Obtener<Contrato>(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(afijar);

            tipoDeCamcioAgentMock.Setup(y => y.TraerTipoDeCambio(It.IsAny<DateTime?>(), "M")).Returns(1);
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
                    },
                    Precio = 100,
                    MonedaId = "ARP  ",
                });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, AperturaPrecioDto>>>(), It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<AperturaPrecioDto>());

            target.BuscarComision(negocio);
            Assert.AreEqual(0, negocio.ImporteComision);
        }

        [Test]
        public void DevolverKilosPendientesAnularFijacionCanjeTestOk()
        {
            var fijacion = new FijacionDePrecioContrato
            {
                Id = 1,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Finalizado },
                EstadoId = (int)EnumEstadoContrato.Finalizado,
                Comercial = new Comercial { ComercialId = 1 },
                Precio = 100,
                MonedaId = "ARP  ",
                Cantidad = 5000,
                FijacionSAP = "1",
                Virtual = true

            };

            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>())).Returns(fijacion);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FijacionVirtualSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
               .Returns(new List<FijacionVirtualSap> {
                   new FijacionVirtualSap{ FijacionCanjeId =2,FijacionVirtualId=1,Cantidad=4000,Id=1,FijacionVirtualNro="1", FijacionCanje = new Negocio{EstadoId = (int)EnumEstadoContrato.Finalizado } }
               });



            var result = target.DevolverKilosPendientesAnularFijacionCanje(1);
            Assert.AreEqual(1000, result.KilosPendientes);
        }

        [Test]
        public void DevolverKilosPendientesAnularFijacionCanjeTestOk1()
        {
            var fijacion = new FijacionDePrecioContrato
            {
                Id = 1,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Confirmado },
                EstadoId = (int)EnumEstadoContrato.Confirmado,
                Comercial = new Comercial { ComercialId = 1 },
                Precio = 100,
                MonedaId = "ARP  ",
                Cantidad = 5000,
                FijacionSAP = "1",
                Virtual = true

            };

            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>())).Returns(fijacion);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FijacionVirtualSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
               .Returns(new List<FijacionVirtualSap> {
                   new FijacionVirtualSap{ FijacionCanjeId =2,FijacionVirtualId=1,Cantidad=4000,Id=1,FijacionVirtualNro="1", FijacionCanje = new Negocio{EstadoId = (int)EnumEstadoContrato.Finalizado } }
               });



            var result = target.DevolverKilosPendientesAnularFijacionCanje(1);
            Assert.AreEqual(0, result.KilosPendientes);
        }


        [Test]
        public void ConfirmarFijacionTestOk()
        {

            var fijacion = new FijacionDePrecioContrato
            {
                Id = 1,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Confirmado },
                EstadoId = (int)EnumEstadoContrato.Confirmado,
                Comercial = new Comercial { ComercialId = 1 },
                Precio = 100,
                MonedaId = "ARP  ",
                Cantidad = 5000,
                FijacionSAP = "1",
                Virtual = true

            };
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>())).Returns(fijacion);
            SetupLogGuardarFijacion();

            var result = target.ConfirmarFijacionSAP("121");

            Assert.NotNull(result);
            Assert.AreEqual(false, result.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

        }

        private void SetupLogGuardarFijacion()
        {
            var fijacion = new FijacionDePrecioContrato
            {
                Id = 1,
                Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Confirmado },
                EstadoId = (int)EnumEstadoContrato.Confirmado,
                Comercial = new Comercial { ComercialId = 1 },
                Precio = 100,
                MonedaId = "ARP  ",
                Cantidad = 5000,
                FijacionSAP = "1",
                Virtual = true

            };
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>())).Returns(fijacion);
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, double>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<double> { 1.0 });
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<Expression<Func<FijacionDePrecioContrato, BasicoContrato>>>()))
                .Returns(new BasicoContrato
                {
                    ContratoId = 1,
                    DatosFijacion = new DatosFijacionDeContratoDto
                    {
                        ContratoId = "00011111",
                        FechaDesde = "2019/10/30",
                        FechaHasta = "2019/11/30",
                        KilosAplicados = "1111",
                        KilosPendiente = "111111"
                    }
                });
            repositorioMock.Setup(y => y.Obtener<FijacionDePrecioContrato>(It.IsAny<int>())).Returns(
                  new FijacionDePrecioContrato
                  {
                      Estado = new EstadoContrato { EstadoContratoId = (int)EnumEstadoContrato.Pendiente, Descripcion = "Pendiente" },
                      Ampliaciones = 2,
                      Proveedor = new Proveedor() { CUIT = "00027362" },
                      ContratoSAP = "0002343211",
                      MaterialId = 1,

                  });
        }

        [Test]
        public void ConfirmarFijacionTestFijacionNullError()
        {
            var result = target.ConfirmarFijacionSAP(null);

            Assert.NotNull(result);
            Assert.AreEqual(true, result.HayError);

        }

        [Test]
        public void ConfirmarFijacionTestFijacionNoEncontradaError()
        {
            var result = target.ConfirmarFijacionSAP("212132132");

            Assert.NotNull(result);
            Assert.AreEqual(true, result.HayError);

        }

    }
}
