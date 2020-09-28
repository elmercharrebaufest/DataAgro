using Autofac.Extras.NLog;
using KendoGridBinder;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using Molinos.DataAgro.Test.Mock;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ContratoAcuerdoManagerTest
    {
        private Mock<ILogDataAgroManager> logDataAgroManagerMock;
        private ContratoAcuerdoManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<IDiasHabilesAgent> diasHabilesAgentMock;
        private Mock<IValidarDocProcPagoAgent> validarPagoAgente;

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            diasHabilesAgentMock = new Mock<IDiasHabilesAgent>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            logDataAgroManagerMock = new Mock<ILogDataAgroManager>();
            validarPagoAgente = new Mock<IValidarDocProcPagoAgent>();

            target = new ContratoAcuerdoManager(logger.Object, repositorioMock.Object, 
            diasHabilesAgentMock.Object, logDataAgroManagerMock.Object, validarPagoAgente.Object);
        }
        [Test]
        public void BorrarAcuerdoTest()
        {
            var acuerdo = new ContratoAcuerdo
            {
                Id = 10,
                MotivoRechazo = "test"
            };
            repositorioMock.Setup(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()))
                .Returns(new ContratoAcuerdo { Id = 1, Estado = new EstadoContrato { EstadoContratoId = 2 }, EstadoId = 2, MotivoRechazo = "test" });
            repositorioMock.Setup(x => x.Obtener<EstadoContrato>(It.IsAny<int>()))
                .Returns(new EstadoContrato { EstadoContratoId = 6 });
            var resultado = target.BorrarAcuerdo(acuerdo);

            Assert.That(!resultado.HayError);
            repositorioMock.Verify(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void BorrarAcuerdoSinMotivoError()
        {
            var acuerdoParaSerializar = new ContratoAcuerdo
            {
                Id = 1,
                Estado = new EstadoContrato { EstadoContratoId = 7 },
                EstadoId = 7,
                MotivoRechazo = "test",
                MaterialId = 1,
                TipoNegocioId = 1,
                Cantidad = 10,
                Precio = 100,
                CampanaId = 1,
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                ProveedorId = 1,
                MonedaId = " USD",
                GrupoCompra = 1,
                ComercialId = 1,
                TrigoEspecial = false,
                DestinoId = 1,
                CondicionFijacionId = 1,
                ComercialCreadorId = 1,
                CorredorId = 1,
                ContratoSAP = "00034343",      
                PrecioPactado = new List<PrecioPactado>() {
                     new PrecioPactado
                     {
                         Precio = 1000,
                         ImportePactado = 100,
                         ContratoId = 1,
                         Id = 1,
                         MonedaPactadoId = "usd",
                         MonedaImportePactadoId = "USD"
                     }
                },
                Calidad = new List<Calidad>()
                {
                    new Calidad
                    {
                        StandardDeCalidadId = 1,
                        NegocioId = 1,
                        Id = 1,
                        CalidadEspecialId = 1,
                        Valor = 10
                    }
                },
                AperturaPrecio = new List<AperturaPrecio>()
                {
                    new AperturaPrecio()
                    {
                        ConceptoAperturaPrecioId= (int)EnumConceptoApertura.Redespacho,
                        Importe = 300
                    },
                    new AperturaPrecio { ConceptoAperturaPrecioId = (int)EnumConceptoApertura.Financiero, Importe =100 }
                }
            };
            var datos = JsonConvert.SerializeObject(acuerdoParaSerializar, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });
            var acuerdo = new ContratoAcuerdo
            {
                Id = 1,
                Estado = new EstadoContrato { EstadoContratoId = 7 },
                EstadoId = 7,
                MotivoRechazo = "test",
                NegocioHistorico = new List<NegocioHistorico>() { new NegocioHistorico
                {
                     Datos = datos
                }
                },
                PrecioPactado = new List<PrecioPactado>() {
                        new PrecioPactado
                        {
                            Precio = 1000,
                            ImportePactado = 100,
                            ContratoId = 1,
                            Id = 1,
                            MonedaPactadoId = "usd",
                            MonedaImportePactadoId = "USD"
                        }
                },
                Calidad = new List<Calidad>()
                {
                    new Calidad
                    {
                        StandardDeCalidadId = 1,
                        NegocioId = 1,
                        Id = 1,
                        CalidadEspecialId = 1,
                        Valor = 10
                    }
                },
                  AperturaPrecio = new List<AperturaPrecio>()
                {
                    new AperturaPrecio()
                    {
                        ConceptoAperturaPrecioId= (int)EnumConceptoApertura.Redespacho,
                        Importe = 300
                    },
                    new AperturaPrecio { ConceptoAperturaPrecioId = (int)EnumConceptoApertura.Financiero, Importe =100 }
                }
            };      
            repositorioMock.Setup(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()))
              .Returns(acuerdo);
            repositorioMock.Setup(x => x.Obtener<EstadoContrato>(It.IsAny<int>()))
                .Returns(new EstadoContrato { EstadoContratoId = 7 });
            var resultado = target.BorrarAcuerdo(acuerdo);
            Assert.That(!resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
        [Test]
        public void BorrarAcuerdoConAmpliacionesReconfirmar()
        {           
            var acuerdo = new ContratoAcuerdo
            {
                Id = 1,
                Estado = new EstadoContrato { EstadoContratoId = 7 },
                EstadoId = 7,
                MotivoRechazo = "test",                
                Ampliaciones = 10
            };
            repositorioMock.Setup(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()))
              .Returns(acuerdo);
            repositorioMock.Setup(x => x.Obtener<EstadoContrato>(It.IsAny<int>()))
                .Returns(new EstadoContrato { EstadoContratoId = 7 });
            var resultado = target.BorrarAcuerdo(acuerdo);
            Assert.That(!resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
        [Test]
        public void BorrarAcuerdoConAmpliacionesConfirmado()
        {
            var acuerdo = new ContratoAcuerdo
            {
                Id = 1,
                Estado = new EstadoContrato { EstadoContratoId = 2 },
                EstadoId = 2,
                MotivoRechazo = "test",
                Ampliaciones = 10,
            };
            repositorioMock.Setup(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()))
              .Returns(acuerdo);
            repositorioMock.Setup(x => x.Obtener<EstadoContrato>(It.IsAny<int>()))
                .Returns(new EstadoContrato { EstadoContratoId = 2 });
            var resultado = target.BorrarAcuerdo(acuerdo);
            Assert.That(!resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void BorrarAcuerdoConError()
        {
            var acuerdo = new ContratoAcuerdo
            {
                Id = 1,
                Estado = new EstadoContrato { EstadoContratoId = 8 },
                EstadoId = 8,
                MotivoRechazo = "test",
                Ampliaciones = 10
            };
            repositorioMock.Setup(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()))
              .Returns(acuerdo);
            repositorioMock.Setup(x => x.Obtener<EstadoContrato>(It.IsAny<int>()))
                .Returns(new EstadoContrato { EstadoContratoId = 8 });
            var resultado = target.BorrarAcuerdo(acuerdo);
            Assert.That(resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void BorrarAcuerdoConEstadoReconfirmarTest()
        {
            var acuerdo = new ContratoAcuerdo
            {
                Id = 10,
            };
            var resultado = target.BorrarAcuerdo(acuerdo);
            Assert.That(resultado.HayError);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void GrabarAcuerdoOkTest()
        {
            var acuerdo = new ContratoAcuerdo
            {
                Id = 0,
                Precio = 10,
                Cantidad = 10,
                ComercialCreadorId = 1,
                DestinoId = 1,
                MaterialId = 1,
                CampanaId = 1,
                MonedaId = "a",
                TipoNegocioId = 2,
                FechaDesde = new DateTime(2020,9,19),
                FechaHasta = new DateTime(2020,9, 30),
                PrecioPactado = new List<PrecioPactado>() {
                        new PrecioPactado
                        {
                            Precio = 1000,
                            ImportePactado = 100,
                            ContratoId = 1,
                            Id = 1,
                            MonedaPactadoId = "usd",
                            MonedaImportePactadoId = "USD"
                        }
                },
                Calidad = new List<Calidad>()
                {
                    new Calidad
                    {
                        StandardDeCalidadId = 1,
                        NegocioId = 1,
                        Id = 1,
                        CalidadEspecialId = 1,
                        Valor = 10
                    }
                },
                AperturaPrecio = new List<AperturaPrecio>()
                {
                    new AperturaPrecio()
                    {
                        ConceptoAperturaPrecioId= (int)EnumConceptoApertura.Redespacho,
                        Importe = 300
                    },
                    new AperturaPrecio { ConceptoAperturaPrecioId = (int)EnumConceptoApertura.Financiero, Importe =100 }
                }
            };
            Thread.CurrentPrincipal = new TestPrincipal(new Claim[] {
            new Claim(ClaimTypes.Role, PermisosDataAgro.NegociosConfirmados.ToString()) });
            repositorioMock.Setup(x => x.Agregar(It.IsAny<ContratoAcuerdo>()));
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoConfirmacionAutomatica, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).
              Returns(new List<RangoConfirmacionAutomatica>() {
                  new RangoConfirmacionAutomatica
                  {
                    FechaDesde = DateTime.Now,
                    FechaHasta = DateTime.Now,
                    TipoNegocioId = 2,
                    MaterialId = 1,
                    MonedaId = "a",
                    PrecioMinimo = 1,
                    PrecioMaximo = 9,
                    DesdeAnio = 2020,
                    DesdeMes = 8,
                    HastaAnio = 2021,
                    HastaMes = 9

                  }
              });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, int>>>()))
             .Returns(1);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, double>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<double>() { 10.0, 11.0 });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, double>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<double>() { 10.0, 11.0});

            var resultado = target.GrabarAcuerdo(acuerdo);

            Assert.That(!resultado.HayError);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<ContratoAcuerdo>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
        [Test]
        public void UpdateAcuerdoOkTest()
        {
            var acuerdo = new ContratoAcuerdo
            {
                Id = 10,
                Precio = 10,
                Cantidad = 10,
                ComercialCreadorId = 1,
                DestinoId = 1,
                CampanaId = 1,
                MaterialId = 1,
                MonedaId = "a",
                FechaHasta = new DateTime(2019, 08, 08),
                Descuentos = new List<DescuentoBonificacion>()
                {
                    new DescuentoBonificacion()
                    {
                        Id= 0
                    }
                },
                PrecioPactado = new List<PrecioPactado>() {
                        new PrecioPactado
                        {
                            Precio = 1000,
                            ImportePactado = 100,
                            ContratoId = 1,
                            Id = 1,
                            MonedaPactadoId = "usd",
                            MonedaImportePactadoId = "USD"
                        }
                },
                Calidad = new List<Calidad>()
                {
                    new Calidad
                    {
                        StandardDeCalidadId = 1,
                        NegocioId = 1,
                        Id = 1,
                        CalidadEspecialId = 1,
                        Valor = 10
                    }
                },
                AperturaPrecio = new List<AperturaPrecio>()
                {
                    new AperturaPrecio()
                    {
                        ConceptoAperturaPrecioId= (int)EnumConceptoApertura.Redespacho,
                        Importe = 300
                    },
                    new AperturaPrecio { ConceptoAperturaPrecioId = (int)EnumConceptoApertura.Financiero, Importe =100 }
                }
            };

            repositorioMock.Setup(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()))
                .Returns(new ContratoAcuerdo { Id = 10, Precio = 10, Cantidad = 10, ComercialCreadorId = 1, DestinoId = 1, MaterialId = 1, MonedaId = "a", FechaHasta = new DateTime(2019, 08, 08),
                    Descuentos = new List<DescuentoBonificacion>()
                {
                    new DescuentoBonificacion()
                    {
                        Id= 0
                    }
                }
                });
            var resultado = target.GrabarAcuerdo(acuerdo);

            Assert.That(!resultado.HayError);
            repositorioMock.Verify(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
        [Test]
        public void GrabarAcuerdoConErrorTest()
        {
            var acuerdo = new ContratoAcuerdo
            {
                Id = 10,
                Precio = -10,
                Cantidad = 0,
                ComercialCreadorId = 0,
                DestinoId = 0,
                MaterialId = 0,
                MonedaId = null,
                FechaHasta = new DateTime()
            };

            repositorioMock.Setup(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()))
                .Returns(new ContratoAcuerdo { Id = 10, Precio = 10, Cantidad = 10, ComercialCreadorId = 1, DestinoId = 1, MaterialId = 1, MonedaId = "a", FechaHasta = new DateTime(2019, 08, 08) });
            var resultado = target.GrabarAcuerdo(acuerdo);

            Assert.That(resultado.HayError);
            Assert.AreEqual(8, resultado.Errores.Count);
            repositorioMock.Verify(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void TraerAcuerdoTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, BasicoContrato>>>()))
                .Returns(new BasicoContrato { Id = 10 });
            var resultado = target.TraerAcuerdo(1);

            Assert.AreEqual(10, resultado.Id);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, BasicoContrato>>>()), Times.Once);
        }
        [Test]
        public void TraerDatosCombo()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Material, MaterialQry>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<MaterialQry>() { new MaterialQry { MaterialId = 1, Descripcion = "a" } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Centro, CentroQry>>>(), It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<CentroQry>() { new CentroQry { Id = 1, Descripcion = "a" } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Comercial, ComercialQry>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ComercialQry>() { new ComercialQry { ComercialId = 1, Nombre = "a" } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Moneda, MonedaQry>>>(), It.IsAny<Expression<Func<Moneda, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<MonedaQry>() { new MonedaQry { MonedaId = "a", Descripcion = "a" } });
            var resultado = target.TraerDatosCombo();

            Assert.AreEqual(1, resultado.comercial.Count);
            Assert.AreEqual(1, resultado.material.Count);
            Assert.AreEqual(1, resultado.moneda.Count);
            Assert.AreEqual(1, resultado.destino.Count);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Material, MaterialQry>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Centro, CentroQry>>>(), It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Comercial, ComercialQry>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Moneda, MonedaQry>>>(), It.IsAny<Expression<Func<Moneda, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
        }
        [Test]
        public void TraerTodoContratoAcuerdoTest()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, ContratoAcuerdoIni>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ContratoAcuerdoIni>() { new ContratoAcuerdoIni { Id = 1, Cantidad = 10 } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<Contrato>());
            var resultado = target.TraerTodoContratoAcuerdo();

            Assert.AreEqual(1, resultado.ContratoAcuerdo.Count);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, ContratoAcuerdoIni>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
        }
        [Test]
        public void ObtenerContratoAcuerdoParaAsociarTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, ContratoAcuerdoDto>>>()))
                .Returns(new ContratoAcuerdoDto { Id = 1 });
            var resultado = target.ObtenerContratoAcuerdoParaAsociar(new DateTime(2019, 8, 8), 1, 1, 1);

            Assert.AreEqual(1, resultado.Id = 1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, ContratoAcuerdoDto>>>()), Times.Once);
        }
        [Test]
        public void ConfirmarContratoAcuerdoTest()
        {
            repositorioMock.Setup(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()))
                .Returns(new ContratoAcuerdo { Id = 1, EstadoId = 1 });
            var resultado = target.ConfirmarContratoAcuerdo(1, 1);

            Assert.That(!resultado.HayError);
            repositorioMock.Verify(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
        [Test]
        public void FinalizarAcuerdoErrorFinalizadoTest()
        {
            repositorioMock.Setup(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()))
                .Returns(new ContratoAcuerdo { Id = 1, EstadoId = 5 });
            repositorioMock.Setup(x => x.Obtener<EstadoContrato>(It.IsAny<int>()))
                .Returns(new EstadoContrato { EstadoContratoId = 5, Descripcion = "a" });
            var resultado = target.FinalizarAcuerdo(1);

            Assert.That(resultado.HayError);
            Assert.AreEqual("El Acuerdo ya se encuentra Finalizado", resultado.Errores[0].Message);
            repositorioMock.Verify(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<EstadoContrato>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }
        [Test]
        public void FinalizarAcuerdoErrorRechazadoTest()
        {
            repositorioMock.Setup(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()))
                .Returns(new ContratoAcuerdo { Id = 1, EstadoId = 6 });
            repositorioMock.Setup(x => x.Obtener<EstadoContrato>(It.IsAny<int>()))
                .Returns(new EstadoContrato { EstadoContratoId = 5, Descripcion = "a" });
            var resultado = target.FinalizarAcuerdo(1);

            Assert.That(resultado.HayError);
            Assert.AreEqual("El Acuerdo ya ha sido Rechazado", resultado.Errores[0].Message);
            repositorioMock.Verify(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<EstadoContrato>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }
        [Test]
        public void AnularAcuerdosTest()
        {
            diasHabilesAgentMock.Setup(x => x.UltimoDiaHabil(null)).Returns(new DateTime(2020, 1, 10));
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ContratoAcuerdo>()
                {
                    new ContratoAcuerdo
                    {
                        Cantidad = 10,
                        EstadoId = 2,
                        Id = 1,
                        Fecha = new DateTime(2020, 1, 9)
                    }                    
                });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, double>>>(), It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<double>() { 10.0, 11.0 });
            target.AnularAcuerdos();
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
    }
}