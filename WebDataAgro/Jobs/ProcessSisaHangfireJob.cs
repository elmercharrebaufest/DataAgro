using NLog;
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
    public interface IProcessSisaHangfireJob : IHangfireJob { }

    public class ProcessSisaHangfireJob : IProcessSisaHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly ISISAManager sisaManager;

        public ProcessSisaHangfireJob(ILogger logger, IRepositorio repositorio, ISISAManager sisaManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.sisaManager = sisaManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ProcessSisaHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("HANGFIRE - ProcessSisa - Iniciando");
            var str = ConfigurationManager.AppSettings["SISA"];

            var objReader = new StreamReader(str.ToString(), System.Text.Encoding.Default);

            string sLine = "";

            var lista = new List<SISA>();
            var cuits = new List<SISA>();
            var j = 0;

            while (sLine != null)
            {
                sLine = objReader.ReadLine();

                if (j == 0)
                {
                    j = 1;
                }
                else if (j == 1)
                {
                    j = 2;
                }
                else
                {

                    if (sLine != null)
                    {
                        var sisa = sLine.Split(';');
                        var sisaList = new SISA();

                        sisaList.CUIT = !String.IsNullOrEmpty(sisa[0]) ? sisa[0].Replace('"', '\0').Replace('\\', '\0') : String.Empty;
                        sisaList.RazonSocial = !String.IsNullOrEmpty(sisa[1]) ? sisa[1].Replace("\"", String.Empty) : String.Empty;
                        sisaList.EstadoCuit = !String.IsNullOrEmpty(sisa[2]) ? Int32.Parse(sisa[2].Replace("\"", String.Empty)) : 0;
                        sisaList.FechaVigenciaEstado = !String.IsNullOrEmpty(sisa[3]) ? (DateTime?)DateTime.Parse(sisa[3]) : null;
                        sisaList.FechaNotifDFEEstado = !String.IsNullOrEmpty(sisa[4]) ? (DateTime?)DateTime.Parse(sisa[4]) : null;
                        sisaList.CBU = !String.IsNullOrEmpty(sisa[5]) ? sisa[5].Replace("\"", String.Empty) : String.Empty;
                        sisaList.FechaActCBU = !String.IsNullOrEmpty(sisa[6]) ? (DateTime?)DateTime.Parse(sisa[6]) : null;

                        sisaList.CodCategoria = !String.IsNullOrEmpty(sisa[8]) ? Int32.Parse(sisa[8].Replace("\"", String.Empty)) : 0;
                        sisaList.Categoria = !String.IsNullOrEmpty(sisa[9]) ? sisa[9].Replace("\"", String.Empty) : String.Empty;
                        sisaList.SituacionCategoria = !String.IsNullOrEmpty(sisa[10]) ? sisa[10].Replace("\"", String.Empty) : String.Empty;
                        sisaList.FechaVigenciaCategoria = !String.IsNullOrEmpty(sisa[11]) ? (DateTime?)DateTime.Parse(sisa[11]) : null;
                        sisaList.FechaNotifDFECategoria = !String.IsNullOrEmpty(sisa[12]) ? (DateTime?)DateTime.Parse(sisa[12]) : null;
                        sisaList.Observaciones = !String.IsNullOrEmpty(sisa[13]) ? sisa[13].Replace("\"", String.Empty) : String.Empty;
                        sisaList.FechaGeneracion = !String.IsNullOrEmpty(sisa[14]) ? (DateTime?)DateTime.Parse(sisa[14]) : null;

                        if (sisaList.FechaVigenciaEstado > DateTime.Now.Date || sisaList.FechaVigenciaCategoria > DateTime.Now.Date)
                        {
                            cuits.Add(sisaList);
                        }
                        else
                        {
                            lista.Add(sisaList);
                        }
                    }
                }
            }
            objReader.Close();
            logger.Info($"ProcessSisa - Lineas leidas: {lista.Count}");
            int lineas = 0;
            try
            {
                lineas = sisaManager.InsertarSISA(lista, cuits);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            logger.Info($"HANGFIRE - ProcessSisa - Fin. Lineas INSERTADAS: {lineas}");
        }
    }
}
