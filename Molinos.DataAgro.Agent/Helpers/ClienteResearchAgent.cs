using Autofac.Extras.NLog;
using Microsoft.SharePoint.Client;
using Molinos.DataAgro.Entities.Dto;
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
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ClienteResearchAgent: IClienteResearchAgent
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

        public ClienteResearchAgent(ILogger logger, IRepositorio repositorio, ILogDataAgroManager logDataAgroManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.logDataAgroManager = logDataAgroManager;
        }

        public List<ResearchDto> ConsultarItems() {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
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

                    List<ResearchDto> listPrecios = new List<ResearchDto>();

                    //armamos la lista DTO o lo que necesitemos para trabajar
                    foreach (ListItem item in items)
                    {
                        ResearchDto itemData = new ResearchDto();

                        //itemData.Title = item["Title"].ToString();
                        //itemData.Usuario = item["Usuariorelevo"].ToString();
                        //itemData.Zona = item["Zona"].ToString();
                        itemData.Id = item["ID"] is int ? int.Parse(item["ID"].ToString()) : (int?)null;
                        //itemData.Sincronizado = item["Sincronizado"] is bool ? (bool)item["Sincronizado"] : (bool?)null;
                        itemData.FechaAlta = item["Created"] is DateTime ? (DateTime)item["Created"] : (DateTime?)null;
                        //itemData.PathDocumentos = (FieldUrlValue)item["Documentos"];

                        itemData.tipoCarga = item["tipoCarga"] == null ? "" : item["tipoCarga"].ToString();
                        itemData.Cultivo = item["Cultivo"] == null ? "" : item["Cultivo"].ToString();
                        itemData.Antecesor = item["Antecesor"] == null ? "" : item["Antecesor"].ToString();
                        itemData.Campana = item["Campa_x00f1_a"] == null ? "" : item["Campa_x00f1_a"].ToString();
                        itemData.EstadioFenologico = item["Estadiofenologico"] == null ? "" : item["Estadiofenologico"].ToString();
                        itemData.CondicionCultivo = item["Condicioncultivo"] == null ? "" : item["Condicioncultivo"].ToString();
                        itemData.HumedadSuelo = item["Humedadsuelo"] == null ? "" : item["Humedadsuelo"].ToString();
                        itemData.Comentarios = item["Comentarios"] == null ? "" : item["Comentarios"].ToString();
                        itemData.Partido = item["Partido"] == null ? "" : item["Partido"].ToString();
                        itemData.Localidad = item["Localidad"] == null ? "" : item["Localidad"].ToString();
                        itemData.Provincia = item["Provincia"] == null ? "" : item["Provincia"].ToString();
                        itemData.Latitud = item["Latitud"] is int ? int.Parse(item["Latitud"].ToString()) : (int?)null;
                        itemData.Longitud = item["Longitud"] is int ? int.Parse(item["Longitud"].ToString()) : (int?)null;
                        itemData.rendimiento = item["rendimiento"] is double ? double.Parse(item["rendimiento"].ToString()) : (double?)null;
                        itemData.MuestraUno = item["Muestra1"] == null ? "" : item["Muestra1"].ToString();
                        itemData.MedidasUno = item["Medidas1"] == null ? "" : item["Medidas1"].ToString();
                        itemData.PromedioMuestraUno = item["Promediomuestra1"] is int ? int.Parse(item["Promediomuestra1"].ToString()) : (int?)null;
                        itemData.MuestraDos = item["Muestra2"] == null ? "" : item["Muestra2"].ToString();
                        itemData.MedidasDos = item["Medidas2"] == null ? "" : item["Medidas2"].ToString();
                        itemData.PromedioMuestraDos = item["Promediomuestra2"] is int ? int.Parse(item["Promediomuestra2"].ToString()) : (int?)null;
                        itemData.MuestraTres = item["Muestra3"] == null ? "" : item["Muestra3"].ToString();
                        itemData.MedidasTres = item["Medidas3"] == null ? "" : item["Medidas3"].ToString();
                        itemData.PromedioMuestraTres = item["Promediomuestra3"] is int ? int.Parse(item["Promediomuestra3"].ToString()) : (int?)null;
                        itemData.DistanciaHileras = item["Distanciahileras"] is double ? double.Parse(item["Distanciahileras"].ToString()) : (double?)null;
                        itemData.Coeficiente = item["Coeficiente"] is double ? double.Parse(item["Coeficiente"].ToString()) : (double?)null;
                        itemData.estadoConectividad = item["estadoConectividad"] == null ? "" : item["estadoConectividad"].ToString();
                        itemData.Attachments = item["Attachments"] is bool ? (bool)item["Attachments"] : (bool?)null;
                        itemData.CapitulosGirasol = item["CapitulosGirasol"] is int ? int.Parse(item["CapitulosGirasol"].ToString()) : (int?)null;
                        //itemData.Author = item["Author"] == null ? "" : item["Author"].ToString();// crear obj
                        //itemData.Editor = item["Editor"] == null ? "" : item["Editor"].ToString();// crear obj

                        // Inicializa otras propiedades

                        string json = JsonConvert.SerializeObject(itemData, Formatting.Indented);
                        Console.WriteLine(json);


                        //// get files
                        //string folderRelativeUrl = $"/sites/ResearchMOA/Documentos Compartidos/{itemData.Title}";
                        //Folder folder = web.GetFolderByServerRelativeUrl(folderRelativeUrl);
                        //FileCollection files = folder.Files;

                        //context.Load(files);
                        //context.ExecuteQuery();

                        //foreach (Microsoft.SharePoint.Client.File file in files)
                        //{
                        //    Console.WriteLine($"Nombre del archivo: {file.Name}, Tamaño: {file.Length}");

                        //    var stream = file.OpenBinaryStream();
                        //    context.ExecuteQuery();
                        //    //save files
                        //    using (var fileStream = new FileStream(Path.Combine(downloadPathResearch, file.Name), FileMode.Create))
                        //    {
                        //        stream.Value.CopyTo(fileStream);
                        //    }
                        //}

                        listPrecios.Add(itemData);

                        //actualizar registro sincronizado
                        //item["Sincronizado"] = true;
                        //item.Update();
                        //context.ExecuteQuery();//ejecutar

                    }
                    return listPrecios;
                }
            }
            catch (Exception e)
            {
                logger.Debug($"Error al Consultar registros de Research");
                logger.Error(e.Message);
                throw;
            }
        }
    }
}
