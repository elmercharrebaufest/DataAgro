using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Dto.Distribucion;
using Molinos.DataAgro.Repository;
using Moq;
using NLog;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Test.Managers
{
    [TestFixture]
    public class DistribucionCuposManagerTests
    {
        private DistribucionCuposManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> loggerMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            loggerMock = new Mock<ILogger>();
            target = new DistribucionCuposManager(repositorioMock.Object, loggerMock.Object);
        }

        [Test]
        public void Calcular_RespetaTopeCuitYDescuentaCcpp()
        {
            var request = new DistribucionCuposRequestDto
            {
                Fecha = "2026-06-11",
                Contratos = new List<ContratoSapImportadoDto>
                {
                    CrearContrato("4500012345", "20-12345678-9", "Soja Poroto", 900000m, "Fijo", "acopiador"),
                    CrearContrato("4500019999", "20-11111111-1", "Soja Poroto", 300000m, "Fijo", "productor")
                },
                CcppByCuitMat = new Dictionary<string, decimal> { { "20-12345678-9|Soja Poroto", 60000m } },
                LimitesPorMaterial = new Dictionary<string, int> { { "Soja Poroto", 20 } },
                Configuracion = new DistribucionConfigDto()
            };

            var resultado = target.Calcular(request);
            var contratoPrincipal = resultado.Resultados.Single(x => x.NumeroSAP == "4500012345");

            Assert.That(contratoPrincipal.KgEfectivo, Is.EqualTo(840000m));
            Assert.That(contratoPrincipal.CuposNecesarios, Is.EqualTo(28));
            Assert.That(contratoPrincipal.CuposAsignados, Is.EqualTo(6));
            Assert.That(resultado.Resultados.Where(x => x.Cuit == "20-12345678-9").Sum(x => x.CuposAsignados), Is.LessThanOrEqualTo(6));
        }

        [Test]
        public void Calcular_AplicaCuotaOperador()
        {
            var request = new DistribucionCuposRequestDto
            {
                Fecha = "2026-06-11",
                Contratos = new List<ContratoSapImportadoDto>
                {
                    CrearContrato("4500012345", "20-12345678-9", "Soja Poroto", 300000m, "Fijo", "acopiador"),
                    CrearContrato("4500012346", "20-12345678-8", "Soja Poroto", 300000m, "Fijo", "acopiador"),
                    CrearContrato("4500012347", "20-12345678-7", "Soja Poroto", 300000m, "Fijo", "productor")
                },
                LimitesPorMaterial = new Dictionary<string, int> { { "Soja Poroto", 20 } },
                Configuracion = new DistribucionConfigDto
                {
                    AplicarCuotaOperador = true,
                    CuotasPorOperador = new Dictionary<string, int>
                    {
                        { "acopiador", 40 },
                        { "productor", 60 }
                    }
                }
            };

            var resultado = target.Calcular(request);
            var cuposAcopiador = resultado.Resultados.Where(x => x.OpType == "acopiador").Sum(x => x.CuposAsignados);
            Assert.That(cuposAcopiador, Is.LessThanOrEqualTo(8));
        }

        [Test]
        public void Calcular_MaterialSinLimite_MarcaSinTope()
        {
            var request = new DistribucionCuposRequestDto
            {
                Fecha = "2026-06-11",
                Contratos = new List<ContratoSapImportadoDto>
                {
                    CrearContrato("4500012345", "20-12345678-9", "Soja Poroto", 300000m, "Fijo", "acopiador")
                },
                LimitesPorMaterial = new Dictionary<string, int> { { "Soja Poroto", 0 } },
                Configuracion = new DistribucionConfigDto()
            };

            var resultado = target.Calcular(request);
            Assert.That(resultado.Resultados[0].Estado, Is.EqualTo("sin_tope"));
            Assert.That(resultado.Resultados[0].CuposAsignados, Is.EqualTo(0));
        }

        [Test]
        public void CalcularMultiDia_DescuentaCcppUnaSolaVezYDistribuyeUniforme()
        {
            var request = new DistribucionMultiDiaRequestDto
            {
                Contratos = new List<ContratoSapImportadoDto>
                {
                    CrearContrato("4500012345", "20-12345678-9", "Soja Poroto", 300000m, "Fijo", "acopiador")
                },
                CcppByCuitMat = new Dictionary<string, decimal> { { "20-12345678-9|Soja Poroto", 60000m } },
                Fechas = new List<string> { "2026-06-11", "2026-06-12" },
                LimitesPorDia = new List<LimiteDiaDto>
                {
                    new LimiteDiaDto { Fecha = "2026-06-11", Limites = new Dictionary<string, int> { { "Soja Poroto", 20 } } },
                    new LimiteDiaDto { Fecha = "2026-06-12", Limites = new Dictionary<string, int> { { "Soja Poroto", 20 } } }
                },
                DistribuirUniforme = true,
                Configuracion = new DistribucionConfigDto()
            };

            var resultado = target.CalcularMultiDia(request);
            var asignaciones = resultado.ResultadosPorDia.SelectMany(x => x.Resultados).ToList();

            Assert.That(asignaciones.Sum(x => x.CuposAsignados), Is.EqualTo(8));
            Assert.That(asignaciones[0].CcppDescontado, Is.EqualTo(60000m));
            Assert.That(asignaciones[0].KgEfectivo, Is.EqualTo(240000m));
            Assert.That(resultado.ResultadosPorDia[0].Resultados[0].CuposAsignados, Is.EqualTo(4));
            Assert.That(resultado.ResultadosPorDia[1].Resultados[0].CuposAsignados, Is.EqualTo(4));
        }

        [Test]
        public void CalcularMultiDia_MasDeSieteFechas_LanzaArgumentException()
        {
            var request = new DistribucionMultiDiaRequestDto
            {
                Fechas = Enumerable.Range(1, 8).Select(x => new DateTime(2026, 6, x).ToString("yyyy-MM-dd")).ToList()
            };

            var ex = Assert.Throws<ArgumentException>(() => target.CalcularMultiDia(request));
            Assert.That(ex.Message, Does.Contain("7"));
        }

        [Test]
        public void CalcularMultiDia_FechaSinLimites_LanzaInvalidOperationException()
        {
            var request = new DistribucionMultiDiaRequestDto
            {
                Contratos = new List<ContratoSapImportadoDto> { CrearContrato("4500012345", "20-12345678-9", "Soja Poroto", 300000m, "Fijo", "acopiador") },
                Fechas = new List<string> { "2026-06-11", "2026-06-12" },
                LimitesPorDia = new List<LimiteDiaDto>
                {
                    new LimiteDiaDto { Fecha = "2026-06-11", Limites = new Dictionary<string, int> { { "Soja Poroto", 10 } } }
                }
            };

            var ex = Assert.Throws<InvalidOperationException>(() => target.CalcularMultiDia(request));
            Assert.That(ex.Message, Does.Contain("2026-06-12"));
        }

        private static ContratoSapImportadoDto CrearContrato(string numeroSap, string cuit, string material, decimal kg, string clase, string opType)
        {
            return new ContratoSapImportadoDto
            {
                Numero = numeroSap,
                NumeroSAP = numeroSap,
                Cuit = cuit,
                Material = material,
                Kg = kg,
                DescCl = clase,
                OpType = opType,
                Proveedor = "Proveedor",
                Rank = 3,
                PriceRank = 0,
                FechaContrato = "2026-01-15",
                FechaDesde = "2026-01-01",
                FechaHasta = "2026-06-30"
            };
        }
    }
}
