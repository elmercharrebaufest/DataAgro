using Microsoft.Win32.TaskScheduler;
using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class TareaProgramadaModel : Resultado
    {
        public List<TaskModel> Datos { get; set; }

        public TareaProgramadaModel()
        {
            Datos = new List<TaskModel>();
        }
    }

    public class TaskModel : Resultado
    {
        public string Name { get; set; }
        public DateTime NextRunTime { get; set; }
        public DateTime LastRunTime { get; set; }
        public virtual string ActionURL { get; set; }
        public int RepeticionEnMinutos { get; set; }
        public DateTime Inicio { get; set; }
    }


}

