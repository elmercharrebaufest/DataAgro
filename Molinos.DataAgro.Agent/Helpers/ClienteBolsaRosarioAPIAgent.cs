using System;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Autofac.Extras.NLog;
using System.Configuration;
using RestSharp;
using System.Net;
using Newtonsoft.Json;
using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ClienteBolsaRosarioAPIAgent : IClienteBolsaRosarioAPIAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        readonly string urlBCR = ConfigurationManager.AppSettings["UrlBaseBolsaRosario"];
        readonly string versionClienteBCR = ConfigurationManager.AppSettings["VersionClienteBolsaRosario"];

        public ClienteBolsaRosarioAPIAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public List<DataBCR> ConsultarPrecios(DateTime fecha, int[] arrIdMateriales)
        {
            try
            {
                logger.Debug($"Gestionando Token BCR para traer precios pizarra...");
                Configuracion datosConfiguracion = repositorio.Obtener<Configuracion>(1);
                var client = new RestClient(urlBCR + "v" + versionClienteBCR + "/Login") { Timeout = -1 };
                var request = new RestRequest(Method.POST);
                request.AddHeader("api_key", datosConfiguracion.ApiKeyBolsaRosario);
                request.AddHeader("secret", datosConfiguracion.SecretBolsaRosario);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                var result = JsonConvert.DeserializeObject<TokenBCR>(response.Content);

                List<DataBCR> listPrecios = new List<DataBCR>();

                if (result != null)
                {
                    var token = result.Data.Token;
                    logger.Debug($"Token obtenido: {token}");
                    string nombresMateriales = "";
                    List<Material> materiales = repositorio.Listar<Material>();
                    for (int i = 0; i < arrIdMateriales.Length; i++)
                    {
                        int codMatBCR = arrIdMateriales[i];
                        int codMatDA = codMatBCR == 1 ? 2 : codMatBCR == 2 ? 1 : codMatBCR == 21 ? 3 : 4;
                        nombresMateriales += (nombresMateriales == "" ? "" : ", ") + materiales.Find(x => x.MaterialId == codMatDA).Descripcion;
                    }

                    logger.Debug($"Se traerán los precios pizarra del día {fecha:yyyy-MM-dd} para los materiales {nombresMateriales}");
                    for (int i = 0; i < arrIdMateriales.Length; i++)
                    {
                        int idMaterial = arrIdMateriales[i];
                        string dia = fecha.ToString("yyyy-MM-dd");
                        string url = urlBCR + "v" + versionClienteBCR + "/PreciosCamara?idGrano=" + idMaterial + "&fechaConcertacionDesde=" + dia + "&fechaConcertacionHasta=" + dia;
                        RestClient client2 = new RestClient(url);
                        client2.Timeout = -1;
                        RestRequest request2 = new RestRequest(Method.GET);
                        request2.AddHeader("Authorization", token);
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        IRestResponse response2 = client2.Execute(request2);
                        Console.WriteLine(response2.Content);
                        PizarraBolsaRosarioDto result2 = JsonConvert.DeserializeObject<PizarraBolsaRosarioDto>(response2.Content);

                        foreach (DataBCR p in result2.data)
                        {
                            p.id_MaterialDA = idMaterial == 1 ? 2 : idMaterial == 2 ? 1 : idMaterial == 20 ? 4 : idMaterial == 3 ? 6 : 3;
                            p.fecha_Operacion_Pizarra = p.fecha_Operacion_Pizarra.Date;
                            logger.Debug("Precio Pizarra - ID Material: " + p.id_MaterialDA + " - Fecha Op.: " + p.fecha_Operacion_Pizarra + " - Precio: " + p.precio_Cotizacion);
                            listPrecios.Add(p);
                        }
                    }
                }
                else logger.Debug($"La respuesta de la API de la bolsa de Rosario fue vacía.");

                return listPrecios;
            }
            catch (Exception e)
            {
                logger.Error("Error al Consultar Precios Pizarra", e.Message);
                throw;
            }
        }
    }
}
