using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.Managers
{
    public class LogManager : ILogManager
    {
        private readonly IRepositorio repositorio;
        public LogManager (IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }
        public List<LogDto> TraerTodoLog(DateTime hoy)
        {
           
            return repositorio.Listar<Log, LogDto>(x => new LogDto
            {
                Id = x.Id,
                Fecha = x.Fecha,
                Xml = x.Xml
               
            }, x => DbFunctions.TruncateTime(x.Fecha) == hoy);
        }
    }
}
