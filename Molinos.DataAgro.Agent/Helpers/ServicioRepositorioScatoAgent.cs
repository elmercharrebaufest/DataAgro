using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ScatoRepositorio;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent
{
    public class ServicioRepositorioScatoAgent : IServicioRepositorioScatoAgent
    {
        public ServicioRepositorioScatoAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        string UserSap = ConfigurationManager.AppSettings["SapUser"];
        string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public List<EstablecimientoStockDto> ListarEstablecimientos(string cuitProveedor, string campania)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                var establecimientos = new List<EstablecimientoStockDto>()
                {

                };
                for (int i = 0; i < 2; i++)
                {
                   var e = new EstablecimientoStockDto
                    {
                       Cantidad = 30000,
                       Establecimiento = "CAPALDI",
                       Cosecha = "19-20",
                       Localidad = "Buenos Aires",
                       Provincia = "Buenos Aires"

                    };
                    establecimientos.Add(e);
                }
                return establecimientos.ToList(); 
            }
            else
            {
                try
                {
                    var agent = new ServicioRepositorioClient();                                   

                    var valor = agent.ListarCampaniaPorCuit(cuitProveedor, campania);
                    logger.Debug(valor.ToXml());

                    var respuesta = valor.Select(x => new EstablecimientoStockDto()
                    {
                        Cantidad = x.StockDeclarado - x.StockUtilizado,
                        Establecimiento = x.NombreEstablecimiento,
                        Cosecha = x.Cosecha,                     
                        Provincia = x.Provincia,
                        Localidad = string.IsNullOrEmpty(x.Localidad) ? "" : x.Localidad.Split('-').Last().Split('(').First(),
                        CodigoEstablecimiento = x.CodigoEstablecimiento
                    }).OrderBy(x => x.Cantidad).ToList();
                    respuesta = ValidarEstablecimiento(respuesta);
                    return respuesta;
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw;
                }
            }
        }

        private List<EstablecimientoStockDto> ValidarEstablecimiento(List<EstablecimientoStockDto> establecimientoStockDtos)
        {
            var establecimientos = new List<EstablecimientoStockDto>();
            foreach (var establecimiento in establecimientoStockDtos)
            {
                int number;

                bool success = int.TryParse(establecimiento.CodigoEstablecimiento, out number);
                if (success && number > 90000)
                {
                    establecimientos.Add(establecimiento);
                }
            }
            return establecimientos;
        }

    }
}
