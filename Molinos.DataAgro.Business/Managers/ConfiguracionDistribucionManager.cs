using Molinos.DataAgro.Entities.Dto.Distribucion;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Business.Managers
{
    public class ConfiguracionDistribucionManager : IConfiguracionDistribucionManager
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;

        public ConfiguracionDistribucionManager(IRepositorio repositorio, ILogger logger)
        {
            this.repositorio = repositorio;
            this.logger = logger;
        }

        public ConfiguracionDistribucionDto ObtenerConfiguracion()
        {
            var entity = repositorio.ObtenerPrimero<ConfiguracionesDistribucionPlanta>(x => x.PlantaCodigo == "SL");
            if (entity == null)
            {
                return CrearDefault();
            }

            return new ConfiguracionDistribucionDto
            {
                PlantaCodigo = entity.PlantaCodigo,
                PlantaNombre = entity.PlantaNombre,
                CupoKg = entity.CupoKg,
                CuitMaxPct = entity.CuitMaxPct,
                CosechasValidas = DeserializarLista(entity.CosechasValidas),
                ClasesExcluidas = DeserializarLista(entity.ClasesExcluidas),
                LimitesPredeterminadosPorMaterial = DeserializarDiccionarioInt(entity.LimitesPredeterminados),
                PreciosReferencia = DeserializarDiccionarioDecimal(entity.PreciosReferencia),
                CuotasPorOperador = DeserializarDiccionarioInt(entity.CuotasPorOperador),
                CuotasPorClase = DeserializarDiccionarioInt(entity.CuotasPorClase),
                UltimaActualizacion = entity.UltimaActualizacion.HasValue ? entity.UltimaActualizacion.Value.ToString("o") : null
            };
        }

        public void GuardarConfiguracion(ConfiguracionDistribucionDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentException("La configuración es obligatoria");
            }

            if (dto.CuitMaxPct < 0m || dto.CuitMaxPct > 1m)
            {
                throw new ArgumentException("cuitMaxPct debe ser un valor entre 0.0 y 1.0");
            }

            ValidarCuotas(dto.CuotasPorOperador, "Las cuotas por operador no pueden superar el 100%");
            ValidarCuotas(dto.CuotasPorClase, "Las cuotas por clase no pueden superar el 100%");

            var entity = repositorio.ObtenerPrimero<ConfiguracionesDistribucionPlanta>(x => x.PlantaCodigo == "SL");
            if (entity == null)
            {
                entity = new ConfiguracionesDistribucionPlanta { Id = 1 };
                repositorio.Agregar(entity);
            }

            entity.PlantaCodigo = string.IsNullOrWhiteSpace(dto.PlantaCodigo) ? "SL" : dto.PlantaCodigo;
            entity.PlantaNombre = string.IsNullOrWhiteSpace(dto.PlantaNombre) ? "Planta San Lorenzo" : dto.PlantaNombre;
            entity.CupoKg = dto.CupoKg <= 0 ? 30000 : dto.CupoKg;
            entity.CuitMaxPct = dto.CuitMaxPct;
            entity.CosechasValidas = JsonConvert.SerializeObject(dto.CosechasValidas ?? new List<string>());
            entity.ClasesExcluidas = JsonConvert.SerializeObject(dto.ClasesExcluidas ?? new List<string>());
            entity.LimitesPredeterminados = JsonConvert.SerializeObject(dto.LimitesPredeterminadosPorMaterial ?? new Dictionary<string, int>());
            entity.PreciosReferencia = JsonConvert.SerializeObject(dto.PreciosReferencia ?? new Dictionary<string, decimal>());
            entity.CuotasPorOperador = JsonConvert.SerializeObject(dto.CuotasPorOperador ?? new Dictionary<string, int>());
            entity.CuotasPorClase = JsonConvert.SerializeObject(dto.CuotasPorClase ?? new Dictionary<string, int>());
            entity.UltimaActualizacion = DateTime.UtcNow;

            repositorio.GuardarCambios();
            logger.Debug("GuardarConfiguracionDistribucion");
        }

        private static void ValidarCuotas(IDictionary<string, int> cuotas, string mensaje)
        {
            if (cuotas == null)
            {
                return;
            }

            if (cuotas.Values.Sum() > 100)
            {
                throw new ArgumentException(mensaje);
            }
        }

        private static ConfiguracionDistribucionDto CrearDefault()
        {
            return new ConfiguracionDistribucionDto
            {
                PlantaCodigo = "SL",
                PlantaNombre = "Planta San Lorenzo",
                CupoKg = 30000,
                CuitMaxPct = 0.30m,
                CosechasValidas = new List<string> { "23-24", "24-25", "25-26" },
                ClasesExcluidas = new List<string> { "MP-Venta granos" },
                LimitesPredeterminadosPorMaterial = new Dictionary<string, int>(),
                PreciosReferencia = new Dictionary<string, decimal>(),
                CuotasPorOperador = new Dictionary<string, int>(),
                CuotasPorClase = new Dictionary<string, int>()
            };
        }

        private static List<string> DeserializarLista(string valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(valor) ?? new List<string>();
        }

        private static Dictionary<string, int> DeserializarDiccionarioInt(string valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? new Dictionary<string, int>()
                : JsonConvert.DeserializeObject<Dictionary<string, int>>(valor) ?? new Dictionary<string, int>();
        }

        private static Dictionary<string, decimal> DeserializarDiccionarioDecimal(string valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? new Dictionary<string, decimal>()
                : JsonConvert.DeserializeObject<Dictionary<string, decimal>>(valor) ?? new Dictionary<string, decimal>();
        }
    }
}
