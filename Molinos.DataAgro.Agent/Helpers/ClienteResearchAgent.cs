using Autofac.Extras.NLog;
using Microsoft.SharePoint.Client;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
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
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ClienteResearchAgent : IClienteResearchAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly ILogDataAgroManager logDataAgroManager;
        readonly String urlResearch = ConfigurationManager.AppSettings["UrlResearch"];
        readonly String libraryNameResearch = ConfigurationManager.AppSettings["LibraryNameResearch"];
        readonly String userNameResearch = ConfigurationManager.AppSettings["UserNameResearch"];
        readonly String passwordResearch = ConfigurationManager.AppSettings["PasswordResearch"];
        readonly String domainResearch = ConfigurationManager.AppSettings["DomainResearch"];
        readonly String downloadPathResearch = ConfigurationManager.AppSettings["DownloadPathResearch"];
        readonly String connectionStringAzure = ConfigurationManager.AppSettings["ConnectionStringAzure"];
        private const string NOMBRE_CONTENEDOR = "research";

        public ClienteResearchAgent(ILogger logger, IRepositorio repositorio, ILogDataAgroManager logDataAgroManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.logDataAgroManager = logDataAgroManager;
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

                    List list = web.Lists.GetByTitle(libraryNameResearch);
                    context.Load(list);
                    context.ExecuteQuery();//este es el que ejecuta lo que armamos antes, sin este es como no hacer nada

                    // a la lista/pagina le pedimos que nos traiga todos los items
                    CamlQuery query = CamlQuery.CreateAllItemsQuery();// aca se puede mejorar para filtrar los ya sinconinizados
                    ListItemCollection items = list.GetItems(query);
                    context.Load(items);
                    context.ExecuteQuery();//ejecutamos

                    List<Research> listaResearchDto = new List<Research>();
                    List<Material> listaMateriales = repositorio.Listar<Material>();

                    List<ResearchEstadioDto> listaEstadioDto = repositorio.Listar<ResearchEstadio>()
                        .Select(x => new ResearchEstadioDto { EstadioId = x.EstadioId, Descripcion = x.Descripcion }).ToList();

                    List<ResearchCondicionDto> listaCondicionDto = repositorio.Listar<ResearchCondicion>()
                        .Select(x => new ResearchCondicionDto { CondicionId = x.CondicionId, Descripcion = x.Descripcion }).ToList();

                    List<ResearchHumedadSueloDto> listaHumedadSueloDto = repositorio.Listar<ResearchHumedadSuelo>()
                        .Select(x => new ResearchHumedadSueloDto { HumedadSueloId = x.HumedadSueloId, Descripcion = x.Descripcion }).ToList();

                    List<LocalidadDto> listaLocalidadDto = repositorio.Listar<Localidad>()
                        .Select(x => new LocalidadDto { LocalidadId = x.LocalidadId, Nombre = x.Nombre, PartidoId = x.PartidoId, ProvinciaId = x.ProvinciaId }).ToList();
                    List<Partido> listaPartido = repositorio.Listar<Partido>().ToList();
                    List<ProvinciaDto> listaProvinciaDto = repositorio.Listar<Provincia>().Select(x => new ProvinciaDto { ProvinciaId = x.ProvinciaId, Nombre = x.Nombre }).ToList();

                    List<ResearchTipoMuestraDto> listaTipoMuestraDto = repositorio.Listar<ResearchTipoMuestra>()
                        .Select(x => new ResearchTipoMuestraDto { TipoMuestraId = x.TipoMuestraId, Descripcion = x.Descripcion }).ToList();

                    List<ResearchTipoCargaDto> listaTipoCargaDto = repositorio.Listar<ResearchTipoCarga>()
                        .Select(x => new ResearchTipoCargaDto { TipoCargaId = x.TipoCargaId, Descripcion = x.Descripcion }).ToList();

                    List<CampañaDto> listaCampañaDto = repositorio.Listar<Campaña>()
                        .Select(x => new CampañaDto { CampañaId = x.CampañaId, Descripcion = x.Descripcion }).ToList();

                    List<ComercialDto> listaComercialDto = repositorio.Listar<Comercial>()
                        .Select(x => new ComercialDto { ComercialId = x.ComercialId, Email = x.Email }).Where(x => x.Email != null).ToList();

                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionStringAzure);
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobClient.GetContainerReference(NOMBRE_CONTENEDOR);
                    // Definir los permisos del SAS token
                    SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy
                    {
                        SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-15),// Tiempo de inicio del acceso (15 minutos antes del tiempo actual)
                        SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1),   // Tiempo de expiración del acceso (1 hora después del tiempo actual)
                        Permissions = SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write
                    };
                    // Generar el SAS token
                    string sasToken = container.GetSharedAccessSignature(sasConstraints, null);
                    // Combinar la cadena de conexión con el SAS token
                    string containerUriWithSas2 = $"{GetContainerUri(connectionStringAzure, NOMBRE_CONTENEDOR)}{sasToken}";
                    // Crear un CloudBlobContainer con la URL del contenedor y el SAS token
                    CloudBlobContainer cloudBlobContainer = new CloudBlobContainer(new Uri(containerUriWithSas2));
                    
                    foreach (ListItem item in items)
                    {
                        Research itemData = new Research();
                        itemError = item;

                        guardarLog(item);

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
                        itemData.ProvinciaId = item["ProvinciaId"] is int ? int.Parse(item["ProvinciaId"].ToString()) : (int?)null;
                        itemData.PartidoId = item["PartidoId"] is int ? int.Parse(item["PartidoId"].ToString()) : (int?)null;
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
                        itemData.Attachments = item["Attachments"] is bool ? (bool)item["Attachments"] : (bool?)null;
                        string rutaArchivos = item["FileDirRef"] == null ? "" : item["FileDirRef"].ToString();

                        string json = JsonConvert.SerializeObject(itemData, Formatting.Indented);
                        Console.WriteLine(json);

                        if (itemData.Attachments == true)
                        {
                            // get files
                            string folderRelativeUrl = $"{rutaArchivos}/Attachments/{itemData.IdPowerApp}";
                            Folder folder = web.GetFolderByServerRelativeUrl(folderRelativeUrl);
                            FileCollection files = folder.Files;

                            context.Load(files);
                            context.ExecuteQuery();

                            foreach (Microsoft.SharePoint.Client.File file in files)
                            {
                                string blobUri = "";
                                var stream = file.OpenBinaryStream();
                                context.ExecuteQuery();

                                string rutaArchivo = itemData.IdPowerApp.ToString() + "/" + file.Name;
                                
                                // Descargar la imagen desde SharePoint
                                byte[] imageBytes = DownloadImageFromSharePoint(stream);

                                CloudBlockBlob blob = container.GetBlockBlobReference(rutaArchivo);

                                using (MemoryStream memoryStream = new MemoryStream(imageBytes))
                                {
                                    blob.UploadFromStream(memoryStream);
                                    blobUri = blob.Uri.ToString();
                                }
                                
                                // Subir la imagen al contenedor de Azure Blob Storage y obtener su url
                                //blobUri = UploadImage(cloudBlobContainer, rutaArchivo, imageBytes);

                                itemData.Adjuntos.Add(new ResearchAdjunto()
                                {
                                    ResearchAdjuntoId = (int)itemData.IdPowerApp,
                                    Path = blobUri,
                                    Nombre = file.Name,
                                });
                            }
                        }

                        listaResearchDto.Add(itemData);

                        repositorio.Agregar(itemData);
                        repositorio.GuardarCambios();

                        // GSIAN: prueba para eliminar registro sincronizado
                        //var idDetener2 = itemError["ID"] is int ? int.Parse(itemError["ID"].ToString()) : (int?)null;
                        //if (idDetener2 == 127)
                        //{
                        //    item.DeleteObject();
                        //    context.ExecuteQuery();
                        //}
                    }
                    return listaResearchDto;
                }
            }
            catch (Exception e)
            {
                logger.Debug($"Error al Consultar registros de Research");
                var id = itemError["ID"] is int ? int.Parse(itemError["ID"].ToString()) : (int?)null;
                logger.Info($"ERROR Research con registro: {id}");
                logger.Error(e.Message);
                throw;
            }
        }

        static string UploadImage(CloudBlobContainer container, string blobName, byte[] imageBytes)
        {
            string blobUri = "";
            // Obtener una referencia al blob
            var blob = container.GetBlockBlobReference(blobName);
            // Crea el blob (simulando la carpeta) en el contenedor
            blob.UploadText("");

            // Subir la imagen al blob
            using (var stream = new MemoryStream(imageBytes))
            {
                try
                {
                    blob.UploadFromStreamAsync(stream);
                    // Obtén la ubicación del blob recién creado
                    blobUri = blob.Uri.ToString();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return blobUri;
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

        static string GetContainerUri(string connectionString, string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            return container.Uri.ToString();
        }

        private void guardarLog(ListItem item)
        {
            var data = new
            {
                Material = item["Cultivo"],
                MaterialAntecesor = item["Antecesor"],
                EstadioFenologico = item["Estadiofenologico"],
                CondicionCultivo = item["Condicioncultivo"],
                HumedadSuelo = item["Humedadsuelo"],
                Comentarios = item["Comentarios"],
                Partido = item["Partido"],
                Localidad = item["Localidad"],
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
    }
}
