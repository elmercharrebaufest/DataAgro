using Autofac.Extras.NLog;
using Microsoft.SharePoint.Client;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces.Agent;
using Molinos.DataAgro.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using Microsoft.WindowsAzure.Storage.Blob;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ClienteResearchAgent : IClienteResearchAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IAzureAgent azureAgent;
        private readonly string urlResearch = ConfigurationManager.AppSettings["UrlResearch"];
        private readonly string libraryNameResearch = ConfigurationManager.AppSettings["LibraryNameResearch"];
        private readonly string attachmentsResearch = ConfigurationManager.AppSettings["AttachmentsResearch"];
        private readonly string userNameResearch = ConfigurationManager.AppSettings["UserNameResearch"];
        private readonly string passwordResearch = ConfigurationManager.AppSettings["PasswordResearch"];

        public ClienteResearchAgent(ILogger logger, IRepositorio repositorio, IAzureAgent azureAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.azureAgent = azureAgent;
        }

        public void SincronizarDatosResearch()
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {
                using (ClientContext context = new ClientContext(urlResearch))
                {
                    SecureString securePassword = new SecureString();
                    foreach (char c in passwordResearch)
                    {
                        securePassword.AppendChar(c);
                    }
                    context.Credentials = new SharePointOnlineCredentials(userNameResearch, securePassword);

                    Web web = context.Web; //selecionar la lista/pagina en sharepoint por nombre

                    List listResearch = web.Lists.GetByTitle(libraryNameResearch);
                    List listAttachments = web.Lists.GetByTitle(attachmentsResearch);
                    context.Load(listResearch);
                    context.Load(listAttachments);
                    context.ExecuteQuery(); //esto es lo que ejecuta lo que armamos antes, sin eso es como no hacer nada

                    // a la lista/pagina le pedimos que nos traiga todos los items
                    CamlQuery query = CamlQuery.CreateAllItemsQuery(); // aca se puede mejorar para filtrar los ya sinconinizados
                    ListItemCollection itemsResearch = listResearch.GetItems(query);
                    ListItemCollection itemsAttachment = listAttachments.GetItems(query);
                    context.Load(itemsResearch);
                    context.Load(itemsAttachment);
                    context.ExecuteQuery();

                    string sasToken = azureAgent.GenerarTokenSAS();
                    CloudBlobContainer cloudBlobContainer = azureAgent.GenerarBlobContainer(sasToken);

                    List<Material> listaMateriales = repositorio.Listar<Material>();
                    List<ResearchEstadioDto> listaEstadioDto = repositorio.Listar<ResearchEstadio, ResearchEstadioDto>(x => new ResearchEstadioDto { EstadioId = x.EstadioId, Descripcion = x.Descripcion });
                    List<ResearchCondicionDto> listaCondicionDto = repositorio.Listar<ResearchCondicion, ResearchCondicionDto>(x => new ResearchCondicionDto { CondicionId = x.CondicionId, Descripcion = x.Descripcion });
                    List<ResearchHumedadSueloDto> listaHumedadSueloDto = repositorio.Listar<ResearchHumedadSuelo, ResearchHumedadSueloDto>(x => new ResearchHumedadSueloDto { HumedadSueloId = x.HumedadSueloId, Descripcion = x.Descripcion });
                    List<LocalidadDto> listaLocalidadDto = repositorio.Listar<Localidad, LocalidadDto>(x => new LocalidadDto { LocalidadId = x.LocalidadId, Nombre = x.Nombre, PartidoId = x.PartidoId, ProvinciaId = x.ProvinciaId });
                    List<Partido> listaPartido = repositorio.Listar<Partido>().ToList();
                    List<ProvinciaDto> listaProvinciaDto = repositorio.Listar<Provincia, ProvinciaDto>(x => new ProvinciaDto { ProvinciaId = x.ProvinciaId, Nombre = x.Nombre });
                    List<ResearchTipoMuestraDto> listaTipoMuestraDto = repositorio.Listar<ResearchTipoMuestra, ResearchTipoMuestraDto>(x => new ResearchTipoMuestraDto { TipoMuestraId = x.TipoMuestraId, Descripcion = x.Descripcion });
                    List<ResearchTipoCargaDto> listaTipoCargaDto = repositorio.Listar<ResearchTipoCarga, ResearchTipoCargaDto>(x => new ResearchTipoCargaDto { TipoCargaId = x.TipoCargaId, Descripcion = x.Descripcion });
                    List<CampañaDto> listaCampañaDto = repositorio.Listar<Campaña, CampañaDto>(x => new CampañaDto { CampañaId = x.CampañaId, Descripcion = x.Descripcion });
                    List<ComercialDto> listaComercialDto = repositorio.Listar<Comercial, ComercialDto>(x => new ComercialDto { ComercialId = x.ComercialId, Email = x.Email });
                    List<ResearchCondicionCultivo> listaCondicionCultivo = repositorio.Listar<ResearchCondicionCultivo>();
                    int contadorAgregados = 0;

                    foreach (ListItem item in itemsResearch.ToList())
                    {
                        Resultado resultado = new Resultado();
                        try
                        {
                            Research itemData = new Research();

                            GuardarLog(item);

                            itemData.MaterialId = listaMateriales.First(x => x.Descripcion.ToUpper() == item["Cultivo"]?.ToString().ToUpper()).MaterialId;
                            itemData.MaterialIdAntecesor = listaMateriales.FirstOrDefault(x => x.Descripcion.ToUpper() == item["Antecesor"]?.ToString().ToUpper())?.MaterialId;
                            itemData.EstadioId = listaEstadioDto.FirstOrDefault(x => x.Descripcion.ToUpper() == item["Estadiofenologico"]?.ToString().ToUpper())?.EstadioId;
                            itemData.CondicionId = listaCondicionDto.FirstOrDefault(x => x.Descripcion.ToUpper() == item["Condicioncultivo"]?.ToString().ToUpper())?.CondicionId;
                            itemData.HumedadSueloId = listaHumedadSueloDto.FirstOrDefault(x => x.Descripcion.ToUpper() == item["Humedadsuelo"]?.ToString().ToUpper())?.HumedadSueloId;
                            itemData.Comentarios = item["Comentarios"] is string comentarios ? comentarios : "";
                            itemData.Provincia = item["Provincia"] is string provincia ? provincia : "";
                            itemData.Partido = item["Partido"] is string partido ? partido : "";
                            itemData.Localidad = item["Localidad"] is string localidad ? localidad : "";
                            itemData.ProvinciaId = item["ProvinciaId"] is string provId ? int.Parse(provId) : listaProvinciaDto.FirstOrDefault(x => x.Nombre.ToUpper() == itemData.Provincia.ToUpper())?.ProvinciaId;
                            itemData.PartidoId = item["PartidoId"] is string partId ? int.Parse(partId) : itemData.ProvinciaId != null ?
                                listaPartido.FirstOrDefault(x => x.Descripcion.ToUpper() == itemData.Partido.ToUpper() && x.ProvinciaId == itemData.ProvinciaId)?.Id : null;
                            itemData.LocalidadId = item["LocalidadId"] is string locId ? int.Parse(locId) : itemData.PartidoId != null ?
                                listaLocalidadDto.FirstOrDefault(x => x.Nombre.ToUpper() == itemData.Localidad.ToUpper() && x.PartidoId == itemData.PartidoId && x.ProvinciaId == itemData.ProvinciaId)?.LocalidadId : null;
                            itemData.Latitud = item["Latitud"] is double latitud ? latitud : (double?)null;
                            itemData.Longitud = item["Longitud"] is double longitud ? longitud : (double?)null;
                            itemData.TipoMuestraIdUno = listaTipoMuestraDto.FirstOrDefault(x => x.Descripcion.ToUpper() == item["Muestra1"]?.ToString().ToUpper())?.TipoMuestraId;
                            itemData.MedidasUno = item["Medidas1"] is string medidas1 ? medidas1 : "";
                            itemData.PromedioMuestraUno = item["Promediomuestra1"] is double prom1 ? prom1 : (double?)null;
                            itemData.TipoMuestraIdDos = listaTipoMuestraDto.FirstOrDefault(x => x.Descripcion.ToUpper() == item["Muestra2"]?.ToString().ToUpper())?.TipoMuestraId;
                            itemData.MedidasDos = item["Medidas2"] is string medidas2 ? medidas2 : "";
                            itemData.PromedioMuestraDos = item["Promediomuestra2"] is double prom2 ? prom2 : (double?)null;
                            itemData.TipoMuestraIdTres = listaTipoMuestraDto.FirstOrDefault(x => x.Descripcion.ToUpper() == item["Muestra3"]?.ToString().ToUpper())?.TipoMuestraId;
                            itemData.MedidasTres = item["Medidas3"] is string medidas3 ? medidas3 : "";
                            itemData.PromedioMuestraTres = item["Promediomuestra3"] is double prom3 ? prom3 : (double?)null;
                            itemData.PromedioGranosVaina = item["PromedioGranosVaina"] is double promGV ? promGV : (double?)null;
                            itemData.DistanciaHileras = item["Distanciahileras"] is double hileras ? hileras : (double?)null;
                            itemData.Coeficiente = item["Coeficiente"] is double coeficiente ? coeficiente : (double?)null;
                            itemData.CampañaId = listaCampañaDto.FirstOrDefault(x => x.Descripcion == item["Campa_x00f1_a"]?.ToString())?.CampañaId;
                            itemData.CapitulosGirasol = item["CapitulosGirasol"] is double capGirasol ? capGirasol : (double?)null;
                            itemData.FechaAlta = item["Created"] is DateTime fechaAlta ? fechaAlta : (DateTime?)null;
                            itemData.Rendimiento = item["rendimiento"] is double rendim ? rendim : (double?)null;
                            itemData.TipoCargaId = listaTipoCargaDto.FirstOrDefault(x => x.Descripcion.ToUpper() == item["tipoCarga"]?.ToString().ToUpper())?.TipoCargaId;
                            itemData.EstadoConectividad = item["estadoConectividad"] is string estadoConec ? estadoConec : "";
                            itemData.IdPowerApp = item["ID"] is int id ? id : (int?)null;
                            itemData.FechaModificacion = item["Modified"] is DateTime fecha ? fecha : (DateTime?)null;
                            FieldUserValue autor = new FieldUserValue();
                            autor = (FieldUserValue)item["Author"];
                            itemData.Author = autor.Email;
                            itemData.ComercialId = listaComercialDto.FirstOrDefault(x => x.Email != null && x.Email.ToUpper() == itemData.Author?.ToUpper())?.ComercialId;
                            FieldUserValue editor = new FieldUserValue();
                            editor = (FieldUserValue)item["Editor"];
                            itemData.Editor = editor.Email;
                            itemData.Attachments = false;
                            itemData.Sincronizado = true;
                            string rutaArchivos = item["FileDirRef"] is string fileDirRef ? fileDirRef : "";

                            if (itemData.TipoCargaId == 1) //Carga completa
                            {
                                double espigas_Plantas_m2 = 0, rendimiento = 0;
                                int p1000 = listaCondicionCultivo.Where(x => x.MaterialId == itemData.MaterialId && x.CondicionId == itemData.CondicionId).Select(x => x.Valor).FirstOrDefault();
                                string valoresCalculo = "";

                                resultado = ValidarResearch(itemData, p1000);

                                switch (itemData.MaterialId)
                                {
                                    case (int)EnumMateriales.MAIZ:
                                        valoresCalculo = $"MAIZ - PromedioMuestraUno: {itemData.PromedioMuestraUno} - DistanciaHileras: {itemData.DistanciaHileras} - PromedioMuestraDos: {itemData.PromedioMuestraDos} - PromedioMuestraTres: {itemData.PromedioMuestraTres} - P1000: {p1000} - Coeficiente: {itemData.Coeficiente}";
                                        double espigas_m2 = (double)(itemData.PromedioMuestraUno / itemData.DistanciaHileras / 10);

                                        rendimiento = (double)(espigas_m2 * itemData.PromedioMuestraDos * itemData.PromedioMuestraTres * p1000 * itemData.Coeficiente);
                                        break;
                                    case (int)EnumMateriales.TRIGO:
                                        valoresCalculo = $"TRIGO - PromedioMuestraUno: {itemData.PromedioMuestraUno} - DistanciaHileras: {itemData.DistanciaHileras} - PromedioMuestraDos: {itemData.PromedioMuestraDos} - P1000: {p1000} - Coeficiente: {itemData.Coeficiente}";
                                        // Espigas/Plantas m2 = Promedio m lineal / Distancia hileras (cm)
                                        espigas_Plantas_m2 = (double)(itemData.PromedioMuestraUno / itemData.DistanciaHileras);
                                        // Prom. Granos x Espiga/planta = itemData.PromedioMuestraDos

                                        rendimiento = (double)(espigas_Plantas_m2 * itemData.PromedioMuestraDos * p1000 * itemData.Coeficiente);
                                        break;
                                    case (int)EnumMateriales.SOJA:
                                        valoresCalculo = $"SOJA - PromedioMuestraUno: {itemData.PromedioMuestraUno} - DistanciaHileras: {itemData.DistanciaHileras} - PromedioMuestraDos: {itemData.PromedioMuestraDos} - PromedioGranosVaina: {itemData.PromedioGranosVaina} - P1000: {p1000} - Coeficiente: {itemData.Coeficiente}";
                                        // Espigas/Plantas m2 = Promedio m lineal / Distancia hileras (cm)
                                        espigas_Plantas_m2 = (double)(itemData.PromedioMuestraUno / itemData.DistanciaHileras);
                                        // Prom. Vainas/planta = itemData.PromedioMuestraDos

                                        rendimiento = (double)(espigas_Plantas_m2 * itemData.PromedioMuestraDos * itemData.PromedioGranosVaina * p1000 * itemData.Coeficiente);
                                        break;
                                    case (int)EnumMateriales.GIRASOL:
                                        valoresCalculo = $"GIRASOL - PromedioMuestraUno: {itemData.PromedioMuestraUno} - CapitulosGirasol: {itemData.CapitulosGirasol} - DistanciaHileras: {itemData.DistanciaHileras} - Coeficiente: {itemData.Coeficiente}";
                                        double promedio_al_cuadrado = Math.Pow((double)itemData.PromedioMuestraUno, 2);
                                        double peso_por_capítulo_grs = (double)(-14.53 + (1.07 * itemData.PromedioMuestraUno) + (0.2 * promedio_al_cuadrado));
                                        rendimiento = (double)(itemData.CapitulosGirasol / itemData.DistanciaHileras / 10 * peso_por_capítulo_grs * itemData.Coeficiente * 10000);
                                        break;
                                    default:
                                        break;
                                }

                                itemData.Rendimiento = Double.IsNaN(rendimiento) ? 0 : (double)Math.Round(rendimiento, 0, MidpointRounding.AwayFromZero); //el rendimiento se muestra solo en su parte entera
                            }

                            //string json = JsonConvert.SerializeObject(itemData, Formatting.Indented);
                            itemData.PromedioMuestraUno = Math.Round((double)itemData.PromedioMuestraUno, 2); //los decimales exactos son necesarios para el cálculo del rendimiento pero no para mostrarse luego
                            itemData.PromedioMuestraDos = Math.Round((double)itemData.PromedioMuestraDos, 2);
                            itemData.PromedioMuestraTres = Math.Round((double)itemData.PromedioMuestraTres, 2);

                            var adjuntos = itemsAttachment.Where(x => Convert.ToInt32(x["ID_Relevamiento"]) == Convert.ToInt32(item["ID"])).ToList();

                            if (adjuntos.Any())
                            {
                                itemData.Attachments = true;

                                foreach (var adjunto in adjuntos)
                                {
                                    string nombre = adjunto["FileLeafRef"].ToString();
                                    string rutaAdjunto = adjunto["FileRef"].ToString();

                                    ClientResult<Stream> fileStream = web.GetFileByServerRelativeUrl(rutaAdjunto).OpenBinaryStream();
                                    context.ExecuteQuery();
                                    byte[] imageBytes = DownloadImageFromSharePoint(fileStream);

                                    string blobUri = azureAgent.GuardarImagenEnAzure(cloudBlobContainer, itemData.IdPowerApp.ToString() + "/" + nombre, imageBytes);

                                    itemData.Adjuntos.Add(new ResearchAdjunto()
                                    {
                                        Path = blobUri,
                                        Nombre = nombre,
                                    });
                                }
                            }

                            if (resultado.HayError)
                            {
                                logger.Info($"INICIO - ERROR Research con registro: {itemData.IdPowerApp}");
                                logger.Error($"{CadenaDeErrores(resultado)}");
                                logger.Info($"FIN - ERROR Research con registro: {itemData.IdPowerApp}");
                            }
                            else
                            {
                                repositorio.Agregar(itemData);
                                contadorAgregados++;
                                //if (ConfigurationManager.AppSettings["AmbientePruebas"] == "1") //Santiago Barbarotta pide borrar al sincronizar también en QA (04/06/24)
                                //{
                                //    logger.Info($"Simula eliminar en SharePoint el registro {itemData.IdPowerApp}");
                                //}
                                //else
                                //{
                                item.DeleteObject();
                                context.ExecuteQuery();
                                //}
                            }
                        }
                        catch (Exception e)
                        {
                            var id = item["ID"] is int itemId ? itemId : (int?)null;
                            logger.Error($"Error Research con IdPowerApp {id}", e.Message);
                            if (e.InnerException?.InnerException != null) logger.Error(e.InnerException?.InnerException?.Message);
                            if (resultado.HayError) logger.Error($"{CadenaDeErrores(resultado)}");
                        }
                    }
                    repositorio.GuardarCambios();
                    logger.Info($"Se agregaron {contadorAgregados} registros a la base de datos.");
                }
            }
            catch (Exception e)
            {
                logger.Error("SincronizarDatosResearch", e);
                throw;
            }
        }

        static byte[] DownloadImageFromSharePoint(ClientResult<Stream> fileStream)
        {
            using (var memoryStream = new MemoryStream())
            {
                fileStream.Value.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private void GuardarLog(ListItem item)
        {
            var data = new
            {
                Material = item["Cultivo"],
                MaterialAntecesor = item["Antecesor"],
                EstadioFenologico = item["Estadiofenologico"],
                CondicionCultivo = item["Condicioncultivo"],
                HumedadSuelo = item["Humedadsuelo"],
                Comentarios = item["Comentarios"],
                PartidoId = item["PartidoId"],
                Partido = item["Partido"],
                LocalidadId = item["LocalidadId"],
                Localidad = item["Localidad"],
                ProvinciaId = item["ProvinciaId"],
                Provincia = item["Provincia"],
                Latitud = item["Latitud"],
                Longitud = item["Longitud"],
                TipoMuestraUno = item["Muestra1"],
                MedidasUno = item["Medidas1"],
                PromedioMuestraUno = item["Promediomuestra1"],
                TipoMuestraDos = item["Muestra2"],
                MedidasDos = item["Medidas2"],
                PromedioMuestraDos = item["Promediomuestra2"],
                TipoMuestraTres = item["Muestra3"],
                MedidasTres = item["Medidas3"],
                PromedioMuestraTres = item["Promediomuestra3"],
                PromedioGranosVaina = item["PromedioGranosVaina"],
                DistanciaHileras = item["Distanciahileras"],
                Coeficiente = item["Coeficiente"],
                Campaña = item["Campa_x00f1_a"],
                CapitulosGirasol = item["CapitulosGirasol"],
                FechaAlta = item["Created"],
                Rendimiento = item["rendimiento"],
                TipoCarga = item["tipoCarga"],
                EstadoConectividad = item["estadoConectividad"],
                IdPowerApp = item["ID"],
                FechaModificacion = item["Modified"],
                Author = item["Author"],
                Editor = item["Editor"],
                Attachments = item["Attachments"],
            };

            string jsonData = JsonConvert.SerializeObject(data);
            logger.Info($"Research a sincronizar IdPowerApp {item["ID"]}: {jsonData}");
        }

        private string CadenaDeErrores(Resultado resultado)
        {
            string errores = "";
            resultado.Errores.ForEach(x => errores += x.Message);
            return errores;
        }

        private Resultado ValidarResearch(Research itemData, int p1000)
        {
            Resultado resultado = new Resultado();

            if (itemData.PromedioMuestraUno == 0) resultado.Errores.Add(new ErrorMessage(400, $"El campo PromedioMuestraUno no puede ser 0 (cero)."));
            if (itemData.PromedioMuestraUno == null) resultado.Errores.Add(new ErrorMessage(400, $"El campo PromedioMuestraUno no puede ser NULL."));

            if (itemData.DistanciaHileras == 0) resultado.Errores.Add(new ErrorMessage(400, $"El campo DistanciaHileras no puede ser 0 (cero)."));
            if (itemData.DistanciaHileras == null) resultado.Errores.Add(new ErrorMessage(400, $"El campo DistanciaHileras no puede ser NULL."));

            if (itemData.PromedioMuestraDos == 0 && (itemData.MaterialId == (int)EnumMateriales.MAIZ || itemData.MaterialId == (int)EnumMateriales.TRIGO || itemData.MaterialId == (int)EnumMateriales.SOJA))
                resultado.Errores.Add(new ErrorMessage(400, $"El campo PromedioMuestraDos no puede ser 0 (cero)."));
            if (itemData.PromedioMuestraDos == null && (itemData.MaterialId == (int)EnumMateriales.MAIZ || itemData.MaterialId == (int)EnumMateriales.TRIGO || itemData.MaterialId == (int)EnumMateriales.SOJA))
                resultado.Errores.Add(new ErrorMessage(400, $"El campo PromedioMuestraDos no puede ser NULL."));

            if (itemData.PromedioMuestraTres == 0 && (itemData.MaterialId == (int)EnumMateriales.MAIZ))
                resultado.Errores.Add(new ErrorMessage(400, $"El campo PromedioMuestraTres no puede ser 0 (cero) para maíz."));
            if (itemData.PromedioMuestraTres == null && (itemData.MaterialId == (int)EnumMateriales.MAIZ))
                resultado.Errores.Add(new ErrorMessage(400, $"El campo PromedioMuestraTres no puede ser NULL para maíz."));

            if (itemData.PromedioGranosVaina == 0 && (itemData.MaterialId == (int)EnumMateriales.SOJA))
                resultado.Errores.Add(new ErrorMessage(400, $"El campo PromedioGranosVaina no puede ser 0 (cero) para soja."));
            if (itemData.PromedioGranosVaina == null && (itemData.MaterialId == (int)EnumMateriales.SOJA))
                resultado.Errores.Add(new ErrorMessage(400, $"El campo PromedioGranosVaina no puede ser NULL para soja."));

            if (p1000 == 0 && (itemData.MaterialId == (int)EnumMateriales.MAIZ || itemData.MaterialId == (int)EnumMateriales.TRIGO || itemData.MaterialId == (int)EnumMateriales.SOJA))
                resultado.Errores.Add(new ErrorMessage(400, $"El campo p1000 no puede ser 0 (cero)."));

            if (itemData.Coeficiente == 0) resultado.Errores.Add(new ErrorMessage(400, $"El campo Coeficiente no puede ser 0 (cero)."));
            if (itemData.Coeficiente == null) resultado.Errores.Add(new ErrorMessage(400, $"El campo Coeficiente no puede ser NULL."));

            if (itemData.CapitulosGirasol == 0 && itemData.MaterialId == (int)EnumMateriales.GIRASOL)
                resultado.Errores.Add(new ErrorMessage(400, $"El campo CapitulosGirasol no puede ser 0 (cero)."));
            if (itemData.CapitulosGirasol == null && itemData.MaterialId == (int)EnumMateriales.GIRASOL)
                resultado.Errores.Add(new ErrorMessage(400, $"El campo CapitulosGirasol no puede ser NULL."));

            return resultado;
        }
    }
}
