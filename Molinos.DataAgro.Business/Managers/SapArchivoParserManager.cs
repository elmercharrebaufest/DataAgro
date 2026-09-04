using Excel;
using Molinos.DataAgro.Entities.Dto.Distribucion;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Molinos.DataAgro.Business.Managers
{
    public class SapArchivoParserManager : ISapArchivoParserManager
    {
        private const string TargetCentro = "Planta San Lorenzo";
        private static readonly HashSet<string> CosechasValidas = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "23-24",
            "24-25",
            "25-26"
        };
        private static readonly HashSet<string> ClasesExcluidas = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "MP-Venta granos",
            string.Empty
        };
        private static readonly Dictionary<string, PrioridadContrato> Prioridades = new Dictionary<string, PrioridadContrato>(StringComparer.OrdinalIgnoreCase)
        {
            { "MP-Fason", new PrioridadContrato(1, "MP-Fasón") },
            { "MP-Prest/Devolución", new PrioridadContrato(2, "MP-Préstamo/Dev.") },
            { "Fijo", new PrioridadContrato(3, "Fijo") },
            { "Contrato Hijo", new PrioridadContrato(3, "Fijo (C.Hijo)") },
            { "A Fijar", new PrioridadContrato(4, "A Fijar") }
        };
        private static readonly Dictionary<string, string> Operadores = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "con_corredor", "Con corredor" },
            { "acopiador", "Acopiador directo" },
            { "productor", "Productor directo" },
            { "otros", "Otros / Sin clasif." }
        };

        private readonly IRepositorio repositorio;
        private readonly ILogger logger;

        public SapArchivoParserManager(IRepositorio repositorio, ILogger logger)
        {
            this.repositorio = repositorio;
            this.logger = logger;
        }

        public ImportacionSapResultadoDto ParsearArchivo(Stream stream, string extension, DateTime fecha)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            var filas = LeerFilas(stream, extension);
            var contratos = new List<ContratoSapImportadoDto>();
            var ccppByCuitMat = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            var ccppByMat = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            DateTime? fechaDesdeMin = null;
            DateTime? fechaDesdeMax = null;
            DateTime? fechaHastaMin = null;
            DateTime? fechaHastaMax = null;

            foreach (var fila in filas)
            {
                var tipo = ObtenerValor(fila, "Tipo");
                var centro = ObtenerValor(fila, "Descripción Centro");
                var material = ObtenerValor(fila, "Descripción del Material");
                var kilos = ParsearDecimal(ObtenerValor(fila, "Kilos a recibir total"));

                if (string.Equals(tipo, "CCPP", StringComparison.OrdinalIgnoreCase) && string.Equals(centro, TargetCentro, StringComparison.OrdinalIgnoreCase))
                {
                    var cuit = ObtenerValor(fila, "CUIT Proveedor");
                    var key = string.Format("{0}|{1}", cuit, material);
                    ccppByCuitMat[key] = ObtenerValorDiccionario(ccppByCuitMat, key) + kilos;
                    ccppByMat[material] = ObtenerValorDiccionario(ccppByMat, material) + kilos;
                    continue;
                }

                if (!string.Equals(tipo, "CTO", StringComparison.OrdinalIgnoreCase) || !string.Equals(centro, TargetCentro, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var descCl = ObtenerValor(fila, "Descripción Cl.Contrato");
                if (ClasesExcluidas.Contains(descCl))
                {
                    continue;
                }

                PrioridadContrato prioridad;
                if (!Prioridades.TryGetValue(descCl, out prioridad) || kilos <= 0)
                {
                    continue;
                }

                var cosecha = ObtenerValor(fila, "Cosecha");
                if (!string.IsNullOrWhiteSpace(cosecha) && !CosechasValidas.Contains(cosecha))
                {
                    continue;
                }

                var fechaDesde = ParsearFecha(ObtenerValor(fila, "Fecha Desde"));
                var fechaHasta = ParsearFecha(ObtenerValor(fila, "Fecha Hasta"));
                var fechaHastaContra = ParsearFecha(ObtenerValor(fila, "Fecha Hasta Contra", "Fecha Hasta Contrato")) ?? fechaHasta;
                if (fechaDesde.HasValue)
                {
                    fechaDesdeMin = Min(fechaDesdeMin, fechaDesde.Value);
                    fechaDesdeMax = Max(fechaDesdeMax, fechaDesde.Value);
                }
                if (fechaHasta.HasValue)
                {
                    fechaHastaMin = Min(fechaHastaMin, fechaHasta.Value);
                    fechaHastaMax = Max(fechaHastaMax, fechaHasta.Value);
                }

                if (fechaDesde.HasValue && fechaDesde.Value.Date > fecha.Date)
                {
                    continue;
                }

                var valor = ParsearDecimal(ObtenerValor(fila, "Valor"));
                var pricePt = kilos > 0 && valor > 0 ? decimal.Round(valor * 1000m / kilos, 2) : 0m;
                var priceRank = (string.Equals(descCl, "Fijo", StringComparison.OrdinalIgnoreCase) || string.Equals(descCl, "Contrato Hijo", StringComparison.OrdinalIgnoreCase)) && valor > 0 ? 0 : 1;
                var opType = ObtenerTipoOperador(fila);

                contratos.Add(new ContratoSapImportadoDto
                {
                    Numero = ObtenerValor(fila, "Numero"),
                    NumeroSAP = NormalizarNumeroSap(ObtenerValor(fila, "Numero")),
                    DescCl = descCl,
                    PrioLabel = prioridad.Label,
                    Rank = prioridad.Rank,
                    Material = material,
                    Kg = kilos,
                    Cosecha = cosecha,
                    FechaContrato = FormatearFecha(ParsearFecha(ObtenerValor(fila, "Fecha"))),
                    FechaDesde = FormatearFecha(fechaDesde),
                    FechaHasta = FormatearFecha(fechaHasta),
                    FechaHastaContra = FormatearFecha(fechaHastaContra),
                    Cuit = ObtenerValor(fila, "CUIT Proveedor"),
                    Proveedor = ObtenerValor(fila, "Descripción Proveedor"),
                    Corredor = ObtenerValor(fila, "Descripción Corredor"),
                    Clasificacion = ObtenerValor(fila, "Clasificación"),
                    OpType = opType,
                    OpLabel = Operadores[opType],
                    IsSust = EsSustentable(fila),
                    Valor = valor,
                    Moneda = ObtenerValor(fila, "Moneda"),
                    PricePt = pricePt,
                    PriceRank = priceRank
                });
            }

            contratos = contratos
                .OrderBy(x => x.Rank)
                .ThenBy(x => x.PriceRank)
                .ThenBy(x => ParsearFecha(x.FechaHasta) ?? DateTime.MaxValue)
                .ThenBy(x => ParsearFecha(x.FechaContrato) ?? DateTime.MaxValue)
                .ToList();

            var resultado = new ImportacionSapResultadoDto();
            resultado.Contratos = contratos;
            resultado.CcppByCuitMat = ccppByCuitMat.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
            resultado.CcppByMat = ccppByMat.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
            resultado.Materiales = contratos.Select(x => x.Material)
                .Concat(ccppByMat.Keys)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();
            resultado.Estadisticas = ConstruirEstadisticas(contratos, fechaDesdeMin, fechaDesdeMax, fechaHastaMin, fechaHastaMax);

            logger.Debug(string.Format("ParsearArchivo contratos={0}", contratos.Count));
            return resultado;
        }

        private List<Dictionary<string, string>> LeerFilas(Stream stream, string extension)
        {
            var extensionNormalizada = (extension ?? string.Empty).ToLowerInvariant();
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            if (extensionNormalizada == ".txt" || extensionNormalizada == ".tsv")
            {
                return LeerFilasTexto(stream);
            }

            if (extensionNormalizada == ".xls" || extensionNormalizada == ".xlsx")
            {
                return LeerFilasExcel(stream, extensionNormalizada);
            }

            throw new ArgumentException("Formato de archivo no soportado", "extension");
        }

        private List<Dictionary<string, string>> LeerFilasTexto(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true))
            {
                var lines = new List<string>();
                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine());
                }

                var headerIndex = -1;
                for (var i = 0; i < Math.Min(lines.Count, 20); i++)
                {
                    var headers = SepararLinea(lines[i]);
                    if (headers.Any(x => string.Equals(x, "Tipo", StringComparison.OrdinalIgnoreCase)))
                    {
                        headerIndex = i;
                        break;
                    }
                }

                if (headerIndex < 0)
                {
                    throw new InvalidOperationException("No se encontró la fila de encabezados esperada con la columna 'Tipo'.");
                }

                var headerRow = SepararLinea(lines[headerIndex]);
                return ConstruirFilas(headerRow, lines.Skip(headerIndex + 1).Select(SepararLinea));
            }
        }

        private List<Dictionary<string, string>> LeerFilasExcel(Stream stream, string extension)
        {
            IExcelDataReader excelReader = null;
            try
            {
                var esOpenXml = EsFirmaOpenXml(stream);
                excelReader = esOpenXml
                    ? ExcelReaderFactory.CreateOpenXmlReader(stream)
                    : ExcelReaderFactory.CreateBinaryReader(stream);

                var dataSet = excelReader.AsDataSet();
                var table = dataSet.Tables.Count > 0 ? dataSet.Tables[0] : null;
                if (table == null)
                {
                    return new List<Dictionary<string, string>>();
                }

                var headerIndex = -1;
                for (var rowIndex = 0; rowIndex < Math.Min(table.Rows.Count, 20); rowIndex++)
                {
                    var values = ObtenerValoresFila(table.Rows[rowIndex]);
                    if (values.Any(x => string.Equals(x, "Tipo", StringComparison.OrdinalIgnoreCase)))
                    {
                        headerIndex = rowIndex;
                        break;
                    }
                }

                if (headerIndex < 0)
                {
                    throw new InvalidOperationException("No se encontró la fila de encabezados esperada con la columna 'Tipo'.");
                }

                var headers = ObtenerValoresFila(table.Rows[headerIndex]);
                var rows = new List<string[]>();
                for (var rowIndex = headerIndex + 1; rowIndex < table.Rows.Count; rowIndex++)
                {
                    rows.Add(ObtenerValoresFila(table.Rows[rowIndex]));
                }

                return ConstruirFilas(headers, rows);
            }
            finally
            {
                if (excelReader != null)
                {
                    excelReader.Close();
                }
            }
        }

        private static List<Dictionary<string, string>> ConstruirFilas(string[] headers, IEnumerable<string[]> rows)
        {
            var resultado = new List<Dictionary<string, string>>();
            foreach (var cells in rows)
            {
                if (cells == null || cells.Length == 0)
                {
                    continue;
                }

                var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (var index = 0; index < headers.Length; index++)
                {
                    var key = headers[index] ?? string.Empty;
                    row[key] = index < cells.Length ? (cells[index] ?? string.Empty).Trim() : string.Empty;
                }

                string tipo;
                if (row.TryGetValue("Tipo", out tipo) && !string.IsNullOrWhiteSpace(tipo))
                {
                    resultado.Add(row);
                }
            }

            return resultado;
        }

        private static bool EsFirmaOpenXml(Stream stream)
        {
            if (!stream.CanSeek)
            {
                throw new InvalidOperationException("El stream de archivo debe permitir posicionamiento (seek) para detectar el formato.");
            }

            var firma = new byte[4];
            var leidos = stream.Read(firma, 0, firma.Length);
            stream.Seek(0, SeekOrigin.Begin);

            // Los archivos .xlsx (OOXML) son ZIP y comienzan con "PK" (0x50 0x4B).
            // Los archivos .xls (BIFF/OLE2) comienzan con 0xD0 0xCF 0x11 0xE0.
            return leidos >= 2 && firma[0] == 0x50 && firma[1] == 0x4B;
        }

        private static string[] ObtenerValoresFila(DataRow row)
        {
            return row.ItemArray.Select(x => (x ?? string.Empty).ToString().Trim()).ToArray();
        }

        private static string[] SepararLinea(string line)
        {
            return (line ?? string.Empty)
                .Split('\t')
                .Select(x => x.Trim().TrimEnd('\r'))
                .ToArray();
        }

        private static string ObtenerValor(IDictionary<string, string> fila, params string[] columnas)
        {
            foreach (var columna in columnas)
            {
                string valor;
                if (fila.TryGetValue(columna, out valor))
                {
                    return valor == null ? string.Empty : valor.Trim();
                }
            }

            return string.Empty;
        }

        private static decimal ParsearDecimal(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return 0m;
            }

            decimal resultado;
            var normalizado = valor.Trim().Replace(" ", string.Empty).Replace(".", string.Empty).Replace(",", ".");
            return decimal.TryParse(normalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out resultado) ? resultado : 0m;
        }

        private static DateTime? ParsearFecha(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return null;
            }

            DateTime fecha;
            var formatos = new[] { "dd.MM.yyyy", "d.M.yyyy", "yyyy-MM-dd", "dd/MM/yyyy", "d/M/yyyy" };
            if (DateTime.TryParseExact(valor.Trim(), formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
            {
                return fecha.Date;
            }

            if (DateTime.TryParse(valor, out fecha))
            {
                return fecha.Date;
            }

            return null;
        }

        private static string FormatearFecha(DateTime? fecha)
        {
            return fecha.HasValue ? fecha.Value.ToString("yyyy-MM-dd") : null;
        }

        private static bool EsSustentable(IDictionary<string, string> fila)
        {
            return string.Equals(ObtenerValor(fila, "Sust."), "X", StringComparison.OrdinalIgnoreCase)
                || string.Equals(ObtenerValor(fila, "EPA"), "X", StringComparison.OrdinalIgnoreCase);
        }

        private static string ObtenerTipoOperador(IDictionary<string, string> fila)
        {
            if (!string.IsNullOrWhiteSpace(ObtenerValor(fila, "Descripción Corredor")) || !string.IsNullOrWhiteSpace(ObtenerValor(fila, "CUIT Corredor")))
            {
                return "con_corredor";
            }

            var clasificacion = ObtenerValor(fila, "Clasificación").ToUpperInvariant();
            if (clasificacion.Contains("ACOPIADOR"))
            {
                return "acopiador";
            }

            if (clasificacion.Contains("PRODUCTOR"))
            {
                return "productor";
            }

            return "otros";
        }

        private static string NormalizarNumeroSap(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
            {
                return string.Empty;
            }

            var normalizado = numero.TrimStart('0');
            return string.IsNullOrWhiteSpace(normalizado) ? numero : normalizado;
        }

        private static decimal ObtenerValorDiccionario(IDictionary<string, decimal> diccionario, string key)
        {
            decimal valor;
            return diccionario.TryGetValue(key, out valor) ? valor : 0m;
        }

        private static DateTime? Min(DateTime? actual, DateTime candidato)
        {
            return !actual.HasValue || candidato < actual.Value ? (DateTime?)candidato : actual;
        }

        private static DateTime? Max(DateTime? actual, DateTime candidato)
        {
            return !actual.HasValue || candidato > actual.Value ? (DateTime?)candidato : actual;
        }

        private static EstadisticasImportacionDto ConstruirEstadisticas(IEnumerable<ContratoSapImportadoDto> contratos, DateTime? fechaDesdeMin, DateTime? fechaDesdeMax, DateTime? fechaHastaMin, DateTime? fechaHastaMax)
        {
            var lista = contratos.ToList();
            var totalKg = lista.Sum(x => x.Kg);
            var estadisticas = new EstadisticasImportacionDto
            {
                Sustentables = lista.Count(x => x.IsSust),
                FechaDesdeMin = FormatearFecha(fechaDesdeMin),
                FechaDesdeMax = FormatearFecha(fechaDesdeMax),
                FechaHastaMin = FormatearFecha(fechaHastaMin),
                FechaHastaMax = FormatearFecha(fechaHastaMax)
            };

            foreach (var grupo in lista.GroupBy(x => new { x.OpType, x.OpLabel }).OrderBy(x => x.Key.OpType))
            {
                var kg = grupo.Sum(x => x.Kg);
                estadisticas.PorTipoOperador.Add(new Dictionary<string, object>
                {
                    { "tipoId", grupo.Key.OpType },
                    { "label", grupo.Key.OpLabel },
                    { "contratos", grupo.Count() },
                    { "kgTotal", kg },
                    { "porcentaje", totalKg == 0 ? 0m : decimal.Round(kg * 100m / totalKg, 1) }
                });
            }

            foreach (var grupo in lista.GroupBy(x => x.DescCl).OrderBy(x => x.Key))
            {
                var kg = grupo.Sum(x => x.Kg);
                estadisticas.PorClase.Add(new Dictionary<string, object>
                {
                    { "clase", grupo.Key },
                    { "contratos", grupo.Count() },
                    { "kgTotal", kg },
                    { "porcentaje", totalKg == 0 ? 0m : decimal.Round(kg * 100m / totalKg, 1) }
                });
            }

            return estadisticas;
        }

        private sealed class PrioridadContrato
        {
            public PrioridadContrato(int rank, string label)
            {
                Rank = rank;
                Label = label;
            }

            public int Rank { get; private set; }
            public string Label { get; private set; }
        }
    }
}
