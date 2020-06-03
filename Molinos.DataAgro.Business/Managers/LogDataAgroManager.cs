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

        public int LogCambiosDataAgro(Cupo cambios, TipoAccionLogDataAgro tipoDeAccion)
        {
            return (cambios.Id == 0) ? LogGuardarCambios(cambios, tipoDeAccion, null) : LogGuardarCambios(cambios, tipoDeAccion, cambios.Id);
        }
        public int LogCambiosDataAgro(List<Cupo> cambios, TipoAccionLogDataAgro tipoDeAccion)
        {
            foreach (var cupo in cambios)
            {
                if (cupo.Id == 0)
                {
                    LogGuardarCambios(cupo, tipoDeAccion, null);
                }
                else
                {
                    LogGuardarCambios(cupo, tipoDeAccion, cupo.Id);
                }
            }
            return repositorio.GuardarCambios();
        }
        public int LogCambiosDataAgro(Proveedor cambios, TipoAccionLogDataAgro tipoDeAccion)
        {
            return LogGuardarCambios(cambios, tipoDeAccion, cambios.ProveedorId);
        }
        public int LogCambiosDataAgro(Negocio cambios, TipoAccionLogDataAgro tipoDeAccion)
        {
            return LogGuardarCambios(cambios, tipoDeAccion, cambios.Id);
        }

        private int LogGuardarCambios<T>(T cambios, TipoAccionLogDataAgro tipoDeAccion, int? id)
        {
            var usuarioComercial = PermisosHelper.ObtenerUsuario();
            var tipoDeObjeto = cambios.GetType().Name.Split('_')[0].Trim().TrimEnd();


            string jsonObjeto = JsonConvert.SerializeObject(cambios, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });

            try
            {
                repositorio.Agregar<LogDataAgro>(new LogDataAgro
                {
                    Usuario = usuarioComercial,
                    Fecha = DateTime.Now,
                    DatoModificado = jsonObjeto,
                    Clase = (cambios.GetType().IsSubclassOf(typeof(Negocio))) ? $"Negocio - { tipoDeObjeto }" : tipoDeObjeto,
                    AccionRealizada = tipoDeAccion.ToString(),
                    CupoId = (tipoDeObjeto == "Cupo") ? id : null,
                    ProveedorId = (tipoDeObjeto == "Proveedor") ? id : null,
                    NegocioId = (cambios.GetType().IsSubclassOf(typeof(Negocio))) ? id : null,
                });

                return repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                throw new Exception($"Cambios Guardados, error en LogDataAgro. {e.Message}", e);

            }

        }

        //public IEnumerable<LogDataAgroDto> ListarDatosLogDataAgro()
        //{



        //    return repositorio.Listar<LogDataAgro, LogDataAgroDto>(x => new LogDataAgroDto
        //    {
        //        Id = x.Id,
        //        AccionRealizada = x.AccionRealizada,
        //        DatoModificado = x.DatoModificado,
        //        Fecha = x.Fecha,
        //        Usuario = x.Usuario,
        //        Clase = x.Clase,
        //        CupoId = x.CupoId,
        //        NegocioId = x.NegocioId,
        //        ProveedorId = x.ProveedorId,
        //    });
        //}
        public DataSourceResult ListarDatosLogDataAgro(DataSourceRequest request, List<int> equipo)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerTodosLogsDataAgro(request, equipo));
        }
    }
}
