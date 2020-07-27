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
using System.Globalization;
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
            var resolver = new IgnorePropertiesResolver(new[] { "Estado", "CantidadMaximaCupo", "Fecha_Order", "GrupoCompra", "Estado_Order", "DesdeFijacionFormateado", "FechaCiertaFormateado", "FechaDesdeFormateado", "FechaFormateado", "FechaHastaFormateado", "FechaOperacionFormateado", "Fecha_DolarizadoFormateado", "HastaFijacionFormateado" });
            string descripcion = string.IsNullOrEmpty(cambios.ContratoSAP) ? cambios.Id.ToString() : cambios.Id.ToString() + " - " + cambios.ContratoSAP.TrimStart('0');
            if (!string.IsNullOrEmpty(cambios.ContratoSAP))
            {
                var tipo = cambios.GetType().Name;
                var logs = repositorio.Listar<LogDataAgro>(x => x.ClaseId == cambios.Id && x.Tipo == tipo);
                foreach (var log in logs)
                {
                    log.Descripcion = descripcion;
                }
                repositorio.GuardarCambios();
            }
            return LogGuardarCambios(cambios, tipoDeAccion, cambios.Id, "Negocio - " + cambios.TipoNegocio, descripcion, resolver);
        }
        public int LogCambiosDataAgro(StoredPorProveedorResult cambios, TipoAccionLogDataAgro tipoDeAccion, int idProveedor)
        {
            var resolver = new IgnorePropertiesResolver(new[] { "BasicoProveedorTraerPorProveedores.EstadoCuit", "BasicoProveedorTraerPorProveedores.Facacop" });
            string descripcion = "";
            cambios.Historial = null;//no registrar el historial de compras
            cambios.ActividadHistoriaTraerPorProveedores = null;//notificaciones
            if (cambios.BasicoProveedorTraerPorProveedores != null && cambios.BasicoProveedorTraerPorProveedores.Count > 1)
            {
                cambios.BasicoProveedorTraerPorProveedores = new List<BasicoProveedor> { cambios.BasicoProveedorTraerPorProveedores.FirstOrDefault() };
            }
            if (cambios.BasicoProveedorTraerPorProveedores != null && cambios.BasicoProveedorTraerPorProveedores.Count > 0)
            {
                descripcion = cambios.BasicoProveedorTraerPorProveedores.First().RazonSocial + " (" + cambios.BasicoProveedorTraerPorProveedores.First().CUIT + ")";
            }
            return LogGuardarCambios(cambios, tipoDeAccion, idProveedor, "Proveedor", descripcion, resolver);
        }
        public int LogCambiosDataAgro(CupoDto cambios, TipoAccionLogDataAgro tipoDeAccion)
        {
            var resolver = new IgnorePropertiesResolver(new[] { "EstadoOrden" });

            string descripcion = string.IsNullOrEmpty(cambios.CupoSap) ? cambios.Id.ToString() : cambios.CupoSap;

            return LogGuardarCambios(cambios, tipoDeAccion, cambios.Id, "Cupo", descripcion, resolver);
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
                    usuarioComercial = OperationContext.Current.ServiceSecurityContext.WindowsIdentity.Name.Split('\\').Last();
                }
                catch (Exception)
                { }
            }

            if (!string.IsNullOrEmpty(usuarioComercial))
            {
                var comercial = repositorio.Obtener<Comercial, string>(x => x.IdActiveDirectory == usuarioComercial, x => x.Nombres + " " + x.Apellido);
                if (comercial != null)
                {
                    usuarioComercial = comercial;
                }
            }

            if (usuarioComercial == null)
            {
                usuarioComercial = "";
            }

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

            //if (logActual.AccionRealizada.Contains("Crear"))
            //{
            //    logAnterior = null;
            //}
            //else
            //{
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

                   }).FirstOrDefault();
            //}
            if (logAnterior == logActual)
                logAnterior = null;

            var jActual = JToken.Parse(logActual.DatoModificado);
            var jAnterior = JToken.Parse(logAnterior == null ? "{}" : logAnterior.DatoModificado);
            var jdp = new JsonDiffPatch();
            JToken diffResult = jdp.Diff(jAnterior, jActual);

            List<DatoModificadosLogDataAgroDto> cambiados = new List<DatoModificadosLogDataAgroDto>();
            if (logAnterior != null)
            {
                if (diffResult != null)
                {
                    //ArmarListaModificar(diffResult, cambiados, "", logActual);
                    foreach (var item in diffResult)
                    {
                        if (((JProperty)item).Name != null && (!((JProperty)item).Name.ToLower().EndsWith("id")) && !((JProperty)item).Name.ToLower().Contains("formateado"))// saco los ids
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
        private void ArmarListaModificar(JToken diffResult, List<DatoModificadosLogDataAgroDto> cambiados, string campoNombre, LogDataAgroDto logActual)
        {
            foreach (var item in diffResult)
            {
                var name = ((JProperty)item).Name;
                var value = ((JProperty)item).Value;
                if (!NombresExcluidos(name))
                {
                    var campo = AddSpacesToSentence(name, ':');
                    if (!string.IsNullOrEmpty(campoNombre))
                    {
                        int num = 0;
                        if (!int.TryParse(name, out num))
                        {
                            campo = campoNombre + " - " + campo;
                        }
                        else
                        {
                            campo = campoNombre;
                        }
                    }
                    if (esLista(item))
                    {
                        ArmarListaModificar(value, cambiados, campo, logActual);
                    }
                    else
                    {
                        if (esItemDeLista(item))
                        {
                            foreach (var item2 in ((JContainer)((JProperty)item).First).First().Children())
                            {
                                if (esLista(item))
                                {
                                    ArmarListaModificar(item, cambiados, campo, logActual);
                                }
                                else
                                {
                                    if (((JProperty)item2).Name != null && !((JProperty)item2).Name.ToLower().EndsWith("id") && !((JProperty)item2).Name.ToLower().Contains("formateado"))
                                        Agregar(cambiados, logActual, item2, campo, item);
                                }

                            }
                        }
                        else
                        {
                            if (!NombresExcluidos(campo))
                                AgregarItem(cambiados, logActual, item, campo);
                        }
                    }
                }

            }
        }

        private void Agregar(List<DatoModificadosLogDataAgroDto> cambiados, LogDataAgroDto logActual, JToken item2, string campo, JToken item)
        {
            var name = ((JProperty)item2).Name;
            if (NombresExcluidos(name))
            {
                return;
            }
            var Anterior = BuscaFechaYFormatea(((JProperty)item2).First.ToString());
            var Actual = BuscaFechaYFormatea(((JProperty)item2).Last.ToString());
            if (((JProperty)item2).Count() == 1)
            {
                bool actual = logActual.DatoModificado.Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace(".00", ".0").Contains(((JProperty)item).Value.ToString().Replace(" ", "").Replace("\r", "").Replace("\n", ""));
                if (actual)
                {
                    Anterior = "";
                }
                else
                {
                    Actual = "";
                }

            }
            var modificado = new DatoModificadosLogDataAgroDto
            {
                Anterior = Anterior,
                Actual = Actual,
                Campo = campo + " - " + AddSpacesToSentence(name, ':')
            };
            if (!(modificado.Anterior == "" && modificado.Actual == ""))
            {
                cambiados.Add(modificado);
            }
        }

        private static bool NombresExcluidos(string name)
        {
            return name == null || name == "_t" || name.ToLower().EndsWith("id") || name.ToLower().EndsWith("formateado");
        }

        void AgregarItem(List<DatoModificadosLogDataAgroDto> cambiados, LogDataAgroDto logActual, JToken item, string campo)
        {
            
            var Anterior = BuscaFechaYFormatea(((JContainer)((JProperty)item).First).First.ToString());
            var Actual = BuscaFechaYFormatea(((JContainer)((JProperty)item).First).Last.ToString());
            if (((JContainer)((JProperty)item).First).Count() == 1)
            {
                bool actual = logActual.DatoModificado.Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace(".00", ".0").Contains(item.ToString().Replace(" ", "").Replace("\r", "").Replace("\n", ""));
                if (actual)
                {
                    Anterior = "";
                }
                else
                {
                    Actual = "";
                }

            }
            var modificado = new DatoModificadosLogDataAgroDto
            {
                Anterior = Anterior,
                Actual = Actual,
                Campo = campo
            };
            if (!(modificado.Anterior == "" && modificado.Actual == ""))
            {
                cambiados.Add(modificado);
            }
        }

        private bool esItemDeLista(JToken item)
        {
            int num = 0;
            var name = ((JProperty)item).Name;
            return int.TryParse(name, out num);
        }

        private bool esLista(JToken item)
        {
            //if (((JProperty)item).Value.ToString().StartsWith("["))
            //{
            //    return true;
            //}
            if (((JProperty)item).Value.ToString().StartsWith("{"))
            {
                return true;
            }
            return false;
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
                        if (((JProperty)item).Value.ToString().StartsWith("{"))
                        {
                            ArmarListaCrear(((JProperty)item).Value, cambiados, campo);
                        }
                        else
                        {
                            var modificado = new DatoModificadosLogDataAgroDto
                            {
                                Anterior = "",
                                Actual = BuscaFechaYFormatea(((JProperty)item).Value.ToString()),
                                Campo = campo
                            };
                            cambiados.Add(modificado);
                        }


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
            return LogGuardarCambios(cambios, tipoDeAccion, cambios.Id, "RangoConfirmacionAutomatica", cambios.Id.ToString());
        }

        public int LogCambiosDataAgro(PrecioMoaDto cambios, TipoAccionLogDataAgro tipoDeAccion)
        {
            return LogGuardarCambios(cambios, tipoDeAccion, cambios.Id, "PrecioMoa", cambios.Id.ToString());
        }
        public int LogCambiosDataAgro(HabilitacionFijacionDto cambios, TipoAccionLogDataAgro tipoDeAccion)
        {
            return LogGuardarCambios(cambios, tipoDeAccion, cambios.Id, "Habilitacion Fijacion", cambios.Id.ToString());
        }

        public int LogCambiosDataAgro(HabilitacionPizarraDto cambios, TipoAccionLogDataAgro tipoDeAccion)
        {
            return LogGuardarCambios(cambios, tipoDeAccion, cambios.Id, "HabilitacionPizarra", cambios.Id.ToString());
        }


        public string AddSpacesToSentence(string text, char limite)
        {
            text = text.Replace("Descuentos", "Descuentos y Bonificaciones");
            text = text.Replace("TipoDBDesc", "TipoDto y Bonif");
            text = text.Replace("TipoPeriodoDBDesc", "TipoPeriodoDto y Bonif");
            text = text.Replace("CalidadEspecialDesc", "CalidadEspecial");
            text = text.Replace("BasicoProveedorTraerPorProveedores", "Datos Generales");
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
            double number = 0;
            if (double.TryParse(s,out number))
            {
                s = string.Format("{0:#,0.00}", number);
            }
            try
            {
                if (formato1.IsMatch(s))
                {
                    Match mat = formato1.Match(s);
                    DateTime date = DateTime.ParseExact(mat.ToString(), "yyyy-MM-dd'T'HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    s = formato1.Replace(s, date.ToString("dd-MM-yyyy HH:mm:ss"));
                }
                if (formato2.IsMatch(s))
                {
                    Match mat = formato2.Match(s);
                    DateTime date = DateTime.ParseExact(mat.ToString(), "yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture);
                    s = formato2.Replace(s, date.ToString("dd-MM-yyyy HH:mm:ss"));
                }
                if (formato3.IsMatch(s))
                {
                    Match mat = formato3.Match(s);
                    DateTime date = DateTime.ParseExact(mat.ToString(), "yyyy-MM-dd 'T'HH:mm:ss", CultureInfo.InvariantCulture);
                    s = formato3.Replace(s, date.ToString("dd-MM-yyyy HH:mm:ss"));
                }

            }
            catch (Exception ex)
            {

            }


            return s;
        }

        public List<int> ObtenerNegociosId(List<string> contratosSap)
        {
            var negocio = repositorio.Listar<Negocio, int>(x => x.Id, x => contratosSap.Contains(x.ContratoSAP));
            return negocio;
        }

        public List<int> ObtenerCuposId(List<string> cupoSap)
        {
            var cupos = repositorio.Listar<Cupo, int>(x => x.Id, x => cupoSap.Contains(x.CupoSap));
            return cupos;
        }
    }
}
