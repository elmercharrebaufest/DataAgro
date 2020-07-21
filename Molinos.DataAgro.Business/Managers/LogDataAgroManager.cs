using Autofac.Extras.NLog;
using JsonDiffer;
using JsonDiffPatchDotNet;
using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.Managers
{
    public class LogDataAgroManager : ILogDataAgroManager
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;

        public LogDataAgroManager(IRepositorio repositorio, ILogger logger)
        {
            this.repositorio = repositorio;
            this.logger = logger;
        }

        public int LogCambiosDataAgro(BasicoContrato cambios, TipoAccionLogDataAgro tipoDeAccion, Type tipoDeContrato)
        {
            var resolver = new IgnorePropertiesResolver(new[] { "Estado", "CantidadMaximaCupo", "EstadoOrder", "FechaOrder", "GrupoCompra" });
            string descripcion = string.IsNullOrEmpty(cambios.ContratoSAP) ? cambios.Id.ToString() : cambios.ContratoSAP;
            return LogGuardarCambios(cambios, tipoDeAccion, cambios.Id, "Negocio - " + cambios.TipoNegocio, descripcion, resolver);
        }
        public int LogCambiosDataAgro(StoredPorProveedorResult cambios, TipoAccionLogDataAgro tipoDeAccion, int idProveedor)
        {
            var resolver = new IgnorePropertiesResolver(new[] { "EstadoCuit", "CantidadMaximaCupo" });
            string descripcion = "";
            if (cambios.BasicoProveedorTraerPorProveedores != null && cambios.BasicoProveedorTraerPorProveedores.Count > 1)
            {
                cambios.BasicoProveedorTraerPorProveedores = new List<BasicoProveedor> { cambios.BasicoProveedorTraerPorProveedores.FirstOrDefault() };
            }
            if (cambios.BasicoProveedorTraerPorProveedores != null && cambios.BasicoProveedorTraerPorProveedores.Count > 0)
            {
                descripcion = cambios.BasicoProveedorTraerPorProveedores.First().RazonSocial + " (" + cambios.BasicoProveedorTraerPorProveedores.First().CUIT + ")";
            }
            return LogGuardarCambios(cambios, tipoDeAccion, idProveedor, "Proveedor", descripcion,resolver);
        }
        public int LogCambiosDataAgro(CupoDto cambios, TipoAccionLogDataAgro tipoDeAccion)
        {
            var resolver = new IgnorePropertiesResolver(new[] { "EstadoOrden" });

            string descripcion = string.IsNullOrEmpty(cambios.CupoSap)? cambios.Id.ToString():cambios.CupoSap;

            return LogGuardarCambios(cambios, tipoDeAccion, cambios.Id, "Cupo",descripcion,resolver);
        }

        private int LogGuardarCambios<T>(T cambios, TipoAccionLogDataAgro tipoDeAccion, int id, string clase, string descripcion, IgnorePropertiesResolver resolver = null)
        {
            if (resolver == null)
            {
                resolver = new IgnorePropertiesResolver(new[] { "" });
            }
            var usuarioComercial = "";
            //logger.Debug("LogGuardarCambios");
            try
            {
                //logger.Debug("LogGuardarCambios ObtenerUsuario");
                usuarioComercial = PermisosHelper.ObtenerUsuario();
                //logger.Debug("LogGuardarCambios ObtenerUsuario :" + (usuarioComercial ?? "null"));

            }
            catch (Exception)
            { }
            if (string.IsNullOrEmpty(usuarioComercial))
            {
                try
                {
                    logger.Debug("LogGuardarCambios OperationContext");
                    usuarioComercial = OperationContext.Current.ServiceSecurityContext.WindowsIdentity.Name.Split('\\').Last();
                    logger.Debug("LogGuardarCambios OperationContext :" + (usuarioComercial ?? "null"));

                }
                catch (Exception)
                { }
            }

            if (!string.IsNullOrEmpty(usuarioComercial))
            {
                logger.Debug("LogGuardarCambios Comercial :" + (usuarioComercial ?? "null"));

                var comercial = repositorio.Obtener<Comercial, string>(x => x.IdActiveDirectory == usuarioComercial, x => x.Nombres + " " + x.Apellido);
                if (comercial != null)
                {
                    usuarioComercial = comercial;
                    logger.Debug("LogGuardarCambios Comercial :" + (usuarioComercial ?? "null"));
                }
            }

            if (usuarioComercial == null)
            {
                usuarioComercial = "";
            }
            logger.Debug("LogGuardarCambios final :" + (usuarioComercial ?? "null"));

            string jsonObjeto = JsonConvert.SerializeObject(cambios, new JsonSerializerSettings()
            {
                ContractResolver = resolver,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                Formatting = Formatting.Indented,
            });

            try
            {
                var logAgregado = new LogDataAgro
                {
                    Usuario = usuarioComercial,
                    Fecha = DateTime.Now,
                    DatoModificado = jsonObjeto,
                    Clase = clase,
                    Tipo = cambios.GetType().Name,
                    AccionRealizada = tipoDeAccion.ToString(),
                    ClaseId = id,
                    Descripcion = descripcion,
                };
                repositorio.Agregar<LogDataAgro>(logAgregado);

                return repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                throw new Exception($"{cambios.GetType().Name} Guardado, error en LogDataAgro. {e.Message}", e);

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
                    ClaseId = log.ClaseId,
                }).FirstOrDefault();

            if (logActual.AccionRealizada.Contains("Crear"))
            {
                logAnterior = null;
            }
            else
            {
                logAnterior = repositorio.Listar<LogDataAgro>(x => x.Id < logActual.Id && x.ClaseId == logActual.ClaseId && logActual.Clase == x.Clase)
                       .OrderByDescending(x => x.Id)
                       .Select(x => new LogDataAgroDto
                       {
                           Id = x.Id,
                           Usuario = x.Usuario,
                           Fecha = x.Fecha,
                           Clase = x.Clase,
                           AccionRealizada = x.AccionRealizada,
                           DatoModificado = x.DatoModificado,
                           ClaseId = x.ClaseId,

                       }).FirstOrDefault() ?? logActual;
            }

            var jActual = JToken.Parse(logActual.DatoModificado);
            var jAnterior = JToken.Parse(logAnterior == null ? "{}" : logAnterior.DatoModificado);
            var jdp = new JsonDiffPatch();
            JToken diffResult = jdp.Diff(jAnterior, jActual);

            List<DatoModificadosLogDataAgroDto> cambiados = new List<DatoModificadosLogDataAgroDto>();
            if (logAnterior != null)
            {
                if (diffResult != null)
                {
                    foreach (var item in diffResult)
                    {
                        if (((JProperty)item).Name != null && (!((JProperty)item).Name.ToLower().EndsWith("id")) && !((JProperty)item).Name.ToLower().Contains("formateado"))// saco los dis
                        {
                            var campo = AddSpacesToSentence(((JProperty)item).Name, ':');
                            if (!((JContainer)((JProperty)item).Value).ToString().StartsWith("{"))
                            {
                                if (!((JProperty)item).Name.ToLower().EndsWith("id") && !((JProperty)item).Name.ToLower().EndsWith("formateado"))
                                {
                                    var modificado = new DatoModificadosLogDataAgroDto
                                    {
                                        Anterior = (((JContainer)((JProperty)item).Value).Count == 1 ? "" : BuscaFechaYFormatea(((JContainer)((JProperty)item).Value).First().ToString())),
                                        Actual = BuscaFechaYFormatea(((JContainer)((JProperty)item).Value).Last().ToString()),
                                        Campo = campo
                                    };
                                    if (!(modificado.Anterior == "" && modificado.Actual == ""))
                                    {
                                        cambiados.Add(modificado);
                                    }

                                }

                            }
                            else//lista modificada
                            {
                                var items = (JContainer)((JProperty)item).Value;
                                foreach (var item2 in items.Children())
                                {
                                    if (((JProperty)item2).Name != "_t")
                                    {
                                        foreach (var item3 in ((JProperty)item2).Value.Children())
                                        {
                                            if (!item3.ToString().StartsWith("{"))
                                            {
                                                if (item3.ToString() != "0")
                                                {
                                                    if (!((JProperty)item3).Name.ToLower().EndsWith("id") && !((JProperty)item3).Name.ToLower().EndsWith("formateado"))
                                                    {
                                                        var modificado = new DatoModificadosLogDataAgroDto
                                                        {
                                                            Anterior = (((JProperty)item3).First.Count() == 1 ? "" : BuscaFechaYFormatea(((JProperty)item3).First.First.ToString())),
                                                            Actual = BuscaFechaYFormatea(((JProperty)item3).First.Last.ToString()),
                                                            Campo = campo + " - " + AddSpacesToSentence(((JProperty)item3).Name, ':')
                                                        };
                                                        if (!(modificado.Anterior == "" && modificado.Actual == ""))
                                                        {
                                                            cambiados.Add(modificado);
                                                        }
                                                    }
                                                }

                                            }
                                            else // item nuevo o borrado en la lista
                                            {
                                                foreach (var item4 in item3.Children())
                                                {
                                                    if (((JProperty)item4).Name != "_t" && !((JProperty)item4).Name.ToLower().EndsWith("id") && !((JProperty)item4).Name.ToLower().EndsWith("formateado"))
                                                    {
                                                        var value = ((JProperty)item4).First == null ? "" : ((JProperty)item4).First.ToString();

                                                        bool actual = logActual.DatoModificado.Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace(".00", ".0").Contains(item3.ToString().Replace(" ", "").Replace("\r", "").Replace("\n", ""));
                                                        var modificado = new DatoModificadosLogDataAgroDto
                                                        {
                                                            Anterior = !actual ? BuscaFechaYFormatea(value) : "",
                                                            Actual = actual ? BuscaFechaYFormatea(value) : "",
                                                            Campo = campo + " - " + AddSpacesToSentence(((JProperty)item4).Name, ':')
                                                        };
                                                        if (!(modificado.Anterior == "" && modificado.Actual == ""))
                                                        {
                                                            cambiados.Add(modificado);
                                                        }
                                                    }

                                                }
                                            }

                                        }
                                    }
                                }
                            }

                        }

                    }
                }

            }
            else// si es crear
            {
                ArmarListaCrear(jActual, cambiados, "");
            }


            return new DatosModificadosLogDataAgroDto
            {
                LogActual = logActual,
                LogAnterior = logAnterior,
                CamposCambiados = cambiados
            };
        }

        private void ArmarListaCrear(JToken jActual, List<DatoModificadosLogDataAgroDto> cambiados, string campoNombre)
        {
            foreach (var item in jActual)
            {
                var campo = AddSpacesToSentence(((JProperty)item).Name, ':');
                if (!string.IsNullOrEmpty(campoNombre))
                {
                    campo = campoNombre + " - " + campo;
                }
                if (((JProperty)item).Name != null && (!((JProperty)item).Name.ToLower().EndsWith("id")) && !((JProperty)item).Name.ToLower().Contains("formateado"))// saco los dis
                {
                    if (((JProperty)item).Value.ToString() == "" || ((JProperty)item).Value.ToString() == "null")
                    {
                        continue;
                    }
                    if (!((JProperty)item).Value.ToString().StartsWith("["))
                    {
                        var modificado = new DatoModificadosLogDataAgroDto
                        {
                            Anterior = "",
                            Actual = BuscaFechaYFormatea(((JProperty)item).Value.ToString()),
                            Campo = campo
                        };
                        cambiados.Add(modificado);
                    }
                    else
                    {
                        var items = ((JProperty)item).Value;
                        if (items.Count() > 0)
                        {
                            foreach (var items2 in items)
                            {
                                ArmarListaCrear(items2, cambiados, campo);
                            }


                        }

                    }
                }


            }
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
                    ClaseId = x.ClaseId,
                    Tipo = x.Tipo,
                    Descripcion = x.Descripcion
                }).FirstOrDefault();

            if (anterior)
            {
                if (log.AccionRealizada == TipoAccionLogDataAgro.Crear.ToString())
                {
                    return new LogDataAgroDto();
                }
                else
                {
                    log = repositorio.Listar<LogDataAgro>(x => x.Id < log.Id && x.ClaseId == log.ClaseId && x.Clase == log.Clase)
                       .OrderByDescending(x => x.Id)
                       .Select(x => new LogDataAgroDto
                       {
                           Id = x.Id,
                           Usuario = x.Usuario,
                           Fecha = x.Fecha,
                           Clase = x.Clase,
                           AccionRealizada = x.AccionRealizada,
                           DatoModificado = x.DatoModificado,
                           ClaseId = x.ClaseId,
                           Tipo = x.Tipo,
                           Descripcion = x.Descripcion
                       }).FirstOrDefault() ?? new LogDataAgroDto();
                }
            }

            return log;

        }

        public int LogCambiosDataAgro(RangoConfirmacionAutomaticaDto cambios, TipoAccionLogDataAgro tipoDeAccion)
        {
            return LogGuardarCambios(cambios, tipoDeAccion, cambios.Id, "RangoConfirmacionAutomatica",cambios.Id.ToString());
        }

        public int LogCambiosDataAgro(PrecioMoaDto cambios, TipoAccionLogDataAgro tipoDeAccion)
        {
            return LogGuardarCambios(cambios, tipoDeAccion, cambios.Id, "PrecioMoa",cambios.Id.ToString());
        }
        public int LogCambiosDataAgro(HabilitacionFijacionDto cambios, TipoAccionLogDataAgro tipoDeAccion)
        {
            return LogGuardarCambios(cambios, tipoDeAccion, cambios.Id, "Habilitacion Fijacion",cambios.Id.ToString());
        }

        public int LogCambiosDataAgro(HabilitacionPizarraDto cambios, TipoAccionLogDataAgro tipoDeAccion)
        {
            return LogGuardarCambios(cambios, tipoDeAccion, cambios.Id, "HabilitacionPizarra",cambios.Id.ToString());
        }


        public string AddSpacesToSentence(string text, char limite)
        {
            text = text.Replace("BasicoProveedorTraerPorProveedores", "Proveedor");
            text = text.Replace("ContactosComercialesTraerPorProveedores", "ContactosComerciales");
            text = text.Replace("ContactosComercialesTraerPorProveedores", "ContactosComerciales");
            text = text.Replace("ActividadHistoriaTraerPorProveedores", "ActividadHistoria");
            text = text.Replace("ProveedorCampoDetalle", "Establecimientos");
            text = text.Replace("AcopioMaterialPorProveedores", "AcopioMaterial");
            text = text.Replace("CampoProduccionAcopioPorProveedores", "Produccion");
            text = text.Replace("ObjetivosTraerPorProveedorId", "Objetivos");
            text = text.Replace("_", " ");
            if (!text.StartsWith("Descripcion") && text.Contains("Descripcion"))
            {
                text = text.Replace("Descripcion", "");
            }
            bool limiteEncontrado = false;
            if (string.IsNullOrWhiteSpace(text))
                return "";
            text = UppercaseFirst(text);
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                var letra = text[i];
                limiteEncontrado = limiteEncontrado ? limiteEncontrado : limite == letra;
                if (char.IsUpper(text[i]) && !char.IsUpper(text[i - 1]) && text[i - 1] != ' ' && !limiteEncontrado)
                    newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        public string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public string BuscaFechaYFormatea(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            //s = s.Trim();
            s = s.Replace("false", "No");
            s = s.Replace("true", "Si");
            s = s.Replace("False", "No");
            s = s.Replace("True", "Si");
            s = s.Replace("null", "");
            s = s.Replace("\"", "");
            s = s.Replace("_", "");

            Regex formato1 = new Regex("[0-9]{4}-[0-1]?[0-9]-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}.[0-9]{3}"); //2020-06-30T16:36:23.597
            Regex formato2 = new Regex("[0-9]{4}-[0-1]?[0-9]-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}"); //2020-06-30T00:00:00
            Regex formato3 = new Regex("[0-9]{4}-[0-1]?[0-9]-[0-9]{2} T[0-9]{2}:[0-9]{2}:[0-9]{2}"); //2020-08-02 T00:00:00

            try
            {
                if (formato1.IsMatch(s))
                {
                    Match mat = formato1.Match(s);
                    DateTime date = DateTime.ParseExact(mat.ToString(), "yyyy-MM-dd'T'HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                    s = formato1.Replace(s, date.Second > 0 || date.Minute > 0 || date.Hour > 0 ? date.ToString("dd-MM-yyyy HH:mm:ss") : date.ToString("dd-MM-yyyy"));
                }
                if (formato2.IsMatch(s))
                {
                    Match mat = formato2.Match(s);
                    DateTime date = DateTime.ParseExact(mat.ToString(), "yyyy-MM-dd'T'HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    s = formato2.Replace(s, date.Second > 0 || date.Minute > 0 || date.Hour > 0 ? date.ToString("dd-MM-yyyy HH:mm:ss") : date.ToString("dd-MM-yyyy"));
                }
                if (formato3.IsMatch(s))
                {
                    Match mat = formato3.Match(s);
                    DateTime date = DateTime.ParseExact(mat.ToString(), "yyyy-MM-dd 'T'HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    s = formato3.Replace(s, date.Second > 0 || date.Minute > 0 || date.Hour > 0 ? date.ToString("dd-MM-yyyy HH:mm:ss") : date.ToString("dd-MM-yyyy"));
                }

            }
            catch (Exception ex)
            {

            }


            return s;
        }

        public List<BasicoContrato> ListarNegocios(string text)
        {
            var negocio = repositorio.Listar<Negocio, BasicoContrato>(x => new BasicoContrato
            {
                Id = x.Id,
                TipoNegocio = (x.TipoNegocio == null ? "" : (x is Contrato && (x as Contrato).Madre == true) ? "CONVENIO" : (x is Contrato && (x as Contrato).Madre == false) ? "FIJ. CONVENIO" : (x is Contrato && (x as Contrato).EsFason == true) ? "FASON MP" : (x is Contrato && (x as Contrato).TipoAgenteCompraId > 0) ? "AGENTE DE COMPRAS MP" : (x is ContratoAcuerdo && (x as ContratoAcuerdo).TipoAgenteCompraId > 0) ? "ACUERDO AGENTE" : x.TipoNegocio.Descripcion),
                Negocio = x.ContratoSAP == null || x.ContratoSAP == "" ? x.Id.ToString() : x.ContratoSAP,
                FechaFormateado = SqlFunctions.DateName("day", x.Fecha).Trim() + "-" + SqlFunctions.StringConvert((double)x.Fecha.Month).TrimStart() + "-" + SqlFunctions.DateName("year", x.Fecha),
                Fecha = x.Fecha,
                Cuit = x.Proveedor == null ? "" : x.Proveedor.CUIT,
                Proveedor = (x is AgenteCompra) ? (x as AgenteCompra).Operador.Descripcion : x.Proveedor == null ? "" : x.Proveedor.RazonSocial,
                Material = x.Material.Descripcion,
            }, x => x.ContratoSAP.Contains(text) || SqlFunctions.StringConvert((double)x.Id).Contains(text)
            , 15, "Fecha", Entities.Helpers.DirOrden.Desc);

            return negocio;
        }
    }
}
