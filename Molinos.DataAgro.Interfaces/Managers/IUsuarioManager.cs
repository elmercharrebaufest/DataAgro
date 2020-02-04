using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces.Managers
{
    public interface IUsuarioManager
    {
        bool AceptoTerminosYCondiciones(string nombre, string Cuit);
        void Aceptar(string nombre, string Cuit);
    }
}
