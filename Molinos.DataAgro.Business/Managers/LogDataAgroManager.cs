using JsonDiffer;
using JsonDiffPatchDotNet;
using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.Managers
{
    public class LogDataAgroManager : ILogDataAgroManager
    {
        private readonly IRepositorio repositorio;

        public LogDataAgroManager(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public int LogCambiosDataAgro(BasicoContrato cambios, TipoAccionLogDataAgro tipoDeAccion, Type tipoDeContrato)
        {
            return LogGuardarCambios(cambios, tipoDeAccion, cambios.Id, tipoDeContrato);
        }
        public int LogCambiosDataAgro(StoredPorProveedorResult cambios, TipoAccionLogDataAgro tipoDeAccion, int? idProveedor)
        {
            return LogGuardarCambios(cambios, tipoDeAccion, idProveedor, typeof(Proveedor));
        }
        public int LogCambiosDataAgro(CupoDto cambios, TipoAccionLogDataAgro tipoDeAccion)
        {
            return (cambios.Id == 0) ? LogGuardarCambios(cambios, tipoDeAccion, null) : LogGuardarCambios(cambios, tipoDeAccion, cambios.Id);
        }

        private int LogGuardarCambios<T>(T cambios, TipoAccionLogDataAgro tipoDeAccion, int? id, Type claseDeObjeto = null)
        {
            var usuarioComercial = PermisosHelper.ObtenerUsuario();
            Type tipoDelObjeto = (claseDeObjeto != null) ? claseDeObjeto : cambios.GetType();
            string nombreDelTipodeObjeto = tipoDelObjeto.Name.Split('_')[0];
            nombreDelTipodeObjeto = nombreDelTipodeObjeto.Replace("Basico", String.Empty).Replace("Dto", String.Empty).Trim();

            string jsonObjeto = JsonConvert.SerializeObject(cambios, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                Formatting = Formatting.Indented
            });

            try
            {
                var logAgregado = new LogDataAgro
                {
                    Usuario = usuarioComercial,
                    Fecha = DateTime.Now,
                    DatoModificado = jsonObjeto,
                    Clase = (tipoDelObjeto.IsSubclassOf(typeof(Negocio))) ? $"Negocio - { nombreDelTipodeObjeto }" : nombreDelTipodeObjeto,
                    AccionRealizada = tipoDeAccion.ToString(),
                    CupoId = (nombreDelTipodeObjeto.Contains("Cupo")) ? id : null,
                    ProveedorId = (nombreDelTipodeObjeto.Contains("Proveedor")) ? id : null,
                    NegocioId = (tipoDelObjeto.IsSubclassOf(typeof(Negocio))) ? id : null,
                    RangoConfirmacionAutomaticaId = (nombreDelTipodeObjeto.Contains("RangoConfirmacionAutomatica")) ? id : null,
                };
                repositorio.Agregar<LogDataAgro>(logAgregado);

                return repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                throw new Exception($"{nombreDelTipodeObjeto} Guardado, error en LogDataAgro. {e.Message}", e);

            }
        }
        public DataSourceResult ListarDatosLogDataAgro(DataSourceRequest request, List<int> equipo)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerTodosLogsDataAgro(request, equipo));
        }

        public DatosModificadosLogDataAgroDto TraerDatosModificadosPorId(int idLogDataAgro)
        {
            LogDataAgroDto logAnterior = null;
            var logActual = repositorio.Listar<LogDataAgro>(x => x.Id == idLogDataAgro)
                .Select(log => new LogDataAgroDto
                {
                    Id = log.Id,
                    Usuario = log.Usuario,
                    Fecha = log.Fecha,
                    Clase = log.Clase,
                    AccionRealizada = log.AccionRealizada,
                    DatoModificado = log.DatoModificado,
                    CupoId = log.CupoId,
                    NegocioId = log.NegocioId,
                    ProveedorId = log.ProveedorId,
                    RangoConfirmacionAutomaticaId = log.RangoConfirmacionAutomaticaId,
                }).FirstOrDefault();

            if (logActual.AccionRealizada.Contains("Crear"))
            {
                logAnterior = logActual;
            }
            else
            {
                logAnterior = repositorio.Listar<LogDataAgro>(x => x.Id < logActual.Id &&
                        (((logActual.CupoId != null && x.CupoId != null) ? (x.CupoId == logActual.CupoId) : false) ||
                        ((logActual.NegocioId != null && x.NegocioId != null) ? (x.NegocioId == logActual.NegocioId) : false) ||
                        ((logActual.ProveedorId != null && x.ProveedorId != null) ? (x.ProveedorId == logActual.ProveedorId) : false) ||
                        ((logActual.RangoConfirmacionAutomaticaId != null && x.RangoConfirmacionAutomaticaId != null) ? (x.RangoConfirmacionAutomaticaId == logActual.RangoConfirmacionAutomaticaId) : false))
                        )
                       .OrderByDescending(x => x.Id)
                       .Select(x => new LogDataAgroDto
                       {
                           Id = x.Id,
                           Usuario = x.Usuario,
                           Fecha = x.Fecha,
                           Clase = x.Clase,
                           AccionRealizada = x.AccionRealizada,
                           DatoModificado = x.DatoModificado,
                           CupoId = x.CupoId,
                           NegocioId = x.NegocioId,
                           ProveedorId = x.ProveedorId,
                       }).FirstOrDefault() ?? logActual;
            }


            if (logAnterior.NegocioId != logActual.NegocioId ||
                logAnterior.CupoId != logActual.CupoId ||
                logAnterior.ProveedorId != logActual.ProveedorId)
            {
                throw new Exception("No se pueden comparar diferentes Tipos de Registros.");
            }

            var j1 = JToken.Parse(logActual.DatoModificado);
            var j2 = JToken.Parse(logAnterior.DatoModificado);


            var jdp = new JsonDiffPatch();
            JToken diffResult = jdp.Diff(j2, j1);
            List<DatoModificadosLogDataAgroDto> cambiados = new List<DatoModificadosLogDataAgroDto>();
            if (diffResult != null)
            {
                foreach (var item in diffResult)
                {
                    cambiados.Add(new DatoModificadosLogDataAgroDto
                    {
                        Anterior = ((JContainer)((JProperty)item).Value).First.ToString(),
                        Actual = ((JContainer)((JProperty)item).Value).Last.ToString(),
                        Campo = ((JProperty)item).Name
                    });
                }
            }



            return new DatosModificadosLogDataAgroDto
            {
                LogActual = logActual,
                LogAnterior = logAnterior,
                CamposCambiados = cambiados
            };
        }

        public LogDataAgroDto Obtener(int idLogDataAgro, bool anterior = false)
        {
            LogDataAgroDto log = repositorio.Listar<LogDataAgro>(x => x.Id == idLogDataAgro)
                .Select(x => new LogDataAgroDto
                {
                    Id = x.Id,
                    Usuario = x.Usuario,
                    Fecha = x.Fecha,
                    Clase = x.Clase,
                    AccionRealizada = x.AccionRealizada,
                    DatoModificado = x.DatoModificado,
                    CupoId = x.CupoId,
                    NegocioId = x.NegocioId,
                    ProveedorId = x.ProveedorId,
                    RangoConfirmacionAutomaticaId = x.RangoConfirmacionAutomaticaId,
                }).FirstOrDefault();

            if (anterior)
            {
                if (log.AccionRealizada == TipoAccionLogDataAgro.Crear.ToString())
                {
                    return new LogDataAgroDto();
                }
                else
                {
                    log = repositorio.Listar<LogDataAgro>(x => x.Id < log.Id &&
                        (
                        ((log.CupoId != null && x.CupoId != null) ? (x.CupoId == log.CupoId) : false) ||
                        ((log.NegocioId != null && x.NegocioId != null) ? (x.NegocioId == log.NegocioId) : false) ||
                        ((log.ProveedorId != null && x.ProveedorId != null) ? (x.ProveedorId == log.ProveedorId) : false) ||
                        ((log.RangoConfirmacionAutomaticaId != null && x.RangoConfirmacionAutomaticaId != null) ? (x.RangoConfirmacionAutomaticaId == log.RangoConfirmacionAutomaticaId) : false)
                        ))
                       .OrderByDescending(x => x.Id)
                       .Select(x => new LogDataAgroDto
                       {
                           Id = x.Id,
                           Usuario = x.Usuario,
                           Fecha = x.Fecha,
                           Clase = x.Clase,
                           AccionRealizada = x.AccionRealizada,
                           DatoModificado = x.DatoModificado,
                           CupoId = x.CupoId,
                           NegocioId = x.NegocioId,
                           ProveedorId = x.ProveedorId,
                           RangoConfirmacionAutomaticaId = x.RangoConfirmacionAutomaticaId
                       }).FirstOrDefault() ?? new LogDataAgroDto();
                }
            }

            return log;

        }

        public int LogCambiosDataAgro(RangoConfirmacionAutomaticaDto cambios, TipoAccionLogDataAgro tipoDeAccion)
        {
            return (cambios.Id == 0) ? LogGuardarCambios(cambios, tipoDeAccion, null) : LogGuardarCambios(cambios, tipoDeAccion, cambios.Id);
        }

    }
}
