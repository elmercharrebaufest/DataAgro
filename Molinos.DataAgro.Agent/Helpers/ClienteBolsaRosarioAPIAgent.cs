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
        private readonly ILogDataAgroManager logDataAgroManager;
        readonly String urlBCR = ConfigurationManager.AppSettings["UrlBaseBolsaRosario"];
        readonly String versionClienteBCR = ConfigurationManager.AppSettings["VersionClienteBolsaRosario"];

        public ClienteBolsaRosarioAPIAgent(ILogger logger, IRepositorio repositorio, ILogDataAgroManager logDataAgroManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.logDataAgroManager = logDataAgroManager;
        }

        public List<DataBCR> ConsultarPrecios(DateTime fecha, int[] arrIdMateriales)
        {
            try
            {
                Configuracion datosConfiguracion = repositorio.Obtener<Configuracion>(1);
                var client = new RestClient(urlBCR + "v" + versionClienteBCR + "/Login");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("api_key", datosConfiguracion.ApiKeyBolsaRosario);
                request.AddHeader("secret", datosConfiguracion.SecretBolsaRosario);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                var result = JsonConvert.DeserializeObject<TokenBCR>(response.Content);
                var token = result.data.token;

                List<DataBCR> listPrecios = new List<DataBCR>();

                //int[] idMateriales = new int[4] { 1, 2, 20, 21 };
                if (result != null)
                {
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
                            p.id_MaterialDA = idMaterial == 1 ? 2 : idMaterial == 2 ? 1 : idMaterial == 20 ? 4 : 3;
                            p.fecha_Operacion_Pizarra = p.fecha_Operacion_Pizarra.Date;
                            listPrecios.Add(p);
                        }
                    }
                }

                return listPrecios;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

}
