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
    public interface IProcessFacacopHangfireJob : IHangfireJob { }

    public class ProcessFacacopHangfireJob : IProcessFacacopHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IFacacopManager facacopManager;

        public ProcessFacacopHangfireJob(ILogger logger, IRepositorio repositorio, IFacacopManager facacopManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.facacopManager = facacopManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ProcessFacacopHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("HANGFIRE - ProcessFacacop - Iniciando");
            var str = ConfigurationManager.AppSettings["Facacop"];

            StreamReader objReader = new StreamReader(str.ToString());

            string sLine = "";

            var lista = new List<FACACOP>();

            var j = 0;

            while (sLine != null)
            {
                sLine = objReader.ReadLine();

                if (j < 3)
                {
                    j += 1;
                }
                else
                {
                    if (sLine != null)
                    {
                        var Fc = sLine.Split(new[] { ',' }, 4);
                        FACACOP obj = new FACACOP();

                        obj.CUIT = !String.IsNullOrEmpty(Fc[0]) ? Fc[0].Replace('"', '\0').Replace('\\', '\0') : String.Empty;
                        obj.Fecha1 = !String.IsNullOrEmpty(Fc[1]) ? DateTime.Parse(Fc[1]) : DateTime.Now;
                        obj.Fecha2 = !String.IsNullOrEmpty(Fc[2]) ? DateTime.Parse(Fc[2]) : DateTime.Now;
                        obj.ObservacionesEspeciales = !String.IsNullOrEmpty(Fc[3]) ? Fc[3] : String.Empty;

                        lista.Add(obj);
                    }
                }
            }
            objReader.Close();
            logger.Info($"ProcessFacacop - Lineas leidas: {lista.Count}");
            try
            {
                facacopManager.InsetarFacacop(lista);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            logger.Info($"HANGFIRE - ProcessFacacop - Fin. Lineas INSERTADAS: {lista.Count}");
        }
    }
}
