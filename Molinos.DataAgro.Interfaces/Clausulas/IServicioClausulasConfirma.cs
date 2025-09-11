using Molinos.DataAgro.Entities.ClausulasBoleto;
using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Molinos.DataAgro.Entities.Entities;
namespace Molinos.DataAgro.Interfaces.Clausulas
{
    public interface IServicioClausulasConfirma
    {
        ResultadoClausula DevolverClausulas(ClausulaConfirma comando);
    }

}
