using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IProcessRg2300HangfireJob : IHangfireJob { }

    public class ProcessRg2300HangfireJob : IProcessRg2300HangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IRG2300Manager rG2300Manager;

        public ProcessRg2300HangfireJob(ILogger logger, IRepositorio repositorio, IRG2300Manager rG2300Manager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.rG2300Manager = rG2300Manager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ProcessRg2300HangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("ProcessRg2300 - Iniciando");
            var str = ConfigurationManager.AppSettings["Rg2300"];

            var objReader = new StreamReader(str.ToString(), System.Text.Encoding.Default);

            string sLine = "";

            var lista = new List<RG2300>();

            var j = 0;

            while (sLine != null)
            {
                sLine = objReader.ReadLine();

                if (j == 0)
                {
                    j = 1;
                }
                else
                {
                    if (sLine != null)
                    {
                        var Rg = sLine.Split(';');
                        lista.Add(new RG2300
                        {
                            CUIT = !String.IsNullOrEmpty(Rg[0]) ? Rg[0].Replace('"', '\0').Replace('\\', '\0') : String.Empty,
                            RazonSocial = !String.IsNullOrEmpty(Rg[1]) ? Rg[1].Replace("\"", String.Empty) : String.Empty,
                            Categoria = !String.IsNullOrEmpty(Rg[2]) ? Rg[2].Replace("\"", String.Empty) : String.Empty,
                            Situacion = !String.IsNullOrEmpty(Rg[3]) ? Rg[3].Replace("\"", String.Empty) : String.Empty,
                            CBU = !String.IsNullOrEmpty(Rg[4]) ? Rg[4].Replace("\"", String.Empty) : String.Empty,
                            FechaActCBU = !String.IsNullOrEmpty(Rg[5]) ? (DateTime?)DateTime.Parse(Rg[5]) : null,
                            FechaPubInclusion = !String.IsNullOrEmpty(Rg[6]) ? (DateTime?)DateTime.Parse(Rg[6]) : null,
                            FechaPubSuspension = !String.IsNullOrEmpty(Rg[7]) ? (DateTime?)DateTime.Parse(Rg[7]) : null,
                            FechaLevSuspension = !String.IsNullOrEmpty(Rg[8]) ? (DateTime?)DateTime.Parse(Rg[8]) : null,
                            FechaNotExclusion = !String.IsNullOrEmpty(Rg[9]) ? (DateTime?)DateTime.Parse(Rg[9]) : null,
                            FechaActRegistro = !String.IsNullOrEmpty(Rg[10]) ? (DateTime?)DateTime.Parse(Rg[10]) : null,
                            Observaciones = !String.IsNullOrEmpty(Rg[11]) ? Rg[11].Replace("\"", String.Empty) : String.Empty,
                            FechaGeneracion = !String.IsNullOrEmpty(Rg[12]) ? (DateTime?)DateTime.Parse(Rg[12]) : null
                        });
                    }
                }
            }
            objReader.Close();
            logger.Info($"ProcessRg2300 - Lineas leidas: {lista.Count}");
            try
            {
                rG2300Manager.InsetarRG2300(lista);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            logger.Info($"ProcessRg2300 - Fin. Lineas INSERTADAS: {lista.Count}");
        }
    }
}
