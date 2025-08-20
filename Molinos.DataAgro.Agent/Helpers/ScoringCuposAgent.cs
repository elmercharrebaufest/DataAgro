using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces.Agent;
using Molinos.DataAgro.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ScoringCuposAgent : IScoringCuposAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string urlScoringCupos = ConfigurationManager.AppSettings["UrlScoringCupos"];

        public ScoringCuposAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public ConsultaScoringCuposDto ConsultarScoringCupos()
        {
            ConsultaScoringCuposDto listaScoringCupos = new ConsultaScoringCuposDto() { data = new List<ScoringCuposDto>() };
            try
            {
                Configuracion datosConfiguracion = repositorio.Obtener<Configuracion>(1);

                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(urlScoringCupos)
                };
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("x-api-key", datosConfiguracion.ApiKeyScoringCupos);
                HttpResponseMessage response = client.GetAsync(urlScoringCupos).Result;
                response.EnsureSuccessStatusCode();
                var res = response.Content.ReadAsAsync<dynamic>().Result;
                var jObject = JObject.Parse(res.ToString());

                ConsultaScoringCuposDto model = JsonConvert.DeserializeObject<ConsultaScoringCuposDto>(jObject["message"].ToString());
                listaScoringCupos.data.AddRange(model.data);

                logger.Info("Consulta a Scoring de Cupos finalizó correctamente");
            }
            catch (Exception ex)
            {
                logger.Error("Error al consultar el Scoring de Cupos", ex);
                throw;
            }

            return listaScoringCupos;
        }
    }
}
