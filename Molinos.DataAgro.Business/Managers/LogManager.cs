using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Molinos.DataAgro.Business.Managers
{
    public class LogManager : ILogManager
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;

        public LogManager(IRepositorio repositorio, ILogger logger)
        {
            this.repositorio = repositorio;
            this.logger = logger;
        }

        public void EliminarLogsAntiguos()
        {
            string unidad = @"L:\";
            List<Archivo> archivosEliminados = BorrarArchivosViejos(unidad, TimeSpan.FromDays(180)); // 6 meses

            if (archivosEliminados == null || archivosEliminados.Count == 0)
            {
                logger.Info($"No se elimino ningun archivo.");
            }
            else
            {
                logger.Info("ARCHIVOS ELIMINADOS:");
                foreach (var info in archivosEliminados)
                {
                    logger.Info($"Nombre: {info.Nombre}, Peso: {info.PesoKB} KB, Fecha creacion: {info.FechaCreacion}, Fecha ultima modificacion {info.FechaUltimaModificacion}");
                }
            }
        }

        // 🔁 Método ahora virtual para poder simularlo en tests
        protected virtual List<Archivo> BorrarArchivosViejos(string path, TimeSpan antiguedad)
        {
            var archivosEliminados = new List<Archivo>();
            DateTime limite = DateTime.Now.Subtract(antiguedad);

            try
            {
                foreach (string archivo in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        FileInfo fi = new FileInfo(archivo);

                        if (fi.LastWriteTime < limite)
                        {
                            archivosEliminados.Add(new Archivo
                            {
                                Nombre = fi.FullName,
                                PesoKB = fi.Length / 1024,
                                FechaCreacion = fi.CreationTime,
                                FechaUltimaModificacion = fi.LastWriteTime
                            });

                            fi.Delete();
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Error al eliminar {archivo}: {ex.Message}", ex);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Error al acceder al directorio: " + e.Message, e);
            }

            return archivosEliminados;
        }

        public List<LogDto> TraerTodoLog(DateTime fecha)
        {
            var fechaInicio = fecha.Date;
            var fechaFin = fecha.Date.AddDays(1).AddTicks(-1);

            return repositorio.Listar<Log, LogDto>(x => new LogDto
            {
                Id = x.Id,
                Fecha = x.Fecha,
                Xml = x.Xml
            }, x => x.Fecha >= fechaInicio && x.Fecha <= fechaFin, 0, "Fecha").OrderByDescending(x => x.Fecha).ToList();
        }
    }
}
