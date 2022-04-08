using System;
using System.Collections.Generic;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IMailProveedorAgent
    {
        List<MailProveedorDto> Ejecutar(List<string> cuits);
    }
}