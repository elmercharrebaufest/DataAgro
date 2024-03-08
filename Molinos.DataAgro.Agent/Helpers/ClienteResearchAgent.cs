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

        public List<Research> ConsultarItems()
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ListItem itemError = null;
            try
            {
                using (ClientContext context = new ClientContext(urlResearch))
                {
                    //pasar credenciales
                    SecureString securePassword = new SecureString();
                    foreach (char c in passwordResearch)
                    {
                        securePassword.AppendChar(c);
                    }
                    context.Credentials = new SharePointOnlineCredentials(userNameResearch, securePassword);

                    //selecionar la lista/pagina en sharepoint por nombre
                    Web web = context.Web;

                    List listResearch = web.Lists.GetByTitle(libraryNameResearch);
                    List listAttachments = web.Lists.GetByTitle(attachmentsResearch);
                    context.Load(listResearch);
                    context.Load(listAttachments);
                    context.ExecuteQuery();//este es el que ejecuta lo que armamos antes, sin este es como no hacer nada

                    // a la lista/pagina le pedimos que nos traiga todos los items
                    CamlQuery query = CamlQuery.CreateAllItemsQuery();// aca se puede mejorar para filtrar los ya sinconinizados
                    ListItemCollection itemsResearch = listResearch.GetItems(query);
                    ListItemCollection itemsAttachment = listAttachments.GetItems(query);
                    context.Load(itemsResearch);
                    context.Load(itemsAttachment);
                    context.ExecuteQuery();

                    List<Research> listaResearchDto = new List<Research>();
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
                    List<ComercialDto> listaComercialDto = repositorio.Listar<Comercial>()
                        .Select(x => new ComercialDto { ComercialId = x.ComercialId, Email = x.Email }).Where(x => x.Email != null).ToList();
                    List<ResearchCondicionCultivo> listaCondicionCultivo = repositorio.Listar<ResearchCondicionCultivo>();

                    string sasToken = azureAgent.GenerarTokenSAS();

                    CloudBlobContainer cloudBlobContainer = azureAgent.GenerarBlobContainer(sasToken);

                    foreach (ListItem item in itemsResearch)
                    {
                        string valoresCalculo = "";
                        Resultado resultado = new Resultado();
                        try
                        {
                            Research itemData = new Research();
                            itemError = item;

                            GuardarLog(item);

                            itemData.MaterialId = listaMateriales.First(x => x.Descripcion.ToUpper() == item["Cultivo"].ToString().ToUpper()).MaterialId;
                            itemData.MaterialIdAntecesor = listaMateriales.FirstOrDefault(x => x.Descripcion.ToUpper() == item["Antecesor"]?.ToString().ToUpper())?.MaterialId;
                            itemData.EstadioId = listaEstadioDto.FirstOrDefault(x => x.Descripcion.ToUpper() == item["Estadiofenologico"]?.ToString().ToUpper())?.EstadioId;
                            itemData.CondicionId = listaCondicionDto.FirstOrDefault(x => x.Descripcion.ToUpper() == item["Condicioncultivo"]?.ToString().ToUpper())?.CondicionId;
                            itemData.HumedadSueloId = listaHumedadSueloDto.FirstOrDefault(x => x.Descripcion.ToUpper() == item["Humedadsuelo"]?.ToString().ToUpper())?.HumedadSueloId;
                            itemData.Comentarios = item["Comentarios"] == null ? "" : item["Comentarios"].ToString();
                            itemData.Partido = item["Partido"] == null ? "" : item["Partido"].ToString();
                            itemData.Localidad = item["Localidad"] == null ? "" : item["Localidad"].ToString();
                            itemData.Provincia = item["Provincia"] == null ? "" : item["Provincia"].ToString();
                            var provinciaId = listaProvinciaDto.FirstOrDefault(x => x.Nombre.ToUpper() == itemData.Provincia.ToUpper())?.ProvinciaId;
                            var partidoId = provinciaId == null ? null : listaPartido.FirstOrDefault(x => x.Descripcion.ToUpper() == itemData.Partido.ToUpper() && x.ProvinciaId == provinciaId)?.Id;
                            itemData.LocalidadId = item["LocalidadId"] != null ? int.Parse(item["LocalidadId"].ToString()) :
                                partidoId == null ? null : listaLocalidadDto.FirstOrDefault(x => x.Nombre.ToUpper() == itemData.Localidad.ToUpper() && x.PartidoId == partidoId && x.ProvinciaId == provinciaId)?.LocalidadId;
                            itemData.ProvinciaId = item["ProvinciaId"] is string ? int.Parse(item["ProvinciaId"].ToString()) : (int?)null;
                            itemData.PartidoId = item["PartidoId"] is string ? int.Parse(item["PartidoId"].ToString()) : (int?)null;
                            itemData.Latitud = item["Latitud"] is double ? double.Parse(item["Latitud"].ToString()) : (double?)null;
                            itemData.Longitud = item["Longitud"] is double ? double.Parse(item["Longitud"].ToString()) : (double?)null;
                            itemData.TipoMuestraIdUno = listaTipoMuestraDto.FirstOrDefault(x => x.Descripcion.ToUpper() == item["Muestra1"]?.ToString().ToUpper())?.TipoMuestraId;
                            itemData.MedidasUno = item["Medidas1"] == null ? "" : item["Medidas1"].ToString();
                            itemData.PromedioMuestraUno = item["Promediomuestra1"] is double ? double.Parse(item["Promediomuestra1"].ToString()) : (double?)null;
                            itemData.TipoMuestraIdDos = listaTipoMuestraDto.FirstOrDefault(x => x.Descripcion.ToUpper() == item["Muestra2"]?.ToString().ToUpper())?.TipoMuestraId;
                            itemData.MedidasDos = item["Medidas2"] == null ? "" : item["Medidas2"].ToString();
                            itemData.PromedioMuestraDos = item["Promediomuestra2"] is double ? double.Parse(item["Promediomuestra2"].ToString()) : (double?)null;
                            itemData.TipoMuestraIdTres = listaTipoMuestraDto.FirstOrDefault(x => x.Descripcion.ToUpper() == item["Muestra3"]?.ToString().ToUpper())?.TipoMuestraId;
                            itemData.MedidasTres = item["Medidas3"] == null ? "" : item["Medidas3"].ToString();
                            itemData.PromedioMuestraTres = item["Promediomuestra3"] is double ? double.Parse(item["Promediomuestra3"].ToString()) : (double?)null;
                            itemData.DistanciaHileras = item["Distanciahileras"] is double ? double.Parse(item["Distanciahileras"].ToString()) : (double?)null;
                            itemData.Coeficiente = item["Coeficiente"] is double ? double.Parse(item["Coeficiente"].ToString()) : (double?)null;
                            itemData.CampañaId = listaCampañaDto.FirstOrDefault(x => x.Descripcion == item["Campa_x00f1_a"]?.ToString())?.CampañaId;
                            itemData.CapitulosGirasol = item["CapitulosGirasol"] is double ? double.Parse(item["CapitulosGirasol"].ToString()) : (double?)null;
                            itemData.FechaAlta = item["Created"] is DateTime ? (DateTime)item["Created"] : (DateTime?)null;
                            itemData.Rendimiento = item["rendimiento"] is double ? double.Parse(item["rendimiento"].ToString()) : (double?)null;
                            itemData.TipoCargaId = listaTipoCargaDto.FirstOrDefault(x => x.Descripcion.ToUpper() == item["tipoCarga"]?.ToString().ToUpper())?.TipoCargaId;
                            itemData.EstadoConectividad = item["estadoConectividad"] == null ? "" : item["estadoConectividad"].ToString();
                            itemData.IdPowerApp = item["ID"] is int ? int.Parse(item["ID"].ToString()) : (int?)null;
                            itemData.FechaModificacion = item["Modified"] is DateTime ? (DateTime)item["Modified"] : (DateTime?)null;
                            FieldUserValue autor = new FieldUserValue();
                            autor = (FieldUserValue)item["Author"];
                            itemData.Author = autor.Email;
                            itemData.ComercialId = listaComercialDto.FirstOrDefault(x => x.Email.ToUpper() == itemData.Author?.ToUpper())?.ComercialId;
                            FieldUserValue editor = new FieldUserValue();
                            editor = (FieldUserValue)item["Editor"];
                            itemData.Editor = editor.Email;
                            string rutaArchivos = item["FileDirRef"] == null ? "" : item["FileDirRef"].ToString();

                            double espigas_Plantas_m2 = 0, rendimiento = 0;
                            int p1000 = listaCondicionCultivo
                                        .Where(x => x.MaterialId == itemData.MaterialId && x.CondicionId == itemData.CondicionId)
                                        .Select(x => x.Valor)
                                        .FirstOrDefault();

                            if (itemData.TipoCargaId == 1) //Carga completa
                            {
                                resultado = validarResearch(itemData, p1000);

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
                                        valoresCalculo = $"SOJA - PromedioMuestraUno: {itemData.PromedioMuestraUno} - DistanciaHileras: {itemData.DistanciaHileras} - PromedioMuestraDos: {itemData.PromedioMuestraDos} - PromedioMuestraTres: {itemData.PromedioMuestraTres} - P1000: {p1000} - Coeficiente: {itemData.Coeficiente}";
                                        // Espigas/Plantas m2 = Promedio m lineal / Distancia hileras (cm)
                                        espigas_Plantas_m2 = (double)(itemData.PromedioMuestraUno / itemData.DistanciaHileras);
                                        // Prom. Vainas/planta = itemData.PromedioMuestraDos
                                        // Prom. Granos por vaina = itemData.PromedioMuestraTres

                                        rendimiento = (double)(espigas_Plantas_m2 * itemData.PromedioMuestraDos * itemData.PromedioMuestraTres * p1000 * itemData.Coeficiente);
                                        break;
                                    case (int)EnumMateriales.GIRASOL:
                                        valoresCalculo = $"GIRASOL - PromedioMuestraUno: {itemData.PromedioMuestraUno} - CapitulosGirasol: {itemData.CapitulosGirasol} - DistanciaHileras: {itemData.DistanciaHileras} - Coeficiente: {itemData.Coeficiente}";
                                        double promedio_al_cuadrado = Math.Pow((double)itemData.PromedioMuestraUno, 2);
                                        // Peso por capítulo (grs) = -14,53 + ( 1,07 * itemData.PromedioMuestraUno) + ( 0,2 * itemData.PromedioMuestraUno * itemData.PromedioMuestraUno)
                                        double peso_por_capítulo_grs = (double)(-14.53 + (1.07 * itemData.PromedioMuestraUno) + (0.2 * promedio_al_cuadrado));
                                        rendimiento = (double)(itemData.CapitulosGirasol / itemData.DistanciaHileras / 10 * peso_por_capítulo_grs * itemData.Coeficiente * 10000);
                                        break;
                                    default:
                                        break;
                                }

                                itemData.Rendimiento = Double.IsNaN(rendimiento) ? 0 : (double)Math.Round(rendimiento, 2, MidpointRounding.AwayFromZero);
                            }

                            string json = JsonConvert.SerializeObject(itemData, Formatting.Indented);

                            var adjuntos = itemsAttachment.Where(x => Convert.ToInt32(x["ID_Relevamiento"]) == Convert.ToInt32(item["ID"])).ToList();

                            if (adjuntos.Any())
                            {
                                if (ConfigurationManager.AppSettings["AmbientePruebas"] == "1")
                                {
                                    itemData.Adjuntos.Add(new ResearchAdjunto()
                                    {
                                        Path = "/Content/Images/MolinosAgro.png",
                                        Nombre = "MolinosAgro.png",
                                    });
                                }
                                else
                                {
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
                            }

                            listaResearchDto.Add(itemData);

                            if (resultado.HayError)
                            {
                                logger.Info($"INICIO - ERROR Research con registro: {itemData.IdPowerApp} - {valoresCalculo}");
                                logger.Error($"{cadenaDeErrores(resultado)}");
                                logger.Info($"FIN - ERROR Research con registro: {itemData.IdPowerApp}");
                            }
                            else
                            {
                                repositorio.Agregar(itemData);
                                repositorio.GuardarCambios();

                                if (ConfigurationManager.AppSettings["AmbientePruebas"] == "1")
                                {
                                    logger.Info($"Simula eliminar en Sharpoint el registro: {itemData.IdPowerApp}");
                                }
                                else
                                {
                                    item.DeleteObject();
                                    context.ExecuteQuery();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            var id = itemError["ID"] is int ? int.Parse(itemError["ID"].ToString()) : (int?)null;
                            logger.Info($"INICIO - ERROR Research con registro: {id} - {valoresCalculo}");
                            logger.Error(e.Message);
                            if (e.InnerException?.InnerException != null) logger.Error(e.InnerException?.InnerException?.Message);
                            if (resultado.HayError) logger.Error($"{cadenaDeErrores(resultado)}");
                            logger.Info($"FIN - ERROR Research con registro: {id}");
                        }
                    }
                    return listaResearchDto;
                }
            }
            catch (Exception e)
            {
                var id = itemError["ID"] is int ? int.Parse(itemError["ID"].ToString()) : (int?)null;
                logger.Error($"Error al consultar registros de Research - ID {id}: {e.Message}");
                throw;
            }
        }

        static byte[] DownloadImageFromSharePoint(ClientResult<Stream> fileStream)
        {
            using (var memoryStream = new MemoryStream())
            {
                fileStream.Value.CopyTo(memoryStream);
                fileStream.Value.Close(); // se agrega por las dudas, para verificar.
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
            logger.Info($"Research a sincronizar ID {int.Parse(item["ID"].ToString())}: {jsonData}");
        }

        private string cadenaDeErrores(Resultado resultado)
        {
            string errores = "";
            resultado.Errores.ForEach(x => errores += x.Message);
            return errores;
        }

        private Resultado validarResearch(Research itemData, int p1000)
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

            if (itemData.PromedioMuestraTres == 0 && (itemData.MaterialId == (int)EnumMateriales.MAIZ || itemData.MaterialId == (int)EnumMateriales.SOJA))
                resultado.Errores.Add(new ErrorMessage(400, $"El campo PromedioMuestraTres no puede ser 0 (cero)."));
            if (itemData.PromedioMuestraTres == null && (itemData.MaterialId == (int)EnumMateriales.MAIZ || itemData.MaterialId == (int)EnumMateriales.SOJA))
                resultado.Errores.Add(new ErrorMessage(400, $"El campo PromedioMuestraTres no puede ser NULL."));

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
