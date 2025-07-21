using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces.Managers
{
    public interface ILogManager
    {
        List<LogDto> TraerTodoLog(DateTime fecha);

        void EliminarLogsAntiguos();
    }
}
