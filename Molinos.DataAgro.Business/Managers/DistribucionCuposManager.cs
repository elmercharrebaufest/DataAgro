using Molinos.DataAgro.Entities.Dto.Distribucion;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Molinos.DataAgro.Business.Managers
{
    public class DistribucionCuposManager : IDistribucionCuposManager
    {
        private const int CupoKg = 30000;
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;

        public DistribucionCuposManager(IRepositorio repositorio, ILogger logger)
        {
            this.repositorio = repositorio;
            this.logger = logger;
        }

        public DistribucionResponseDto Calcular(DistribucionCuposRequestDto request)
        {
            request = request ?? new DistribucionCuposRequestDto();
            var configuracion = request.Configuracion ?? new DistribucionConfigDto();
            var contratos = PrepararContratos(request.Contratos, configuracion);
            var ccppBalance = new Dictionary<string, decimal>(request.CcppByCuitMat ?? new Dictionary<string, decimal>(), StringComparer.OrdinalIgnoreCase);
            var limites = new Dictionary<string, int>(request.LimitesPorMaterial ?? new Dictionary<string, int>(), StringComparer.OrdinalIgnoreCase);
            var materiales = contratos.Select(x => x.Material)
                .Concat(limites.Keys)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            var cuitMaxPct = configuracion.CuitMaxPct.HasValue && configuracion.CuitMaxPct.Value > 0 ? configuracion.CuitMaxPct.Value : 0.30m;
            var remaining = materiales.ToDictionary(x => x, x => Math.Max(0, ObtenerLimite(limites, x)), StringComparer.OrdinalIgnoreCase);
            var cuitCaps = materiales.ToDictionary(x => x, x => Math.Max(1, (int)Math.Floor(ObtenerLimite(limites, x) * cuitMaxPct)), StringComparer.OrdinalIgnoreCase);
            var cuitUsed = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var operatorBudgets = ConstruirPresupuestosOperador(materiales, limites, configuracion);
            var claseBudgets = ConstruirPresupuestosClase(materiales, limites, configuracion);
            var resultados = new List<ResultadoContratoDistribucionDto>();

            foreach (var contrato in contratos)
            {
                var necesidad = CalcularNecesidad(contrato, ccppBalance);
                var resultado = CrearResultadoBase(contrato, necesidad);
                var limiteMaterial = ObtenerLimite(limites, contrato.Material);
                if (limiteMaterial <= 0)
                {
                    resultado.Estado = "sin_tope";
                    resultados.Add(resultado);
                    continue;
                }

                if (resultado.CuposNecesarios <= 0)
                {
                    resultado.Estado = "sin_cupos";
                    resultados.Add(resultado);
                    continue;
                }

                var asignables = remaining.ContainsKey(contrato.Material) ? remaining[contrato.Material] : 0;
                asignables = Math.Min(asignables, ObtenerDisponibleCuit(cuitCaps, cuitUsed, contrato));
                asignables = Math.Min(asignables, ObtenerDisponible(operatorBudgets, contrato.Material, contrato.OpType));
                asignables = Math.Min(asignables, ObtenerDisponible(claseBudgets, contrato.Material, contrato.DescCl));

                var asignados = Math.Min(resultado.CuposNecesarios, Math.Max(0, asignables));
                resultado.CuposAsignados = asignados;
                resultado.Estado = asignados <= 0 ? "sin_cupos" : (asignados < resultado.CuposNecesarios ? "parcial" : "completo");

                if (asignados > 0)
                {
                    remaining[contrato.Material] = Math.Max(0, remaining[contrato.Material] - asignados);
                    DescontarDisponible(cuitUsed, string.Format("{0}|{1}", contrato.Cuit, contrato.Material), asignados);
                    DescontarDisponible(operatorBudgets, string.Format("{0}|{1}", contrato.Material, contrato.OpType), asignados);
                    DescontarDisponible(claseBudgets, string.Format("{0}|{1}", contrato.Material, contrato.DescCl), asignados);
                }

                resultados.Add(resultado);
            }

            var response = new DistribucionResponseDto();
            response.Fecha = request.Fecha;
            response.Resultados = resultados;
            response.ResumenPorMaterial = ConstruirResumenPorMaterial(resultados, limites, materiales);
            response.ResumenPorOperador = ConstruirResumenPorOperador(resultados, limites, configuracion, materiales);
            response.ResumenPorClase = ConstruirResumenPorClase(resultados, limites, configuracion, materiales);
            response.Sap = resultados.Where(x => x.CuposAsignados > 0)
                .Select(x => new FilaSapDto
                {
                    FechaSugerida = FormatearFechaSap(request.Fecha),
                    CantidadDeCupos = x.CuposAsignados,
                    ContratoSAP = x.NumeroSAP
                })
                .ToList();

            logger.Debug(string.Format("Distribucion calcular contratos={0}", resultados.Count));
            return response;
        }

        public DistribucionMultiDiaResponseDto CalcularMultiDia(DistribucionMultiDiaRequestDto request)
        {
            request = request ?? new DistribucionMultiDiaRequestDto();
            if (request.Fechas == null || request.Fechas.Count == 0)
            {
                return new DistribucionMultiDiaResponseDto();
            }

            if (request.Fechas.Count > 7)
            {
                throw new ArgumentException("El máximo de días permitido es 7");
            }

            var configuracion = request.Configuracion ?? new DistribucionConfigDto();
            var limitesPorDia = (request.LimitesPorDia ?? new List<LimiteDiaDto>()).ToDictionary(x => x.Fecha, x => x.Limites ?? new Dictionary<string, int>(), StringComparer.OrdinalIgnoreCase);
            foreach (var fecha in request.Fechas)
            {
                if (!limitesPorDia.ContainsKey(fecha))
                {
                    throw new InvalidOperationException(string.Format("La fecha {0} no tiene límites definidos", fecha));
                }
            }

            var contratos = PrepararContratos(request.Contratos, configuracion);
            var ccppBalance = new Dictionary<string, decimal>(request.CcppByCuitMat ?? new Dictionary<string, decimal>(), StringComparer.OrdinalIgnoreCase);
            var estados = contratos.Select(x => CrearEstado(x, CalcularNecesidad(x, ccppBalance))).ToList();
            var response = new DistribucionMultiDiaResponseDto();
            var materiales = contratos.Select(x => x.Material)
                .Concat(limitesPorDia.SelectMany(x => x.Value.Keys))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            var cuitMaxPct = configuracion.CuitMaxPct.HasValue && configuracion.CuitMaxPct.Value > 0 ? configuracion.CuitMaxPct.Value : 0.30m;
            var contratosAsignados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var totalKgEfectivo = estados.Sum(x => x.KgEfectivo);

            for (var diaIndex = 0; diaIndex < request.Fechas.Count; diaIndex++)
            {
                var fecha = request.Fechas[diaIndex];
                var limitesDia = new Dictionary<string, int>(limitesPorDia[fecha], StringComparer.OrdinalIgnoreCase);
                var remaining = materiales.ToDictionary(x => x, x => Math.Max(0, ObtenerLimite(limitesDia, x)), StringComparer.OrdinalIgnoreCase);
                var cuitCaps = materiales.ToDictionary(x => x, x => Math.Max(1, (int)Math.Floor(ObtenerLimite(limitesDia, x) * cuitMaxPct)), StringComparer.OrdinalIgnoreCase);
                var cuitUsed = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                var operatorBudgets = ConstruirPresupuestosOperador(materiales, limitesDia, configuracion);
                var claseBudgets = ConstruirPresupuestosClase(materiales, limitesDia, configuracion);
                var resultadosDia = new List<ResultadoContratoDistribucionDto>();
                var sapDia = new List<FilaSapDto>();

                for (var contratoIndex = 0; contratoIndex < estados.Count; contratoIndex++)
                {
                    var estado = estados[contratoIndex];
                    if (estado.CuposRestantes <= 0 || estado.KgEfectivo <= 0)
                    {
                        continue;
                    }

                    var limiteMaterial = ObtenerLimite(limitesDia, estado.Contrato.Material);
                    if (limiteMaterial <= 0)
                    {
                        continue;
                    }

                    var daysLeft = request.Fechas.Count - diaIndex;
                    var objetivoDelDia = request.DistribuirUniforme ? (int)Math.Ceiling((decimal)estado.CuposRestantes / daysLeft) : estado.CuposRestantes;
                    var asignables = remaining.ContainsKey(estado.Contrato.Material) ? remaining[estado.Contrato.Material] : 0;
                    asignables = Math.Min(asignables, objetivoDelDia);
                    asignables = Math.Min(asignables, ObtenerDisponibleCuit(cuitCaps, cuitUsed, estado.Contrato));
                    asignables = Math.Min(asignables, ObtenerDisponible(operatorBudgets, estado.Contrato.Material, estado.Contrato.OpType));
                    asignables = Math.Min(asignables, ObtenerDisponible(claseBudgets, estado.Contrato.Material, estado.Contrato.DescCl));
                    if (asignables <= 0)
                    {
                        continue;
                    }

                    var asignados = Math.Min(estado.CuposRestantes, asignables);
                    estado.CuposRestantes -= asignados;
                    remaining[estado.Contrato.Material] = Math.Max(0, remaining[estado.Contrato.Material] - asignados);
                    DescontarDisponible(cuitUsed, string.Format("{0}|{1}", estado.Contrato.Cuit, estado.Contrato.Material), asignados);
                    DescontarDisponible(operatorBudgets, string.Format("{0}|{1}", estado.Contrato.Material, estado.Contrato.OpType), asignados);
                    DescontarDisponible(claseBudgets, string.Format("{0}|{1}", estado.Contrato.Material, estado.Contrato.DescCl), asignados);
                    contratosAsignados.Add(estado.Contrato.NumeroSAP + "|" + estado.Contrato.Cuit + "|" + estado.Contrato.Material);

                    resultadosDia.Add(new ResultadoContratoDistribucionDto
                    {
                        NumeroSAP = estado.Contrato.NumeroSAP,
                        Proveedor = estado.Contrato.Proveedor,
                        Cuit = estado.Contrato.Cuit,
                        Material = estado.Contrato.Material,
                        OpType = estado.Contrato.OpType,
                        Clase = estado.Contrato.DescCl,
                        KgContrato = estado.Contrato.Kg,
                        CcppDescontado = estado.CcppDescontado,
                        KgEfectivo = estado.KgEfectivo,
                        CuposNecesarios = estado.CuposNecesarios,
                        CuposAsignados = asignados,
                        Estado = estado.CuposRestantes <= 0 ? "completo" : "parcial",
                        IsSust = estado.Contrato.IsSust,
                        PricePt = estado.Contrato.PricePt
                    });

                    sapDia.Add(new FilaSapDto
                    {
                        FechaSugerida = FormatearFechaSap(fecha),
                        CantidadDeCupos = asignados,
                        ContratoSAP = estado.Contrato.NumeroSAP
                    });
                }

                response.ResultadosPorDia.Add(new ResultadoDiaDto
                {
                    Fecha = fecha,
                    Resultados = resultadosDia,
                    Sap = sapDia
                });
                response.SapConsolidado.AddRange(sapDia);
            }

            var totalCuposAsignados = response.SapConsolidado.Sum(x => x.CantidadDeCupos);
            response.ResumenGlobal = new ResumenGlobalDto
            {
                TotalCuposAsignados = totalCuposAsignados,
                TotalContratosAsignados = contratosAsignados.Count,
                PorcentajeCoberturaKg = totalKgEfectivo <= 0 ? 100m : decimal.Round(Math.Min(100m, totalCuposAsignados * CupoKg * 100m / totalKgEfectivo), 1)
            };

            return response;
        }

        private static List<ContratoSapImportadoDto> PrepararContratos(IEnumerable<ContratoSapImportadoDto> contratos, DistribucionConfigDto configuracion)
        {
            var lista = (contratos ?? Enumerable.Empty<ContratoSapImportadoDto>()).Where(x => x != null);
            if (configuracion.ExcluirFason)
            {
                lista = lista.Where(x => !string.Equals(x.DescCl, "MP-Fason", StringComparison.OrdinalIgnoreCase));
            }

            if (configuracion.ExcluirAgenteCompra)
            {
                lista = lista.Where(x => !EsAgenteCompra(x));
            }

            if (configuracion.HabilitarFiltroFechas)
            {
                var filtroFechaDesdeMin = ParsearFecha(configuracion.FiltroFechaDesdeMin);
                var filtroFechaDesdeMax = ParsearFecha(configuracion.FiltroFechaDesdeMax);
                var filtroFechaHastaMin = ParsearFecha(configuracion.FiltroFechaHastaMin);
                var filtroFechaHastaMax = ParsearFecha(configuracion.FiltroFechaHastaMax);

                lista = lista.Where(x => CumpleFiltro(x, filtroFechaDesdeMin, filtroFechaDesdeMax, filtroFechaHastaMin, filtroFechaHastaMax));
            }

            return lista.OrderBy(x => x.Rank)
                .ThenBy(x => x.PriceRank)
                .ThenBy(x => configuracion.PriorizarSustentables ? (x.IsSust ? 0 : 1) : 0)
                .ThenBy(x => ParsearFecha(x.FechaHasta) ?? DateTime.MaxValue)
                .ThenBy(x => ParsearFecha(x.FechaContrato) ?? DateTime.MaxValue)
                .ToList();
        }

        private static bool CumpleFiltro(ContratoSapImportadoDto contrato, DateTime? fechaDesdeMin, DateTime? fechaDesdeMax, DateTime? fechaHastaMin, DateTime? fechaHastaMax)
        {
            var fechaDesde = ParsearFecha(contrato.FechaDesde);
            var fechaHasta = ParsearFecha(contrato.FechaHasta);
            if (fechaDesdeMin.HasValue && fechaDesde.HasValue && fechaDesde.Value < fechaDesdeMin.Value)
            {
                return false;
            }
            if (fechaDesdeMax.HasValue && fechaDesde.HasValue && fechaDesde.Value > fechaDesdeMax.Value)
            {
                return false;
            }
            if (fechaHastaMin.HasValue && fechaHasta.HasValue && fechaHasta.Value < fechaHastaMin.Value)
            {
                return false;
            }
            if (fechaHastaMax.HasValue && fechaHasta.HasValue && fechaHasta.Value > fechaHastaMax.Value)
            {
                return false;
            }
            return true;
        }

        private static bool EsAgenteCompra(ContratoSapImportadoDto contrato)
        {
            var texto = string.Join("|", new[]
            {
                contrato.Clasificacion,
                contrato.Corredor,
                contrato.OpType,
                contrato.OpLabel,
                contrato.Proveedor
            }).ToUpperInvariant();

            return texto.Contains("AG. DE COMPRA") || texto.Contains("AGENTE DE COMPRA") || texto.Contains("AG.COMPRA");
        }

        private static NecesidadContrato CalcularNecesidad(ContratoSapImportadoDto contrato, IDictionary<string, decimal> ccppBalance)
        {
            var key = string.Format("{0}|{1}", contrato.Cuit, contrato.Material);
            decimal balance;
            ccppBalance.TryGetValue(key, out balance);
            var kgEfectivo = Math.Max(0m, contrato.Kg - balance);
            var ccppDescontado = contrato.Kg - kgEfectivo;
            if (balance > 0)
            {
                ccppBalance[key] = Math.Max(0m, balance - contrato.Kg);
            }

            return new NecesidadContrato
            {
                CcppDescontado = ccppDescontado,
                KgEfectivo = kgEfectivo,
                CuposNecesarios = kgEfectivo > 0 ? (int)Math.Ceiling(kgEfectivo / CupoKg) : 0
            };
        }

        private static ResultadoContratoDistribucionDto CrearResultadoBase(ContratoSapImportadoDto contrato, NecesidadContrato necesidad)
        {
            return new ResultadoContratoDistribucionDto
            {
                NumeroSAP = contrato.NumeroSAP,
                Proveedor = contrato.Proveedor,
                Cuit = contrato.Cuit,
                Material = contrato.Material,
                OpType = contrato.OpType,
                Clase = contrato.DescCl,
                KgContrato = contrato.Kg,
                CcppDescontado = necesidad.CcppDescontado,
                KgEfectivo = necesidad.KgEfectivo,
                CuposNecesarios = necesidad.CuposNecesarios,
                CuposAsignados = 0,
                IsSust = contrato.IsSust,
                PricePt = contrato.PricePt
            };
        }

        private static ContratoEstado CrearEstado(ContratoSapImportadoDto contrato, NecesidadContrato necesidad)
        {
            return new ContratoEstado
            {
                Contrato = contrato,
                CcppDescontado = necesidad.CcppDescontado,
                KgEfectivo = necesidad.KgEfectivo,
                CuposNecesarios = necesidad.CuposNecesarios,
                CuposRestantes = necesidad.CuposNecesarios
            };
        }

        private static Dictionary<string, int> ConstruirPresupuestosOperador(IEnumerable<string> materiales, IDictionary<string, int> limites, DistribucionConfigDto configuracion)
        {
            var presupuestos = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            if (!configuracion.AplicarCuotaOperador)
            {
                return presupuestos;
            }

            foreach (var material in materiales)
            {
                var limite = ObtenerLimite(limites, material);
                foreach (var cuota in configuracion.CuotasPorOperador ?? new Dictionary<string, int>())
                {
                    presupuestos[string.Format("{0}|{1}", material, cuota.Key)] = (int)Math.Floor(limite * cuota.Value / 100m);
                }
            }

            return presupuestos;
        }

        private static Dictionary<string, int> ConstruirPresupuestosClase(IEnumerable<string> materiales, IDictionary<string, int> limites, DistribucionConfigDto configuracion)
        {
            var presupuestos = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            if (!configuracion.AplicarCuotaClase)
            {
                return presupuestos;
            }

            foreach (var material in materiales)
            {
                var limite = ObtenerLimite(limites, material);
                foreach (var cuota in configuracion.CuotasPorClase ?? new Dictionary<string, int>())
                {
                    presupuestos[string.Format("{0}|{1}", material, cuota.Key)] = (int)Math.Floor(limite * cuota.Value / 100m);
                }
            }

            return presupuestos;
        }

        private static List<ResumenDistribucionDto> ConstruirResumenPorMaterial(IEnumerable<ResultadoContratoDistribucionDto> resultados, IDictionary<string, int> limites, IEnumerable<string> materiales)
        {
            return materiales.Select(material =>
            {
                var grupo = resultados.Where(x => string.Equals(x.Material, material, StringComparison.OrdinalIgnoreCase)).ToList();
                var asignados = grupo.Sum(x => x.CuposAsignados);
                var limite = ObtenerLimite(limites, material);
                return new ResumenDistribucionDto
                {
                    Material = material,
                    Limite = limite,
                    CuposAsignados = asignados,
                    CuposRestantes = Math.Max(0, limite - asignados),
                    ContratosAsignados = grupo.Count(x => x.CuposAsignados > 0),
                    ContratosSinCupos = grupo.Count(x => x.CuposAsignados == 0)
                };
            }).ToList();
        }

        private static List<ResumenDistribucionDto> ConstruirResumenPorOperador(IEnumerable<ResultadoContratoDistribucionDto> resultados, IDictionary<string, int> limites, DistribucionConfigDto configuracion, IEnumerable<string> materiales)
        {
            return resultados.GroupBy(x => x.OpType).Select(grupo => new ResumenDistribucionDto
            {
                Operador = grupo.Key,
                Limite = configuracion.AplicarCuotaOperador && configuracion.CuotasPorOperador.ContainsKey(grupo.Key)
                    ? materiales.Sum(material => (int)Math.Floor(ObtenerLimite(limites, material) * configuracion.CuotasPorOperador[grupo.Key] / 100m))
                    : 0,
                CuposAsignados = grupo.Sum(x => x.CuposAsignados),
                CuposRestantes = 0,
                ContratosAsignados = grupo.Count(x => x.CuposAsignados > 0),
                ContratosSinCupos = grupo.Count(x => x.CuposAsignados == 0)
            }).OrderBy(x => x.Operador).ToList();
        }

        private static List<ResumenDistribucionDto> ConstruirResumenPorClase(IEnumerable<ResultadoContratoDistribucionDto> resultados, IDictionary<string, int> limites, DistribucionConfigDto configuracion, IEnumerable<string> materiales)
        {
            return resultados.GroupBy(x => x.Clase).Select(grupo => new ResumenDistribucionDto
            {
                Clase = grupo.Key,
                Limite = configuracion.AplicarCuotaClase && configuracion.CuotasPorClase.ContainsKey(grupo.Key)
                    ? materiales.Sum(material => (int)Math.Floor(ObtenerLimite(limites, material) * configuracion.CuotasPorClase[grupo.Key] / 100m))
                    : 0,
                CuposAsignados = grupo.Sum(x => x.CuposAsignados),
                CuposRestantes = 0,
                ContratosAsignados = grupo.Count(x => x.CuposAsignados > 0),
                ContratosSinCupos = grupo.Count(x => x.CuposAsignados == 0)
            }).OrderBy(x => x.Clase).ToList();
        }

        private static int ObtenerDisponible(IDictionary<string, int> presupuesto, string material, string clave)
        {
            if (presupuesto == null || presupuesto.Count == 0)
            {
                return int.MaxValue;
            }

            int disponible;
            return presupuesto.TryGetValue(string.Format("{0}|{1}", material, clave), out disponible) ? Math.Max(0, disponible) : int.MaxValue;
        }

        private static int ObtenerDisponibleCuit(IDictionary<string, int> caps, IDictionary<string, int> usados, ContratoSapImportadoDto contrato)
        {
            var key = string.Format("{0}|{1}", contrato.Cuit, contrato.Material);
            var materialKey = contrato.Material ?? string.Empty;
            int cap;
            caps.TryGetValue(materialKey, out cap);
            int used;
            usados.TryGetValue(key, out used);
            return Math.Max(0, cap - used);
        }

        private static int ObtenerLimite(IDictionary<string, int> limites, string key)
        {
            if (limites == null || string.IsNullOrWhiteSpace(key))
            {
                return 0;
            }

            int value;
            return limites.TryGetValue(key, out value) ? value : 0;
        }

        private static void DescontarDisponible(IDictionary<string, int> diccionario, string key, int cantidad)
        {
            if (diccionario == null || !diccionario.ContainsKey(key))
            {
                return;
            }

            diccionario[key] = Math.Max(0, diccionario[key] - cantidad);
        }

        private static DateTime? ParsearFecha(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return null;
            }

            DateTime fecha;
            var formatos = new[] { "yyyy-MM-dd", "dd/MM/yyyy", "dd.MM.yyyy" };
            if (DateTime.TryParseExact(valor, formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
            {
                return fecha.Date;
            }

            if (DateTime.TryParse(valor, out fecha))
            {
                return fecha.Date;
            }

            return null;
        }

        private static string FormatearFechaSap(string fecha)
        {
            var parsed = ParsearFecha(fecha);
            return parsed.HasValue ? parsed.Value.ToString("dd/MM/yyyy") : fecha;
        }

        private sealed class NecesidadContrato
        {
            public decimal CcppDescontado { get; set; }
            public decimal KgEfectivo { get; set; }
            public int CuposNecesarios { get; set; }
        }

        private sealed class ContratoEstado
        {
            public ContratoSapImportadoDto Contrato { get; set; }
            public decimal CcppDescontado { get; set; }
            public decimal KgEfectivo { get; set; }
            public int CuposNecesarios { get; set; }
            public int CuposRestantes { get; set; }
        }
    }
}
