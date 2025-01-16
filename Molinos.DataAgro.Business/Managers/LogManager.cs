using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Business.Managers
{
    public class LogManager : ILogManager
    {
        private readonly IRepositorio repositorio;
        public LogManager(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }
        public List<LogDto> TraerTodoLog(DateTime hoy)
        {
            // Ajustamos las fechas para el rango del día completo
            var fechaInicio = hoy.Date;
            var fechaFin = hoy.Date.AddDays(1).AddTicks(-1);

            return repositorio.Listar<Log, LogDto>(x => new LogDto
            {
                Id = x.Id,
                Fecha = x.Fecha,
                Xml = x.Xml
            }, x => x.Fecha >= fechaInicio && x.Fecha <= fechaFin, 0, "Fecha");
        }
    }
}
